using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTR_TWOFISH.CryptInterfaces;
using XTR_TWOFISH.CypherEnums;
using XTR_TWOFISH.FeistelImplementation;
using XTR_TwofishAlgs.HelpFunctions;

namespace XTR_TWOFISH
{
    public class TwoFishImplementation : ISymmetricEncryption
    {
        private readonly FeistelNetwork _feistel;

        public TwoFishImplementation(FeistelNetwork feistel){
            _feistel = feistel;
        }

        private byte[] MakeCryptOperation(ref byte[] bytes, CryptOperation cryptStatus)
        {
            return _feistel.Execute(bytes, cryptStatus);
        }
        public byte[] Encrypt(ref byte[] bytes)
        {
            return MakeCryptOperation(ref bytes, CryptOperation.ENCRYPT);
        }


        public byte[] Decrypt(ref byte[] bytes)
        {
            return MakeCryptOperation(ref bytes, CryptOperation.DECRYPT);
        }
    }
}
