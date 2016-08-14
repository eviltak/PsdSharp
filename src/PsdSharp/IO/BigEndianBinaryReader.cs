// Copyright (c) 2016 Arav Singhal
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of PsdSharp and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation but with attribution the rights
// to use, copy, modify, merge and/or publish copies of the Software but NOT distribute, sublicense 
// or sell copies of the Software without prior permission and attribution of the author(s), and to 
// permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software. 
// Furthermore, the above copyright notice shall not be removed from this file.
// 
// Include the MIT License NO WARRANTY clause here.

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