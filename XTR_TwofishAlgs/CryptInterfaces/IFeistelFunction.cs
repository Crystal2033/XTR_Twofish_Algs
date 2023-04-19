using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTR_TWOFISH.CryptInterfaces
{
    public interface IFeistelFunction
    {
        public (byte[] F0Res, byte[] F1Res) FeistelFunction(byte[] R0, byte[] R1, byte[] evenRoundKey, byte[] oddRoundKey, List<byte[]> sBox, int raundNumber);//raundNumber to get needed raund key
    }
}
