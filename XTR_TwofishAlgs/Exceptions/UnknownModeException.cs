using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTR_TwofishAlgs.Exceptions
{
    public sealed class UnknownModeException : Exception
    {
        public UnknownModeException(string msg) : base(msg) { }
    }
}
