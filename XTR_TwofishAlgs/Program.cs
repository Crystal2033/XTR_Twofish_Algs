using XTR_TWOFISH.CryptInterfaces;
using XTR_TWOFISH.FeistelImplementation;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        byte[] plainText = new byte[16] { 123, 43, 135, 23, 233, 111, 13, 123, 54, 78, 239, 21, 231, 2, 28, 43 };
        byte[] mainKey = new byte[7] { 123, 43, 135, 23, 233, 231, 23 };
        byte[] initVector = new byte[8] { 125, 67, 111, 110, 203, 211, 255, 11 };
        IKeyExpansion keyExpansion = null;
        IFeistelFunction feistelFunction = null;

        FeistelNetwork feistelKernel = new FeistelNetwork(keyExpansion, feistelFunction);
        feistelKernel.Execute(plainText, 128, XTR_TWOFISH.CypherEnums.CryptOperation.ENCRYPT);
        //ISymmetricEncryption desImpl = new DESImplementation(feistelKernel);


    }
}