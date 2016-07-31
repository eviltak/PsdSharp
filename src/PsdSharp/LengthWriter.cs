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

namespace PsdSharp
{
    internal class LengthWriter : IDisposable
    {
        private BinaryReverseWriter writer;
        private long lengthPosition;
        private long startPosition;

        public LengthWriter(BinaryReverseWriter writer)
        {
            this.writer = writer;

            // we will write the correct length later, so remember
            // the position
            lengthPosition = writer.BaseStream.Position;
            writer.Write(0xFEEDFEED);

            // remember the start  position for calculation Image
            // resources length
            startPosition = writer.BaseStream.Position;
        }

        public void Dispose()
        {
            Write();
        }

        public void Write()
        {
            if (lengthPosition != long.MinValue)
            {
                long endPosition = writer.BaseStream.Position;

                writer.BaseStream.Position = lengthPosition;
                long length = endPosition - startPosition;
                writer.Write((uint) length);
                writer.BaseStream.Position = endPosition;

                lengthPosition = long.MinValue;
            }
        }
    }
}