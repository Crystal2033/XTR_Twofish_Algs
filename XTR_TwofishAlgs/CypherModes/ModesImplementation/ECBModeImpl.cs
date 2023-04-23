using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XTR_TWOFISH.CryptInterfaces;
using XTR_TWOFISH.CypherEnums;
using XTR_TWOFISH.ThreadingWork;

namespace XTR_TWOFISH.CypherModes.ModesImplementation
{
    public class ECBModeImpl : CryptModeImpl
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ECBModeImpl(byte[] mainKey, ISymmetricEncryption cryptAlgorithm, int textBlockSizeInBytes) : base(mainKey, cryptAlgorithm, textBlockSizeInBytes)
        {

        }
        public override async Task DecryptWithMode(string fileToDecrypt, string decryptResultFile)
        {
            await Execute(fileToDecrypt, decryptResultFile, CryptOperation.DECRYPT);
        }

        public override async Task EncryptWithMode(string fileToEncrypt, string encryptResultFile)
        {
            await Execute(fileToEncrypt, encryptResultFile, CryptOperation.ENCRYPT);
        }

        private async Task Execute(string inputFile, string outputFile, CryptOperation cryptOperation)
        {
            FileDataLoader loader = new(inputFile, outputFile);
            if(cryptOperation == CryptOperation.DECRYPT)
            {
                if(loader.TextReadSize % _textBlockSizeInBytes != 0)
                {
                    _log.Error($"Text for decryption in {inputFile} is not compatible. Size % {_textBlockSizeInBytes} != 0.");
                    loader.CloseStreams();
                    return;
                }
            }

            BaseModeThread[] ecbThreads = new ECBThread[ThreadsInfo.VALUE_OF_THREAD];
            

            Barrier barrier = new Barrier(ThreadsInfo.VALUE_OF_THREAD, (bar) =>
            {
                loader.reloadTextBlockAndOutputInFile();

                if (loader.FactTextBlockSize == 0) // There is nothing to read
                {

                    for (int i = 0; i < ThreadsInfo.VALUE_OF_THREAD; i++)
                    {
                        ecbThreads[i].SetThreadToStartPosition();
                    }
                    //Wake up main thread
                }
            });

            List<Task> tasks = new List<Task>();

            for (int i = 0; i < ThreadsInfo.VALUE_OF_THREAD; i++)
            {
                ecbThreads[i] = new ECBThread(i, loader, _cryptAlgorithm, barrier, _textBlockSizeInBytes);
                
            }
            for (int i = 0; i < ThreadsInfo.VALUE_OF_THREAD; i++)
            {
                var task = ecbThreads[i];
                tasks.Add(Task.Run(() =>
                {
                    
                    task.Run(cryptOperation);
                }));
            }

            Task.WaitAll(tasks.ToArray());
            loader.CloseStreams();

        }
    }
}
