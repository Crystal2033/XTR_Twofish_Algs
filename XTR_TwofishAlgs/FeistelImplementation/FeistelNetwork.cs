using XTR_TWOFISH.CypherEnums;
using XTR_TWOFISH.CryptInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTR_TwofishAlgs.HelpFunctions;

namespace XTR_TWOFISH.FeistelImplementation
{
    public sealed class FeistelNetwork
    {

        private List<byte[]> _raundKeys;
        private readonly int _valueOfRaunds = 16;
        private readonly byte[] _mainKey;
        public byte[] MainKey{
            get =>
                MainKey;
            
            init {
                _mainKey = value;
                _raundKeys = KeyExpander.GenerateRoundKeys(_mainKey);
            }
        }

        public IKeyExpansion KeyExpander { get; init; }
        public IFeistelFunction FeistelFunction { get; init; }
        public FeistelNetwork(IKeyExpansion keyExpander, IFeistelFunction feistelFunction){
            KeyExpander = keyExpander;
            FeistelFunction = feistelFunction;
        }

        public byte[] Execute(in byte[] partOfText, int sizeInBits, CryptOperation cryptStatus)
        {
            CryptSimpleFunctions.SliceArrayOnTwoArrays(partOfText, sizeInBits / 2, sizeInBits / 2, out byte[] leftPart, out byte[] rightPart);
            byte[] nextLeftPart = default;
            byte[] nextRightPart = default;

            for(int i = 0; i < _valueOfRaunds; i++){
                nextLeftPart = (byte[])rightPart.Clone();
                nextRightPart = CryptSimpleFunctions.XorByteArrays(leftPart, 
                    FeistelFunction.FeistelFunction(ref rightPart, _raundKeys[(cryptStatus == CryptOperation.ENCRYPT) ? i : _valueOfRaunds - i -1], i));

                leftPart = nextLeftPart;
                rightPart = nextRightPart;
            }

            return CryptSimpleFunctions.ConcatTwoBitParts(rightPart, sizeInBits / 2, leftPart, sizeInBits / 2);
        }

    }
}
