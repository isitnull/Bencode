using BenDecoder.src.Exceptions;
using BenDecoder.src.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenDecoder.src
{
    public class BencodeStream
    {
        private Stream _stream;
        private Encoding _encoding;

        public BencodeStream(Stream stream, Encoding encoding)
        {
            _stream = stream;
            _encoding = encoding;
        }

        public BencodeStream(Stream stream)
        {
            _stream = stream;
            _encoding = Encoding.UTF8;
        }

        public async Task<char> Peek()
        {
            byte[] b = new byte[1];
            var read = await _stream.ReadAsync(b);

            if (read != 1)
            {
                throw new BendecoderException("Unexpectedly reached end of stream trying to Peek");
            }
            // Might throw here if the stream isn't seekable
            _stream.Position = --_stream.Position;

            return _encoding.GetChars(b)[0];
        }


        public async Task<bool> AtEndOfStream()
        {
            byte[] a = new byte[1];
            var bytesRead = await _stream.ReadAsync(a);
            _stream.Position = --_stream.Position;

            return bytesRead != 1;
        }

        public async Task<char> GetNextCharacter()
        {
            byte[] a = new byte[1];
            int read = await _stream.ReadAsync(a);
            if (read != 1)
            {
                throw new BendecoderException("Unexpectedly reached end of stream");
            }

            return _encoding.GetChars(a)[0];
        }

        public async Task<byte[]> GetBytes(int numBytes)
        {
            byte[] buffer = new byte[numBytes];
            var bytesRead = await _stream.ReadAsync(buffer);
            if (bytesRead != numBytes)
            {
                throw new BendecoderException($"Unexepectedly reached end of stream while reading {numBytes} bytes");
            }
            return buffer;
        }

    }
}
