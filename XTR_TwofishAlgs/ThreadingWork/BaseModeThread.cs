using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XTR_TWOFISH.CryptInterfaces;

namespace XTR_TWOFISH.ThreadingWork
{
    public abstract class BaseModeThread
    {
        protected FileDataLoader _loader;
        protected int _threadId;
        protected ISymmetricEncryption _algorithm;
        protected Barrier _barrier;
        protected int _bytesTransformed;
        protected int _textBlockSizeInBytes;

        protected int BytesTransformedInBlock { get => _bytesTransformed; set { _bytesTransformed = value; } }
        public BaseModeThread(int id, FileDataLoader loader, ISymmetricEncryption algorithm, Barrier barrier, int textBlockSizeInBytes)
        {
            _threadId = id;
            _algorithm = algorithm;
            _barrier = barrier;
            _loader = loader;
            _textBlockSizeInBytes = textBlockSizeInBytes;
        }

        public void SetThreadToStartPosition()
        {
            BytesTransformedInBlock = 0;
        }
        public abstract void Run(object obj=null);
    }
}
