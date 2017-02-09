using System.IO;

namespace PsdSharp.IO
{
    public static class Extensions
    {
        public static string ReadPascalString(this BinaryReader reader)
        {
            byte stringLength = reader.ReadByte();
            char[] c = reader.ReadChars(stringLength);

            return new string(c);
        }

        public static string ReadPaddedPascalString(this BinaryReader reader)
        {
            string s = reader.ReadPascalString();

            if (s.Length % 2 == 0) reader.ReadByte();

            return s;
        }
    }
}