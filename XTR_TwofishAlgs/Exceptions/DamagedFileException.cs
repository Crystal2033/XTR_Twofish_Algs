using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTR_TwofishAlgs.Exceptions
{
    public sealed class DamagedFileException : Exception
    {
        public DamagedFileException(string msg) : base(msg) { }
    }
}
