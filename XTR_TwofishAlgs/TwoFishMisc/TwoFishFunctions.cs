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
        /// <returns></returns>
        public static byte[] hFunction(byte[] X, List<byte[]> sBox, int k) //out Z
        {
            byte[] connectBytes = X;
            if (k == 4)
            {
                byte[] zeroBlockRes = new byte[4]
                {
                    q1Function(connectBytes[0]),
                    q0Function(connectBytes[1]),
                    q0Function(connectBytes[2]),
                    q1Function(connectBytes[3])
                };
               
                connectBytes = CryptSimpleFunctions.XorByteArrays(zeroBlockRes, sBox[3]);
            }
            if (k == 4 || k == 3)
            {
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

            byte[] secondBlockRes = new byte[4]
            {
                q1Function(connectBytes[0]),
                q0Function(connectBytes[1]),
                q1Function(connectBytes[2]),
                q0Function(connectBytes[3])
            };

            connectBytes = CryptSimpleFunctions.XorByteArrays(secondBlockRes, sBox[1]);

            byte[] thirdBlockRes = new byte[4]
            {
                q1Function(connectBytes[0]),
                q1Function(connectBytes[1]),
                q0Function(connectBytes[2]),
                q0Function(connectBytes[3])
            };
            connectBytes = CryptSimpleFunctions.XorByteArrays(thirdBlockRes, sBox[0]);

            byte[] fourthBlockRes = new byte[4]
            {
                q0Function(connectBytes[0]),
                q1Function(connectBytes[1]),
                q0Function(connectBytes[2]),
                q1Function(connectBytes[3])
            };
            byte[] zVector = MatrixOperationsGF256.MultMatrixesTwoFish(TwoFishMatrixes.MDS, CryptSimpleFunctions.RevertBytes(fourthBlockRes), MathBase.GaloisField.IrreduciblePolynoms.X8X6X5X3_1);
            return CryptSimpleFunctions.RevertBytes(zVector);
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
            uint aInt = CryptSimpleFunctions.FromBytesToInt(a, 32);
            uint bInt = CryptSimpleFunctions.FromBytesToInt(b, 32);
            newA = CryptSimpleFunctions.FromIntToBytes(aInt + bInt);
            newB = CryptSimpleFunctions.FromIntToBytes(aInt + 2 * bInt);
            return (newA, newB);
        }

        public static byte[] SumMod32(byte[] first, byte[] second)
        {
            uint firstInt = CryptSimpleFunctions.FromBytesToInt(first, 32);
            uint secondInt = CryptSimpleFunctions.FromBytesToInt(second, 32);
            return CryptSimpleFunctions.FromIntToBytes(firstInt + secondInt);
        }

        public static byte[] SumMod32(params byte[][] bytes)
        {
            uint sum = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                sum += CryptSimpleFunctions.FromBytesToInt(bytes[i], 32);
            }
            return CryptSimpleFunctions.FromIntToBytes(sum);
        }

        
        public static byte qFunctionGeneral(byte x, byte[,] tMatrix)
        {
            byte a0 = (byte)(x / 16);
            byte b0 = (byte)(x % 16);

            byte a1 = (byte)(a0 ^ b0);
            byte b1 = (byte)(a0 ^ CryptSimpleFunctions.CycleRightShiftInByte(b0, 4, 8, 1) ^ 8 * a0 % 16);

            byte a2 = tMatrix[0, a1];
            byte b2 = tMatrix[1, b1];

            byte a3 = (byte)(a2 ^ b2);
            byte b3 = (byte)(a2 ^ CryptSimpleFunctions.CycleRightShiftInByte(b2, 4, 8, 1) ^ 8 * a2 % 16);

            byte a4 = tMatrix[2, a3];
            byte b4 = tMatrix[3, b3];

            return (byte)(16 * b4 + a4);
        }
    }
}
