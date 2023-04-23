using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTR_TWOFISH.CryptInterfaces;
using XTR_TWOFISH.HelpFunctionsAndData;
using XTR_TwofishAlgs.HelpFunctions;

namespace XTR_TWOFISH.ThreadingWork
{
    internal class CFBDecryptThread : BaseModeThread
    {
        private byte[] _initVector;
        private byte[] _copiedPrevCyphredTextBlock;
        private byte[] _copiedCurrCyphredTextBlock;
        private int writtenTextBlocks = 0;
        public CFBDecryptThread(int id, FileDataLoader loader, ISymmetricEncryption algorithm, Barrier barrier, int textBlockSizeInBytes,
            byte[] initVector, byte[] copiedPrevCyphredTextBlock, byte[] copiedCurrCyphredTextBlock)
            : base(id, loader, algorithm, barrier, textBlockSizeInBytes)
        {
            _initVector = initVector;
            SetNewPrevAndCurrentCypheredTextBlocks(copiedPrevCyphredTextBlock, copiedCurrCyphredTextBlock);
        }


        public override void Run(object obj = null)
        {
            int posInTextBlock = _threadId * _textBlockSizeInBytes;
            int realPlainTextPartSize = CryptConstants.TWOFISH_PART_TEXT_BYTES;
            //byte[] partOfTextBlock;
            byte[] plainPartOfText;
            byte[] prevCypheredPartOfText = _initVector;

            while (_loader.FactTextBlockSize != 0)
            {
                while (posInTextBlock < _loader.FactTextBlockSize)
                {
                    prevCypheredPartOfText = GetPrevCypherText(posInTextBlock);
                    plainPartOfText = GetDecryptValue(prevCypheredPartOfText, _loader, posInTextBlock, out prevCypheredPartOfText);

                    realPlainTextPartSize = CryptSimpleFunctions.GetPureTextWithoutPaddingSize(ref plainPartOfText, _loader);
                    TextBlockOperations.InsertPartInTextBlock(posInTextBlock, plainPartOfText, realPlainTextPartSize, _loader);

                    BytesTransformedInBlock++;
                    posInTextBlock = (BytesTransformedInBlock * ThreadsInfo.VALUE_OF_THREAD + _threadId) * _textBlockSizeInBytes;
                }

                BytesTransformedInBlock = 0;
                posInTextBlock = _threadId * _textBlockSizeInBytes;
                writtenTextBlocks++;
                _barrier.SignalAndWait();
            }
        }

        private byte[] GetPrevCypherText(int posInTextBlock)
        {
            if (_threadId == 0 && writtenTextBlocks == 0 && posInTextBlock == 0) //its initial case, need to take init vector. The start of decrypting
            {
                return _initVector;
            }
            else if (_threadId == 0 && posInTextBlock == 0) //need to take prev part of block from prev textBlock
            {
                return TextBlockOperations.GetPartOfTextBlockWithoutPadding(
                    FileDataLoader.TextBlockSize - CryptConstants.TWOFISH_PART_TEXT_BYTES, _copiedPrevCyphredTextBlock, _textBlockSizeInBytes);
            }
            else // Normal case, need to take prev cypher block
            {
                return TextBlockOperations.GetPartOfTextBlockWithoutPadding(
                    posInTextBlock - CryptConstants.TWOFISH_PART_TEXT_BYTES, _copiedCurrCyphredTextBlock, _textBlockSizeInBytes);
            }
        }

        private byte[] GetDecryptValue(byte[] bytesToCrypt, FileDataLoader loader, int curPosInText, out byte[] prevCypheredPartOfText)
        {
            byte[] cryptedInitValue;
            byte[] cryptedPartOfText;

            cryptedInitValue = CryptSimpleFunctions.GetBytesAfterCryptOperation(CypherEnums.CryptOperation.ENCRYPT, ref bytesToCrypt, _algorithm);

            cryptedPartOfText = TextBlockOperations.GetPartOfTextBlock(curPosInText, loader, _textBlockSizeInBytes);
            prevCypheredPartOfText = (byte[])cryptedPartOfText.Clone();

            return CryptSimpleFunctions.XorByteArrays(cryptedPartOfText, cryptedInitValue);
        }


        public void SetNewPrevAndCurrentCypheredTextBlocks(byte[] copiedPrevCyphredTextBlock, byte[] copiedCurrCyphredTextBlock)
        {
            _copiedPrevCyphredTextBlock = copiedPrevCyphredTextBlock;
            _copiedCurrCyphredTextBlock = copiedCurrCyphredTextBlock;
        }
    }
}
