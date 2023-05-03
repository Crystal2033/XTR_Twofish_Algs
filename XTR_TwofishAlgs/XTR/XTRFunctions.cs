using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XTR_TwofishAlgs.MathBase;
using XTR_TwofishAlgs.MathBase.GaloisField;

namespace XTR_TwofishAlgs.XTR
{
    
    public sealed class TripleValsInGFP2
    {
        private GFP2.Polynom1DegreeCoeffs first;
        private GFP2.Polynom1DegreeCoeffs second;
        private GFP2.Polynom1DegreeCoeffs third;

        public GFP2.Polynom1DegreeCoeffs First { get => first; set => first = value; }
        public GFP2.Polynom1DegreeCoeffs Second { get => second; set => second = value; }
        public GFP2.Polynom1DegreeCoeffs Third { get => third; set => third = value; }

        public TripleValsInGFP2(GFP2.Polynom1DegreeCoeffs first, GFP2.Polynom1DegreeCoeffs second, GFP2.Polynom1DegreeCoeffs third)
        {
            this.first = first;
            this.second = second;
            this.third = third;
        }
    }
    public sealed class XTRFunctions
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private long _p;
        private long _q;
        private GFP2 galoisFieldP2;

        public XTRFunctions(long p, long q)
        {
            _q = q;
            _p = p;
            galoisFieldP2 = new GFP2(p);
        }

        //private GFP2.Polynom1DegreeCoeffs GetC2n(GFP2.Polynom1DegreeCoeffs cn)
        //{
        //    return galoisFieldP2.Substract(galoisFieldP2.Pow2(cn), galoisFieldP2.Mult(2, galoisFieldP2.PowP(cn)));
        //}

        //private GFP2.Polynom1DegreeCoeffs GetCnPlus2(GFP2.Polynom1DegreeCoeffs cn_1, GFP2.Polynom1DegreeCoeffs cn, GFP2.Polynom1DegreeCoeffs cnPlus1,
        //    GFP2.Polynom1DegreeCoeffs c)
        //{

        //    return galoisFieldP2.Add(galoisFieldP2.Substract(galoisFieldP2.Mult(c, cnPlus1), galoisFieldP2.Mult(galoisFieldP2.PowP(c), cn)), cn_1);
        //}

        //private GFP2.Polynom1DegreeCoeffs GetC2n_1(GFP2.Polynom1DegreeCoeffs cn_1, GFP2.Polynom1DegreeCoeffs cn, GFP2.Polynom1DegreeCoeffs cnPlus1,
        //    GFP2.Polynom1DegreeCoeffs c)
        //{
        //    return galoisFieldP2.Add(galoisFieldP2.Substract(galoisFieldP2.Mult(cn_1, cn),
        //        galoisFieldP2.Mult(galoisFieldP2.PowP(c), galoisFieldP2.PowP(cn))), galoisFieldP2.PowP(cnPlus1));
        //}

        //private GFP2.Polynom1DegreeCoeffs GetC2nPlus1(GFP2.Polynom1DegreeCoeffs cn_1, GFP2.Polynom1DegreeCoeffs cn, GFP2.Polynom1DegreeCoeffs cnPlus1,
        //    GFP2.Polynom1DegreeCoeffs c)
        //{
        //    return galoisFieldP2.Add(galoisFieldP2.Substract(galoisFieldP2.Mult(cnPlus1, cn),
        //        galoisFieldP2.Mult(c, galoisFieldP2.PowP(cn))), galoisFieldP2.PowP(cn_1));
        //}

        private GFP2.Polynom1DegreeCoeffs GetC2n(int n, List<TripleValsInGFP2> sVals)
        {
            if(n > sVals.Count)
            {
                _log.Error($"Value n={n} in SFunction greater than SList size={sVals.Count}.");
            }
            return galoisFieldP2.Substract(galoisFieldP2.Pow2(sVals[n].Second), galoisFieldP2.Mult(2, galoisFieldP2.PowP(sVals[n].Second)));
        }

        private GFP2.Polynom1DegreeCoeffs GetCnPlus2(int n, List<TripleValsInGFP2> sVals) // c1 == c == SVals[0].Third or SVals[1].Second
        {
            if (n > sVals.Count)
            {
                _log.Error($"Value n={n} in SFunction greater than SList size={sVals.Count}.");
            }
            return galoisFieldP2.Add(galoisFieldP2.Substract(galoisFieldP2.Mult(sVals[1].Second, sVals[n].Third), 
                galoisFieldP2.Mult(galoisFieldP2.PowP(sVals[1].Second), sVals[n].Second)), sVals[n].First);
        }

        private GFP2.Polynom1DegreeCoeffs GetC2n_1(int n, List<TripleValsInGFP2> sVals)
        {
            if (n > sVals.Count)
            {
                _log.Error($"Value n={n} in SFunction greater than SList size={sVals.Count}.");
            }
            return galoisFieldP2.Add(galoisFieldP2.Substract(galoisFieldP2.Mult(sVals[n].First, sVals[n].Second),
                galoisFieldP2.Mult(galoisFieldP2.PowP(sVals[1].Second), galoisFieldP2.PowP(sVals[n].Second))), galoisFieldP2.PowP(sVals[n].Third));
        }

        private GFP2.Polynom1DegreeCoeffs GetC2nPlus1(int n, List<TripleValsInGFP2> sVals)
        {
            if (n > sVals.Count)
            {
                _log.Error($"Value n={n} in SFunction greater than SList size={sVals.Count}.");
            }
            return galoisFieldP2.Add(galoisFieldP2.Substract(galoisFieldP2.Mult(sVals[n].Third, sVals[n].Second),
                galoisFieldP2.Mult(sVals[1].Second, galoisFieldP2.PowP(sVals[n].Second))), galoisFieldP2.PowP(sVals[n].First));
        }

        public TripleValsInGFP2 SFunction(int n, GFP2.Polynom1DegreeCoeffs c)
        {
            
            if (n < 0)
            {
                return SFunction(-n, c);
            }

            List<TripleValsInGFP2> SkList = new(n + 1);
            SkList.Insert(0, new TripleValsInGFP2(galoisFieldP2.PowP(c), galoisFieldP2.SetConstant(3), c));
            SkList.Insert(1, new TripleValsInGFP2(SkList[0].Second, SkList[0].Third, GetC2n(1, SkList)));
            SkList.Insert(2, new TripleValsInGFP2(SkList[1].Second, SkList[1].Third, GetCnPlus2(1, SkList)));

            if(n <= 2)
            {
                return SkList[n];
            }
            else
            {
                int m_ = n;
                if (m_ % 2 == 0)
                {
                    m_--;
                }

                int m = (m_ - 1) / 2;
                int highest2Degree = StandartMathTricks.getHighest2DegreeValue(m);
                int k = 1;
                for (int i = highest2Degree - 1; i > 0; i--)
                {
                    SkList.Insert(2 * k + 1, new TripleValsInGFP2(SkList[2 * k].Second, SkList[2 * k].Third, GetCnPlus2(2 * k, SkList)));
                    byte currentBit = (byte)((m >> i) & 1);
                    if (currentBit == 0)
                    {
                        SkList.Insert(2 * (2 * k + 1), new TripleValsInGFP2(GetC2n(2 * k, SkList), GetC2n_1(2 * k + 1, SkList), GetC2n(2 * k + 1, SkList)));
                    }
                    else
                    {
                        SkList.Insert(2 * (2 * k + 1) + 1, new TripleValsInGFP2(GetC2n(2 * k + 1, SkList), GetC2nPlus1(2 * k + 1, SkList), GetC2n(2 * k + 2, SkList)));
                    }
                    k = 2 * k + currentBit;
                }
                if(n % 2 == 0)
                {
                    SkList.Insert(m_ + 1, new TripleValsInGFP2(SkList[m_].Second, SkList[m_].Third, GetCnPlus2(m_, SkList)));
                    m_++;
                }
            }

            return SkList[n];


        }
    }
}
