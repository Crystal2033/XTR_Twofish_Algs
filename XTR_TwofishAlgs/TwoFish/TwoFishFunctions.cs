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
            byte[] zVector = MatrixOperationsGF256.multMatrixesTwoFish(TwoFishMatrixes.MDS, fourthBlockRes, MathBase.GaloisField.IrreduciblePolynoms.X8X6X5X3_1);
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

        public static byte qFunctionGeneral(byte x, byte[,] tMatrix)//q1 is the same, but different tMatrix
        {
            CryptSimpleFunctions.ShowBinaryView(x, "X");
            byte a0 = (byte)(x / 16);
            //CryptSimpleFunctions.ShowBinaryView(a0, "a0");
            byte b0 = (byte)(x % 16);
            //CryptSimpleFunctions.ShowBinaryView(b0, "b0");
            byte a1 = (byte)(a0 ^ b0);
            
            byte b1 = (byte)(a0 ^ CryptSimpleFunctions.CycleRightShiftInByte(b0, 4, 8, 1) ^ 8 * a0 % 16);

            //CryptSimpleFunctions.ShowBinaryView(b1, "b0");
            byte a2 = tMatrix[0, a1];
            //CryptSimpleFunctions.ShowBinaryView(a2, "a2");
            byte b2 = tMatrix[1, b1];
            //CryptSimpleFunctions.ShowBinaryView(b2, "b2");

            byte a3 = (byte)(a2 ^ b2);
            //CryptSimpleFunctions.ShowBinaryView(a3, "a3 = a2 ^ b2");

            //CryptSimpleFunctions.ShowBinaryView(CryptSimpleFunctions.CycleRightShiftInByte(b2, 4, 8, 1), "ROR4(b2,1)");
            //CryptSimpleFunctions.ShowBinaryView(a2 ^ CryptSimpleFunctions.CycleRightShiftInByte(b2, 4, 8, 1), "a2 ^ ROR4(b2,1)");
            //CryptSimpleFunctions.ShowBinaryView(8 * a2 % 16, "8 * a2 % 16");
            //CryptSimpleFunctions.ShowBinaryView(a2 ^ CryptSimpleFunctions.CycleRightShiftInByte(b2, 4, 8, 1) ^ (8 * a2) % 16, "a2 ^ ROR4(b2,1) ^ 8 * a2 % 16");
            byte b3 = (byte)(a2 ^ CryptSimpleFunctions.CycleRightShiftInByte(b2, 4, 8, 1) ^ 8 * a2 % 16);

            byte a4 = tMatrix[2, a3];
            byte b4 = tMatrix[3, b3];
            //CryptSimpleFunctions.ShowBinaryView(a4, "a4");
            //CryptSimpleFunctions.ShowBinaryView(b4, "b4");

            byte y = (byte)(16 * b4 + a4);
            //CryptSimpleFunctions.ShowBinaryView(y, "y");
            return y;
        }
    }
}
