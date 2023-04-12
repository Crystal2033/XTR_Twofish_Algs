﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTR_TWOFISH.CypherEnums;
using XTR_TWOFISH.CryptInterfaces;

namespace XTR_TwofishAlgs.HelpFunctions
{
    internal static class CryptSimpleFunctions
    {

        public static void ShowBinaryView(in byte[] viewBytes, in string message = "")
        {
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine(message);
            for (int i = 0; i < viewBytes.Length; i++)
            {
                Console.Write(Convert.ToString(viewBytes[i], 2).PadLeft(CryptConstants.BITS_IN_BYTE, '0'));
                Console.Write(" ");
            }
            Console.WriteLine();
            Console.WriteLine("---------------------------------------------");
        }
        public static void ClearBytes(ref byte[] bytes, int startFrom = 0)
        {
            for (int i = startFrom; i < bytes.Length; i++)
            {
                bytes[i] = 0;
            }
        }

        //public static int GetPureTextWithoutPaddingSize(ref byte[] checkingBytes, FileDataLoader loader)
        //{
        //    int realCypherPartSize = CryptConstants.DES_PART_TEXT_BYTES;

        //    byte lastByteValue = checkingBytes[checkingBytes.Length - 1];
        //    if (lastByteValue < CryptConstants.DES_PART_TEXT_BYTES) //There is a padding PKCS7
        //    {
        //        loader.FactTextBlockSize -= lastByteValue;
        //        realCypherPartSize = CryptConstants.DES_PART_TEXT_BYTES - lastByteValue;
        //        ClearBytes(ref checkingBytes, checkingBytes.Length - lastByteValue);
        //    }
        //    return realCypherPartSize;
        //}

        public static byte GetBitFromPos(in byte myByte, byte index0FromLeft)
        {
            return (byte)(myByte >> (CryptConstants.BITS_IN_BYTE - index0FromLeft - 1) & 1);
        }

        public static void SetBitOnPos(ref byte myByte, byte index0FromLeft, byte currBit)
        {
            byte mask = (byte)(1 << (CryptConstants.BITS_IN_BYTE - index0FromLeft - 1));
            myByte = (byte)((myByte & ~mask) |
                  ((currBit << (CryptConstants.BITS_IN_BYTE - index0FromLeft - 1)) & mask));
        }

        public static void Permutation(ref byte[] bytes, in byte[] pBlock)
        {
            byte[] result = new byte[pBlock.Length / CryptConstants.BITS_IN_BYTE];
            for (byte i = 0; i < pBlock.Length; i++)
            {
                byte byteIndex = (byte)((pBlock[i] - 1) / CryptConstants.BITS_IN_BYTE);

                byte currBit = (byte)(bytes[byteIndex] >> ((byteIndex + 1) * CryptConstants.BITS_IN_BYTE - pBlock[i]) & 1);

                result[i / CryptConstants.BITS_IN_BYTE] = (byte)(result[i / CryptConstants.BITS_IN_BYTE] | (currBit << CryptConstants.BITS_IN_BYTE - (i % CryptConstants.BITS_IN_BYTE + 1)));
            }
            bytes = (byte[])result.Clone();
            ClearBytes(ref result);
        }


        public static void SliceArrayOnTwoArrays(in byte[] startBytes, int leftBlockSize, int rightBlockSize, out byte[] leftPart, out byte[] rightPart)
        {
            leftPart = new byte[(int)Math.Ceiling((double)leftBlockSize / (double)CryptConstants.BITS_IN_BYTE)];
            rightPart = new byte[(int)Math.Ceiling((double)rightBlockSize / (double)CryptConstants.BITS_IN_BYTE)];
            SetRangeOfBits(startBytes, 0, 0, leftBlockSize, leftPart, 0, 0);
            SetRangeOfBits(startBytes, leftBlockSize / CryptConstants.BITS_IN_BYTE, (byte)(leftBlockSize % CryptConstants.BITS_IN_BYTE),
                rightBlockSize, rightPart, 0, 0);
        }

        public static List<byte[]> SliceArrayOnArrays(in byte[] plainText, int valueOfBitsInText, int valueOfBlocks)
        {
            ShowBinaryView(plainText, " Plain text");
            int valueBitsInSlicedText = valueOfBitsInText / valueOfBlocks;
            int valueOfBytesInBlock = (int)Math.Ceiling((double)valueBitsInSlicedText / (double)CryptConstants.BITS_IN_BYTE);
            List<byte[]> slicedBytes = new();
            for(int i = 0; i < valueOfBlocks; i++)
            {
                slicedBytes.Add(new byte[valueOfBytesInBlock]);
                SetRangeOfBits(plainText, i * valueBitsInSlicedText / CryptConstants.BITS_IN_BYTE,
                    (byte)(valueBitsInSlicedText % CryptConstants.BITS_IN_BYTE), valueBitsInSlicedText, slicedBytes[i], 0, 0);
                ShowBinaryView(slicedBytes[i]);
            }
            return slicedBytes;
        }

        public static byte[] CycleLeftShift(in byte[] bytes, int sizeInBits, int leftShiftValue)
        {
            leftShiftValue = leftShiftValue % sizeInBits;
            byte[] result = new byte[bytes.Length];

            SetRangeOfBits(bytes, leftShiftValue / CryptConstants.BITS_IN_BYTE, (byte)(leftShiftValue % CryptConstants.BITS_IN_BYTE),
                sizeInBits - leftShiftValue, result, 0, 0);

            SetRangeOfBits(bytes, 0, 0, leftShiftValue, result,
                (sizeInBits - leftShiftValue) / CryptConstants.BITS_IN_BYTE, (byte)((sizeInBits - leftShiftValue) % CryptConstants.BITS_IN_BYTE));
            return result;
        }


        public static byte[] XorByteArrays(in byte[] first, in byte[] second)
        {
            if (first.Length != second.Length)
            {
                throw new ArgumentException("Two arrays are not compatible for XOR operation");
            }
            byte[] result = new byte[first.Length];
            for (int i = 0; i < first.Length; i++)
            {
                result[i] = (byte)(first[i] ^ second[i]);
            }
            return result;
        }
        public static byte[] ConcatTwoBitParts(in byte[] leftPart, int leftSize, in byte[] rightPart, int rightSize)
        {
            byte[] concatArr = new byte[(int)Math.Ceiling(((double)(leftSize + rightSize)) / (double)(CryptConstants.BITS_IN_BYTE))];
            SetRangeOfBits(leftPart, 0, 0, leftSize, concatArr, 0, 0);
            SetRangeOfBits(rightPart, 0, 0, rightSize, concatArr, leftSize / CryptConstants.BITS_IN_BYTE, (byte)(leftSize % CryptConstants.BITS_IN_BYTE));
            return concatArr;
        }

        public static byte[] ConcatBitParts(List<byte[]> parts, int bitSize)
        {
            byte[] concatArr = new byte[(int)Math.Ceiling(((double)(parts.Count * bitSize)) / (double)(CryptConstants.BITS_IN_BYTE))];
            for(int i =0; i < parts.Count; i++)
            {
                SetRangeOfBits(parts[i], 0, 0, bitSize, concatArr, i * bitSize / CryptConstants.BITS_IN_BYTE, (byte)((i * bitSize) % CryptConstants.BITS_IN_BYTE));
            }
            ShowBinaryView(concatArr);
            //SetRangeOfBits(leftPart, 0, 0, leftSize, concatArr, 0, 0);
            //SetRangeOfBits(rightPart, 0, 0, rightSize, concatArr, leftSize / CryptConstants.BITS_IN_BYTE, (byte)(leftSize % CryptConstants.BITS_IN_BYTE));
            return concatArr;
        }

        /**
         * copyFrom byte array from which copying bits
         * startByteFrom copyFrom start BYTE to copy
         * startBitFrom copyFrom start BIT to copy from 0 to 7 little-endian (0 1 2 3 4 5 6 7)
         * valueOfBits how many bits need to insert. Iterating [0;valueOfBits)
         * copyTo resultArr
         * startByteTo copyTo start BYTE to copy
         * startBitTo copyTo start BIT to copy from 0 to 7 little-endian (0 1 2 3 4 5 6 7)
         */
        public static void SetRangeOfBits(in byte[] copyFrom, int startByteFrom, byte startBitFrom, int valueOfBits,
                                         byte[] copyTo, int startByteTo, byte startBitTo)
        {
            for (int i = 0; i < valueOfBits; i++)
            {
                byte currBit = GetBitFromPos(copyFrom[startByteFrom + ((i + startBitFrom) / CryptConstants.BITS_IN_BYTE)], (byte)((startBitFrom + (i % CryptConstants.BITS_IN_BYTE)) % CryptConstants.BITS_IN_BYTE));
                SetBitOnPos(ref copyTo[startByteTo + ((i + startBitTo) / CryptConstants.BITS_IN_BYTE)],
                    (byte)((startBitTo + (i % CryptConstants.BITS_IN_BYTE)) % CryptConstants.BITS_IN_BYTE), currBit);
            }
        }

        public static void PKCS7Padding(byte[] bytes, int actualSize)
        {
            for (int i = actualSize; i < CryptConstants.DES_PART_TEXT_BYTES; i++)
            {
                bytes[i] = (byte)(CryptConstants.DES_PART_TEXT_BYTES - actualSize);
            }
        }

        public static byte[] GetBytesAfterCryptOperation(CryptOperation operation, ref byte[] partOfText, ISymmetricEncryption algorithm)
        {
            if (operation == CryptOperation.ENCRYPT)
            {
                return algorithm.Encrypt(ref partOfText);
            }
            else
            {
                return algorithm.Decrypt(ref partOfText);
            }
        }

        public static void GetLettersFromBytes(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                Console.Write((char)bytes[i]);
            }
            Console.WriteLine();
        }

        public static void GetNewPartOfText(in byte[] textInBytes, byte[] buffer, int startIndex)
        {
            int textSizeWithoutBlock = textInBytes.Length - startIndex;
            int pureTextSize = (textSizeWithoutBlock < CryptConstants.DES_PART_TEXT_BYTES && textSizeWithoutBlock != 0)
                ? textSizeWithoutBlock % CryptConstants.DES_PART_TEXT_BYTES
                : CryptConstants.DES_PART_TEXT_BYTES;

            Array.Copy(textInBytes, startIndex, buffer, 0, pureTextSize);

            if (pureTextSize != CryptConstants.DES_PART_TEXT_BYTES)
            {
                PKCS7Padding(buffer, pureTextSize);
            }
        }
    }
}