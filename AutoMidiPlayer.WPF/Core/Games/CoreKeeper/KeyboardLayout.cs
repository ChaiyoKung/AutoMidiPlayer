using System.Collections.Generic;

namespace AutoMidiPlayer.WPF.Core.Instruments
{
    /// <summary>
    /// Core Keeper keyboard layouts.
    /// All instruments share the same 24-note chromatic layout (C3–B4) across 2 rows.
    /// Top row:    q 2 w 3 e r 5 t 6 y 7 u  (C4–B4)
    /// Bottom row: z s x d c v g b h n j m  (C3–B3)
    /// </summary>
    internal static class CoreKeeperKeyboardLayouts
    {
        public static readonly KeyboardLayoutConfig QWERTY = new(
            name: "QWERTY",
            keys: [
                "q", "2", "w", "3", "e", "r", "5", "t", "6", "y", "7", "u",
                "z", "s", "x", "d", "c", "v", "g", "b", "h", "n", "j", "m",
            ]);
    }
}