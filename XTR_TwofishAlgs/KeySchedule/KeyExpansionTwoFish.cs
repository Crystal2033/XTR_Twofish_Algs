using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTR_TWOFISH.CryptInterfaces;
using XTR_TwofishAlgs.HelpFunctions;
using XTR_TwofishAlgs.MathBase;
using XTR_TwofishAlgs.MathBase.GaluaField;
using static XTR_TwofishAlgs.HelpFunctions.CryptConstants;

//https://www.schneier.com/wp-content/uploads/2016/02/paper-twofish-paper.pdf  page 8

namespace XTR_TwofishAlgs.KeySchedule
{
    public sealed class KeyExpansionTwoFish : IKeyExpansion
    {
        public List<byte[]> GenerateRoundKeys(byte[] preparedKey, TwoFishKeySizes keySizeInBits, out List<byte> sBoxes)
        {
            int k = (int)keySizeInBits / 64;// M (preparedKey) key consists of 8k bytes
            List<byte[]> Mi = getListOfMi(preparedKey, k);
            (List<byte[]> Mo, List<byte[]> Me) = getMoAndMeVectors(Mi, k);
            List<byte[]> totalSBlock = new();

            for (int i = 0; i < k; i++)
            {
                byte[] packOfMiniM = new byte[8];
                for(int j = 0; j < packOfMiniM.Length; j++)
                {
                    packOfMiniM[i] = preparedKey[8 * i + j];
                }
                byte[] miniSVector = MatrixOperationsGF256.multMatrixesTwoFish(TwoFish.TwoFishMatrixes.RS, packOfMiniM, IrreduciblePolynoms.X8X6X3X2_1);
                byte[] Si = CryptSimpleFunctions.ConcatBitParts(miniSVector);
                totalSBlock.Insert(0, Si);
            }
            //Result: totalSBlock, Mo, Me
            
            throw new NotImplementedException();
        }

        private List<byte[]> getListOfMi(byte[] preparedKey, int k)
        {
            List<byte[]> Mi = new List<byte[]>();
            for (int i = 0; i < 2 * k; i++)
            {
                Mi.Add(CryptSimpleFunctions.ConcatBitParts(preparedKey[4 * i], preparedKey[4 * i + 1],
                    preparedKey[4 * i + 2], preparedKey[4 * i + 3]));
            }
            return Mi;
        }

        private (List<byte[]> Mo, List<byte[]> Me) getMoAndMeVectors(List<byte[]> Mi, int k)
        {
            List<byte[]> Mo = new List<byte[]>();
            List<byte[]> Me = new List<byte[]>();
            for(int i = 0; i < Mi.Count; i++)
            {
                if(i % 2 == 0)
                {
                    Mo.Add(Mi[i]);
                }
                else
                {
                    Me.Add(Mi[i]);
                }
            }
            return (Mo, Me);
        }
    }
}
