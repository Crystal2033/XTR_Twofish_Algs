using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTR_TwofishAlgs.HelpFunctions;
using XTR_TwofishAlgs.MathBase;
using static XTR_TwofishAlgs.HelpFunctions.CryptConstants;

namespace XTR_TwofishAlgs.TwoFish
{
    public static class TwoFishFunctions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="X"> Input bytes in h functions (4 bytes)</param>
        /// <param name="sBox">inverted SBoxes generated in KeyExpansion. If h fucntion SBlock is constant tMatrix </param>
        /// <param name="keySize">KeySize in alg. For k-value(2,3 or 4)</param>
        /// <returns></returns>
        public static byte[] hFunction(byte[] X, List<byte[]> sBox, TwoFishKeySizes keySize) //out Z
        {
            byte[] connectBytes = X;
            if (keySize == TwoFishKeySizes.HARD)
            {
                //TODO: for k == 4
                byte[] zeroBlockRes = new byte[4]
                {
                    q1Function(connectBytes[0]),
                    q0Function(connectBytes[1]),
                    q0Function(connectBytes[2]),
                    q1Function(connectBytes[3])
                };
                connectBytes = CryptSimpleFunctions.XorByteArrays(zeroBlockRes, sBox[3]);
            }
            if (keySize == TwoFishKeySizes.HARD || keySize == TwoFishKeySizes.MIDDLE)
            {
                //TODO: for k >= 3
                byte[] firstBlockRes = new byte[4]
                {
                    q0Function(connectBytes[0]),
                    q0Function(connectBytes[1]),
                    q1Function(connectBytes[2]),
                    q1Function(connectBytes[3])
                };
                connectBytes = CryptSimpleFunctions.XorByteArrays(firstBlockRes, sBox[2]);
            }
            //k >=2

            //byte[] secondBlockRes = new byte[4]
            //{
            //    q1Function(connectBytes[3]),
            //    q0Function(connectBytes[2]),
            //    q1Function(connectBytes[1]),
            //    q0Function(connectBytes[0])
            //};

            //connectBytes = CryptSimpleFunctions.XorByteArrays(secondBlockRes, sBox[1]);

            //byte[] thirdBlockRes = new byte[4]
            //{
            //    q1Function(connectBytes[3]),
            //    q1Function(connectBytes[2]),
            //    q0Function(connectBytes[1]),
            //    q0Function(connectBytes[0])
            //};
            //connectBytes = CryptSimpleFunctions.XorByteArrays(thirdBlockRes, sBox[0]);

            //byte[] fourthBlockRes = new byte[4]
            //{
            //    q0Function(connectBytes[3]),
            //    q1Function(connectBytes[2]),
            //    q0Function(connectBytes[1]),
            //    q1Function(connectBytes[0])
            //};

            byte[] result = new byte[4]
            {
                q1Function((byte)(q0Function((byte)(q0Function(connectBytes[3]) ^ sBox[1][3])) ^ sBox[0][3])),
                q0Function((byte)(q0Function((byte)(q1Function(connectBytes[2]) ^ sBox[1][2])) ^ sBox[0][2])),
                q1Function((byte)(q1Function((byte)(q0Function(connectBytes[1]) ^ sBox[1][1])) ^ sBox[0][1])),
                q0Function((byte)(q1Function((byte)(q1Function(connectBytes[0]) ^ sBox[1][0])) ^ sBox[0][0]))
            };
            byte[] zVector = MatrixOperationsGF256.multMatrixesTwoFish(TwoFishMatrixes.MDS, result, MathBase.GaloisField.IrreduciblePolynoms.X8X6X5X3_1);
            return zVector;
        }

        

        public static byte q0Function(byte x)//q1 is the same, but different tMatrix
        {
            return qFunctionGeneral(x, TwoFishMatrixes.Q0TMatrix); ;
        }
        public static byte q1Function(byte x)
        { 
            return qFunctionGeneral(x, TwoFishMatrixes.Q1TMatrix); ;
        }

        /// <summary>
        /// a' = a+b mod32
        /// b' = a+2b mod32
        /// </summary>
        /// <param name="a">a value</param>
        /// <param name="b">b value</param>
        /// <returns>(a',b')</returns>
        public static (byte[], byte[]) PseudoHadamardTransforms(byte[] a, byte[] b)
        {
            byte[] newA = new byte[4];
            byte[] newB = new byte[4];
            //CryptSimpleFunctions.ShowBinaryView(a, "a");
            int aInt = CryptSimpleFunctions.FromBytesToInt(a, 32);
            //CryptSimpleFunctions.ShowBinaryView(aInt, "aInt");
            //CryptSimpleFunctions.ShowBinaryView(b, "b");
            int bInt = CryptSimpleFunctions.FromBytesToInt(b, 32);
            //CryptSimpleFunctions.ShowBinaryView(bInt, "bInt");
            newA = fromIntToBytes(aInt + bInt);
            //CryptSimpleFunctions.ShowBinaryView(newA, "newA");
            newB = fromIntToBytes(aInt + 2*bInt);
           //CryptSimpleFunctions.ShowBinaryView(newB, "newB");
            return (newA, newB);
        }

        public static byte[] fromIntToBytes(int value)
        {
            byte[] bytes = new byte[4];

            bytes[0] = (byte)((value >> 24) & 0xFF);
            bytes[1] = (byte)((value >> 16) & 0xFF);
            bytes[2] = (byte)((value >> 8) & 0xFF);
            bytes[3] = (byte)(value & 0xFF);
            return bytes;
        }
        public static byte qFunctionGeneral(byte x, byte[,] tMatrix)//q1 is the same, but different tMatrix
        {
            CryptSimpleFunctions.ShowBinaryView(x, "X");
            byte a0 = (byte)(x / 16);
            CryptSimpleFunctions.ShowBinaryView(a0, "a0");
            byte b0 = (byte)(x % 16);
            CryptSimpleFunctions.ShowBinaryView(b0, "b0");
            byte a1 = (byte)(a0 ^ b0);
            
            byte b1 = (byte)(a0 ^ CryptSimpleFunctions.CycleRightShiftInByte(b0, 4, 8, 1) ^ 8 * a0 % 16);

            CryptSimpleFunctions.ShowBinaryView(b1, "b1");
            byte a2 = tMatrix[0, a1];
            CryptSimpleFunctions.ShowBinaryView(a2, "a2");
            byte b2 = tMatrix[1, b1];
            CryptSimpleFunctions.ShowBinaryView(b2, "b2");

            byte a3 = (byte)(a2 ^ b2);
            CryptSimpleFunctions.ShowBinaryView(a3, "a3 = a2 ^ b2");

            CryptSimpleFunctions.ShowBinaryView(CryptSimpleFunctions.CycleRightShiftInByte(b2, 4, 8, 1), "ROR4(b2,1)");
            CryptSimpleFunctions.ShowBinaryView(a2 ^ CryptSimpleFunctions.CycleRightShiftInByte(b2, 4, 8, 1), "a2 ^ ROR4(b2,1)");
            CryptSimpleFunctions.ShowBinaryView(8 * a2 % 16, "8 * a2 % 16");
            CryptSimpleFunctions.ShowBinaryView(a2 ^ CryptSimpleFunctions.CycleRightShiftInByte(b2, 4, 8, 1) ^ (8 * a2) % 16, "a2 ^ ROR4(b2,1) ^ 8 * a2 % 16");
            byte b3 = (byte)(a2 ^ CryptSimpleFunctions.CycleRightShiftInByte(b2, 4, 8, 1) ^ 8 * a2 % 16);

            byte a4 = tMatrix[2, a3];
            byte b4 = tMatrix[3, b3];
            CryptSimpleFunctions.ShowBinaryView(a4, "a4");
            CryptSimpleFunctions.ShowBinaryView(b4, "b4");

            byte y = (byte)(16 * b4 + a4);
            CryptSimpleFunctions.ShowBinaryView(y, "y");
            return y;
        }
    }
}
