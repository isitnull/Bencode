using BenDecoder.src.Interfaces;
using System;

namespace BenDecoder.src.Types
{
    public class DecodedInteger : IDecodeble
    {
        readonly int _i;
        public DecodedInteger(int i)
        {
            _i = i;
        }

        public override bool Equals(object obj)
        {
            return obj is DecodedInteger integer &&
                   _i == integer._i;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_i);
        }

        public override string ToString()
        {
            return _i.ToString();
        }
    }

}
