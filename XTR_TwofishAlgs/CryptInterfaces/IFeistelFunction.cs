using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTR_TWOFISH.CryptInterfaces
{
    public interface IFeistelFunction
    {
        public byte[] FeistelFunction(ref byte[] R0, ref byte[] R1, in byte[] raundKey, List<byte[]> sBox, int raundNumber);//raundNumber to get needed raund key
    }
}
