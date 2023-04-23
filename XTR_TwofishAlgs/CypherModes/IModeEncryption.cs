using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTR_TWOFISH.CypherModes
{
    public interface IModeEncryption
    {
        public void EncryptWithMode(string fileToEncrypt, string encryptResultFile);
               
        public void DecryptWithMode(string fileToDecrypt, string decryptResultFile);
    }
}
