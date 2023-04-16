using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTR_TwofishAlgs.HelpFunctions;
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
            return new byte[2];
        }

        public static byte q0Function(byte x)
        {
            CryptSimpleFunctions.ShowBinaryView(x, "X");
            byte a0 = (byte)(x / 16);
            CryptSimpleFunctions.ShowBinaryView(a0, "a0");
            byte b0 = (byte)(x % 16);
            CryptSimpleFunctions.ShowBinaryView(b0, "b0");
            byte a1 = (byte)(a0 ^ b0);
            //CryptSimpleFunctions.ShowBinaryView(a1, "a1");
            //CryptSimpleFunctions.ShowBinaryView(CryptSimpleFunctions.CycleRightShiftInByte(b0, 4, 8, 1), "ROR4(b0,1)");
            //CryptSimpleFunctions.ShowBinaryView(a0 ^ CryptSimpleFunctions.CycleRightShiftInByte(b0, 4, 8, 1), "a0 ^ ROR4(b0,1)");
            //CryptSimpleFunctions.ShowBinaryView(8 * a0 % 16, "8 * a0 % 16");
            //CryptSimpleFunctions.ShowBinaryView(a0 ^ CryptSimpleFunctions.CycleRightShiftInByte(b0, 4, 8, 1) ^ (8 * a0) % 16, "a0 ^ ROR4(b0,1) ^ 8 * a0 % 16");
            byte b1 = (byte)(a0 ^ CryptSimpleFunctions.CycleRightShiftInByte(b0, 4, 8, 1) ^ 8*a0%16);
            byte a2 = 

            return new byte();
        }
    }
}
