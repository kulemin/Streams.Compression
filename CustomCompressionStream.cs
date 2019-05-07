using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streams.Compression
{
    public class CustomCompressionStream : Stream
    {
        Stream stream;
        bool firstWriting = true;
        static Queue<int> zip = new Queue<int>();
        int bufferCount = 0;
        int value = 0;
        public CustomCompressionStream(MemoryStream stream, bool b)
        {
            this.stream = new BufferedStream(stream);
        }

        public override bool CanRead => throw new NotImplementedException();

        public override bool CanSeek => throw new NotImplementedException();

        public override bool CanWrite => throw new NotImplementedException();

        public override long Length => throw new NotImplementedException();

        public override long Position
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public override void Flush()
        {
            stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            for (int i = 0; i < buffer.Length;)
                if (zip.Count() > 0)
                {
                    if (bufferCount == 0)
                    {
                        bufferCount = -zip.Dequeue();
                        value = zip.Dequeue();
                    }
                    var u = bufferCount;
                    for (int j = 0; j < u; j++)
                    {
                        buffer[i] = (byte)value;
                        i++;
                        bufferCount--;
                        if (i >= buffer.Length) break;
                    }
                }
                else
                {
                    stream.Flush();
                    return i;
                }
            return buffer.Length;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            int index = 1;
            int lastByte = 0;
            foreach (var e in buffer)
            {
                if (firstWriting)
                {
                    lastByte = e;
                    firstWriting = false;
                    continue;
                }

                if (e == lastByte) index++;
                else
                {
                    zip.Enqueue(-index);
                    zip.Enqueue(lastByte);
                    lastByte = e;
                    index = 1;
                }
            }

            zip.Enqueue(-index);
            zip.Enqueue(lastByte);
        }
    }
}