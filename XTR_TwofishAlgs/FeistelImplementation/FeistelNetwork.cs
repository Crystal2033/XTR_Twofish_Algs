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

            for (int k = 0; k < slicedBlocks.Count; k++)
            {
                CryptSimpleFunctions.ShowHexView(slicedBlocks[k], $"CryptedText[{k}]");
            }

            inputWhitenning(slicedBlocks, cryptStatus);

            CryptSimpleFunctions.ShowHexView(slicedBlocks[0], "Whitening");
            CryptSimpleFunctions.ShowHexView(slicedBlocks[1], "Whitening");
            CryptSimpleFunctions.ShowHexView(slicedBlocks[2], "Whitening");
            CryptSimpleFunctions.ShowHexView(slicedBlocks[3], "Whitening");

            for (int i = 0; i < _valueOfRaunds; i++)
            {
                byte[] savedR0 = (byte[])slicedBlocks[0].Clone();
                byte[] savedR1 = (byte[])slicedBlocks[1].Clone();
                (byte[] F0, byte[] F1) = FeistelFunction.FeistelFunction(slicedBlocks[0], slicedBlocks[1], GetRoundKeyByIndexAndOperation(2*i+8, cryptStatus), GetRoundKeyByIndexAndOperation(2*i + 9, cryptStatus), sBoxes); // not need i or let list<byte[]> keys in function
                CryptSimpleFunctions.ShowHexView(slicedBlocks[2], "Resr2");
                CryptSimpleFunctions.ShowHexView(slicedBlocks[3], "Resr3");
                if(cryptStatus == CryptOperation.DECRYPT)
                {
                    slicedBlocks[0] = CryptSimpleFunctions.XorByteArrays(CryptSimpleFunctions.CycleLeftShift(slicedBlocks[2], 32, 1), F0);
                }
                else
                {
                    slicedBlocks[0] = CryptSimpleFunctions.CycleRightShift(CryptSimpleFunctions.XorByteArrays(F0, slicedBlocks[2]), 32, 1);
                }

                if (cryptStatus == CryptOperation.DECRYPT)
                {
                    slicedBlocks[1] = CryptSimpleFunctions.CycleRightShift(CryptSimpleFunctions.XorByteArrays(F1, slicedBlocks[3]), 32, 1);
                }
                else
                {
                    slicedBlocks[1] = CryptSimpleFunctions.XorByteArrays(CryptSimpleFunctions.CycleLeftShift(slicedBlocks[3], 32, 1), F1);
                }

                slicedBlocks[2] = savedR0;
                slicedBlocks[3] = savedR1;
                
                for (int k = 0; k < slicedBlocks.Count; k++)
                {
                    CryptSimpleFunctions.ShowHexView(slicedBlocks[k], $"Round[{i}]");
                }
            }

            outputWhitenning(slicedBlocks, cryptStatus);
            for (int k = 0; k < slicedBlocks.Count; k++)
            {
                CryptSimpleFunctions.ShowHexView(slicedBlocks[k], $"OutpuWhiten");
            }

            transformOutputCryptText(slicedBlocks);


            for(int i = 0; i < slicedBlocks.Count; i++)
            {
                CryptSimpleFunctions.ShowHexView(slicedBlocks[i], $"Cipher[{i}]");
            }
            return CryptSimpleFunctions.ConcatBitParts(slicedBlocks, 32);
        }

        private void transformOutputCryptText(List<byte[]> cryptText)
        {
            for(int i = 0; i < cryptText.Count; i++)
            {
                cryptText[i] = CryptSimpleFunctions.RevertBytes(cryptText[i]);
            }
        }
        private void inputWhitenning(List<byte[]> textToCrypt, CryptOperation cryptStatus) 
        {
            if (cryptStatus == CryptOperation.ENCRYPT)
            {
                textToCrypt[0] = CryptSimpleFunctions.XorByteArrays(textToCrypt[0], _raundKeys[0]); //whitening
                textToCrypt[1] = CryptSimpleFunctions.XorByteArrays(textToCrypt[1], _raundKeys[1]); //whitening
                textToCrypt[2] = CryptSimpleFunctions.XorByteArrays(textToCrypt[2], _raundKeys[2]); //whitening
                textToCrypt[3] = CryptSimpleFunctions.XorByteArrays(textToCrypt[3], _raundKeys[3]); //whitening
            }
            else
            {
                textToCrypt[0] = CryptSimpleFunctions.XorByteArrays(textToCrypt[0], _raundKeys[4]); //whitening
                textToCrypt[1] = CryptSimpleFunctions.XorByteArrays(textToCrypt[1], _raundKeys[5]); //whitening
                textToCrypt[2] = CryptSimpleFunctions.XorByteArrays(textToCrypt[2], _raundKeys[6]); //whitening
                textToCrypt[3] = CryptSimpleFunctions.XorByteArrays(textToCrypt[3], _raundKeys[7]); //whitening
            }
        }

        private void outputWhitenning(List<byte[]> textToCrypt, CryptOperation cryptStatus)
        {
            byte[] savedBytes0 = (byte[])textToCrypt[0].Clone();
            byte[] savedBytes1 = (byte[])textToCrypt[1].Clone();
            if (cryptStatus == CryptOperation.ENCRYPT)
            {
                textToCrypt[0] = CryptSimpleFunctions.XorByteArrays(textToCrypt[2], _raundKeys[4]); //whitening
                textToCrypt[1] = CryptSimpleFunctions.XorByteArrays(textToCrypt[3], _raundKeys[5]); //whitening
                textToCrypt[2] = CryptSimpleFunctions.XorByteArrays(savedBytes0, _raundKeys[6]); //whitening
                textToCrypt[3] = CryptSimpleFunctions.XorByteArrays(savedBytes1, _raundKeys[7]); //whitening
            }
            else
            {
                textToCrypt[0] = CryptSimpleFunctions.XorByteArrays(textToCrypt[2], _raundKeys[0]); //whitening
                textToCrypt[1] = CryptSimpleFunctions.XorByteArrays(textToCrypt[3], _raundKeys[1]); //whitening
                textToCrypt[2] = CryptSimpleFunctions.XorByteArrays(savedBytes0, _raundKeys[2]); //whitening
                textToCrypt[3] = CryptSimpleFunctions.XorByteArrays(savedBytes1, _raundKeys[3]); //whitening
            }
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
            }
        }

        
    }
}
