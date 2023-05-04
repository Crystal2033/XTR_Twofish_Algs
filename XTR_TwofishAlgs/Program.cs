using log4net;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography;
using XTR_TWOFISH.CryptInterfaces;
using XTR_TWOFISH.CypherModes;
using XTR_TWOFISH.FeistelImplementation;
using XTR_TWOFISH.Presenters;
using XTR_TwofishAlgs.Exceptions;
using XTR_TwofishAlgs.FeistelImplementation;
using XTR_TwofishAlgs.HelpFunctions;
using XTR_TwofishAlgs.KeySchedule;
using XTR_TwofishAlgs.MathBase;
using XTR_TwofishAlgs.MathBase.GaloisField;
using XTR_TwofishAlgs.TwoFish;
using XTR_TwofishAlgs.XTR;
using static XTR_TwofishAlgs.HelpFunctions.CryptConstants;
[assembly: log4net.Config.XmlConfigurator(Watch = true)]
internal class Program
{
    private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    //private static void Main(string[] args)
    //{
    //    //byte[] plainText = new byte[16] { 123, 43, 135, 23, 233, 111, 13, 123, 54, 78, 239, 21, 231, 2, 28, 43 };
    //    byte[] plainText = new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };


    //    //byte[] mainKey = new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    //    //byte[] mainKey = new byte[16] { 123, 43, 135, 23, 233, 111, 13, 123, 54, 78, 239, 21, 231, 2, 28, 43 };
    //    //byte[] mainKey = new byte[12] { 123, 43, 135, 23, 233, 111, 13, 123, 54, 78, 239, 21 };
    //    //byte[] mainKey = { 1, 2, 3 };
    //    //byte[] mainKey = new byte[24] { 0x01, 0x23, 0x45, 0x67, 0x89,
    //    //    0xab, 0xcd, 0xef, 0xfe, 0xdc, 0xba,
    //    //    0x98, 0x76, 0x54, 0x32, 0x10, 0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66,0x77};
    //    byte[] mainKey = new byte[32] { 0x01, 0x23, 0x45, 0x67, 0x89,
    //        0xab, 0xcd, 0xef, 0xfe, 0xdc, 0xba,
    //        0x98, 0x76, 0x54, 0x32, 0x10, 0x00, 0x11, 0x22,
    //        0x33, 0x44, 0x55, 0x66,0x77, 0x88, 0x99, 0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff };
    //    //byte[] mainKey = new byte[32] {
    //    //0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,
    //    //0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,
    //    //0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0,
    //    //0x0,0x0,0x0,0x0,0x0,0x0,0x0,0x0};

    //    byte[] initVector = new byte[16] { 125, 67, 111, 110, 203, 211, 255, 11, 125, 67, 111, 110, 203, 211, 255, 11 };
    //    IKeyExpansion keyExpansion = new KeyExpansionTwoFish();
    //    IFeistelFunction feistelFunction = new TwofishFeistelFuncImpl();
    //    try
    //    {
    //        string MAINPATH = @"D:\Paul\Programming\C#\XTR_Twofish\XTR_TwofishAlgs\XTR_TwofishAlgs\Data\";
    //        string CHILDPATH = @"Video\";
    //        string FILENAME = @"TestVideo.mp4";
    //        FeistelNetwork feistelKernel = new FeistelNetwork(keyExpansion, feistelFunction) { MainKey = mainKey };


    //        DemonstrationCypher.DemonstrateMode(MAINPATH + CHILDPATH + FILENAME, MAINPATH + CHILDPATH + @"EncryptECB.mp4",
    //            MAINPATH + CHILDPATH + @"DecryptECB.mp4", XTR_TWOFISH.CypherEnums.CypherMode.ECB, mainKey, initVector).Wait();

    //        DemonstrationCypher.DemonstrateMode(MAINPATH + CHILDPATH + FILENAME, MAINPATH + CHILDPATH + @"EncryptCBC.mp4",
    //            MAINPATH + CHILDPATH + @"DecryptCBC.mp4", XTR_TWOFISH.CypherEnums.CypherMode.CBC, mainKey, initVector).Wait();

    //        DemonstrationCypher.DemonstrateMode(MAINPATH + CHILDPATH + FILENAME, MAINPATH + CHILDPATH + @"EncryptCFB.mp4",
    //            MAINPATH + CHILDPATH + @"DecryptCFB.mp4", XTR_TWOFISH.CypherEnums.CypherMode.CFB, mainKey, initVector).Wait();

    //        DemonstrationCypher.DemonstrateMode(MAINPATH + CHILDPATH + FILENAME, MAINPATH + CHILDPATH + @"EncryptOFB.mp4",
    //            MAINPATH + CHILDPATH + @"DecryptOFB.mp4", XTR_TWOFISH.CypherEnums.CypherMode.OFB, mainKey, initVector).Wait();

    //        DemonstrationCypher.DemonstrateMode(MAINPATH + CHILDPATH + FILENAME, MAINPATH + CHILDPATH + @"EncryptCTR.mp4",
    //            MAINPATH + CHILDPATH + @"DecryptCTR.mp4", XTR_TWOFISH.CypherEnums.CypherMode.CTR, mainKey, initVector).Wait();

    //        DemonstrationCypher.DemonstrateMode(MAINPATH + CHILDPATH + FILENAME, MAINPATH + CHILDPATH + @"EncryptRD.mp4",
    //            MAINPATH + CHILDPATH + @"DecryptRD.mp4", XTR_TWOFISH.CypherEnums.CypherMode.RD, mainKey, initVector).Wait();

    //        DemonstrationCypher.DemonstrateMode(MAINPATH + CHILDPATH + FILENAME, MAINPATH + CHILDPATH + @"EncryptRDH.mp4",
    //            MAINPATH + CHILDPATH + @"DecryptRDH.mp4", XTR_TWOFISH.CypherEnums.CypherMode.RDH, mainKey, initVector).Wait();

    //        //byte[] cipher = feistelKernel.Execute(plainText, XTR_TWOFISH.CypherEnums.CryptOperation.ENCRYPT);
    //        //CryptSimpleFunctions.ShowHexView(cipher, "CT");
    //        //plainText = feistelKernel.Execute(cipher, XTR_TWOFISH.CypherEnums.CryptOperation.DECRYPT);
    //        //CryptSimpleFunctions.ShowHexView(plainText, "PT");

    //        //for (int i = 1; i < 50; i++)
    //        //{
    //        //    var savedColor = Console.ForegroundColor;
    //        //    Console.ForegroundColor = ConsoleColor.Green;
    //        //    Console.WriteLine($"{i}");
    //        //    Console.ForegroundColor = savedColor;
    //        //    CryptSimpleFunctions.ShowHexView(plainText, "Start PT");
    //        //    byte[] cipher = feistelKernel.Execute(plainText, XTR_TWOFISH.CypherEnums.CryptOperation.ENCRYPT);
    //        //    CryptSimpleFunctions.ShowHexView(cipher, "CT");
    //        //    plainText = feistelKernel.Execute(cipher, XTR_TWOFISH.CypherEnums.CryptOperation.DECRYPT);
    //        //    CryptSimpleFunctions.ShowHexView(plainText, "Result PT");
    //        //    plainText = cipher;
    //        //}
    //    }
    //    catch(GaloisOutOfFieldException ex)
    //    {
    //        _log.ErrorFormat("Message: {0}\nStackTrace: {1}\nException: {2}", ex.Message, ex.StackTrace, ex);
    //    }
    //    catch (MainKeyException ex)
    //    {
    //        _log.ErrorFormat("Message: {0}\nStackTrace: {1}\nException: {2}\n", ex.Message, ex.StackTrace, ex);
    //    }
    //}


    private static void Main(string[] args)
    {
        //(int nod, (int x, int y)) = Euclid.ExtendedGcd(a, b);
        //_log.Info($"{a} * {x} + {b} * {y} = {nod}");
        //GFP2 galoisFieldP2 = new GFP2(11);
        //var tmp = galoisFieldP2.Values[100];
        //GFP2.Polynom1DegreeCoeffs valInGFP2 = tmp;

        //_log.Info($"({valInGFP2.First},{valInGFP2.Second})");
        //for (int i = 0; i < galoisFieldP2.Primary - 1; i++)
        //{
        //    valInGFP2 = galoisFieldP2.Mult(valInGFP2, tmp);
        //}
        //_log.Info($"POW=({valInGFP2.First},{valInGFP2.Second})");

        //var powP = galoisFieldP2.PowP(tmp);
        //_log.Info($"POW=({powP.First},{powP.Second})");

        BigInteger a1 = BigInteger.Parse("1298074214633706835075030044377081");
        BigInteger a2 = BigInteger.Parse("123123213124141451515151");

        bool isPrime = StandartMathTricks.FermaTest(a1, 10);
        if (isPrime)
        {
            _log.Info($"The {a1} is prime.");
        }
        else
        {
            _log.Info($"The {a1} is NOT prime.");
        }

        //GFP2 galoisFieldP2 = new GFP2(BigInteger.Parse("92233720368547758071231254152563463634"));
        //XTRFunctions xtrFunctions = new(galoisFieldP2.Primary, BigInteger.Parse("9223352563463634"));
        //GFP2.Polynom1DegreeCoeffs trace = xtrFunctions.GenerateTrace().Second;

        //for (int a = 1; a < 2; a++)
        //{
        //    for (int b = 1; b < 2; b++)
        //    {
        //        GFP2.Polynom1DegreeCoeffs alicesTracePowA = xtrFunctions.SFunction(a, trace).Second;
        //        GFP2.Polynom1DegreeCoeffs bobTracePowB = xtrFunctions.SFunction(b, trace).Second;
        //        //_log.Info($"({alicesTracePowA.First}, {alicesTracePowA.Second})");
        //        //_log.Info($"({bobTracePowB.First}, {bobTracePowB.Second})");

        //        GFP2.Polynom1DegreeCoeffs keyBobs = xtrFunctions.SFunction(b, alicesTracePowA).Second;
        //        GFP2.Polynom1DegreeCoeffs keyAlices = xtrFunctions.SFunction(a, bobTracePowB).Second;
        //        _log.Info($"({keyBobs.First}, {keyBobs.Second})");
        //        _log.Info($"({keyAlices.First}, {keyAlices.Second})");
        //        if (keyBobs.First != keyAlices.First || keyBobs.Second != keyAlices.Second)
        //        {
        //            //_log.Info($"({keyBobs.First}, {keyBobs.Second})");
        //            //_log.Info($"({keyAlices.First}, {keyAlices.Second})");
        //            _log.Info($"Fuck");
        //        }
        //    }
        //}







        //for(int i = 0; i < 1000; i++)
        //{
        //    _log.Info(i);
        //    var result = xtrFunctions.SFunction(i, galoisFieldP2.Values[50]);
        //    _log.Info($"({result.Second.First}, {result.Second.Second})");
        //}
    }
}