using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTR_TWOFISH.CryptInterfaces;
using XTR_TwofishAlgs.HelpFunctions;
using XTR_TwofishAlgs.TwoFish;
using static XTR_TwofishAlgs.HelpFunctions.CryptConstants;

namespace XTR_TwofishAlgs.FeistelImplementation
{
    internal class TwofishFeistelFuncImpl : IFeistelFunction
    {
        public (byte[] F0Res, byte[] F1Res) FeistelFunction(byte[] R0, byte[] R1, byte[] evenRoundKey, byte[] oddRoundKey, List<byte[]> sBox, int raundNumber)
        {
            byte[] T0 = gFunction(R0, sBox);
            byte[] T1 = gFunction(CryptSimpleFunctions.CycleLeftShift(R1, 32, 8), sBox);
            CryptSimpleFunctions.ShowBinaryView(T0, "T0");
            CryptSimpleFunctions.ShowBinaryView(T1, "T1");
            (byte[] T0PHT, byte[] T1PHT) = TwoFishFunctions.PseudoHadamardTransforms(T0, T1);

            byte[] F0 = TwoFishFunctions.SumMod32(T0PHT, evenRoundKey);
            byte[] F1 = TwoFishFunctions.SumMod32(T1PHT, oddRoundKey);
            return (F0, F1);
        }

        private byte[] gFunction(byte[] bytes, List<byte[]> sBox) //bytes here is 32 bits value (1/4 from part of plain text)
        {
            TwoFishKeySizes keySize;
            if (sBox.Count == 2)
            {
                keySize = TwoFishKeySizes.EASY;
            }
            else if (sBox.Count == 3)
            {
                keySize = TwoFishKeySizes.MIDDLE;
            }
            else if (sBox.Count == 4)
            {
                keySize = TwoFishKeySizes.HARD;
            }
            else
            {
                Console.WriteLine("Wrong sBox size");
                return null;
            }
            byte[] result = TwoFishFunctions.hFunction(bytes, sBox, keySize);

            return result;
        }
    }
}
