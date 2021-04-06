using BenDecoder.src;
using BenDecoder.src.Constants;
using BenDecoder.src.Exceptions;
using BenDecoder.src.ExtensionMethods;
using BenDecoder.src.Interfaces;
using BenDecoder.src.Types;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bencoder
{
    public class Decoder
    {
        private BencodeStream _bStream;
        List<IDecodeble> result = new();
        public Decoder(BencodeStream bStream)
        {
            _bStream = bStream;
        }

        public async Task<List<IDecodeble>> DecodeStream()
        {

            while(! await _bStream.AtEndOfStream())
            {
                IDecodeble decoded = await Decode(await _bStream.Peek());

                result.Add(decoded);

            }

            return result;

        }

        private async Task<IDecodeble> Decode(char currentCharacter)
        {
            return currentCharacter switch
            {
                char _ when currentCharacter.IsIntegerStart() => await this.DecodeInteger(),
                char _ when currentCharacter.IsByteStringStart() => await this.DecodeByteString(),
                char _ when currentCharacter.IsListStart() => await this.DecodeList(),
                char _ when currentCharacter.IsDictionaryStart() => await this.DecodeDictionary(),
                _ => throw new BendecoderException($"Unrecognized start of token {currentCharacter}")
            };
        }

        private async Task<IDecodeble> DecodeDictionary()
        {
            var dict = new DecodedDictionary();

            var currentCharacter = await _bStream.GetNextCharacter();

            while(currentCharacter != Constants.END)
            {
                // decode bytestring for the key
                var key = await DecodeByteString() as DecodedByteString;

                // decode the value (anything)
                var value = await Decode(await _bStream.Peek());
                dict[key] = value;

                currentCharacter = await _bStream.Peek();
            }

            await _bStream.GetNextCharacter();


            return dict;
       }

        private async Task<IDecodeble> DecodeInteger()
        {
            var stringBuilder = new StringBuilder();
            await _bStream.GetNextCharacter(); 
            char currentChar = await _bStream.GetNextCharacter();

            if (currentChar == '-')
            {
                stringBuilder.Append(currentChar);
                currentChar = await _bStream.GetNextCharacter();
                if (currentChar == '0')
                {
                    throw new BendecoderException("-0 is not allowed in Integers");
                }
            } else if (currentChar == '0')
            {
                currentChar = await _bStream.GetNextCharacter();
                if (currentChar != 'e')
                {
                    throw new BendecoderException("No leading 0's allowed");
                }
                return new DecodedInteger(0);
            }

            while (currentChar != Constants.END)
            {
                stringBuilder.Append(currentChar);
                currentChar = await _bStream.GetNextCharacter();
            }

            return new DecodedInteger(int.Parse(stringBuilder.ToString()));
        }

        private async Task<IDecodeble> DecodeByteString()
        {
           
            StringBuilder stringLength = new();
            
            stringLength.Append(await _bStream.GetNextCharacter());
            // keep going until we hit a non digit

            char currentCharacter = await _bStream.GetNextCharacter();
            while(char.IsDigit(currentCharacter))
            {
                stringLength.Append(currentCharacter);
                currentCharacter = await _bStream.GetNextCharacter();
            }

            if(!int.TryParse(stringLength.ToString(), out int length))
            {
                throw new BendecoderException($"Bytestring length could not be parsed {stringLength}");
            }


            var content = await _bStream.GetBytes(length);

            return new DecodedByteString(content);
        }

        private async Task<IDecodeble> DecodeList()
        {
            DecodedList l = new DecodedList();
            // go past the l
            await _bStream.GetNextCharacter();

            char nextChar = await _bStream.Peek();
            
            while(nextChar != Constants.END)
            {
                l.Add(await Decode(nextChar));
                nextChar = await _bStream.Peek();
            }

            await _bStream.GetNextCharacter();


            return l;
        }
    }
}
