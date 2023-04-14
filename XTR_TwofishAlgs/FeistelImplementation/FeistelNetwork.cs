using XTR_TWOFISH.CypherEnums;
using XTR_TWOFISH.CryptInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTR_TwofishAlgs.HelpFunctions;
using XTR_TwofishAlgs.KeySchedule;
using static XTR_TwofishAlgs.HelpFunctions.CryptConstants;

namespace XTR_TWOFISH.FeistelImplementation
{
    public sealed class FeistelNetwork
    {

        private List<byte[]> _raundKeys;
        private readonly int _valueOfRaunds = 16;
        private readonly byte[] _mainKey;
        private readonly TwoFishKeySizes keySize=TwoFishKeySizes.EASY;
        private List<byte[]> sBoxes;
        public byte[] MainKey{
            get =>
                MainKey;
            
            init {
                _mainKey = value;
                _raundKeys = KeyExpander.GenerateRoundKeys(_mainKey, keySize, out sBoxes);
            }
        }

        public IKeyExpansion KeyExpander { get; init; }
        public IFeistelFunction FeistelFunction { get; init; }
        public FeistelNetwork(IKeyExpansion keyExpander, IFeistelFunction feistelFunction, params object[] additionalInfo){
            KeyExpander = keyExpander;
            FeistelFunction = feistelFunction;
            if (additionalInfo.Length != 0)
            {
                if (additionalInfo[0] is TwoFishKeySizes newKeySize)
                {
                    keySize = newKeySize;
                }
            }

        }

        public byte[] Execute(in byte[] partOfText, int sizeInBits, CryptOperation cryptStatus)
        {
            List<byte[]> slicedBlocks = CryptSimpleFunctions.SliceArrayOnArrays(partOfText, 128, 4);

            //for(int i = 0; i < _valueOfRaunds; i++){
            //    nextLeftPart = (byte[])rightPart.Clone();
            //    nextRightPart = CryptSimpleFunctions.XorByteArrays(leftPart, 
            //        FeistelFunction.FeistelFunction(ref rightPart, _raundKeys[(cryptStatus == CryptOperation.ENCRYPT) ? i : _valueOfRaunds - i -1], i));

            //    leftPart = nextLeftPart;
            //    rightPart = nextRightPart;
            //}

            return CryptSimpleFunctions.ConcatBitParts(slicedBlocks, 32);
        }


        
    }
}
