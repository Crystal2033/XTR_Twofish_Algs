using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XTR_TWOFISH.CryptInterfaces;
using XTR_TWOFISH.CypherEnums;
using XTR_TWOFISH.CypherModes;
using XTR_TwofishAlgs.HelpFunctions;

namespace XTR_TWOFISH.Presenters
{
    
    internal sealed class DemonstrationCypher
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ISymmetricEncryption _symmetricImplementation;
        public DemonstrationCypher(ISymmetricEncryption dESImplementation)
        {
            _symmetricImplementation = dESImplementation;
        }

        private bool isCorrectDecryption(string startFile, string decryptedFile)
        {
            return false;
        }

        public static bool CheckFilesEquals(string first, string second)
        {
            byte[] buffer1 = new byte[2048];
            byte[] buffer2 = new byte[2048];
            using (FileStream mainFileStream = File.OpenRead(first))
            using (FileStream decryptedFileStream = File.OpenRead(second))
            {
                if(mainFileStream.Length != decryptedFileStream.Length)
                {
                    return false;
                }
                else
                {
                    while (mainFileStream.Read(buffer1, 0, buffer1.Length) != 0)
                    {
                        decryptedFileStream.Read(buffer2, 0, buffer2.Length);
                        if(!buffer1.SequenceEqual(buffer2))
                        {
                            return false;
                        }
                    }
                }
                
            }
            return true;
        }

        public static async Task DemonstrateMode(string inFile, string encryptFile, string decryptFile, CypherMode mode, byte[] mainKey, byte[] initVector=null, params object[] optionalParams)
        {

            AdvancedCypherSym advancedCypherSym = new(mainKey, mode, CypherEnums.SymmetricAlgorithm.TWOFISH, initVector);
            var watch = System.Diagnostics.Stopwatch.StartNew();
            // the code that you want to measure comes here
            
            await advancedCypherSym.EncryptAsync(inFile, encryptFile);
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine(elapsedMs.ToString(), " ms");

            Console.WriteLine($"EncryptAsync {mode} is done");

            await advancedCypherSym.DecryptAsync(encryptFile, decryptFile);

            Console.WriteLine($"DecryptAsync {mode} is done");

            if (!CheckFilesEquals(inFile, decryptFile))
            {
                var tmp = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Cyphering failed. Files are not equal!");
                Console.ForegroundColor = tmp;
            }
            else
            {
                var tmp = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Success cyphering! Files are equal!");
                Console.ForegroundColor = tmp;
            }
        }
    }
}
