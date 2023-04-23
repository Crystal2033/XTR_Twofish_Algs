using log4net;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using XTR_TWOFISH.CryptInterfaces;
using XTR_TWOFISH.FeistelImplementation;
using XTR_TwofishAlgs.Exceptions;
using XTR_TwofishAlgs.FeistelImplementation;
using XTR_TwofishAlgs.HelpFunctions;
using XTR_TwofishAlgs.KeySchedule;
using XTR_TwofishAlgs.MathBase;
using XTR_TwofishAlgs.MathBase.GaloisField;
using XTR_TwofishAlgs.TwoFish;
using static XTR_TwofishAlgs.HelpFunctions.CryptConstants;
[assembly: log4net.Config.XmlConfigurator(Watch = true)]
internal class Program
{
    private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private static void Main(string[] args)
    {
        //byte[] plainText = new byte[16] { 123, 43, 135, 23, 233, 111, 13, 123, 54, 78, 239, 21, 231, 2, 28, 43 };
        byte[] plainText = new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };


        //byte[] mainKey = new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        //byte[] mainKey = new byte[16] { 123, 43, 135, 23, 233, 111, 13, 123, 54, 78, 239, 21, 231, 2, 28, 43 };
        //byte[] mainKey = new byte[12] { 123, 43, 135, 23, 233, 111, 13, 123, 54, 78, 239, 21 };
        //byte[] mainKey = { 1, 2, 3 };
        //byte[] mainKey = new byte[24] { 0x01, 0x23, 0x45, 0x67, 0x89,
        //    0xab, 0xcd, 0xef, 0xfe, 0xdc, 0xba,
        //    0x98, 0x76, 0x54, 0x32, 0x10, 0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66,0x77};
        //byte[] mainKey = new byte[32] { 0x01, 0x23, 0x45, 0x67, 0x89,
        //    0xab, 0xcd, 0xef, 0xfe, 0xdc, 0xba,
        //    0x98, 0x76, 0x54, 0x32, 0x10, 0x00, 0x11, 0x22,
        //    0x33, 0x44, 0x55, 0x66,0x77, 0x88, 0x99, 0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff };
        byte[] mainKey = new byte[33] {
        0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,
        0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,
        0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,
        0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0, 0x0};

        //byte[] initVector = new byte[8] { 125, 67, 111, 110, 203, 211, 255, 11 };
        IKeyExpansion keyExpansion = new KeyExpansionTwoFish();
        IFeistelFunction feistelFunction = new TwofishFeistelFuncImpl();
        try
        {
            FeistelNetwork feistelKernel = new FeistelNetwork(keyExpansion, feistelFunction) { MainKey = mainKey };


            //for (int i = 1; i < 50; i++)
            //{
            //    var savedColor = Console.ForegroundColor;
            //    Console.ForegroundColor = ConsoleColor.Green;
            //    Console.WriteLine($"{i}");
            //    Console.ForegroundColor = savedColor;
            //    CryptSimpleFunctions.ShowHexView(plainText, "Start PT");
            //    byte[] cipher = feistelKernel.Execute(plainText, XTR_TWOFISH.CypherEnums.CryptOperation.ENCRYPT);
            //    CryptSimpleFunctions.ShowHexView(cipher, "CT");
            //    plainText = feistelKernel.Execute(cipher, XTR_TWOFISH.CypherEnums.CryptOperation.DECRYPT);
            //    CryptSimpleFunctions.ShowHexView(plainText, "Result PT");
            //    plainText = cipher;
            //}
        }
        catch(GaloisOutOfFieldException ex)
        {
            _log.ErrorFormat("Message: {0}\nStackTrace: {1}\nException: {2}", ex.Message, ex.StackTrace, ex);
        }
        catch (MainKeyException ex)
        {
            _log.ErrorFormat("Message: {0}\nStackTrace: {1}\nException: {2}\n", ex.Message, ex.StackTrace, ex);
        }







    }
}