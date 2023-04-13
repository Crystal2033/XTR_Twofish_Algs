using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTR_TWOFISH.CryptInterfaces;
using static XTR_TwofishAlgs.HelpFunctions.CryptConstants;

namespace XTR_TwofishAlgs.KeySchedule
{
    public sealed class KeyExpansionTwoFish : IKeyExpansion
    {
        public List<byte[]> GenerateRoundKeys(in byte[] preparedKey, TwoFishKeySizes keySizeInBits, out List<byte> sBoxes)
        {
            int k = (int)keySizeInBits / 64;// M (preparedKey) key consists of 8k bytes

            throw new NotImplementedException();
        }
    }
}
