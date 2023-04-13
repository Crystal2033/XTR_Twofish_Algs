using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTR_TwofishAlgs.HelpFunctions;

namespace XTR_TwofishAlgs.MathBase.GaluaField
{
    public sealed class GF256
    {
        public GF256(int value)
        {
            _value = value;
        }

        public static byte[] getVectorFromGaluaVector(GF256[] galuaVector)
        {
            byte[] result = new byte[galuaVector.Length];
            for (int i = 0; i < galuaVector.Length; i++)
            {
                result[i] = (byte)galuaVector[i].Value;
            }
            return result;
        }

        public static GF256[] getGaluaVectorByByteVector(byte[] bytes)
        {
            GF256[] result = new GF256[bytes.Length];
            for(int i =0; i < bytes.Length; i++)
            {
                result[i] = new GF256(bytes[i]);
            }
            return result;
        }

        public static GF256[,] getGaluaMatrixByByteMatrix(byte[,] bytes)
        {
            GF256[,] result = new GF256[bytes.Length, bytes.GetLength(0)];
            for (int i = 0; i < bytes.Length; i++)
            {
                for (int j = 0; j < bytes.GetLength(i); j++)
                {
                    result[i,j] = new GF256(bytes[i,j]);
                }
            }
            return result;
        }

        private int _value = 0;
        public int Value { get => _value; set => _value = value; }

        public static GF256 operator +(GF256 left, GF256 right)
        {
            return new GF256(left.Value ^ right.Value);
        }

        


        public static GF256 operator *(GF256 left, GF256 right)
        {
            GF256 copiedLeft = new GF256(left.Value);
            GF256 copiedRight = new GF256(right.Value);

            int res = 0;
            while(copiedRight.Value != 0)
            {
                if((copiedRight.Value & 1) == 1)
                {
                    res = res ^ copiedLeft.Value;
                }
                copiedLeft.Value = copiedLeft.Value << 1;
                copiedRight.Value = copiedRight.Value >> 1;
            }
            return new GF256(res);
        }

        public static implicit operator GF256(int val)
        {
            return new GF256(val);
        }

        public static int getHighest2DegreeValue(int value)
        {
            int degree = 0;
            while(value != 1)
            {
                value >>= 1;
                degree++;
            }
            return degree;
        }
        public static int divideByColumn(int dividing, int divider)
        {
            int nextDivider2DegreeGreater = getHighest2DegreeValue(divider);

            while (dividing > divider)
            {
                CryptSimpleFunctions.ShowBinaryView(dividing, "dividing");
                CryptSimpleFunctions.ShowBinaryView(divider << ((getHighest2DegreeValue(dividing) - nextDivider2DegreeGreater)), "shifted divider");
                CryptSimpleFunctions.ShowBinaryView((divider << ((getHighest2DegreeValue(dividing) - nextDivider2DegreeGreater))) ^ dividing, "result");
                dividing = (divider << ((getHighest2DegreeValue(dividing) - nextDivider2DegreeGreater))) ^ dividing;
                
            }
            return dividing;
        }

        public static GF256 Mult(GF256 left, GF256 right, IrreduciblePolynoms polynom =IrreduciblePolynoms.X4X_1)
        {
            int res = 0;
            //CryptSimpleFunctions.ShowBinaryView(left.Value, "Left");
            //CryptSimpleFunctions.ShowBinaryView(right.Value, "Right");
            while (right.Value != 0)
            {
                if ((right.Value & 1) == 1)
                {
                    res ^= left.Value;
                }
                left.Value <<= 1;
                right.Value >>= 1;
                //CryptSimpleFunctions.ShowBinaryView(left.Value, "Left");
                //CryptSimpleFunctions.ShowBinaryView(right.Value, "Right");
            }
            return new GF256(divideByColumn(res, (int)polynom));
        }
        
    }
}
