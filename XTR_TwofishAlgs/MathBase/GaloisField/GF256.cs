using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTR_TwofishAlgs.Exceptions;
using XTR_TwofishAlgs.HelpFunctions;

namespace XTR_TwofishAlgs.MathBase.GaloisField
{
    public sealed class GF256
    {
        private uint _value = 0;
        public uint Value { get => _value; set => _value = value; }
        public GF256(uint value)
        {
            if(value >= 256)
            {
                throw new GaloisOutOfFieldException($"Galois value {value} is out of GF256.");
            }
            _value = value;
        }

        public GF256()
        {
            _value = 0;
        }

        public static byte[] getVectorFromGaloisVector(GF256[] galuaVector)
        {
            byte[] result = new byte[galuaVector.Length];
            for (int i = 0; i < galuaVector.Length; i++)
            {
                result[i] = (byte)galuaVector[i].Value;
            }
            return result;
        }

        public static GF256[] getEmptyVector(int size)
        {
            GF256[] vector = new GF256[size];
            for (int i = 0; i < size; i++)
            {
                vector[i] = new GF256();
            }
            return vector;
        }

        public static GF256[] getGaloisVectorByByteVector(byte[] bytes)
        {
            GF256[] result = new GF256[bytes.Length];
            for(int i =0; i < bytes.Length; i++)
            {
                result[i] = new GF256(bytes[i]);
            }
            return result;
        }

        public static GF256[,] getGaloisMatrixByByteMatrix(byte[,] bytes) //checked
        {
            GF256[,] result = new GF256[bytes.GetLength(0), bytes.GetLength(1)];
            for (int i = 0; i < bytes.GetLength(0); i++)
            {
                for (int j = 0; j < bytes.GetLength(1); j++)
                {
                    result[i,j] = new GF256(bytes[i,j]);
                }
            }
            return result;
        }

        

        public static GF256 operator +(GF256 left, GF256 right)
        {
            return new GF256(left.Value ^ right.Value);
        }

        


        public static GF256 operator *(GF256 left, GF256 right)
        {
            GF256 copiedLeft = new GF256(left.Value);
            GF256 copiedRight = new GF256(right.Value);

            UInt32 res = 0;
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

        public static implicit operator GF256(uint val)
        {
            return new GF256(val);
        }

        public static int getHighest2DegreeValue(uint value)
        {
            int degree = 0;
            while(value != 1)
            {
                value >>= 1;
                degree++;
            }
            return degree;
        }
        public static uint divideByColumn(uint dividing, uint divider)
        {
            int nextDivider2DegreeGreater = getHighest2DegreeValue(divider);
            int maxFieldValue = 1 << nextDivider2DegreeGreater;
            while (dividing >= maxFieldValue)
            {
                dividing = (divider << ((getHighest2DegreeValue(dividing) - nextDivider2DegreeGreater))) ^ dividing;
            }
            return dividing;
        }

        public static GF256 Mult(in GF256 left, in GF256 right, IrreduciblePolynoms polynom =IrreduciblePolynoms.X4X_1)
        {
            GF256 copiedLeft = new GF256(left.Value);
            GF256 copiedRight = new GF256(right.Value);
            uint res = 0;
            while (copiedRight.Value != 0)
            {
                if ((copiedRight.Value & 1) == 1)
                {
                    res ^= copiedLeft.Value;
                }
                copiedLeft.Value <<= 1;
                if ((copiedLeft.Value & 0x100) != 0)
                {
                    copiedLeft.Value ^= (uint)polynom;
                }
                copiedRight.Value >>= 1;
            }
            return new GF256(res);
            //return new GF256(divideByColumn(res, (uint)polynom));
        }
        
    }
}
