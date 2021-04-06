using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenDecoder.src.Exceptions
{
    public class BendecoderException : Exception
    {
        public BendecoderException(String reason) : base(reason)
        {
        }
    }

}
