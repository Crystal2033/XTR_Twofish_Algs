using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTR_TWOFISH.CryptInterfaces;
using XTR_TwofishAlgs.HelpFunctions;

namespace XTR_TwofishAlgs.FeistelImplementation
{
    internal class TwofishFeistelFuncImpl : IFeistelFunction
    {
        public byte[] FeistelFunction(ref byte[] R0, ref byte[] R1, in byte[] raundKey, int raundNumber)
        {
            byte[] T0 = gFunction(R0);
            byte[] T1 = gFunction(CryptSimpleFunctions.CycleLeftShift(R1, 32, 8));

            return new byte[1];
        }

        private byte[] gFunction(byte[] bytes) //bytes here is 32 bits value (1/4 from part of plain text)
        {
            List<byte[]> splitedBytes = CryptSimpleFunctions.SliceArrayOnArrays(bytes, 32, 4);
            return new byte[10];
        }
    }
}
