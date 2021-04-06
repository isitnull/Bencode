using BenDecoder.src.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenDecoder.src.Types
{
    public class DecodedList : List<IDecodeble>, IDecodeble
    {
        public DecodedList()
        {
        }
    }
}
