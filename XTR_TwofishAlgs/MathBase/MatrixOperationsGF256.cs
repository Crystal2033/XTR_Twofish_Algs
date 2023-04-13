using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTR_TwofishAlgs.MathBase.GaloisField;

namespace XTR_TwofishAlgs.MathBase
{
    public static class MatrixOperationsGF256
    {
        public static byte[] multMatrixesTwoFish(byte[,] matrix, byte[] vector, IrreduciblePolynoms polynom)
        {
            byte[] result = new byte[matrix.GetLength(0)];//4x8 matrix * 8x1 vector = 4x1 vector
            GF256[] resultGalua = GF256.getEmptyVector(matrix.GetLength(0));

            GF256[,] galuaMatrix = GF256.getGaloisMatrixByByteMatrix(matrix);
            GF256[]  galuaVector = GF256.getGaloisVectorByByteVector(vector);
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for(int j = 0; j < matrix.GetLength(1); j++)
                {
                    resultGalua[i] = resultGalua[i] + GF256.Mult(galuaMatrix[i, j],  galuaVector[j], polynom);
                }
                result[i] = (byte)resultGalua[i].Value;
            }

            return result; //out 4 bytes
        }
    }
}
