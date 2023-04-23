using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTR_TWOFISH.ThreadingWork
{
	public sealed class FileDataLoader
	{
		public long TextReadSize {get; set;}
		public static int TextBlockSize = 2048;
		private byte[] _textBlock = null;
		private int _factTextBlockSize;
		public int FactTextBlockSize { get { return _factTextBlockSize; } set { _factTextBlockSize = value; } }
			
		public byte[] TextBlock
		{
			get { return _textBlock ?? (_textBlock = new byte[TextBlockSize]); }
			set { _textBlock = value; }
		}

		private int currentPosInFile;
		public int CurrentPosInFile { get => currentPosInFile; set => currentPosInFile = value; }
		private BinaryReader _fileReadFrom;
		private BinaryWriter _fileWriteTo;


        public FileDataLoader(string fileReadFrom, string fileWriteTo)
		{
			FileStream readFileStream = File.Open(fileReadFrom, FileMode.Open);
			TextReadSize = readFileStream.Length;
            _fileReadFrom = new BinaryReader(readFileStream, Encoding.UTF8);

			if (File.Exists(fileWriteTo))
			{
                _fileWriteTo = new BinaryWriter(File.Open(fileWriteTo, FileMode.Truncate), Encoding.UTF8);
            }
			else
			{
                _fileWriteTo = new BinaryWriter(File.Open(fileWriteTo, FileMode.OpenOrCreate), Encoding.UTF8);
            }
            

            reloadTextBlockAndOutputInFile();
        }

		public void InsertHashValue(long hashValue)
		{
			_fileWriteTo.Write(hashValue);
		}

        public long GetHashValue()
        {
			return _fileReadFrom.ReadInt64();
        }
        public void reloadTextBlockAndOutputInFile() 
		{
			if(currentPosInFile != 0)
			{
				InsertTextBlockInFile();
            }

            _factTextBlockSize = _fileReadFrom.Read(TextBlock, 0, TextBlockSize);
			currentPosInFile += _factTextBlockSize;
        }

		public void CloseStreams()
		{
			_fileReadFrom.Close();
			_fileWriteTo.Close();

        }
        private void InsertTextBlockInFile()
        {
			if(_factTextBlockSize != 0)
			{
                _fileWriteTo.Write(TextBlock, 0, _factTextBlockSize);
			}
        }
    }
}
