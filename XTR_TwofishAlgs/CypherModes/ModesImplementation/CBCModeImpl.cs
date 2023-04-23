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
    public sealed class CBCModeImpl : CryptModeImpl
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private byte[] _initVector;
        private byte[] _prevEncryptedTextBlock;
        private byte[] _currEncryptedTextBlock;
        public CBCModeImpl(byte[] mainKey, ISymmetricEncryption algorithm, int textBlockSizeInBytes, byte[] initVector) : base(mainKey, algorithm, textBlockSizeInBytes)
        {
            _initVector = initVector;
        }

        public override async Task DecryptWithMode(string fileToDecrypt, string decryptResultFile)
        {
            FileDataLoader loader = new(fileToDecrypt, decryptResultFile);
            _prevEncryptedTextBlock = (byte[])loader.TextBlock.Clone();
            _currEncryptedTextBlock = (byte[])loader.TextBlock.Clone();

            if (loader.TextReadSize % _textBlockSizeInBytes != 0)
            {
                _log.Error($"Text for decryption in {fileToDecrypt} is not compatible. Size % {_textBlockSizeInBytes} != 0.");
                loader.CloseStreams();
                return;
            }


            BaseModeThread[] cbcThreads = new CBCDecryptThread[ThreadsInfo.VALUE_OF_THREAD];

            Barrier barrier = new Barrier(ThreadsInfo.VALUE_OF_THREAD, (bar) =>
            {
                _prevEncryptedTextBlock = (byte[])_currEncryptedTextBlock.Clone();
                loader.ReloadTextBlockAndOutputInFile();
                _currEncryptedTextBlock = (byte[])loader.TextBlock.Clone();
                for (int i = 0; i < ThreadsInfo.VALUE_OF_THREAD; i++)
                {
                    ((CBCDecryptThread)cbcThreads[i]).SetNewPrevAndCurrentCypheredTextBlocks(_prevEncryptedTextBlock, _currEncryptedTextBlock);
                }

                if (loader.FactTextBlockSize == 0) // There is nothing to read
                {
                    for (int i = 0; i < ThreadsInfo.VALUE_OF_THREAD; i++)
                    {
                        cbcThreads[i].SetThreadToStartPosition();
                    }
                }
            });

            List<Task> tasks = new List<Task>();

            for (int i = 0; i < ThreadsInfo.VALUE_OF_THREAD; i++)
            {
                cbcThreads[i] = new CBCDecryptThread(i, loader, _cryptAlgorithm, barrier, _textBlockSizeInBytes, _initVector, _prevEncryptedTextBlock, _currEncryptedTextBlock);
            }

            for (int i = 0; i < ThreadsInfo.VALUE_OF_THREAD; i++)
            {
                var task = cbcThreads[i];
                tasks.Add(Task.Run(() =>
                {
                    task.Run(CryptOperation.DECRYPT);
                }));
            }

            Task.WaitAll(tasks.ToArray());
            loader.CloseStreams();
        }

        public override async Task EncryptWithMode(string fileToEncrypt, string encryptResultFile)
        {
            FileDataLoader loader = new(fileToEncrypt, encryptResultFile);
            int curPosInTextBlock;
            byte[] partOfTextBlock;
            byte[] xoredMessage;
            byte[] cryptedPartOfText = _initVector;

            while(loader.FactTextBlockSize != 0)
            {
                curPosInTextBlock = 0;
                while (curPosInTextBlock < loader.FactTextBlockSize)
                {
                    partOfTextBlock = TextBlockOperations.GetPartOfTextBlock(curPosInTextBlock, loader, _textBlockSizeInBytes);
                    
                    xoredMessage = CryptSimpleFunctions.XorByteArrays(partOfTextBlock, cryptedPartOfText); //cryptedPartOfText(i-1)

                    cryptedPartOfText = CryptSimpleFunctions.GetBytesAfterCryptOperation(CryptOperation.ENCRYPT, ref xoredMessage, _cryptAlgorithm); //cryptedPartOfText(i)
                    
                    TextBlockOperations.InsertPartInTextBlock(curPosInTextBlock, cryptedPartOfText, cryptedPartOfText.Length, loader);
                    curPosInTextBlock += partOfTextBlock.Length;
                }
                loader.ReloadTextBlockAndOutputInFile();
            }
            loader.CloseStreams();
        }
    }
}
