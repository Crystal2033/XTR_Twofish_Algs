using XTR_TWOFISH.CryptInterfaces;
using XTR_TWOFISH.FeistelImplementation;
using XTR_TwofishAlgs.HelpFunctions;
using XTR_TwofishAlgs.MathBase.GF256;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        //byte[] plainText = new byte[16] { 123, 43, 135, 23, 233, 111, 13, 123, 54, 78, 239, 21, 231, 2, 28, 43 };
        byte[] plainText = new byte[16] { 0,0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        byte[] mainKey = new byte[7] { 123, 43, 135, 23, 233, 231, 23 };
        byte[] initVector = new byte[8] { 125, 67, 111, 110, 203, 211, 255, 11 };
        IKeyExpansion keyExpansion = null;
        IFeistelFunction feistelFunction = null;

        FeistelNetwork feistelKernel = new FeistelNetwork(keyExpansion, feistelFunction);
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