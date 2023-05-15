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
using XTR_TwofishAlgs.Exceptions;

namespace XTR_TWOFISH.FeistelImplementation
{
    public sealed class FeistelNetwork
    {

        private List<byte[]> _raundKeys;
        private readonly int _valueOfRaunds = 16;
        private readonly byte[] _mainKey;
        private List<byte[]> sBoxes;
        public byte[] MainKey{
            get =>
                MainKey;
            
            init {
                _mainKey = PrepareMainKey(value);
                _raundKeys = KeyExpander.GenerateRoundKeys(_mainKey, out sBoxes);
            }
        }

        public IKeyExpansion KeyExpander { get; init; }
        public IFeistelFunction FeistelFunction { get; init; }
        public FeistelNetwork(IKeyExpansion keyExpander, IFeistelFunction feistelFunction){
            KeyExpander = keyExpander;
            FeistelFunction = feistelFunction;
        }

        public byte[] Execute(in byte[] partOfText, CryptOperation cryptStatus)
        {
            List<byte[]> slicedBlocks = new();
            for (int i = 0; i < 4; i++)
            {
                slicedBlocks.Add(new byte[4] { partOfText[3*i + i % 4 + 3], partOfText[3 * i + (i % 4) + 2], partOfText[3 * i +(i % 4) + 1], partOfText[3 * i + (i % 4)] });
            }

            InputWhitenning(slicedBlocks, cryptStatus);

            for (int i = 0; i < _valueOfRaunds; i++)
            {
                byte[] savedR0 = (byte[])slicedBlocks[0].Clone();
                byte[] savedR1 = (byte[])slicedBlocks[1].Clone();
                (byte[] F0, byte[] F1) = FeistelFunction.FeistelFunction(slicedBlocks[0], slicedBlocks[1], GetRoundKeyByIndexAndOperation(2*i+8, cryptStatus), GetRoundKeyByIndexAndOperation(2*i + 9, cryptStatus), sBoxes); // not need i or let list<byte[]> keys in function

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
                
            }

            OutputWhitenning(slicedBlocks, cryptStatus);
            TransformOutputCryptText(slicedBlocks);

            return CryptSimpleFunctions.ConcatPureBytes(slicedBlocks);
        }

        private void TransformOutputCryptText(List<byte[]> cryptText)
        {
            for(int i = 0; i < cryptText.Count; i++)
            {
                cryptText[i] = CryptSimpleFunctions.RevertBytes(cryptText[i]);
            }
        }

        private byte[] PrepareMainKey(byte[] mainKey)
        {
            byte[] newKey;
            if (mainKey.Length == 16 || mainKey.Length == 24 || mainKey.Length == 32)
            {
                return mainKey;
            }
            else if(mainKey.Length > 32)
            {
                throw new MainKeyException("Your key is not compatible with algorithm. It`s too big. Your value should have 16, 24, 32 bytes length.");
            }
            else if(mainKey.Length > 24)
            {
                newKey = SupplementKeyWithZeroes(mainKey, 32);
                
            }
            else if(mainKey.Length > 16)
            {
                newKey = SupplementKeyWithZeroes(mainKey, 24);
            }
            else
            {
                newKey = SupplementKeyWithZeroes(mainKey, 16);
            }
            CryptSimpleFunctions.ClearBytes(mainKey, 0);
            return newKey;
        }

        private byte[] SupplementKeyWithZeroes(byte[] mainKey, int newKeySize)
        {
            byte[] newKey = new byte[newKeySize];
            for (int i = 0; i < newKeySize; i++)
            {
                if (i >= mainKey.Length)
                {
                    newKey[i] = 0;
                }
                else
                {
                    newKey[i] = mainKey[i];
                }
            }
            return newKey;
        }

        private void InputWhitenning(List<byte[]> textToCrypt, CryptOperation cryptStatus) 
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

        private void OutputWhitenning(List<byte[]> textToCrypt, CryptOperation cryptStatus)
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
