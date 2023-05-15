using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace XTR_TwofishAlgs.MathBase.GaloisField
{
    public sealed class GFP2
    {
        public class Polynom1DegreeCoeffs
        {
            public Polynom1DegreeCoeffs(BigInteger x1, BigInteger x2)
            {
                First = x1;
                Second = x2;
            }

            public BigInteger First { get; set; }
            public BigInteger Second { get; set; }

            public override string ToString()
            {
                return $"{First}:{Second}";
            }
        }

        private BigInteger ModGFP(BigInteger value)
        {
            if(value < 0)
            {
                return _primaryNumber + (value % _primaryNumber);
            }
            else
            {
                return value % _primaryNumber;
            }
        }

        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private Polynom1DegreeCoeffs[] _values;
        public Polynom1DegreeCoeffs[] Values { get => _values; set => _values = value; }
        private BigInteger _primaryNumber;
        public BigInteger Primary { get => _primaryNumber; set => _primaryNumber = value; }

        private BigInteger _orderValue;

        public GFP2(BigInteger p)
        {
            _primaryNumber = p;
            _orderValue = _primaryNumber * _primaryNumber;
        }

        public Polynom1DegreeCoeffs GenerateRandomValue()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                BigInteger first = StandartMathTricks.RandomInRange(rng, 2, _primaryNumber);
                BigInteger second = StandartMathTricks.RandomInRange(rng, 2, _primaryNumber);
                return new Polynom1DegreeCoeffs(first, second);
            }
        }


        public Polynom1DegreeCoeffs Mult(Polynom1DegreeCoeffs x, Polynom1DegreeCoeffs y)
        {
            BigInteger newFirstValue = ModGFP(x.Second * y.Second - x.First * y.Second - x.Second * y.First);
            BigInteger newSecondValue = ModGFP(x.First * y.First - x.First * y.Second - x.Second * y.First);
            return new Polynom1DegreeCoeffs(newFirstValue, newSecondValue);
        }

        public Polynom1DegreeCoeffs Mult(BigInteger coeff, Polynom1DegreeCoeffs x)
        {
            BigInteger newFirstValue = ModGFP(coeff*x.First);
            BigInteger newSecondValue = ModGFP(coeff * x.Second);
            return new Polynom1DegreeCoeffs(newFirstValue, newSecondValue);
        }

        public Polynom1DegreeCoeffs Add(Polynom1DegreeCoeffs x, Polynom1DegreeCoeffs y)
        {
            BigInteger newFirstValue = ModGFP(x.First + y.First);
            BigInteger newSecondValue = ModGFP(x.Second + y.Second);
            return new Polynom1DegreeCoeffs(newFirstValue, newSecondValue);
        }

        public Polynom1DegreeCoeffs Substract(Polynom1DegreeCoeffs x, Polynom1DegreeCoeffs y)
        {
            BigInteger newFirstValue = ModGFP(x.First - y.First);
            BigInteger newSecondValue = ModGFP(x.Second - y.Second);
            return new Polynom1DegreeCoeffs(newFirstValue, newSecondValue);
        }

        public Polynom1DegreeCoeffs Pow2(Polynom1DegreeCoeffs x)
        {
            BigInteger newFirstValue = ModGFP(x.Second * (x.Second - 2 * x.First));
            BigInteger newSecondValue = ModGFP(x.First * (x.First - 2*x.Second));
            return new Polynom1DegreeCoeffs(newFirstValue, newSecondValue);
        }

        public Polynom1DegreeCoeffs PowP(Polynom1DegreeCoeffs x)
        {
            return new Polynom1DegreeCoeffs(x.Second, x.First);
        }

        public Polynom1DegreeCoeffs SetConstant(int n)
        {
            return new Polynom1DegreeCoeffs(ModGFP(-n), ModGFP(-n));
        }

        public Polynom1DegreeCoeffs XZMinusYZPowP(Polynom1DegreeCoeffs x, Polynom1DegreeCoeffs y, Polynom1DegreeCoeffs z)
        {
            BigInteger newFirstValue = ModGFP(z.First*(y.First - x.Second - y.Second) + z.Second*(x.Second - x.First + y.Second));
            BigInteger newSecondValue = ModGFP(z.First*(x.First- x.Second + y.First) + z.Second*(y.Second - x.First - y.First));
            return new Polynom1DegreeCoeffs(newFirstValue, newSecondValue);
        }        
    }
}
