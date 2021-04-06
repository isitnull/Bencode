using Bencoder;
using BenDecoder.src.Exceptions;
using BenDecoder.src.Types;
using System.IO;
using Xunit;
using BenDecoder.src;

namespace BenDecoder
{
    public class UnitTest1
    {
        [Fact]
        public async void Decoder_canDecode_properlyFormattedInt()
        {
            var decoder = new Decoder(new BencodeStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes("i123e")), System.Text.Encoding.UTF8));
            var result = await decoder.DecodeStream();
            Assert.True(result.Count == 1);
            Assert.Equal("123", result[0].ToString());
        }

        [Fact]
        public async void Decoder_canDecode_properlyFormattedNegativeInt()
        {
            var decoder = new Decoder(new BencodeStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes("i-123e")), System.Text.Encoding.UTF8));
            var result = await decoder.DecodeStream();
            Assert.True(result.Count == 1);
            Assert.Equal("-123", result[0].ToString());
        }

        [Fact]
        public async void Decoder_canDecode_properlyFormattedZero()
        {
            var decoder = new Decoder(new BencodeStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes("i0e")), System.Text.Encoding.UTF8));
            var result = await decoder.DecodeStream();
            Assert.True(result.Count == 1);
            Assert.Equal("0", result[0].ToString());
        }

        [Fact]
        public void Decoder_throwsException_leadingZero()
        {
            var decoder = new Decoder(new BencodeStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes("i023e")), System.Text.Encoding.UTF8)); 
            Assert.ThrowsAsync<BendecoderException>(async () => await decoder.DecodeStream());
        }

        [Fact]
        public void Decoder_throwsException_negativeZero()
        {
            var decoder = new Decoder(new BencodeStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes("i-0e")), System.Text.Encoding.UTF8));
            Assert.ThrowsAsync<BendecoderException>(async () => await decoder.DecodeStream());
        }

        [Fact]
        public async void Decoder_decodesString()
        {
            var decoder = new Decoder(new BencodeStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes("3:cat")), System.Text.Encoding.UTF8));
            var result = await decoder.DecodeStream();
            Assert.True(result.Count == 1);
            Assert.Equal("cat", ((DecodedByteString)result[0]).ToString(System.Text.Encoding.UTF8));

        }

        [Fact]
        public async void Decoder_decodesString2()
        {
            var decoder = new Decoder(new BencodeStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes("13:something??`1")), System.Text.Encoding.UTF8));
            var result = await decoder.DecodeStream();
            Assert.True(result.Count == 1);
            Assert.Equal("something??`1", ((DecodedByteString)result[0]).ToString(System.Text.Encoding.UTF8));

        }

        [Fact]
        public async void Decoder_decodesListWithStringItem()
        {
            var decoder = new Decoder(new BencodeStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes("l3:cat4:doge2:dee")), System.Text.Encoding.UTF8));
            var result = await decoder.DecodeStream();
            Assert.True(result.Count == 1);
            DecodedList l = Assert.IsType<DecodedList>(result[0]);
            Assert.Equal(3, l.Count);
            Assert.True(EncodeStringUTF8(((DecodedByteString)l[0]).GetBytes()) == "cat");
            Assert.True(EncodeStringUTF8(((DecodedByteString)l[1]).GetBytes()) == "doge");
        }

        [Fact]
        public async void Decoder_decodesListWithAList()
        {
            var decoder = new Decoder(new BencodeStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes("ll3:cat4:doge2:de3:cowee")), System.Text.Encoding.UTF8));
            var result = await decoder.DecodeStream();
            Assert.True(result.Count == 1);
            DecodedList outerList = Assert.IsType<DecodedList>(result[0]);
            Assert.Equal(1, outerList.Count);
            DecodedList innerList = Assert.IsType<DecodedList>(outerList[0]);
            DecodedByteString cow = Assert.IsType<DecodedByteString>(innerList[3]);
            Assert.Equal("cow", EncodeStringUTF8(cow.GetBytes()));
            Assert.Equal(4, innerList.Count);
            Assert.True(EncodeStringUTF8(((DecodedByteString)innerList[0]).GetBytes()) == "cat");
            Assert.True(EncodeStringUTF8(((DecodedByteString)innerList[1]).GetBytes()) == "doge");
        }

        [Fact]
        public async void Decoder_decodesDictionaryWithOneItem()
        {
            var decoder = new Decoder(new BencodeStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes("d3:bar4:spam3:fooi42ee")), System.Text.Encoding.UTF8));
            var result = await decoder.DecodeStream();

            Assert.True(result.Count == 1);
            DecodedDictionary dictionary = Assert.IsType<DecodedDictionary>(result[0]);

            Assert.Equal(2, dictionary.Count);
            dictionary.ContainsKey(new DecodedByteString(System.Text.Encoding.UTF8.GetBytes("bar")));
            dictionary.ContainsKey(new DecodedByteString(System.Text.Encoding.UTF8.GetBytes("foo")));

        }

        [Fact]
        public async void Decoder_decodesMoreThanOneItem()
        {
            var decoder = new Decoder(new BencodeStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes("3:cat3:cat3:cat")), System.Text.Encoding.UTF8));
            var result = await decoder.DecodeStream();

            Assert.True(result.Count == 3);
            var r0 = Assert.IsType<DecodedByteString>(result[0]);
            Assert.Equal("cat", EncodeStringUTF8(r0.GetBytes()));


        }

        private static string EncodeStringUTF8(byte[] bytes)
        {
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
    }
}
