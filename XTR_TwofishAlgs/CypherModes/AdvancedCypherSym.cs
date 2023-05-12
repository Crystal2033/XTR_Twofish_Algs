using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XTR_TWOFISH.CryptInterfaces;
using XTR_TWOFISH.CypherEnums;
using XTR_TWOFISH.CypherModes.ModesImplementation;
using XTR_TWOFISH.FeistelImplementation;
using XTR_TwofishAlgs.Exceptions;
using XTR_TwofishAlgs.FeistelImplementation;
using XTR_TwofishAlgs.HelpFunctions;
using XTR_TwofishAlgs.KeySchedule;

namespace XTR_TWOFISH.CypherModes
{
    public sealed class AdvancedCypherSym
    {
        private byte[] _cypherKey;
        private CypherMode _mode;
        private byte[] _initVector;
        private object[] objs;

        private CryptModeImpl _encriptionModeImpl;
        private ISymmetricEncryption _cryptAlgorithm;
        private int _textBlockSizeInBytes;

        public AdvancedCypherSym(byte[] cypherKey, CypherMode mode, SymmetricAlgorithm symmetricAlgorithm, byte[] initVector = null, params object[] obj)
        {
            _cypherKey = cypherKey;
            _mode = mode;
            _initVector = initVector;
            objs = obj;
            

            _cryptAlgorithm = GetSymmetricAlgorithm(symmetricAlgorithm);
            _encriptionModeImpl = GetModeImplementation();
        }

        private ISymmetricEncryption GetSymmetricAlgorithm(SymmetricAlgorithm algType)
        {
            switch (algType)
            {
                case SymmetricAlgorithm.TWOFISH:
                    _textBlockSizeInBytes = 16;
                    IKeyExpansion keyExpansion = new KeyExpansionTwoFish();
                    IFeistelFunction feistelFunction = new TwofishFeistelFuncImpl();
                    FeistelNetwork feistelKernel = new FeistelNetwork(keyExpansion, feistelFunction) { MainKey = _cypherKey };
                    return new TwoFishImplementation(feistelKernel);

                default:
                    return null;

            }
        } 
        private CryptModeImpl GetModeImplementation()
        {
            switch (_mode)
            {
                case CypherMode.ECB:
                    return new ECBModeImpl(_cypherKey, _cryptAlgorithm, _textBlockSizeInBytes);
                case CypherMode.CBC:
                    return new CBCModeImpl(_cypherKey, _cryptAlgorithm, _textBlockSizeInBytes, _initVector);
                case CypherMode.CFB:
                    return new CFBModeImpl(_cypherKey, _cryptAlgorithm, _textBlockSizeInBytes,  _initVector);
                case CypherMode.OFB:
                    return new OFBModeImpl(_cypherKey, _cryptAlgorithm, _textBlockSizeInBytes, _initVector);
                case CypherMode.CTR:
                    return new RDModeImpl(_cypherKey, _cryptAlgorithm, _textBlockSizeInBytes, _initVector);
                case CypherMode.RD:
                    return new RDModeImpl(_cypherKey, _cryptAlgorithm, _textBlockSizeInBytes, _initVector, 9); 
                case CypherMode.RDH:
                    byte[] hashCode = new byte[16] { 102,22,32,44,251,63,74,85, 102, 22, 32, 44, 251, 63, 74, 85 };
                    return new RDModeImpl(_cypherKey, _cryptAlgorithm, _textBlockSizeInBytes, _initVector, 23) { HashCode=hashCode};
                default:
                    throw new UnknownModeException($"Unknown mode {_mode} exception.");
            }
        }

        public Task EncryptAsync(string fileToEncrypt, string encryptResultFile, CancellationToken token = default)
        {

            return _encriptionModeImpl.EncryptWithModeAsync(fileToEncrypt, encryptResultFile, token);
        }

        public Task DecryptAsync(string fileToDecrypt, string decryptResultFile, CancellationToken token = default)
        {
            return _encriptionModeImpl.DecryptWithModeAsync(fileToDecrypt, decryptResultFile, token);
        }

    }
}
