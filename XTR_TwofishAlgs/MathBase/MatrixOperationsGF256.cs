using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTR_TwofishAlgs.MathBase.g;
using XTR_TwofishAlgs.MathBase.GaluaField;

namespace XTR_TwofishAlgs.MathBase
{
    public static class MatrixOperationsGF256
    {
        public static byte[] multMatrixesTwoFish(byte[,] matrix, byte[] vector, IrreduciblePolynoms polynom)
        {
            byte[] result = new byte[matrix.Length];//4x8 matrix * 8x1 vector = 4x1 vector
            GF256[] resultGalua = new GF256[matrix.Length];

            GF256[,] galuaMatrix = GF256.getGaluaMatrixByByteMatrix(matrix);
            GF256[]  galuaVector = GF256.getGaluaVectorByByteVector(vector);
            for (int i = 0; i < matrix.Length; i++)
            {
                for(int j = 0; j < matrix.GetLength(i); j++)
                {
                    resultGalua[i] = resultGalua[i] + GF256.Mult(galuaMatrix[i, j],  galuaVector[j], polynom);
                }
            }

            return result; //out 4 bytes
        }
    }
}
