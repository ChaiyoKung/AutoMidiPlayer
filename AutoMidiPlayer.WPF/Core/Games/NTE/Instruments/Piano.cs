using System.Collections.Generic;

namespace AutoMidiPlayer.WPF.Core.Instruments;

public static partial class NTEInstruments
{
    public static readonly InstrumentConfig Piano21k = new(
        game: "Neverness to Everness",
        name: "Piano (21 Key)",
        notes: [
            60, 62, 64, 65, 67, 69, 71, // C4 D4 E4 F4 G4 A4 B4
            48, 50, 52, 53, 55, 57, 59, // C3 D3 E3 F3 G3 A3 B3
            36, 38, 40, 41, 43, 45, 47, // C2 D2 E2 F2 G2 A2 B2
        ],
        keyboardLayouts: [NTEKeyboardLayouts.QWERTY_21Keys]
    );

    public static readonly InstrumentConfig Piano36k = new(
    game: "Neverness to Everness",
    name: "Piano (36 Key)",
    notes: [
            84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, // C6 C#6 D6 D#6 E6 F6 F#6 G6 G#6 A6 A#6 B6
            72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, // C5 C#5 D5 D#5 E5 F5 F#5 G5 G#5 A5 A#5 B5
            60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, // C4 C#4 D4 D#4 E4 F4 F#4 G4 G#4 A4 A#4 B4
    ],
    keyboardLayouts: [NTEKeyboardLayouts.QWERTY_36Keys]
    );
}
