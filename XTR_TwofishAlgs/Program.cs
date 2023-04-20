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
        Console.WriteLine("Hello, World!");
        //byte[] plainText = new byte[16] { 123, 43, 135, 23, 233, 111, 13, 123, 54, 78, 239, 21, 231, 2, 28, 43 };
        byte[] plainText = new byte[16] { 0,0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        byte[] mainKey = new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        //byte[] mainKey = new byte[16] { 123, 43, 135, 23, 233, 111, 13, 123, 54, 78, 239, 21, 231, 2, 28, 43 };
        //byte[] mainKey = new byte[24] { 0x01, 0x23, 0x34, 0x56, 0x78,
        //    0x9a, 0xbc, 0xde, 0xff, 0xed, 0xcb,
        //    0xa9, 0x87, 0x65, 0x43, 0x21, 0x00, 0x01, 0x12, 0x23, 0x34, 0x45, 0x56,0x67};
        //byte[] initVector = new byte[8] { 125, 67, 111, 110, 203, 211, 255, 11 };
        IKeyExpansion keyExpansion = new KeyExpansionTwoFish();
        IFeistelFunction feistelFunction = new TwofishFeistelFuncImpl();

        FeistelNetwork feistelKernel = new FeistelNetwork(keyExpansion, feistelFunction, TwoFishKeySizes.EASY) { MainKey = mainKey };
        byte[] cipher = feistelKernel.Execute(plainText, 128, XTR_TWOFISH.CypherEnums.CryptOperation.ENCRYPT);
        for(int i = 0; i < cipher.Length; i++)
        {
            CryptSimpleFunctions.ShowBinaryView(cipher[i]);
        }
        byte[] pt = feistelKernel.Execute(cipher, 128, XTR_TWOFISH.CypherEnums.CryptOperation.DECRYPT);


        //feistelKernel.Execute(plainText, 128, XTR_TWOFISH.CypherEnums.CryptOperation.ENCRYPT);
        //ISymmetricEncryption desImpl = new DESImplementation(feistelKernel);
        //TwoFishFunctions.q0Function(testByte);


        //byte[] bytesVector = new byte[4] { 213, 83, 83, 22 };
        //byte[] result1 = new byte[TwoFishMatrixes.MDS.GetLength(0)];//4x8 matrix * 8x1 vector = 4x1 vector
        //byte[] result2 = new byte[TwoFishMatrixes.MDS.GetLength(0)];//4x8 matrix * 8x1 vector = 4x1 vector
        //GF256[] resultGalua = GF256.getEmptyVector(matrix.GetLength(0));

        //GF256[,] galuaMatrix = GF256.getGaloisMatrixByByteMatrix(matrix);
        //GF256[] galuaVector = GF256.getGaloisVectorByByteVector(vector);


        //for (int i = 0; i < TwoFishMatrixes.MDS.GetLength(0); i++)
        //{
        //    for (int j = 0; j < TwoFishMatrixes.MDS.GetLength(1); j++)
        //    {
        //        //GF256 galoisRes = GF256.Mult(galuaMatrix[i, j], galuaVector[j], polynom);
        //        ////CryptSimpleFunctions.ShowBinaryView(galoisRes.Value, "Galois res after mult");
        //        //resultGalua[i] = resultGalua[i] + galoisRes;

        //        byte galoisRes = gmul(TwoFishMatrixes.MDS[i, j], bytesVector[j]);
        //        result1[i] = gadd(galoisRes, result1[i]);
        //    }
        //}

        //for (int i = 0; i < TwoFishMatrixes.MDS.GetLength(0); i++)
        //{
        //    for (int j = 0; j < TwoFishMatrixes.MDS.GetLength(1); j++)
        //    {
        //        //GF256 galoisRes = GF256.Mult(galuaMatrix[i, j], galuaVector[j], polynom);
        //        ////CryptSimpleFunctions.ShowBinaryView(galoisRes.Value, "Galois res after mult");
        //        //resultGalua[i] = resultGalua[i] + galoisRes;

        //        byte galoisRes = (byte)GF256.Mult(new GF256(TwoFishMatrixes.MDS[i, j]), new GF256(bytesVector[j]), IrreduciblePolynoms.X8X6X5X3_1).Value;
        //        result2[i] = (byte)(result2[i] ^ galoisRes);
        //    }
        //}


        //GF256 b = 131;
        //CryptSimpleFunctions.ShowBinaryView(a.Value, "A= ");
        //CryptSimpleFunctions.ShowBinaryView(b.Value, "B= ");
        //CryptSimpleFunctions.ShowBinaryView((a + b).Value, "A + B= ");
        //CryptSimpleFunctions.ShowBinaryView((a * b).Value, "A * B= ");
        //CryptSimpleFunctions.ShowBinaryView(GF256.Mult(a, b, IrreduciblePolynoms.X4X_1).Value, "(A * B)= mod X^4 + X + 1");

    }
    static byte gadd(byte a, byte b)
    {
        return (byte)(a ^ b);
    }

    /* Multiply two numbers in the GF(2^8) finite field defined 
     * by the modulo polynomial relation x^8 + x^4 + x^3 + x + 1 = 0
     * (the other way being to do carryless multiplication followed by a modular reduction)
     */
    static byte gmul(UInt16 a, UInt16 b)
    {
        UInt16 p = 0; /* accumulator for the product of the multiplication */
        while (a != 0 && b != 0)
        {
            if ((b & 1) == 1) /* if the polynomial for b has a constant term, add the corresponding a to p */
            {
                p ^= a; /* addition in GF(2^m) is an XOR of the polynomial coefficients */
            }
            a <<= 1; /* equivalent to a*x */
            if ((a & 0x100) != 0) /* GF modulo: if a has a nonzero term x^7, then must be reduced when it becomes x^8 */
            {
                a = (byte)(a ^ 0x169); /* subtract (XOR) the primitive polynomial x^8 + x^4 + x^3 + x + 1 (0b1_0001_1011) – you can change it but it must be irreducible */
            }
            b >>= 1;
        }
        return (byte)p;
    }
}