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

using System.IO;

namespace PsdSharp
{
    /// <summary>Writes primitive data types as binary values in in big-endian format</summary>
    public class BinaryReverseWriter : BinaryWriter
    {
        public bool AutoFlush { get; set; }

        public BinaryReverseWriter(Stream stream)
            : base(stream)
        {
        }

        public override void Write(short value)
        {
            value = Util.SwapBytes(value);
            base.Write(value);

            if (AutoFlush)
                Flush();
        }

        public override void Write(int value)
        {
            value = Util.SwapBytes(value);
            base.Write(value);

            if (AutoFlush)
                Flush();
        }

        public override void Write(long value)
        {
            value = Util.SwapBytes(value);
            base.Write(value);

            if (AutoFlush)
                Flush();
        }

        public override void Write(ushort value)
        {
            value = Util.SwapBytes(value);
            base.Write(value);

            if (AutoFlush)
                Flush();
        }

        public override void Write(uint value)
        {
            value = Util.SwapBytes(value);
            base.Write(value);

            if (AutoFlush)
                Flush();
        }

        public override void Write(ulong value)
        {
            value = Util.SwapBytes(value);
            base.Write(value);

            if (AutoFlush)
                Flush();
        }

        public void WritePascalString(string s)
        {
            char[] c;
            if (s.Length > 255)
                c = s.Substring(0, 255).ToCharArray();
            else
                c = s.ToCharArray();

            base.Write((byte) c.Length);
            base.Write(c);

            int realLength = c.Length + 1;

            if (realLength % 2 == 0)
                return;

            for (int i = 0; i < 2 - realLength % 2; i++)
                base.Write((byte) 0);

            if (AutoFlush)
                Flush();
        }
    }
}