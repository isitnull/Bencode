using BenDecoder.src.Interfaces;
using System.Text;

namespace BenDecoder.src.Types
{
    public class DecodedByteString : IDecodeble
    {
        readonly byte[] _byteString;

        public DecodedByteString(byte[] bytes)
        {
            _byteString = bytes;
        }

        public string ToString(Encoding encoding)
        {
            return encoding.GetString(_byteString);
        }

        public byte[] GetBytes()
        {
            return _byteString;
        }

    }

}
