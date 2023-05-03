using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTR_TwofishAlgs.MathBase
{
    public static class StandartMathTricks
    {
        public static int getHighest2DegreeValue(uint value)
        {
            int degree = 0;
            while (value != 1)
            {
                value >>= 1;
                degree++;
            }
            return degree;
        }
    }
}
