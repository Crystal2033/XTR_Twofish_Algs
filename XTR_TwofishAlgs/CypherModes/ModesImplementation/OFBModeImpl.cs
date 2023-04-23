using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XTR_TWOFISH.CryptInterfaces;
using XTR_TWOFISH.CypherEnums;
using XTR_TWOFISH.HelpFunctionsAndData;
using XTR_TWOFISH.ThreadingWork;
using XTR_TwofishAlgs.HelpFunctions;

namespace XTR_TWOFISH.CypherModes.ModesImplementation
{
    public sealed class OFBModeImpl : CryptModeImpl
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private byte[] _initVector;

        public OFBModeImpl(byte[] mainKey, ISymmetricEncryption algorithm, int textBlockSizeInBytes, byte[] initVector) : base(mainKey, algorithm, textBlockSizeInBytes)
        {
            _initVector = initVector;
        }

        public override async Task DecryptWithMode(string fileToDecrypt, string decryptResultFile)
        {
            FileDataLoader loader = new(fileToDecrypt, decryptResultFile);
            int posInTextBlock;
            byte[] prevCypheredPartOfText;
            byte[] cypheredInitVector = _initVector;
            int realPlainTextPartSize = CryptConstants.TWOFISH_PART_TEXT_BYTES;

            while (loader.FactTextBlockSize != 0)
            {
                posInTextBlock = 0;
                while (posInTextBlock < loader.FactTextBlockSize)
                {
                    prevCypheredPartOfText = GetCryptValue(cypheredInitVector, loader, posInTextBlock, out cypheredInitVector);
                    realPlainTextPartSize = CryptSimpleFunctions.GetPureTextWithoutPaddingSize(ref prevCypheredPartOfText, loader, posInTextBlock);
                    TextBlockOperations.InsertPartInTextBlock(posInTextBlock, prevCypheredPartOfText, realPlainTextPartSize, loader);

                    posInTextBlock += realPlainTextPartSize;
                }
                loader.reloadTextBlockAndOutputInFile();
            }
            loader.CloseStreams();
        }

        private byte[] GetCryptValue(byte[] bytesToCrypt, FileDataLoader loader, int curPosInText, out byte[] nextCryptInitValue)
        {
            byte[] cryptedInitValue;
            byte[] partOfTextBlock;

            cryptedInitValue = CryptSimpleFunctions.GetBytesAfterCryptOperation(CryptOperation.ENCRYPT, ref bytesToCrypt, _cryptAlgorithm);
            nextCryptInitValue = cryptedInitValue;

            partOfTextBlock = TextBlockOperations.GetPartOfTextBlock(curPosInText, loader, _textBlockSizeInBytes);

            return CryptSimpleFunctions.XorByteArrays(partOfTextBlock, cryptedInitValue);
        }
        public override async Task EncryptWithMode(string fileToEncrypt, string encryptResultFile)
        {
            FileDataLoader loader = new(fileToEncrypt, encryptResultFile);
            int curPosInTextBlock;
            byte[] cypherText;
            byte[] cryptedInitVector = _initVector;

            while (loader.FactTextBlockSize != 0)
            {
                curPosInTextBlock = 0;
                while (curPosInTextBlock < loader.FactTextBlockSize)
                {
                    cypherText = GetCryptValue(cryptedInitVector, loader, curPosInTextBlock, out cryptedInitVector);
                    TextBlockOperations.InsertPartInTextBlock(curPosInTextBlock, cypherText, cypherText.Length, loader);
                    curPosInTextBlock += cypherText.Length;
                }
                loader.reloadTextBlockAndOutputInFile();
            }
            loader.CloseStreams();
        }
    }
}
