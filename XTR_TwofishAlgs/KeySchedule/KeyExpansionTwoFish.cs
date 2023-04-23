using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using XTR_TWOFISH.CryptInterfaces;
using XTR_TwofishAlgs.HelpFunctions;
using XTR_TwofishAlgs.MathBase;
using XTR_TwofishAlgs.MathBase.GaloisField;
using XTR_TwofishAlgs.TwoFish;
using static XTR_TwofishAlgs.HelpFunctions.CryptConstants;

//https://www.schneier.com/wp-content/uploads/2016/02/paper-twofish-paper.pdf  page 8

namespace XTR_TwofishAlgs.KeySchedule
{
    public sealed class KeyExpansionTwoFish : IKeyExpansion
    {
        public List<byte[]> GenerateRoundKeys(byte[] mainKey, out List<byte[]> sBoxes)
        {
            List<byte[]> roundKeys = new();

            (List<byte[]> sBlock, (List<byte[]> Mo, List<byte[]> Me)) = getBasisOfKeySchedule(mainKey);
            sBoxes = sBlock;

            int k = mainKey.Length / 8;

            for (byte i = 0; i < 20; i++)
            {
                byte[] Ai = TwoFishFunctions.hFunction(getFilledBytesWithNumber(4, 2 * i), Me, k);
                byte[] Bi = CryptSimpleFunctions.CycleLeftShift(TwoFishFunctions.hFunction(getFilledBytesWithNumber(4, (2 * i) + 1), Mo, k), 32, 8);
                (byte[]newA, byte[] newB) = TwoFishFunctions.PseudoHadamardTransforms(Ai, Bi);
                byte[] K2i = newA;
                byte[] K2iPlus1 = CryptSimpleFunctions.CycleLeftShift(newB, 32, 9);
                roundKeys.Add(K2i);
                roundKeys.Add(K2iPlus1);
            }
            return roundKeys;
        }
        

        private byte[] getFilledBytesWithNumber(int valueOfBytes, int number)
        {
            byte[] result = new byte[valueOfBytes];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (byte)number;
            }
            return result;
        }

        private (List<byte[]> totalSBlock, (List<byte[]> Mo, List<byte[]> Me)) getBasisOfKeySchedule(byte[] mainKey) 
        {
            int k = mainKey.Length / 8;
            List<byte[]> Mi = getListOfMi(mainKey);
            (List<byte[]> Me, List<byte[]> Mo) = getMoAndMeVectors(Mi);
            List<byte[]> totalSBlock = new();

            for (int i = 0; i < k; i++)
            {
                byte[] packOfMiniM = new byte[8];
                for (int j = 0; j < packOfMiniM.Length; j++)
                {
                    packOfMiniM[j] = mainKey[8 * i + j];
                }
                byte[] miniSVector = MatrixOperationsGF256.MultMatrixesTwoFish(TwoFish.TwoFishMatrixes.RS, packOfMiniM, IrreduciblePolynoms.X8X6X3X2_1);
                totalSBlock.Insert(0, miniSVector);
            }
            return (totalSBlock, (Mo, Me));
        } 

        private List<byte[]> getListOfMi(byte[] preparedKey)
        {
            int k = preparedKey.Length / 8;
            List<byte[]> Mi = new List<byte[]>();
            for (int i = 0; i < 2 * k; i++)
            {
                Mi.Add(CryptSimpleFunctions.ConcatBitParts(preparedKey[4 * i], preparedKey[4 * i + 1],
                    preparedKey[4 * i + 2], preparedKey[4 * i + 3]));
            }
            return Mi;
        }

        private (List<byte[]> Mo, List<byte[]> Me) getMoAndMeVectors(List<byte[]> Mi)
        {
            List<byte[]> Mo = new List<byte[]>();
            List<byte[]> Me = new List<byte[]>();
            for(int i = 0; i < Mi.Count; i++)
            {
                if(i % 2 == 0)
                {
                    Me.Add(Mi[i]);
                }
                else
                {
                    Mo.Add(Mi[i]);
                }
            }
            return (Me, Mo);
        }
    }
}
