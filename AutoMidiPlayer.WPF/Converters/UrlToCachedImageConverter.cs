using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AutoMidiPlayer.WPF.Services.MidiShow;
using SkiaSharp;

namespace AutoMidiPlayer.WPF.Converters;

/// <summary>
/// Converts a remote thumbnail URL (string) into a shared <see cref="BitmapSource"/>.
/// Uses SkiaSharp for deterministic unmanaged memory disposal, completely bypassing WPF's leaky image decoder.
/// </summary>
public sealed class UrlToCachedImageConverter : IValueConverter
{
    private const int DecodeWidth = 128;
    private const int MaxMemoryCacheSize = 60;

    private static readonly ConcurrentDictionary<string, BitmapSource> Cache =
        new(StringComparer.OrdinalIgnoreCase);

    private static readonly Lazy<HttpClient> Http = new(() => new HttpClient());

    private static readonly ConcurrentQueue<string> _lruQueue = new();

    private static void UpdateLru(string url)
    {
        _lruQueue.Enqueue(url);
    }

    public static void ClearMemoryCache() 
    {
        Cache.Clear();
        _lruQueue.Clear();
    }

    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string url || string.IsNullOrWhiteSpace(url))
            return null;

        if (!Uri.TryCreate(url.Trim(), UriKind.Absolute, out var uri))
            return null;

        if (Cache.Count > MaxMemoryCacheSize && !Cache.ContainsKey(uri.AbsoluteUri))
        {
            Cache.Clear();
        }

        return Cache.GetOrAdd(uri.AbsoluteUri, key =>
        {
            var cachedPath = MidiShowCache.TryGetAvatarPath(key);
            if (cachedPath is not null)
                return LoadFromFile(cachedPath);

            // Create a small placeholder for immediate return while fetching
            return _placeholder;
        });
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();

    private static readonly BitmapSource _placeholder = CreatePlaceholder();

    private static BitmapSource CreatePlaceholder()
    {
        var bmp = BitmapSource.Create(
            DecodeWidth, DecodeWidth, 96, 96, PixelFormats.Bgra32, null,
            new byte[DecodeWidth * DecodeWidth * 4], DecodeWidth * 4);
        bmp.Freeze();
        return bmp;
    }

    private static BitmapSource DecodeWithSkia(byte[] data)
    {
        using var skBitmap = SKBitmap.Decode(data);
        if (skBitmap == null) return _placeholder;

        // Calculate aspect-ratio-preserving dimensions
        int targetWidth = DecodeWidth;
        int targetHeight = (int)(skBitmap.Height * ((float)DecodeWidth / skBitmap.Width));

        var info = new SKImageInfo(targetWidth, targetHeight);
        using var resizedBitmap = skBitmap.Resize(info, new SKSamplingOptions(SKFilterMode.Linear));
        if (resizedBitmap == null) return _placeholder;

        // Must create BitmapSource on the UI thread
        var wpfBitmap = Application.Current?.Dispatcher?.Invoke(() =>
        {
            var size = resizedBitmap.RowBytes * resizedBitmap.Height;
            var bmp = BitmapSource.Create(
                resizedBitmap.Width,
                resizedBitmap.Height,
                96, 96,
                PixelFormats.Bgra32,
                null,
                resizedBitmap.GetPixels(),
                size,
                resizedBitmap.RowBytes);
            bmp.Freeze();
            return bmp;
        });

        return wpfBitmap ?? _placeholder;
    }

    private static BitmapSource LoadFromFile(string path)
    {
        try
        {
            var data = File.ReadAllBytes(path);
            return DecodeWithSkia(data);
        }
        catch
        {
            return _placeholder;
        }
    }

    public static async System.Threading.Tasks.Task<BitmapSource?> GetImageAsync(string url, System.Threading.CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(url)) return null;
        
        if (Cache.TryGetValue(url, out var memHit))
        {
            UpdateLru(url);
            return memHit;
        }

        var cachedPath = MidiShowCache.TryGetAvatarPath(url);
        if (cachedPath is not null)
        {
            var bmp = LoadFromFile(cachedPath);
            Cache[url] = bmp;
            UpdateLru(url);
            EvictOldImages();
            return bmp;
        }

        try
        {
            var data = await Http.Value.GetByteArrayAsync(url, ct);
            _ = MidiShowCache.SaveAvatarAsync(url, data);

            var bitmap = DecodeWithSkia(data);

            Cache[url] = bitmap;
            UpdateLru(url);
            EvictOldImages();

            return bitmap;
        }
        catch (OperationCanceledException)
        {
            return null;
        }
        catch
        {
            return null;
        }
    }

    private static void EvictOldImages()
    {
        while (_lruQueue.Count > MaxMemoryCacheSize && _lruQueue.TryDequeue(out var oldestUrl))
        {
            Cache.TryRemove(oldestUrl, out _);
        }
    }
}
