
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTR_TWOFISH.ThreadingWork;
using XTR_TwofishAlgs.HelpFunctions;

namespace XTR_TWOFISH.HelpFunctionsAndData
{
    public static class TextBlockOperations
    {
        public static byte[] GetPartOfTextBlock(long posInTextBlock, FileDataLoader loader, int textBlockSize)
        {
            byte[] partOfTextBlock = new byte[textBlockSize];
            long readTextSize = (loader.FactTextBlockSize - posInTextBlock < textBlockSize) ? loader.FactTextBlockSize - posInTextBlock : partOfTextBlock.Length;
            for (int i = 0; i < readTextSize; i++)
            {
                partOfTextBlock[i] = loader.TextBlock[posInTextBlock + i];
            }
            loader.FactTextBlockSize += partOfTextBlock.Length - readTextSize;
            CryptSimpleFunctions.PKCS7Padding(partOfTextBlock, readTextSize);
            return partOfTextBlock;
        }

        public static byte[] GetPartOfTextBlockWithoutPadding(int posInTextBlock, byte[] textBlock, int textBlockSize)
        {
            byte[] partOfTextBlock = new byte[textBlockSize];
            for (int i = 0; i < partOfTextBlock.Length; i++)
            {
                partOfTextBlock[i] = textBlock[posInTextBlock + i];
            }
            return partOfTextBlock;
        }

        public static void InsertPartInTextBlock(long posInTextBlock, byte[] source, long sourceSize, FileDataLoader loader)
        {
            for (long i = 0; i < sourceSize; i++)
            {
                loader.TextBlock[posInTextBlock + i] = source[i];
            }
        }
    }
}
