using System;
using System.Collections.Generic;
using System.IO;
using PsdSharp.ImageResources;
using PsdSharp.Internal;
using PsdSharp.IO;

namespace PsdSharp
{
    public class PsdDocument
    {
        private static int width;
        private static int height;
        private short channelCount;
        private short depth;

        public DocumentColorMode ColorMode { get; set; }

        public byte[] ColorModeData { get; set; }

        public Dictionary<ImageResourceId, ImageResource> ImageResources { get; set; }

        public short ChannelCount
        {
            get { return channelCount; }
            set
            {
                Utility.ValidatePropertyRange(value, Constants.MinChannelCount, Constants.MaxChannelCount);

                channelCount = value;
            }
        }

        public int Width
        {
            get { return width; }
            set
            {
                // TODO: Add support for PSB (PSB document can be up to 300,000 pixels wide)
                Utility.ValidatePropertyRange(value, Constants.MinDocumentWidth, Constants.MaxDocumentWidth);

                width = value;
            }
        }

        public int Height
        {
            get { return height; }
            set
            {
                // TODO: Add support for PSB (PSB document can be up to 300,000 pixels wide)
                Utility.ValidatePropertyRange(value, Constants.MinDocumentHeight, Constants.MaxDocumentHeight);

                height = value;
            }
        }

        public short Depth
        {
            get { return depth; }
            set
            {
                if (value != 1 && value != 8 && value != 16 && value != 32)
                    throw new ArgumentOutOfRangeException(nameof(Depth),
                                                          "Supported values of Depth are 1, 8, 16 and 32.");
                depth = value;
            }
        }

        public static PsdDocument Load(Stream stream)
        {
            PsdDocument psdDocument = new PsdDocument();

            BigEndianBinaryReader reader = new BigEndianBinaryReader(stream);

            ReadFileHeader(psdDocument, reader);

            ReadColorModeData(psdDocument, reader);

            ReadImageResources(psdDocument, reader);

            return psdDocument;
        }

        private static void ReadImageResources(PsdDocument psdDocument, BigEndianBinaryReader reader)
        {
            int imageResourceSectionLength = reader.ReadInt32();

            psdDocument.ImageResources = new Dictionary<ImageResourceId, ImageResource>();

            long startPosition = reader.BaseStream.Position;

            while (reader.BaseStream.Position - startPosition < imageResourceSectionLength)
            {
                // Read an image resource

                string signature = new string(reader.ReadChars(4));

                if (!signature.Equals(Constants.ImageResourceSignature))
                    throw new IOException("Invalid image resource.");

                ImageResourceId id = (ImageResourceId) reader.ReadInt16();

                string name = reader.ReadPaddedPascalString();

                int dataLength = reader.ReadInt32();
                // If odd, pad to make it even
                dataLength += dataLength % 2;

                byte[] data = reader.ReadBytes(dataLength);

                ImageResource imageResource = new ImageResource(psdDocument, id, name, data);
                psdDocument.ImageResources[id] = imageResource;
            }
        }

        private static void ReadColorModeData(PsdDocument psdDocument, BigEndianBinaryReader reader)
        {
            // If ColorMode is DocumentColorMode.Indexed or DocumentColorMode.Duotone
            // length > 0
            int length = reader.ReadInt32();

            // If ColorMode is DocumentColorMode.Indexed, then the following 768 bytes contain
            // the non-interleaved color table for the image.
            psdDocument.ColorModeData = reader.ReadBytes(length);
        }

        private static void ReadFileHeader(PsdDocument psdDocument, BigEndianBinaryReader reader)
        {
            // TODO: Store version and signature as properties?

            string signature = new string(reader.ReadChars(4));

            if (!signature.Equals(Constants.FileSignature))
                throw new IOException("Supplied file is not a valid Photoshop file.");

            int version = reader.ReadInt16();

            // Version of file must be 1
            // TODO: Add support for PSB (For PSBs version will be 2)
            if (version != 1)
                throw new IOException("Unsupported file version.");

            // 6 bytes reserved, skip
            reader.BaseStream.Position += 6;

            psdDocument.ChannelCount = reader.ReadInt16();

            psdDocument.Height = reader.ReadInt32();
            psdDocument.Width = reader.ReadInt32();

            psdDocument.Depth = reader.ReadInt16();

            psdDocument.ColorMode = (DocumentColorMode) reader.ReadInt16();
        }

        public static PsdDocument Load(string filePath)
        {
            using (Stream stream = File.OpenRead(filePath))
            {
                return Load(stream);
            }
        }
    }
}