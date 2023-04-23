using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTR_TWOFISH.CryptInterfaces;
using XTR_TWOFISH.CypherEnums;
using XTR_TWOFISH.HelpFunctionsAndData;
using XTR_TwofishAlgs.HelpFunctions;

namespace XTR_TWOFISH.ThreadingWork
{
    public sealed class RDThread : BaseModeThread
    {
        private readonly byte[] _initVector;
        private long initVectorAsNumber;
        private long _delta;
        private long absWrittenBytes = 0;
        public RDThread(int id, FileDataLoader loader, ISymmetricEncryption algorithm, Barrier barrier, int delta, int textBlockSizeInBytes, byte[] initVector) : base(id, loader, algorithm, barrier, textBlockSizeInBytes)
        {
            _initVector = initVector;
            initVectorAsNumber = GetNumberByBytes(_initVector);
            _delta = delta;
        }


        private long GetNumberByBytes(byte[] bytePresentation)
        {
            return BitConverter.ToInt64(bytePresentation);
        }
        public override void Run(object obj = null)
        {
            CryptOperation cryptOperation = (CryptOperation)obj;
            int posInTextBlock = _threadId * _textBlockSizeInBytes;
            byte[] partOfTextBlock;
            int realCypherPartSize = CryptConstants.TWOFISH_PART_TEXT_BYTES;

            while (_loader.FactTextBlockSize != 0)
            {
                while (posInTextBlock < _loader.FactTextBlockSize)
                {
                    long newCounter = _threadId + absWrittenBytes * ThreadsInfo.VALUE_OF_THREAD + _delta;

                    byte[] newInitVector = CryptSimpleFunctions.ConcatTwoBitParts(BitConverter.GetBytes(newCounter + initVectorAsNumber), 64,
                        BitConverter.GetBytes(newCounter + initVectorAsNumber), 64);

                    byte[] cryptedBytes = CryptSimpleFunctions.GetBytesAfterCryptOperation(CryptOperation.ENCRYPT,
                        ref newInitVector, _algorithm);

                    partOfTextBlock = TextBlockOperations.GetPartOfTextBlock(posInTextBlock, _loader, _textBlockSizeInBytes);
                    

                    byte[] totalCyphredBytes = CryptSimpleFunctions.XorByteArrays(cryptedBytes, partOfTextBlock);

                    if (cryptOperation == CryptOperation.DECRYPT) // checking padding for decryption
                    {
                        realCypherPartSize = CryptSimpleFunctions.GetPureTextWithoutPaddingSize(ref totalCyphredBytes, _loader, posInTextBlock);
                    }

                    TextBlockOperations.InsertPartInTextBlock(posInTextBlock, totalCyphredBytes, realCypherPartSize, _loader);

                    BytesTransformedInBlock++;
                    absWrittenBytes++;
                    posInTextBlock = (BytesTransformedInBlock * ThreadsInfo.VALUE_OF_THREAD + _threadId) * _textBlockSizeInBytes;
                }

                BytesTransformedInBlock = 0;
                posInTextBlock = _threadId * _textBlockSizeInBytes;
                _barrier.SignalAndWait();
            }
        }
    }
}
