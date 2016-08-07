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

using System.Collections.Generic;
using System.IO;

namespace PsdSharp.ImageResources
{
    /// <summary>The names of the alpha channels</summary>
    public class AlphaChannels : ImageResource
    {
        #region Fields

        private List<string> m_channelNames = new List<string>();

        #endregion Fields

        #region Constructors

        public AlphaChannels(ImageResource imgRes)
            : base(imgRes)
        {
            BinaryReverseReader reader = imgRes.DataReader;
            // the names are pascal strings without padding!!!
            while ((reader.BaseStream.Length - reader.BaseStream.Position) > 0)
            {
                byte stringLength = reader.ReadByte();
                string s = new string(reader.ReadChars(stringLength));
                if (s.Length > 0)
                    m_channelNames.Add(s);
            }
            reader.Close();
        }

        public AlphaChannels()
            : base((short)ResourceIDs.AlphaChannelNames)
        { }

        #endregion Constructors

        #region Properties

        public IList<string> ChannelNames
        {
            get { return m_channelNames; }
        }

        #endregion Properties

        #region Methods

        #region Protected Methods

        protected override void StoreData()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryReverseWriter(stream))
                {
                    foreach (string name in m_channelNames)
                    {
                        writer.Write((byte)name.Length);
                        writer.Write(name.ToCharArray());
                    }

                    Data = stream.ToArray();
                }
            }
        }

        #endregion Protected Methods

        #endregion Methods
    }
}
