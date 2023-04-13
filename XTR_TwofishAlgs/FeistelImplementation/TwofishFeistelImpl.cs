using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTR_TWOFISH.CryptInterfaces;

namespace XTR_TwofishAlgs.FeistelImplementation
{
    internal class TwofishFeistelImpl : IFeistelFunction
    {
        public byte[] FeistelFunction(ref byte[] R0, ref byte[] R1, in byte[] raundKey, int raundNumber)
        {
            throw new NotImplementedException();
        }
    }
}
