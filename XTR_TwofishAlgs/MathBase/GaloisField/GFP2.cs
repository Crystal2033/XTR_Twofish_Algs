using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

        private long ModGFP(long value)
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

        public Polynom1DegreeCoeffs GenerateRandomValue()
        {
            Random r = new();
            long first = r.NextInt64(_primaryNumber);
            long second = r.NextInt64(_primaryNumber);
            return new Polynom1DegreeCoeffs(first, second);
        }
        public Polynom1DegreeCoeffs Mult(Polynom1DegreeCoeffs x, Polynom1DegreeCoeffs y)
        {
            long newFirstValue = ModGFP(x.Second * y.Second - x.First * y.Second - x.Second * y.First);
            long newSecondValue = ModGFP(x.First * y.First - x.First * y.Second - x.Second * y.First);
            //_log.Info($"({newFirstValue},{newSecondValue})");
            return new Polynom1DegreeCoeffs(newFirstValue, newSecondValue);
        }

        public Polynom1DegreeCoeffs Mult(long coeff, Polynom1DegreeCoeffs x)
        {
            long newFirstValue = ModGFP(coeff*x.First);
            long newSecondValue = ModGFP(coeff * x.Second);
            //_log.Info($"({newFirstValue},{newSecondValue})");
            return new Polynom1DegreeCoeffs(newFirstValue, newSecondValue);
        }

        public Polynom1DegreeCoeffs Add(Polynom1DegreeCoeffs x, Polynom1DegreeCoeffs y)
        {
            long newFirstValue = ModGFP(x.First + y.First);
            long newSecondValue = ModGFP(x.Second + y.Second);
            //_log.Info($"({newFirstValue},{newSecondValue})");
            return new Polynom1DegreeCoeffs(newFirstValue, newSecondValue);
        }

        public Polynom1DegreeCoeffs Substract(Polynom1DegreeCoeffs x, Polynom1DegreeCoeffs y)
        {
            long newFirstValue = ModGFP(x.First - y.First);
            long newSecondValue = ModGFP(x.Second - y.Second);
            //_log.Info($"({newFirstValue},{newSecondValue})");
            return new Polynom1DegreeCoeffs(newFirstValue, newSecondValue);
        }

        public Polynom1DegreeCoeffs Pow2(Polynom1DegreeCoeffs x)
        {
            long newFirstValue = ModGFP(x.Second * (x.Second - 2*x.First));
            long newSecondValue = ModGFP(x.First * (x.First - 2*x.Second));
            //_log.Info($"({newFirstValue},{newSecondValue})");
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
            long newFirstValue = ModGFP(z.First*(y.First - x.Second - y.Second) + z.Second*(x.Second - x.First + y.Second));
            long newSecondValue = ModGFP(z.First*(x.First- x.Second + y.First) + z.Second*(y.Second - x.First - y.First));
            //_log.Info($"({newFirstValue},{newSecondValue})");
            return new Polynom1DegreeCoeffs(newFirstValue, newSecondValue);
        }

        private void CreateGaloisFieldP2()
        {
            for(int i = 0; i < _primaryNumber; i++)
            {
                for (int j = 0; j < _primaryNumber; j++)
                {
                    _values[i * _primaryNumber + j] = new Polynom1DegreeCoeffs(i, j);
                    //_log.Info($"GF({_primaryNumber}^2)[{i * _primaryNumber + j}] = ({_values[i * _primaryNumber + j].First}, {_values[i * _primaryNumber + j].Second})");
                }
            }
        }

        
    }
}
