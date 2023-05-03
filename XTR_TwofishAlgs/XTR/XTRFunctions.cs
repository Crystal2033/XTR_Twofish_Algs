using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime;
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

        public TripleValsInGFP2()
        {
            first = new(0, 0);
            second = new(0, 0);
            third = new(0, 0);
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

        private GFP2.Polynom1DegreeCoeffs GetC2n(GFP2.Polynom1DegreeCoeffs cn)
        {
            return galoisFieldP2.Substract(galoisFieldP2.Pow2(cn), galoisFieldP2.Mult(2, galoisFieldP2.PowP(cn)));
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

        public TripleValsInGFP2 GenerateTrace()
        {
            while (true)
            {
                long n = _p + 1;
                GFP2.Polynom1DegreeCoeffs c;
                TripleValsInGFP2 cpPlus1;
                do
                {
                    c = galoisFieldP2.GenerateRandomValue();
                    cpPlus1 = SFunction(n, c);
                } while (cpPlus1.Second.First == cpPlus1.Second.Second);

                n = (_p * _p - _p + 1) / _q;

                TripleValsInGFP2 d = SFunction(n, c);
                if (d.Second.First == 3 && d.Second.Second == 3)
                {
                    continue;
                }
                else
                {
                    return d;
                }
            }
            
        }
        public TripleValsInGFP2 SFunction(long n, GFP2.Polynom1DegreeCoeffs c)
        {
            if (n < 0)
            {
                return SFunction(-n, c);
            }

            List<TripleValsInGFP2> SkList = new();

            SkList.Add(new TripleValsInGFP2(galoisFieldP2.PowP(c), galoisFieldP2.SetConstant(3), c));
            if (n == 0)
            {
                return SkList[0];
            }
            SkList.Add(new TripleValsInGFP2(SkList[0].Second, SkList[0].Third,
                galoisFieldP2.Substract(galoisFieldP2.Pow2(SkList[0].Third), galoisFieldP2.Mult(2, galoisFieldP2.PowP(SkList[0].Third)))));
            if (n == 1)
            {
                return SkList[1];
            }
            SkList.Add(new TripleValsInGFP2(SkList[1].Second, SkList[1].Third, GetCnPlus2(1, SkList)));
            if(n == 2)
            {
                return SkList[2];
            }
            else
            {
                long m_ = n;
                if (m_ % 2 == 0)
                {
                    m_--;
                }

                long m = (m_ - 1) / 2;
                int highest2Degree = StandartMathTricks.getHighest2DegreeValue((uint)m);
                long k = 1;
                SkList.Add(new TripleValsInGFP2(SkList[2].Second, SkList[2].Third, GetCnPlus2(2, SkList)));

                for (int i = highest2Degree - 1; i >= 0; i--)
                {

                    byte currentBit = (byte)((m >> i) & 1);
                    if (currentBit == 0) //4 * k + 1 is a middle element in S_2K => need to take this index
                    {
                        SkList.Add(new TripleValsInGFP2(GetC2n(SkList[SkList.Count - 1].First), GetC2n_1(SkList.Count - 1, SkList), GetC2n(SkList.Count - 1, SkList)));
                    }
                    else//4 * k + 3 is a middle element in S_2K => need to take this index
                    {
                        SkList.Add(new TripleValsInGFP2(GetC2n(SkList.Count - 1, SkList), GetC2nPlus1(SkList.Count - 1, SkList), GetC2n(SkList[SkList.Count - 1].Third)));
                    }
                    k = 2 * k + currentBit;
                }
                if(n % 2 == 0)
                {
                    SkList.Add(new TripleValsInGFP2(SkList[SkList.Count - 1].Second, SkList[SkList.Count - 1].Third, GetCnPlus2(SkList.Count - 1, SkList)));
                    m_++;
                }
            }

            return SkList[SkList.Count - 1];
        }
    }
}
