using System;
using System.Threading;
using System.Threading.Tasks;

namespace AutoMidiPlayer.WPF.Services;

/// <summary>
/// The GarbageMan is responsible for taking out the trash.
/// 
/// "Why does this work? Nobody knows. Only God and the GarbageMan know."
/// 
/// In all seriousness, WPF's unmanaged memory usage (specifically when rapidly decoding
/// remote BitmapImages and networking buffers) can sometimes outpace the .NET Garbage Collector's 
/// heuristic triggers on lower-end devices. This service steps in to forcefully take out the trash.
/// </summary>
public static class GarbageManService
{
    private static DateTime _lastCollection = DateTime.MinValue;
    private static readonly TimeSpan _minInterval = TimeSpan.FromSeconds(2);
    private static int _isTrailingSweepScheduled = 0;

    /// <summary>
    /// Forcefully compacts the Large Object Heap and performs a full Gen 2 garbage collection.
    /// Use sparingly, as this halts the execution engine.
    /// </summary>
    public static void TakeOutTheTrash(bool aggressive = false)
    {
        // If we are spammed with requests, always schedule a trailing sweep to run a few seconds 
        // later. This guarantees that unmanaged thumbnail downloads that finish asynchronously 
        // AFTER the page loads will still be cleaned up when the user stops clicking "Next".
        if (Interlocked.Exchange(ref _isTrailingSweepScheduled, 1) == 0)
        {
            _ = Task.Run(async () =>
            {
                await Task.Delay(_minInterval);
                _isTrailingSweepScheduled = 0;
                DoSweep();
            });
        }

        var now = DateTime.UtcNow;
        if (!aggressive && now - _lastCollection < _minInterval)
            return; // Don't overwork the GarbageMan

        _lastCollection = now;
        _ = Task.Run(DoSweep);
    }

    [System.Runtime.InteropServices.DllImport("psapi.dll")]
    private static extern int EmptyWorkingSet(IntPtr hwProc);

    private static void DoSweep()
    {
        // "I don't know why we have to tell the computer to clean up after itself. 
        // It's 2026, you'd think it would know better." - The GarbageMan
        
        try 
        {
            // Compact the Large Object Heap (where big network strings and arrays get stuck)
            System.Runtime.GCSettings.LargeObjectHeapCompactionMode = 
                System.Runtime.GCLargeObjectHeapCompactionMode.CompactOnce;
            
            // Sweep the generations. Must be blocking: true for LOH compaction to actually occur!
            GC.Collect(2, GCCollectionMode.Forced, blocking: true, compacting: true);
            
            // Wait for finalizers (this is where WPF's unmanaged BitmapImages actually get freed)
            GC.WaitForPendingFinalizers();
            
            // One more sweep just to be sure we got the finalized objects
            GC.Collect(2, GCCollectionMode.Forced, blocking: true);

            // Force the OS to page out unused CLR segments so Task Manager shows the real memory footprint
            var process = System.Diagnostics.Process.GetCurrentProcess();
            EmptyWorkingSet(process.Handle);
        }
        catch 
        {
            // The GarbageMan never complains, he just keeps working.
        }
    }
}
