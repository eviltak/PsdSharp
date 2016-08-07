/////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006, Frank Blumenberg
//
// See License.txt for complete licensing and attribution information.
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

/////////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////////
//
// This code is adapted from code in the Endogine sprite engine by Jonas Beckeman.
// http://www.endogine.com/CS/
//

/////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;

namespace PsdSharp
{
    public class BinaryReverseReader : BinaryReader
    {
        public BinaryReverseReader(Stream stream)
            : base(stream)
        {
        }

        public override Int16 ReadInt16()
        {
            return Utilities.SwapBytes(base.ReadInt16());
        }

        public override Int32 ReadInt32()
        {
            return Utilities.SwapBytes(base.ReadInt32());
        }

        public override Int64 ReadInt64()
        {
            return Utilities.SwapBytes(base.ReadInt64());
        }

        public override UInt16 ReadUInt16()
        {
            return Utilities.SwapBytes(base.ReadUInt16());
        }

        public override UInt32 ReadUInt32()
        {
            return Utilities.SwapBytes(base.ReadUInt32());
        }

        public override UInt64 ReadUInt64()
        {
            return Utilities.SwapBytes(base.ReadUInt64());
        }

        public String ReadPascalString()
        {
            Byte stringLength = base.ReadByte();

            Char[] c = base.ReadChars(stringLength);

            if ((stringLength % 2) == 0) base.ReadByte();

            return new String(c);
        }
    }
    /// <summary>
    /// Writes primitive data types as binary values in in big-endian format
    /// </summary>
    public class BinaryReverseWriter : BinaryWriter
    {
        public Boolean AutoFlush { get; set; }

        public BinaryReverseWriter(Stream stream)
            : base(stream)
        {

        }

        public void WritePascalString(String s)
        {
            Char[] c = s.Length > 255 ? s.Substring(0, 255).ToCharArray() : s.ToCharArray();

            base.Write((Byte)c.Length);
            base.Write(c);

            Int32 realLength = c.Length + 1;

            if ((realLength % 2) == 0) return;

            for (Int32 i = 0; i < (2 - (realLength % 2)); i++) base.Write((Byte)0);

            if (AutoFlush) Flush();
        }

        public override void Write(Int16 val)
        {
            val = Utilities.SwapBytes(val);
            base.Write(val);

            if (AutoFlush) Flush();
        }

        public override void Write(Int32 val)
        {
            val = Utilities.SwapBytes(val);
            base.Write(val);

            if (AutoFlush) Flush();
        }

        public override void Write(Int64 val)
        {
            val = Utilities.SwapBytes(val);
            base.Write(val);

            if (AutoFlush) Flush();
        }

        public override void Write(UInt16 val)
        {
            val = Utilities.SwapBytes(val);
            base.Write(val);

            if (AutoFlush) Flush();
        }

        public override void Write(UInt32 val)
        {
            val = Utilities.SwapBytes(val);
            base.Write(val);

            if (AutoFlush) Flush();
        }

        public override void Write(UInt64 val)
        {
            val = Utilities.SwapBytes(val);
            base.Write(val);

            if (AutoFlush) Flush();
        }
    }
    class LengthWriter : IDisposable
    {
        #region Fields

        BinaryReverseWriter m_writer;
        long m_lengthPosition = long.MinValue;
        long m_startPosition;

        #endregion Fields

        #region Constructors

        public LengthWriter(BinaryReverseWriter writer)
        {
            m_writer = writer;

            // we will write the correct length later, so remember
            // the position
            m_lengthPosition = m_writer.BaseStream.Position;
            m_writer.Write((uint)0xFEEDFEED);

            // remember the start  position for calculation Image
            // resources length
            m_startPosition = m_writer.BaseStream.Position;
        }

        #endregion Constructors

        #region Methods

        #region Public Methods

        public void Dispose()
        {
            Write();
        }

        public void Write()
        {
            if (m_lengthPosition != long.MinValue)
            {
                long endPosition = m_writer.BaseStream.Position;

                m_writer.BaseStream.Position = m_lengthPosition;
                long length = endPosition - m_startPosition;
                m_writer.Write((uint)length);
                m_writer.BaseStream.Position = endPosition;

                m_lengthPosition = long.MinValue;
            }
        }

        #endregion Public Methods

        #endregion Methods
    }

    public class Utilities
    {
        public static UInt16 SwapBytes(UInt16 x)
        {
            return (ushort)((ushort)((x & 0xff) << 8) | ((x >> 8) & 0xff));
        }

        public static UInt32 SwapBytes(UInt32 x)
        {
            // swap adjacent 16-bit blocks
            x = (x >> 16) | (x << 16);
            // swap adjacent 8-bit blocks
            return ((x & 0xFF00FF00) >> 8) | ((x & 0x00FF00FF) << 8);
        }

        public static UInt64 SwapBytes(UInt64 x)
        {
            // swap adjacent 32-bit blocks
            x = (x >> 32) | (x << 32);
            // swap adjacent 16-bit blocks
            x = ((x & 0xFFFF0000FFFF0000) >> 16) | ((x & 0x0000FFFF0000FFFF) << 16);
            // swap adjacent 8-bit blocks
            return ((x & 0xFF00FF00FF00FF00) >> 8) | ((x & 0x00FF00FF00FF00FF) << 8);
        }

        public static Int16 SwapBytes(Int16 x)
        {
            return (Int16)SwapBytes((UInt16)x);
        }

        public static Int32 SwapBytes(Int32 x)
        {
            return (Int32)SwapBytes((UInt32)x);
        }

        public static Int64 SwapBytes(Int64 x)
        {
            return (Int64)SwapBytes((UInt64)x);
        }
    }
}
