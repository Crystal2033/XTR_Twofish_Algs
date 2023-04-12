using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTR_TWOFISH.CryptInterfaces
{
    public interface IFeistelFunction
    {
        public byte[] FeistelFunction(ref byte[] bytes, in byte[] raundKey, int raundNumber);
    }
}
