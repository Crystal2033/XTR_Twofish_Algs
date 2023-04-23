using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTR_TwofishAlgs.Exceptions
{
    public sealed class GaloisOutOfFieldException : Exception
    {
        public GaloisOutOfFieldException(string msg): base(msg) { }
    }
}
