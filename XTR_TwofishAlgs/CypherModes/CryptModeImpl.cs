using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTR_TWOFISH.CryptInterfaces;

namespace XTR_TWOFISH.CypherModes
{
    public abstract class CryptModeImpl : IModeEncryption
    {
        protected byte[] _mainKey;
        protected ISymmetricEncryption _cryptAlgorithm;
        protected int _textBlockSizeInBytes;

        public CryptModeImpl(byte[] mainKey, ISymmetricEncryption cryptAlgorithm, int textBlockSizeBytes)
        {
            _mainKey = mainKey;
            _cryptAlgorithm = cryptAlgorithm;
            _textBlockSizeInBytes = textBlockSizeBytes;
        }
        public abstract Task DecryptWithModeAsync(string fileToDecrypt, string decryptResultFile, CancellationToken token = default);


        public abstract Task EncryptWithModeAsync(string fileToEncrypt, string encryptResultFile, CancellationToken token = default);

    }
}
