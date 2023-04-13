using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTR_TwofishAlgs.HelpFunctions
{
    public static class CryptConstants
    {
        public const byte BITS_IN_BYTE = 8;
        public const byte DES_PART_TEXT_BYTES = 8;
        public enum TwoFishKeySizes
        {
            EASY=128, MIDDLE=192, HARD=256
        }
    }
}
