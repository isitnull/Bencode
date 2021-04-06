using BenDecoder.src.Constants;
using System.IO;

namespace BenDecoder.src.ExtensionMethods
{

    public static class ExtensionMethods
    {
        public static bool IsIntegerStart(this char c) => c == Constants.Constants.INTEGER_START;
        public static bool IsByteStringStart(this char c) => char.IsDigit(c);
        public static bool IsListStart(this char c) => c == Constants.Constants.LIST_START;
        public static bool IsDictionaryStart(this char c) => c == Constants.Constants.DICTIONARY_START;

    }
}
