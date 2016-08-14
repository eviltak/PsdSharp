using System;
using System.IO;

namespace PsdSharp.IO
{
    /// <summary>
    /// Reads values stored in big endian from the given stream.
    /// </summary>
    internal class BigEndianBinaryReader : BinaryReader
    {
        public BigEndianBinaryReader(Stream input)
            : base(input)
        {
        }

        public override int ReadInt32()
        {
            byte[] buffer = ReadBytes(4);
            Array.Reverse(buffer);
            return BitConverter.ToInt32(buffer, 0);
        }

        public override short ReadInt16()
        {
            byte[] buffer = ReadBytes(2);
            Array.Reverse(buffer);
            return BitConverter.ToInt16(buffer, 0);
        }

        public override long ReadInt64()
        {
            byte[] buffer = ReadBytes(8);
            Array.Reverse(buffer);
            return BitConverter.ToInt64(buffer, 0);
        }

        public override ushort ReadUInt16()
        {
            return (ushort) ReadInt16();
        }

        public override uint ReadUInt32()
        {
            return (uint) ReadInt32();
        }

        public override ulong ReadUInt64()
        {
            return (ulong) ReadInt64();
        }

        public string ReadPascalString()
        {
            byte stringLength = ReadByte();
            char[] c = ReadChars(stringLength);

            return new string(c);
        }

        public string ReadPaddedPascalString()
        {
            string s = ReadPascalString();

            if (s.Length % 2 == 0) ReadByte();

            return s;
        }
    }
}