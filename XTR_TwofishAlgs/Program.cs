using XTR_TWOFISH.CryptInterfaces;
using XTR_TWOFISH.FeistelImplementation;
using XTR_TwofishAlgs.FeistelImplementation;
using XTR_TwofishAlgs.HelpFunctions;
using XTR_TwofishAlgs.KeySchedule;
using XTR_TwofishAlgs.MathBase.GaloisField;
using static XTR_TwofishAlgs.HelpFunctions.CryptConstants;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        //byte[] plainText = new byte[16] { 123, 43, 135, 23, 233, 111, 13, 123, 54, 78, 239, 21, 231, 2, 28, 43 };
        byte[] plainText = new byte[16] { 0,0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        //byte[] mainKey = new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        byte[] mainKey = new byte[16] { 123, 43, 135, 23, 233, 111, 13, 123, 54, 78, 239, 21, 231, 2, 28, 43 };
        byte[] initVector = new byte[8] { 125, 67, 111, 110, 203, 211, 255, 11 };
        IKeyExpansion keyExpansion = new KeyExpansionTwoFish();
        IFeistelFunction feistelFunction = new TwofishFeistelFuncImpl();

        FeistelNetwork feistelKernel = new FeistelNetwork(keyExpansion, feistelFunction, TwoFishKeySizes.EASY) { MainKey = mainKey};
        feistelKernel.Execute(plainText, 128, XTR_TWOFISH.CypherEnums.CryptOperation.ENCRYPT);
        //feistelKernel.Execute(plainText, 128, XTR_TWOFISH.CypherEnums.CryptOperation.ENCRYPT);
        //ISymmetricEncryption desImpl = new DESImplementation(feistelKernel);
        CryptSimpleFunctions.ShowBinaryView(mainKey, "Mainkey before");
        CryptSimpleFunctions.ShowBinaryView(CryptSimpleFunctions.CycleRightShift(mainKey, 7 * 8, 7*4), "MainKey after");

        //GF256 a = 87;
        //GF256 b = 131;
        //CryptSimpleFunctions.ShowBinaryView(a.Value, "A= ");
        //CryptSimpleFunctions.ShowBinaryView(b.Value, "B= ");
        //CryptSimpleFunctions.ShowBinaryView((a + b).Value, "A + B= ");
        //CryptSimpleFunctions.ShowBinaryView((a * b).Value, "A * B= ");
        //CryptSimpleFunctions.ShowBinaryView(GF256.Mult(a, b, IrreduciblePolynoms.X4X_1).Value, "(A * B)= mod X^4 + X + 1");

    }
}