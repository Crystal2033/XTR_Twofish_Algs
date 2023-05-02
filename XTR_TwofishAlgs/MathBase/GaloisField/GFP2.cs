using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XTR_TwofishAlgs.MathBase.GaloisField
{
    public sealed class GFP2
    {
        public class Polynom1DegreeCoeffs
        {
            public Polynom1DegreeCoeffs(long x1, long x2)
            {
                First = x1;
                Second = x2;
            }

            public long First { get; set; }
            public long Second { get; set; }
        }

        private static long ModGFP(long value, long mod)
        {
            if(value < 0)
            {
                return mod + (value % mod);
            }
            else
            {
                return value % mod;
            }
        }

        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private Polynom1DegreeCoeffs[] _values;
        public Polynom1DegreeCoeffs[] Values { get => _values; set => _values = value; }
        private long _primaryNumber;
        public long Primary { get => _primaryNumber; set => _primaryNumber = value; }

        private long _orderValue;

        public GFP2(long p)
        {
            _primaryNumber = p;
            _orderValue = _primaryNumber * _primaryNumber;
            _values = new Polynom1DegreeCoeffs[_orderValue];
            CreateGaloisFieldP2();
        }

        public static Polynom1DegreeCoeffs Mult(Polynom1DegreeCoeffs x, Polynom1DegreeCoeffs y, long mod)
        {
            long newFirstValue = ModGFP(x.Second * y.Second - x.First * y.Second - x.Second * y.First, mod);
            long newSecondValue = ModGFP(x.First * y.First - x.First * y.Second - x.Second * y.First, mod);
            _log.Info($"({newFirstValue},{newSecondValue})");
            return new Polynom1DegreeCoeffs(newFirstValue, newSecondValue);
        }

        public static Polynom1DegreeCoeffs Pow2(Polynom1DegreeCoeffs x, long mod)
        {
            long newFirstValue = ModGFP(x.Second * (x.Second - 2*x.First), mod);
            long newSecondValue = ModGFP(x.First * (x.First - 2*x.Second), mod);
            _log.Info($"({newFirstValue},{newSecondValue})");
            return new Polynom1DegreeCoeffs(newFirstValue, newSecondValue);
        }

        public static Polynom1DegreeCoeffs PowP(Polynom1DegreeCoeffs x)
        {
            return new Polynom1DegreeCoeffs(x.Second, x.First);
        }

        public static Polynom1DegreeCoeffs XZMinusYZPowP(Polynom1DegreeCoeffs x, Polynom1DegreeCoeffs y, Polynom1DegreeCoeffs z, long mod)
        {
            long newFirstValue = ModGFP(z.First*(y.First - x.Second - y.Second) + z.Second*(x.Second - x.First + y.Second), mod);
            long newSecondValue = ModGFP(z.First*(x.First- x.Second + y.First) + z.Second*(y.Second - x.First - y.First), mod);
            _log.Info($"({newFirstValue},{newSecondValue})");
            return new Polynom1DegreeCoeffs(newFirstValue, newSecondValue);
        }

        private void CreateGaloisFieldP2()
        {
            for(int i = 0; i < _primaryNumber; i++)
            {
                for (int j = 0; j < _primaryNumber; j++)
                {
                    _values[i * _primaryNumber + j] = new Polynom1DegreeCoeffs(i, j);
                    _log.Info($"GF({_primaryNumber}^2)[{i * _primaryNumber + j}] = ({_values[i * _primaryNumber + j].First}, {_values[i * _primaryNumber + j].Second})");
                }
            }
        }

        
    }
}
