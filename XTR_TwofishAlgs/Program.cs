using System.Numerics;
using XTR_TWOFISH.CryptInterfaces;
using XTR_TWOFISH.FeistelImplementation;
using XTR_TwofishAlgs.FeistelImplementation;
using XTR_TwofishAlgs.HelpFunctions;
using XTR_TwofishAlgs.KeySchedule;
using XTR_TwofishAlgs.MathBase;
using XTR_TwofishAlgs.MathBase.GaloisField;
using XTR_TwofishAlgs.TwoFish;
using static XTR_TwofishAlgs.HelpFunctions.CryptConstants;

internal class Program
{
    private static void Main(string[] args)
    {
        //byte[] plainText = new byte[16] { 123, 43, 135, 23, 233, 111, 13, 123, 54, 78, 239, 21, 231, 2, 28, 43 };
        byte[] plainText = new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        //byte[] mainKey = new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        //byte[] mainKey = new byte[16] { 123, 43, 135, 23, 233, 111, 13, 123, 54, 78, 239, 21, 231, 2, 28, 43 };
        //byte[] mainKey = new byte[24] { 0x01, 0x23, 0x45, 0x67, 0x89,
        //    0xab, 0xcd, 0xef, 0xfe, 0xdc, 0xba,
        //    0x98, 0x76, 0x54, 0x32, 0x10, 0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66,0x77};
        byte[] mainKey = new byte[32] { 0x01, 0x23, 0x45, 0x67, 0x89,
            0xab, 0xcd, 0xef, 0xfe, 0xdc, 0xba,
            0x98, 0x76, 0x54, 0x32, 0x10, 0x00, 0x11, 0x22,
            0x33, 0x44, 0x55, 0x66,0x77, 0x88, 0x99, 0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff };
        //byte[] initVector = new byte[8] { 125, 67, 111, 110, 203, 211, 255, 11 };
        IKeyExpansion keyExpansion = new KeyExpansionTwoFish();
        IFeistelFunction feistelFunction = new TwofishFeistelFuncImpl();

        FeistelNetwork feistelKernel = new FeistelNetwork(keyExpansion, feistelFunction) { MainKey = mainKey };
        byte[] cipher = feistelKernel.Execute(plainText, XTR_TWOFISH.CypherEnums.CryptOperation.ENCRYPT);
        CryptSimpleFunctions.ShowHexView(cipher);
        byte[] pt = feistelKernel.Execute(cipher, XTR_TWOFISH.CypherEnums.CryptOperation.DECRYPT);
        CryptSimpleFunctions.ShowHexView(pt);



    }
}