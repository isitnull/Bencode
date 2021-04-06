using BenDecoder.src.Interfaces;
using System.Collections.Generic;

namespace BenDecoder.src.Types
{
    public class DecodedDictionary : Dictionary<DecodedByteString, IDecodeble>, IDecodeble
    {
        public DecodedDictionary()
        {
        }
    }
}
