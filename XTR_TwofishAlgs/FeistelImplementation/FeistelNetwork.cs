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
            for(int i = 0; i < slicedBlocks.Count; i++)
            {
                slicedBlocks[i] = CryptSimpleFunctions.RevertBytes(slicedBlocks[i]);
            }

            if (cryptStatus == CryptOperation.DECRYPT)
            {
                CryptSimpleFunctions.ShowHexView(slicedBlocks[0], "Cipher blocks[0]");
                CryptSimpleFunctions.ShowHexView(slicedBlocks[1], "Cipher blocks[1]");
                CryptSimpleFunctions.ShowHexView(slicedBlocks[2], "Cipher blocks[2]");
                CryptSimpleFunctions.ShowHexView(slicedBlocks[3], "Cipher blocks[3]");
            }
            byte[] whitenM0;
            byte[] whitenM1;
            byte[] whitenM2;
            byte[] whitenM3;
            if (cryptStatus == CryptOperation.DECRYPT)
            {
                whitenM0 = CryptSimpleFunctions.XorByteArrays(slicedBlocks[0], _raundKeys[4]); //whitening
                whitenM1 = CryptSimpleFunctions.XorByteArrays(slicedBlocks[1], _raundKeys[5]); //whitening
                whitenM2 = CryptSimpleFunctions.XorByteArrays(slicedBlocks[2], _raundKeys[6]); //whitening
                whitenM3 = CryptSimpleFunctions.XorByteArrays(slicedBlocks[3], _raundKeys[7]); //whitening
            }
            else
            {
                whitenM0 = CryptSimpleFunctions.XorByteArrays(slicedBlocks[0], _raundKeys[0]); //whitening
                whitenM1 = CryptSimpleFunctions.XorByteArrays(slicedBlocks[1], _raundKeys[1]); //whitening
                whitenM2 = CryptSimpleFunctions.XorByteArrays(slicedBlocks[2], _raundKeys[2]); //whitening
                whitenM3 = CryptSimpleFunctions.XorByteArrays(slicedBlocks[3], _raundKeys[3]); //whitening
            }
             
            CryptSimpleFunctions.ShowHexView(whitenM0, "Whitening");
            CryptSimpleFunctions.ShowHexView(whitenM1, "Whitening");
            CryptSimpleFunctions.ShowHexView(whitenM2, "Whitening");
            CryptSimpleFunctions.ShowHexView(whitenM3, "Whitening");

            byte[] resR0 = whitenM0;
            byte[] resR1 = whitenM1;
            byte[] resR2 = whitenM2;
            byte[] resR3 = whitenM3;

            for (int i = 0; i < _valueOfRaunds; i++)
            {
                byte[] savedR0 = (byte[])resR0.Clone();
                byte[] savedR1 = (byte[])resR1.Clone();
                (byte[] F0, byte[] F1) = FeistelFunction.FeistelFunction(resR0, resR1, GetRoundKeyByIndexAndOperation(2*i+8, cryptStatus), GetRoundKeyByIndexAndOperation(2*i + 9, cryptStatus), sBoxes); // not need i or let list<byte[]> keys in function
                CryptSimpleFunctions.ShowHexView(resR2, "Resr2");
                CryptSimpleFunctions.ShowHexView(resR3, "Resr3");
                if(cryptStatus == CryptOperation.DECRYPT)
                {
                    resR0 = CryptSimpleFunctions.XorByteArrays(CryptSimpleFunctions.CycleLeftShift(resR2, 32, 1), F0);
                }
                else
                {
                    resR0 = CryptSimpleFunctions.CycleRightShift(CryptSimpleFunctions.XorByteArrays(F0, resR2), 32, 1);
                }

                if (cryptStatus == CryptOperation.DECRYPT)
                {
                    resR1 = CryptSimpleFunctions.CycleRightShift(CryptSimpleFunctions.XorByteArrays(F1, resR3), 32, 1);
                }
                else
                {
                    resR1 = CryptSimpleFunctions.XorByteArrays(CryptSimpleFunctions.CycleLeftShift(resR3, 32, 1), F1);
                }
                
                resR2 = savedR0;
                resR3 = savedR1;
                List<byte[]> roundParts = new() { resR0, resR1, resR2, resR3 };
                for (int k = 0; k < roundParts.Count; k++)
                {
                    CryptSimpleFunctions.ShowHexView(roundParts[k], $"Round[{i}]");
                }
            }

            byte[] cipherResR0;
            byte[] cipherResR1;
            byte[] cipherResR2;
            byte[] cipherResR3;
            if(cryptStatus == CryptOperation.DECRYPT)
            {
                cipherResR0 = CryptSimpleFunctions.XorByteArrays(resR2, _raundKeys[0]); //outputwhitening
                cipherResR1 = CryptSimpleFunctions.XorByteArrays(resR3, _raundKeys[1]); //outputwhitening
                cipherResR2 = CryptSimpleFunctions.XorByteArrays(resR0, _raundKeys[2]); //outputwhitening
                cipherResR3 = CryptSimpleFunctions.XorByteArrays(resR1, _raundKeys[3]); //outputwhitening
            }                                                                             
            else                                                                          
            {                                                                             
                cipherResR0 = CryptSimpleFunctions.XorByteArrays(resR2, _raundKeys[4]); //outputwhitening
                cipherResR1 = CryptSimpleFunctions.XorByteArrays(resR3, _raundKeys[5]); //outputwhitening
                cipherResR2 = CryptSimpleFunctions.XorByteArrays(resR0, _raundKeys[6]); //outputwhitening
                cipherResR3 = CryptSimpleFunctions.XorByteArrays(resR1, _raundKeys[7]); //outputwhitening
            }

            List<byte[]> cipherParts = new() 
            {
                CryptSimpleFunctions.RevertBytes(cipherResR0), CryptSimpleFunctions.RevertBytes(cipherResR1)
                , CryptSimpleFunctions.RevertBytes(cipherResR2), CryptSimpleFunctions.RevertBytes(cipherResR3)
            };

            for(int i = 0; i < cipherParts.Count; i++)
            {
                CryptSimpleFunctions.ShowHexView(cipherParts[i], $"Cipher[{i}]");
            }
            return CryptSimpleFunctions.ConcatBitParts(cipherParts, 32);
        }


        private byte[] GetRoundKeyByIndexAndOperation(int index, CryptOperation cryptStatus)
        {
            if(cryptStatus == CryptOperation.ENCRYPT)
            {
                return _raundKeys[index];
            }
            else
            {
                if(index % 2 == 0)
                {
                    return _raundKeys[_raundKeys.Count - (index - 8) - 2];
                }
                else
                {
                    return _raundKeys[_raundKeys.Count - (index - 8)];
                }
                
                //return _raundKeys[(_raundKeys.Count - 1) - index + 8 - delta];
            }
        }

        
    }
}
