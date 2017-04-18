using System;

namespace Duration 
{
    /// <summary>
    /// A value for a note or rest.
    /// </summary>
    /// <remarks>
    /// In music, the duration value of each note is indicated independently of tempo or absolute time.
    /// The values below are expressed as powers of two.
    /// This enum has extension methods, such as Midi.ChannelExtensionMethods.Name(Midi.Channel)
    /// and Midi.ChannelExtensionMethods.IsValid(Midi.Channel), defined in Midi.ChannelExtensionMethods.
    /// </remarks>   
    public enum Value
    {
        /// <summary>
        /// Octuple whole note or octuple note or Large note
        /// </summary>   
        OctupleWhole = 3,
        /// <summary>
        /// Quadruple whole note or quadruple note or Long note
        /// </summary>
        QuadrupleWhole = 2,
        /// <summary>
        /// Whole note or double note or Breve
        /// </summary>
        DoubleWhole = 1,
        /// <summary> 
        /// Whole note or Semibreve
        /// </summary>
        Whole = 0,
        /// <summary>
        /// Half note or Minim
        /// </summary>
        Half = -1,
        /// <summary>
        /// Quarter note or Crotchet
        /// </summary>
        Quarter = -2,
        /// <summary>
        /// Eighth note or Quaver
        /// </summary>
        Eighth = -3,
        /// <summary>
        /// Sixteenth note or Semiquaver
        /// </summary>
        Sixteenth = -4,
        /// <summary>
        /// Thirty-second note or Demisemiquaver
        /// </summary>
        ThirtySecond = -5,
        /// <summary>
        /// Sixty-fourth note or Hemidemisemiquaver
        /// </summary>
        SixtyFourth = -6,
        /// <summary>
        /// Hundred twenty-eighth note or Semihemidemisemiquaver
        /// </summary>
        HundredTwentyEighth = -7,
        /// <summary>
        /// Two hundred fifty-sixth note or Demisemihemidemisemiquaver
        /// </summary>
        TwoHundredFiftySixth = -8    
    };

    public static class ValueExtensionMethods
    {
        private static string[] valueNames = new string [] 
        {
            "256th",
            "128th",
            "64th",
            "32nd",
            "Sixteenth",
            "Eighth",
            "Quarter",
            "Half",
            "Whole",
            "Double",
            "Quadruple",
            "Octuple"
        };

        public static bool isValid(this Value value)
        {
            return (int)value >= -8 && (int)value <= 3;
        }

        public static string Name(this Value value)
        {
            if (isValid(value))
            {
                return valueNames[(int)value + 8];
            }
            
            return "Invalid Note Value";
        }
    }
}
