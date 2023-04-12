using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTR_TWOFISH.CryptInterfaces
{
    public interface IKeyExpansion
    {
        public List<byte[]> GenerateRoundKeys(in byte[] preparedKey);
    }
}
