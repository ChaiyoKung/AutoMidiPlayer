namespace AutoMidiPlayer.WPF.Core.Instruments;

/// <summary>
/// Neverness to Everness keyboard layouts.
/// </summary>
internal static class NTEKeyboardLayouts
{
    public static readonly KeyboardLayoutConfig QWERTY_21Keys = new(
        name: "QWERTY",
        keys: [
            "q", "w", "e", "r", "t", "y", "u",
            "a", "s", "d", "f", "g", "h", "j",
            "z", "x", "c", "v", "b", "n", "m",
        ]);
    public static readonly KeyboardLayoutConfig QWERTY_36Keys = new(
        name: "QWERTY",
        keys: [
            "q", "Q", "w", "^e", "e", "r", "R", "t", "T", "y", "^u", "u",
            "a", "A", "s", "^d", "d", "f", "F", "g", "G", "h", "^j", "j",
            "z", "Z", "x", "^c", "c", "v", "V", "b", "B", "n", "^m", "m"
        ]);
    public static readonly KeyboardLayoutConfig QWERTY_Guitar = new(
        name: "QWERTY",
        keys: [
            "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "-", "=",
            "q", "w", "e", "r", "t", "y", "u", "i", "o", "p", "[", "]",
            "a", "s", "d", "f", "g", "h", "j", "k", "l", ";", "'", "Enter"
        ]);
}
