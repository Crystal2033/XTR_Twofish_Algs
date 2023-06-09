﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using XTR_TwofishAlgs.MathBase.GaloisField;

namespace XTR_TwofishAlgs.MathBase
{
    public static class Euclid
    {
        public static (int, (int, int)) ExtendedGcd(int a, int b)
        {
            (int oldR, int r) = (a, b); //r - это остаток от деления 
            (int oldS, int s) = (1, 0);
            (int oldT, int t) = (0, 1);
            while(r != 0)
            {
                if(oldR < r)
                {
                    (oldR, r) = (r, oldR);
                }
                int quotient = oldR / r;
 
                (oldR, r) = (r, oldR - quotient * r);
                (oldS, s) = (s, oldS - quotient * s);
                (oldT, t) = (t, oldT - quotient * t);
            }
            return (oldR, (oldS, oldT));
        }

        public static (BigInteger nod, (BigInteger x, BigInteger y) coeffs) ExtendedGcd(BigInteger a, BigInteger b)
        {
            (BigInteger oldR, BigInteger r) = (a, b); //r - это остаток от деления 
            (BigInteger oldS, BigInteger s) = (1, 0);
            (BigInteger oldT, BigInteger t) = (0, 1);
            while (r != 0)
            {
                if (oldR < r)
                {
                    (oldR, r) = (r, oldR);
                }
                BigInteger quotient = oldR / r;

                (oldR, r) = (r, oldR - quotient * r);
                (oldS, s) = (s, oldS - quotient * s);
                (oldT, t) = (t, oldT - quotient * t);
            }
            return (oldR, (oldS, oldT));
        }

        public static (long, long) FindOptimalNormalBasis(long p)
        {
            for(long i = 0; i < p; i++)
            {
                if((i*i + i + 1) % p == 0)
                {
                    return (i, i * i);
                }
            }
            return (0, 0);
        }
    }
}
