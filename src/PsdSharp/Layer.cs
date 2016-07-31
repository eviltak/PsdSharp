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
// This code contains code from the Endogine sprite engine by Jonas Beckeman.
// http://www.endogine.com/CS/
//

/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;

namespace PsdSharp
{
    public class Layer
    {
        public enum BlendModes
        {
            /// <summary>
            /// Normal
            /// </summary>
            Norm,

            /// <summary>
            /// Darken
            /// </summary>
            Dark,
            Lite,
            Hue,
            Sat,
            Colr,
            Lum,
            Mul,
            Scrn,
            Diss,
            Over,
            HLit,
            SLit,
            Diff,
            Smud,
            Div,
            Idiv
        }

        private BitVector32 flags;

        private static int protectTransBit = BitVector32.CreateMask();

        private static int visibleBit = BitVector32.CreateMask(protectTransBit);

        private Rectangle rect = Rectangle.Empty;

        private string blendModeKey = "norm";

        /// <summary>false = base, true = non–base</summary>
        public bool Clipping { get; set; }

        /// <summary>Protect the transparency</summary>
        public bool ProtectTrans
        {
            get { return flags[protectTransBit]; }
            set { flags[protectTransBit] = value; }
        }

        /// <summary>If true, the layer is visible.</summary>
        public bool Visible
        {
            get { return !flags[visibleBit]; }
            set { flags[visibleBit] = !value; }
        }

        /// <summary>0 = transparent ... 255 = opaque</summary>
        public byte Opacity { get; set; }

        public BlendingRanges BlendingRangesData { get; set; }

        public Mask MaskData { get; set; }

        public List<Channel> Channels { get; } = new List<Channel>();

        internal List<AdjusmentLayerInfo> AdjustmentInfo { get; set; } = new List<AdjusmentLayerInfo>();

        internal PsdFile PsdFile { get; }

        /// <summary>The rectangle containing the contents of the layer.</summary>
        public Rectangle Rect
        {
            get { return rect; }
            set { rect = value; }
        }

        public SortedList<short, Channel> SortedChannels { get; } = new SortedList<short, Channel>();

        /// <summary>The blend mode key for the layer.</summary>
        /// <remarks>
        /// <list type="table">
        /// <term>norm</term><description>normal</description>
        /// <term>dark</term><description>darken</description>
        /// <term>lite</term><description>lighten</description>
        /// <term>hue </term><description>hue</description>
        /// <term>sat </term><description>saturation</description>
        /// <term>colr</term><description>color</description>
        /// <term>lum </term><description>luminosity</description>
        /// <term>mul </term><description>multiply</description>
        /// <term>scrn</term><description>screen</description>
        /// <term>diss</term><description>dissolve</description>
        /// <term>over</term><description>overlay</description>
        /// <term>hLit</term><description>hard light</description>
        /// <term>sLit</term><description>soft light</description>
        /// <term>diff</term><description>difference</description>
        /// <term>smud</term><description>exclusion</description>
        /// <term>div </term><description>color dodge</description>
        /// <term>idiv</term><description>color burn</description>
        /// </list>
        /// </remarks>

        // TODO: Use an enum instead.
        // TODO: Why is the setter not assigning anything? Circumvent check, look at above TODO.
        public string BlendModeKey
        {
            get { return blendModeKey; }
            set
            {
                if (value.Length != 4) throw new ArgumentException("Key length must be 4");
            }
        }

        /// <summary>The descriptive layer name</summary>
        public string Name { get; set; }


        public Layer(BinaryReverseReader reader, PsdFile psdFile)
        {
            Debug.WriteLine("Layer started at " + reader.BaseStream.Position);

            PsdFile = psdFile;
            rect = new Rectangle();
            rect.Y = reader.ReadInt32();
            rect.X = reader.ReadInt32();
            rect.Height = reader.ReadInt32() - rect.Y;
            rect.Width = reader.ReadInt32() - rect.X;

            //-----------------------------------------------------------------------

            int numberOfChannels = reader.ReadUInt16();
            Channels.Clear();

            for (int channel = 0; channel < numberOfChannels; channel++)
            {
                Channel ch = new Channel(reader, this);
                Channels.Add(ch);
                SortedChannels.Add(ch.Id, ch);
            }

            //-----------------------------------------------------------------------

            string signature = new string(reader.ReadChars(4));
            if (signature != "8BIM")
                throw new IOException("Layer Channelheader error!");

            blendModeKey = new string(reader.ReadChars(4));
            Opacity = reader.ReadByte();

            Clipping = reader.ReadByte() > 0;

            //-----------------------------------------------------------------------

            byte flagsByte = reader.ReadByte();
            flags = new BitVector32(flagsByte);

            //-----------------------------------------------------------------------

            reader.ReadByte(); //padding

            //-----------------------------------------------------------------------

            Debug.WriteLine("Layer extraDataSize started at " + reader.BaseStream.Position);

            // this is the total size of the MaskData, the BlendingRangesData, the
            // Name and the AdjustmenLayerInfo
            uint extraDataSize = reader.ReadUInt32();

            // remember the start position for calculation of the
            // AdjustmenLayerInfo size
            long extraDataStartPosition = reader.BaseStream.Position;

            MaskData = new Mask(reader, this);
            BlendingRangesData = new BlendingRanges(reader, this);

            //-----------------------------------------------------------------------

            long namePosition = reader.BaseStream.Position;

            Name = reader.ReadPascalString();

            int paddingBytes = (int) ((reader.BaseStream.Position - namePosition) % 4);

            Debug.Print("Layer {0} padding bytes after name", paddingBytes);
            reader.ReadBytes(paddingBytes);

            //-----------------------------------------------------------------------

            AdjustmentInfo.Clear();

            long adjustmenLayerEndPos = extraDataStartPosition + extraDataSize;
            while (reader.BaseStream.Position < adjustmenLayerEndPos)
            {
                try
                {
                    AdjustmentInfo.Add(new AdjusmentLayerInfo(reader, this));
                }
                catch
                {
                    reader.BaseStream.Position = adjustmenLayerEndPos;
                }
            }

            //-----------------------------------------------------------------------
            // make sure we are not on a wrong offset, so set the stream position
            // manually
            reader.BaseStream.Position = adjustmenLayerEndPos;
        }

        public Layer(PsdFile psdFile)
        {
            PsdFile = psdFile;
            PsdFile.Layers.Add(this);
        }


        public void Save(BinaryReverseWriter writer)
        {
            Debug.WriteLine("Layer Save started at " + writer.BaseStream.Position);

            writer.Write(rect.Top);
            writer.Write(rect.Left);
            writer.Write(rect.Bottom);
            writer.Write(rect.Right);

            //-----------------------------------------------------------------------

            writer.Write((short) Channels.Count);
            foreach (Channel ch in Channels)
                ch.Save(writer);

            //-----------------------------------------------------------------------

            string signature = "8BIM";
            writer.Write(signature.ToCharArray());
            writer.Write(blendModeKey.ToCharArray());
            writer.Write(Opacity);
            writer.Write((byte) (Clipping ? 1 : 0));

            writer.Write((byte) flags.Data);

            //-----------------------------------------------------------------------

            writer.Write((byte) 0);

            //-----------------------------------------------------------------------

            using (new LengthWriter(writer))
            {
                MaskData.Save(writer);
                BlendingRangesData.Save(writer);

                long namePosition = writer.BaseStream.Position;

                writer.WritePascalString(Name);

                int paddingBytes = (int) ((writer.BaseStream.Position - namePosition) % 4);
                Debug.Print("Layer {0} write padding bytes after name", paddingBytes);

                for (int i = 0; i < paddingBytes; i++)
                    writer.Write((byte) 0);

                foreach (AdjusmentLayerInfo info in AdjustmentInfo)
                {
                    info.Save(writer);
                }
            }
        }


        internal class AdjusmentLayerInfo
        {
            public BinaryReverseReader DataReader => new BinaryReverseReader(new MemoryStream(Data));

            public byte[] Data { get; set; }

            /// <summary>The layer to which this info belongs</summary>
            internal Layer Layer { get; private set; }

            public string Key { get; set; }

            public AdjusmentLayerInfo(string key, Layer layer)
            {
                Key = key;
                Layer = layer;
                Layer.AdjustmentInfo.Add(this);
            }

            public AdjusmentLayerInfo(BinaryReverseReader reader, Layer layer)
            {
                Debug.WriteLine("AdjusmentLayerInfo started at " + reader.BaseStream.Position);

                Layer = layer;

                string signature = new string(reader.ReadChars(4));
                if (signature != "8BIM")
                {
                    throw new IOException("Could not read an image resource");
                }

                Key = new string(reader.ReadChars(4));

                uint dataLength = reader.ReadUInt32();
                Data = reader.ReadBytes((int) dataLength);
            }

            public void Save(BinaryReverseWriter writer)
            {
                Debug.WriteLine("AdjusmentLayerInfo Save started at " + writer.BaseStream.Position);

                string signature = "8BIM";

                writer.Write(signature.ToCharArray());
                writer.Write(Key.ToCharArray());
                writer.Write((uint) Data.Length);
                writer.Write(Data);
            }
        }

        public class BlendingRanges
        {
            public BlendingRanges(BinaryReverseReader reader, Layer layer)
            {
                Debug.WriteLine("BlendingRanges started at " + reader.BaseStream.Position);

                Layer = layer;
                int dataLength = reader.ReadInt32();
                if (dataLength <= 0)
                    return;

                Data = reader.ReadBytes(dataLength);
            }

            public BlendingRanges(Layer layer)
            {
                Layer = layer;
                Layer.BlendingRangesData = this;
            }

            public byte[] Data { get; set; } = new byte[0];

            /// <summary>The layer to which this blending range belongs.</summary>
            public Layer Layer { get; private set; }

            public void Save(BinaryReverseWriter writer)
            {
                Debug.WriteLine("BlendingRanges Save started at " + writer.BaseStream.Position);

                writer.Write((uint) Data.Length);
                writer.Write(Data);
            }
        }

        public class Channel
        {
            public BinaryReverseReader DataReader
            {
                get
                {
                    if (Data == null)
                        return null;

                    return new BinaryReverseReader(new MemoryStream(Data));
                }
            }

            /// <summary>The length of the compressed channel data.</summary>
            public int Length { get; set; }

            /// <summary>
            /// The compressed raw channel data.
            /// </summary>
            public byte[] Data { get; set; }

            /// <summary>
            /// The raw channel image data.
            /// </summary>
            public byte[] ImageData { get; set; }

            public ImageCompression Compression { get; set; }

            /// <summary>
            /// The layer to which this channel belongs
            /// </summary>
            public Layer Layer { get; private set; }

            /// <summary>
            /// 0 = red, 1 = green, etc.
            /// Â–1 = transparency mask
            /// Â–2 = user supplied layer mask
            /// </summary>
            public short Id { get; set; }

            internal Channel(short id, Layer layer)
            {
                Id = id;
                Layer = layer;
                Layer.Channels.Add(this);
                Layer.SortedChannels.Add(Id, this);
            }

            internal Channel(BinaryReverseReader reader, Layer layer)
            {
                Debug.WriteLine("Channel started at " + reader.BaseStream.Position);

                Id = reader.ReadInt16();
                Length = reader.ReadInt32();
            }

            internal void LoadPixelData(BinaryReverseReader reader)
            {
                Debug.WriteLine("Channel.LoadPixelData started at " + reader.BaseStream.Position);

                Data = reader.ReadBytes(Length);

                using (BinaryReverseReader readerImg = DataReader)
                {
                    Compression = (ImageCompression) readerImg.ReadInt16();

                    int bytesPerRow = 0;

                    switch (Layer.PsdFile.Depth)
                    {
                        case 1:
                            bytesPerRow = Layer.rect.Width; // NOT Sure
                            break;
                        case 8:
                            bytesPerRow = Layer.rect.Width;
                            break;
                        case 16:
                            bytesPerRow = Layer.rect.Width * 2;
                            break;
                    }

                    ImageData = new byte[Layer.rect.Height * bytesPerRow];

                    switch (Compression)
                    {
                        case ImageCompression.Raw:

                            readerImg.Read(ImageData, 0, ImageData.Length);
                            break;

                        case ImageCompression.Rle:
                        
                            int[] rowLenghtList = new int[Layer.rect.Height];
                            for (int i = 0; i < rowLenghtList.Length; i++)
                                rowLenghtList[i] = readerImg.ReadInt16();

                            for (int i = 0; i < Layer.rect.Height; i++)
                            {
                                int rowIndex = i * Layer.rect.Width;
                                RleUtility.DecodedRow(readerImg.BaseStream, ImageData, rowIndex, bytesPerRow);
                            }
                        
                            break;
                    }
                }
            }

            internal void Save(BinaryReverseWriter writer)
            {
                Debug.WriteLine("Channel Save started at " + writer.BaseStream.Position);

                writer.Write(Id);

                CompressImageData();

                writer.Write(Data.Length + 2); // 2 bytes for the image compression
            }

            internal void SavePixelData(BinaryReverseWriter writer)
            {
                Debug.WriteLine("Channel SavePixelData started at " + writer.BaseStream.Position);

                writer.Write((short) Compression);
                writer.Write(ImageData);
            }


            private void CompressImageData()
            {
                if (Compression == ImageCompression.Rle)
                {
                    MemoryStream dataStream = new MemoryStream();
                    BinaryReverseWriter writer = new BinaryReverseWriter(dataStream);

                    // we will write the correct lengths later, so remember
                    // the position
                    long lengthPosition = writer.BaseStream.Position;

                    int[] rleRowLenghs = new int[Layer.rect.Height];

                    if (Compression == ImageCompression.Rle)
                    {
                        for (int i = 0; i < rleRowLenghs.Length; i++)
                        {
                            writer.Write((short) 0x1234);
                        }
                    }

                    //---------------------------------------------------------------

                    int bytesPerRow = 0;

                    switch (Layer.PsdFile.Depth)
                    {
                        case 1:
                            bytesPerRow = Layer.rect.Width; // NOT Sure
                            break;
                        case 8:
                            bytesPerRow = Layer.rect.Width;
                            break;
                        case 16:
                            bytesPerRow = Layer.rect.Width * 2;
                            break;
                    }

                    //---------------------------------------------------------------

                    for (int row = 0; row < Layer.rect.Height; row++)
                    {
                        int rowIndex = row * Layer.rect.Width;
                        rleRowLenghs[row] = RleUtility.EncodedRow(writer.BaseStream, ImageData, rowIndex,
                                                                 bytesPerRow);
                    }

                    //---------------------------------------------------------------

                    long endPosition = writer.BaseStream.Position;

                    writer.BaseStream.Position = lengthPosition;

                    for (int i = 0; i < rleRowLenghs.Length; i++)
                    {
                        writer.Write((short) rleRowLenghs[i]);
                    }

                    writer.BaseStream.Position = endPosition;

                    dataStream.Close();

                    Data = dataStream.ToArray();

                    dataStream.Dispose();
                }
                else
                {
                    Data = (byte[]) ImageData.Clone();
                }
            }
        }

        public class Mask
        {
            private BitVector32 flags;

            private static int isPositionRelativeBit = BitVector32.CreateMask();
            private static int disabledBit = BitVector32.CreateMask(isPositionRelativeBit);
            private static int invertOnBlendBit = BitVector32.CreateMask(disabledBit);

            private Rectangle rect = Rectangle.Empty;

            internal Mask(BinaryReverseReader reader, Layer layer)
            {
                Debug.WriteLine("Mask started at " + reader.BaseStream.Position);

                Layer = layer;

                uint maskLength = reader.ReadUInt32();

                if (maskLength <= 0)
                    return;

                long startPosition = reader.BaseStream.Position;

                //-----------------------------------------------------------------------

                rect = new Rectangle();
                rect.Y = reader.ReadInt32();
                rect.X = reader.ReadInt32();
                rect.Height = reader.ReadInt32() - rect.Y;
                rect.Width = reader.ReadInt32() - rect.X;

                DefaultColor = reader.ReadByte();

                //-----------------------------------------------------------------------

                byte maskFlags = reader.ReadByte();
                flags = new BitVector32(maskFlags);

                //-----------------------------------------------------------------------

                if (maskLength == 36)
                {
                    BitVector32 realFlags = new BitVector32(reader.ReadByte());

                    byte realUserMaskBackground = reader.ReadByte();

                    rect = new Rectangle();
                    rect.Y = reader.ReadInt32();
                    rect.X = reader.ReadInt32();
                    rect.Height = reader.ReadInt32() - rect.Y;
                    rect.Width = reader.ReadInt32() - rect.X;
                }

                // there is other stuff following, but we will ignore this.
                reader.BaseStream.Position = startPosition + maskLength;
            }

            internal Mask(Layer layer)
            {
                Layer = layer;
                layer.MaskData = this;
            }

            public bool Disabled
            {
                get { return flags[disabledBit]; }
                set { flags[disabledBit] = value; }
            }

            /// <summary>
            /// if true, invert the mask when blending.
            /// </summary>
            public bool InvertOnBlend
            {
                get { return flags[invertOnBlendBit]; }
                set { flags[invertOnBlendBit] = value; }
            }

            /// <summary>
            /// If true, the position of the mask is relative to the layer.
            /// </summary>
            public bool IsPositionRelative
            {
                get { return flags[isPositionRelativeBit]; }
                set { flags[isPositionRelativeBit] = value; }
            }

            /// <summary>
            /// The raw image data from the channel.
            /// </summary>
            public byte[] ImageData { get; set; }

            public byte DefaultColor { get; set; }

            /// <summary>
            /// The layer to which this mask belongs.
            /// </summary>
            public Layer Layer { get; private set; }

            /// <summary>
            /// The rectangle enclosing the mask.
            /// </summary>
            public Rectangle Rect
            {
                get { return rect; }

                set { rect = value; }
            }

            public void Save(BinaryReverseWriter writer)
            {
                Debug.WriteLine("Mask Save started at " + writer.BaseStream.Position);

                if (rect.IsEmpty)
                {
                    writer.Write((uint) 0);
                    return;
                }

                using (new LengthWriter(writer))
                {
                    writer.Write(rect.Top);
                    writer.Write(rect.Left);
                    writer.Write(rect.Bottom);
                    writer.Write(rect.Right);

                    writer.Write(DefaultColor);

                    writer.Write((byte) flags.Data);

                    // padding 2 bytes so that size is 20
                    writer.Write(0);
                }
            }


            internal void LoadPixelData(BinaryReverseReader reader)
            {
                Debug.WriteLine("Mask.LoadPixelData started at " + reader.BaseStream.Position);

                if (rect.IsEmpty || Layer.SortedChannels.ContainsKey(-2) == false)
                    return;

                Channel maskChannel = Layer.SortedChannels[-2];

                maskChannel.Data = reader.ReadBytes(maskChannel.Length);

                using (BinaryReverseReader readerImg = maskChannel.DataReader)
                {
                    maskChannel.Compression = (ImageCompression) readerImg.ReadInt16();

                    int bytesPerRow = 0;

                    switch (Layer.PsdFile.Depth)
                    {
                        case 1:
                            bytesPerRow = rect.Width; // NOT Sure
                            break;
                        case 8:
                            bytesPerRow = rect.Width;
                            break;
                        case 16:
                            bytesPerRow = rect.Width * 2;
                            break;
                    }

                    maskChannel.ImageData = new byte[rect.Height * bytesPerRow];
                    // Fill Array
                    for (int i = 0; i < maskChannel.ImageData.Length; i++)
                    {
                        maskChannel.ImageData[i] = 0xAB;
                    }

                    ImageData = (byte[]) maskChannel.ImageData.Clone();

                    switch (maskChannel.Compression)
                    {
                        case ImageCompression.Raw:
                            readerImg.Read(maskChannel.ImageData, 0, maskChannel.ImageData.Length);
                            break;
                        case ImageCompression.Rle:
                        {
                            int[] rowLenghtList = new int[rect.Height];

                            for (int i = 0; i < rowLenghtList.Length; i++)
                                rowLenghtList[i] = readerImg.ReadInt16();

                            for (int i = 0; i < rect.Height; i++)
                            {
                                int rowIndex = i * rect.Width;
                                RleUtility.DecodedRow(readerImg.BaseStream, maskChannel.ImageData, rowIndex,
                                                     bytesPerRow);
                            }
                        }
                            break;
                    }

                    ImageData = (byte[]) maskChannel.ImageData.Clone();
                }
            }

            internal void SavePixelData(BinaryReverseWriter writer)
            {
                //writer.Write(data);
            }
        }
    }
}