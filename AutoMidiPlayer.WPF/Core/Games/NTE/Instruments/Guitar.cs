using System.Collections.Generic;

namespace AutoMidiPlayer.WPF.Core.Instruments;

public static partial class NTEInstruments
{
    public static readonly InstrumentConfig Guitar = new(
        game: "Neverness to Everness",
        name: "Guitar",
        notes: [
            76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, // E5 F5 F#5 G5 G#5 A5 A#5 B5 C6 C#6 D6 D#6
            64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, // E4 F4 F#4 G4 G#4 A4 A#4 B4 C5 C#5 D5 D#5
            52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, // E3 F3 F#3 G3 G#3 A3 A#3 B3 C4 C#4 D4 D#4
        ],
        keyboardLayouts: [NTEKeyboardLayouts.QWERTY_Guitar]
    );
}