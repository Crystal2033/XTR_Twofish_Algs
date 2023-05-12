using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XTR_TWOFISH.CryptInterfaces;
using XTR_TWOFISH.CypherEnums;
using XTR_TWOFISH.HelpFunctionsAndData;
using XTR_TWOFISH.ThreadingWork;
using XTR_TwofishAlgs.Exceptions;
using XTR_TwofishAlgs.HelpFunctions;

namespace XTR_TWOFISH.CypherModes.ModesImplementation
{
    internal class CFBModeImpl : CryptModeImpl
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private byte[] _initVector;
        private byte[] _prevEncryptedTextBlock;
        private byte[] _currEncryptedTextBlock;

        public CFBModeImpl(byte[] mainKey, ISymmetricEncryption algorithm, int textBlockSizeInBytes, byte[] initVector) : base(mainKey, algorithm, textBlockSizeInBytes)
        {
            _initVector = initVector;
        }
        public override async Task DecryptWithModeAsync(string fileToDecrypt, string decryptResultFile, CancellationToken token)
        {
            FileDataLoader loader = new(fileToDecrypt, decryptResultFile);
            _prevEncryptedTextBlock = (byte[])loader.TextBlock.Clone();
            _currEncryptedTextBlock = (byte[])loader.TextBlock.Clone();

            if (loader.TextReadSize % _textBlockSizeInBytes != 0)
            {
                _log.Error($"Text for decryption in {fileToDecrypt} is not compatible. Size % {_textBlockSizeInBytes} != 0.");
                loader.CloseStreams();
                throw new DamagedFileException($"Text for decryption in {fileToDecrypt} is not compatible. Size % {_textBlockSizeInBytes} != 0.");
            }


            BaseModeThread[] cfbThreads = new CFBDecryptThread[ThreadsInfo.VALUE_OF_THREAD];

            Barrier barrier = new Barrier(ThreadsInfo.VALUE_OF_THREAD, (bar) =>
            {
                if (token.IsCancellationRequested)
                {
                    loader.CloseStreams();
                    return;
                }
                _prevEncryptedTextBlock = (byte[])_currEncryptedTextBlock.Clone();
                loader.ReloadTextBlockAndOutputInFile();
                _currEncryptedTextBlock = (byte[])loader.TextBlock.Clone();
                for (int i = 0; i < ThreadsInfo.VALUE_OF_THREAD; i++)
                {
                    ((CFBDecryptThread)cfbThreads[i]).SetNewPrevAndCurrentCypheredTextBlocks(_prevEncryptedTextBlock, _currEncryptedTextBlock);
                }

                if (loader.FactTextBlockSize == 0) // There is nothing to read
                {
                    for (int i = 0; i < ThreadsInfo.VALUE_OF_THREAD; i++)
                    {
                        cfbThreads[i].SetThreadToStartPosition();
                    }
                }
            });

            List<Task> tasks = new List<Task>();

            for (int i = 0; i < ThreadsInfo.VALUE_OF_THREAD; i++)
            {
                cfbThreads[i] = new CFBDecryptThread(i, loader, _cryptAlgorithm, barrier, _textBlockSizeInBytes, _initVector, _prevEncryptedTextBlock, _currEncryptedTextBlock);
            }

            for (int i = 0; i < ThreadsInfo.VALUE_OF_THREAD; i++)
            {
                var task = cfbThreads[i];
                tasks.Add(Task.Run(() =>
                {
                    task.Run(CryptOperation.DECRYPT);
                }));
            }

            Task.WaitAll(tasks.ToArray());
            loader.CloseStreams();

        }

        public override async Task EncryptWithModeAsync(string fileToEncrypt, string encryptResultFile, CancellationToken token)
        {
            FileDataLoader loader = new(fileToEncrypt, encryptResultFile);
            int curPosInTextBlock;
            byte[] cypherText = _initVector;
            byte[] cryptedInitVector;

            while (loader.FactTextBlockSize != 0)
            {
                curPosInTextBlock = 0;
                while (curPosInTextBlock < loader.FactTextBlockSize)
                {
                    cypherText = GetEncryptValue(cypherText, loader, curPosInTextBlock);
                    TextBlockOperations.InsertPartInTextBlock(curPosInTextBlock, cypherText, cypherText.Length, loader);
                    curPosInTextBlock += cypherText.Length;
                }
                loader.ReloadTextBlockAndOutputInFile();
            }
            loader.CloseStreams();
        }

        private byte[] GetEncryptValue(byte[] bytesToCrypt, FileDataLoader loader, int curPosInText)
        {
            byte[] cryptedInitValue;
            byte[] partOfTextBlock;

            cryptedInitValue = CryptSimpleFunctions.GetBytesAfterCryptOperation(CryptOperation.ENCRYPT, ref bytesToCrypt, _cryptAlgorithm);
            
            partOfTextBlock = TextBlockOperations.GetPartOfTextBlock(curPosInText, loader, _textBlockSizeInBytes);

            return CryptSimpleFunctions.XorByteArrays(partOfTextBlock, cryptedInitValue);
        }
        
    }
}
