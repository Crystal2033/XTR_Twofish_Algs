using log4net.Repository.Hierarchy;
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
    public sealed class ECBThread : BaseModeThread
    {
        public ECBThread(int id, FileDataLoader loader, ISymmetricEncryption algorithm, Barrier barrier, int textBlockSizeInBytes) : base(id, loader, algorithm, barrier, textBlockSizeInBytes)
        { }   

        public override void Run(object obj)
        {
            CryptOperation cryptOperation = (CryptOperation)obj;
            long posInTextBlock = _threadId * _textBlockSizeInBytes;
            long realCypherPartSize = _textBlockSizeInBytes;
            while (_loader.FactTextBlockSize != 0)
            { 
                while (posInTextBlock < _loader.FactTextBlockSize)
                {
                    byte[] partOfTextBlock = TextBlockOperations.GetPartOfTextBlock(posInTextBlock, _loader, _textBlockSizeInBytes);

                    byte[] newBytes = CryptSimpleFunctions.GetBytesAfterCryptOperation(cryptOperation, ref partOfTextBlock, _algorithm);
                    
                    if (cryptOperation == CryptOperation.DECRYPT) // checking padding for decryption
                    {
                        realCypherPartSize = CryptSimpleFunctions.GetPureTextWithoutPaddingSize(ref newBytes, _loader, posInTextBlock);
                    }

                    TextBlockOperations.InsertPartInTextBlock(posInTextBlock, newBytes, realCypherPartSize, _loader);
                    
                    BytesTransformedInBlock++;
                    posInTextBlock = (BytesTransformedInBlock * ThreadsInfo.VALUE_OF_THREAD + _threadId) * _textBlockSizeInBytes;
                }

                BytesTransformedInBlock = 0;
                posInTextBlock = _threadId * _textBlockSizeInBytes;
                _barrier.SignalAndWait();
            }
        }
    }
}
