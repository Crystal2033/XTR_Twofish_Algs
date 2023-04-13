using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static XTR_TwofishAlgs.HelpFunctions.CryptConstants;

namespace XTR_TWOFISH.CryptInterfaces
{
    public interface IKeyExpansion
    {
        public List<byte[]> GenerateRoundKeys(byte[] preparedKey, TwoFishKeySizes keySizeInBits, out List<byte[]> sBoxes);
    }
}
