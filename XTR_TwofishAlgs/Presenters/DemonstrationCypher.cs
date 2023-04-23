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

        public static void DemonstrateMode(string inFile, string encryptFile, string decryptFile, CypherMode mode, byte[] mainKey, byte[] initVector=null, params object[] optionalParams)
        {

            AdvancedCypherSym advancedCypherSym = new(mainKey, mode, CypherEnums.SymmetricAlgorithm.TWOFISH, initVector);
            Task.Run(() =>
            {
                advancedCypherSym.Encrypt(inFile, encryptFile);
            }).Wait();
            Console.WriteLine($"Encrypt {mode} is done");

            Task.Run(() =>
            {
                advancedCypherSym.Decrypt(encryptFile, decryptFile);
            }).Wait();
            Console.WriteLine($"Decrypt {mode} is done");

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
        //public void encrypt(string userFile, string encryptTo)
        //{
        //    try
        //    {
        //        using (var inStream = File.Open(userFile, FileMode.Open))
        //        using (var outSream = File.Open(encryptTo, FileMode.OpenOrCreate))
        //        {

        //            using (BinaryReader reader = new BinaryReader(inStream, Encoding.UTF8))
        //            using (BinaryWriter writer = new BinaryWriter(outSream, Encoding.UTF8))
        //            {
        //                byte[] textBlock = new byte[2048];
        //                byte[] currentPart = new byte[8];
        //                int writtenBytes;
        //                while ((writtenBytes = reader.Read(textBlock, 0, textBlock.Length)) != 0)
        //                {
        //                    int textPartsCounter = 0;
        //                    while (textPartsCounter * CryptConstants.TWOFISH_PART_TEXT_BYTES < writtenBytes)
        //                    {
        //                        CryptSimpleFunctions.GetNewPartOfText(textBlock, currentPart, textPartsCounter * CryptConstants.TWOFISH_PART_TEXT_BYTES);
        //                        textPartsCounter++;
        //                        byte[] cipher = _symmetricImplementation.Encrypt(ref currentPart);
        //                        writer.Write(cipher);
        //                    }
        //                }

        //            }

        //        }
        //    }
        //    catch (Exception ae)
        //    {
        //        _log.Error(ae);
        //    }
        //}

        //public void decrypt(string encryptedFile, string decryptTo)
        //{
        //    //need to check that in encrypt file is always divided by 8. 1 byte in encrypted file is not correct file value
        //    using (var inStream = File.Open(encryptedFile, FileMode.OpenOrCreate))
        //    using (var outSream = File.Open(decryptTo, FileMode.OpenOrCreate))
        //    {

        //        using (BinaryReader reader = new BinaryReader(inStream, Encoding.UTF8))
        //        using (BinaryWriter writer = new BinaryWriter(outSream, Encoding.UTF8))
        //        {
        //            byte[] cryptedTextBlock = new byte[2048];
        //            byte[] currentPart = new byte[8];
        //            if(inStream.Length % CryptConstants.BITS_IN_BYTE != 0){
        //                Console.WriteLine($"Encryption file error! Size is not divide by 8. Length = {inStream.Length}");
        //            }

        //            int writtenBytes;
        //            while ((writtenBytes = reader.Read(cryptedTextBlock, 0, cryptedTextBlock.Length)) != 0)
        //            {
        //                int textPartsCounter = 0;
        //                while (textPartsCounter * CryptConstants.DES_PART_TEXT_BYTES < writtenBytes)
        //                {
        //                    CryptSimpleFunctions.GetNewPartOfText(cryptedTextBlock, currentPart, textPartsCounter * CryptConstants.DES_PART_TEXT_BYTES);
        //                    textPartsCounter++;
        //                    byte[] plainText = _symmetricImplementation.Decrypt(ref currentPart);
        //                    int remainedBytes = cryptedTextBlock.Length - textPartsCounter * CryptConstants.DES_PART_TEXT_BYTES;
        //                    if (remainedBytes < 0)
        //                    {
        //                        CryptSimpleFunctions.ClearBytes(ref plainText, CryptConstants.DES_PART_TEXT_BYTES - Math.Abs(remainedBytes));
        //                    }
        //                    writer.Write(plainText);
        //                }
        //            }

        //        }

        //    }


            
        //}
    }
}
