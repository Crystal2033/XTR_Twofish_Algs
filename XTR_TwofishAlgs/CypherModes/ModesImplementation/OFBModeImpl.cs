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
using XTR_TwofishAlgs.Exceptions;
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

        public override async Task DecryptWithModeAsync(string fileToDecrypt, string decryptResultFile, CancellationToken token)
        {
            FileDataLoader loader = new(fileToDecrypt, decryptResultFile);
            if (loader.TextReadSize % _textBlockSizeInBytes != 0)
            {
                _log.Error($"Text for decryption in {fileToDecrypt} is not compatible. Size % {_textBlockSizeInBytes} != 0.");
                loader.CloseStreams();
                throw new DamagedFileException($"Text for decryption in {fileToDecrypt} is not compatible. Size % {_textBlockSizeInBytes} != 0.");
            }

            int posInTextBlock;
            byte[] prevCypheredPartOfText;
            byte[] cypheredInitVector = _initVector;
            int realPlainTextPartSize = CryptConstants.TWOFISH_PART_TEXT_BYTES;

            while (loader.FactTextBlockSize != 0)
            {
                posInTextBlock = 0;
                while (posInTextBlock < loader.FactTextBlockSize)
                {
                    if (token.IsCancellationRequested)
                    {
                        loader.CloseStreams();
                        return;
                    }
                    prevCypheredPartOfText = GetCryptValue(cypheredInitVector, loader, posInTextBlock, out cypheredInitVector);
                    realPlainTextPartSize = CryptSimpleFunctions.GetPureTextWithoutPaddingSize(ref prevCypheredPartOfText, loader, posInTextBlock);
                    TextBlockOperations.InsertPartInTextBlock(posInTextBlock, prevCypheredPartOfText, realPlainTextPartSize, loader);

                    posInTextBlock += realPlainTextPartSize;
                }
                loader.ReloadTextBlockAndOutputInFile();
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
        public override async Task EncryptWithModeAsync(string fileToEncrypt, string encryptResultFile, CancellationToken token)
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
                    if (token.IsCancellationRequested)
                    {
                        loader.CloseStreams();
                        return;
                    }
                    cypherText = GetCryptValue(cryptedInitVector, loader, curPosInTextBlock, out cryptedInitVector);
                    TextBlockOperations.InsertPartInTextBlock(curPosInTextBlock, cypherText, cypherText.Length, loader);
                    curPosInTextBlock += cypherText.Length;
                }
                loader.ReloadTextBlockAndOutputInFile();
            }
            loader.CloseStreams();
        }
    }
}
