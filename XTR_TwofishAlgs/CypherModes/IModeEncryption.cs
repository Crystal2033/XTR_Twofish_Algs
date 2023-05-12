using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTR_TWOFISH.CypherModes
{
    public interface IModeEncryption
    {
        public Task EncryptWithModeAsync(string fileToEncrypt, string encryptResultFile, CancellationToken token);
               
        public Task DecryptWithModeAsync(string fileToDecrypt, string decryptResultFile, CancellationToken token);
    }
}
