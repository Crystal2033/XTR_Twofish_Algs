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
using XTR_TwofishAlgs.FeistelImplementation;

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
            List<byte[]> slicedBlocks = CryptSimpleFunctions.SliceArrayOnArrays(partOfText, 128, 4); //4 bytes in each block
            byte[] whitenM0 = CryptSimpleFunctions.XorByteArrays(slicedBlocks[0], _raundKeys[0]); //whitening
            byte[] whitenM1 = CryptSimpleFunctions.XorByteArrays(slicedBlocks[1], _raundKeys[1]); //whitening
            byte[] whitenM2 = CryptSimpleFunctions.XorByteArrays(slicedBlocks[2], _raundKeys[2]); //whitening
            byte[] whitenM3 = CryptSimpleFunctions.XorByteArrays(slicedBlocks[3], _raundKeys[3]); //whitening

            byte[] resR0 = whitenM0;
            byte[] resR1 = whitenM1;
            byte[] resR2 = whitenM2;
            byte[] resR3 = whitenM3;

            for (int i = 0; i < _valueOfRaunds; i++)
            {
                byte[] savedR0 = (byte[])resR0.Clone();
                byte[] savedR1 = (byte[])resR1.Clone();
                (byte[] F0, byte[] F1) = FeistelFunction.FeistelFunction(resR0, resR1, _raundKeys[2 * i + 8], _raundKeys[2 * i + 9], sBoxes, i);
                resR0 = CryptSimpleFunctions.CycleRightShift(CryptSimpleFunctions.XorByteArrays(F0, resR2), 32, 1);
                resR1 = CryptSimpleFunctions.XorByteArrays(CryptSimpleFunctions.CycleLeftShift(resR3, 32, 1), F1);
                resR2 = savedR0;
                resR3 = savedR1;
            }

            byte[] cipherResR0 = CryptSimpleFunctions.XorByteArrays(resR2, _raundKeys[4]); //whitening
            byte[] cipherResR1 = CryptSimpleFunctions.XorByteArrays(resR3, _raundKeys[5]); //whitening
            byte[] cipherResR2 = CryptSimpleFunctions.XorByteArrays(resR0, _raundKeys[6]); //whitening
            byte[] cipherResR3 = CryptSimpleFunctions.XorByteArrays(resR1, _raundKeys[7]); //whitening
            List<byte[]> cipherParts = new() { cipherResR0, cipherResR1, cipherResR2, cipherResR3 };

            return CryptSimpleFunctions.ConcatBitParts(cipherParts, 32);
        }


        
    }
}
