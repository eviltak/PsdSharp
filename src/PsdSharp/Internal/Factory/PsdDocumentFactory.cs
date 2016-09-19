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
using System.Collections.Generic;
using System.IO;
using PsdSharp.ImageResources;
using PsdSharp.IO;
using PsdSharp.Layers;

namespace PsdSharp.Internal.Factory
{
    internal static class PsdDocumentFactory
    {
        private static void ReadLayers(PsdDocument psdDocument, BigEndianBinaryReader reader)
        {
            // TODO: Add Support for PSB (length is 8 bytes)
            uint sectionLength = reader.ReadUInt32();
            long startPosition = reader.BaseStream.Position;

            short layerCount = Math.Abs(reader.ReadInt16());

            psdDocument.Layers = new List<Layer>(layerCount);

            for (int layerIndex = 0; layerIndex < layerCount; layerIndex++)
            {
                // Read layer
                Layer.LoadIntoDocument(psdDocument, reader);
            }

            // Pad to make sure the next section starts at the correct position
            reader.BaseStream.Position = startPosition + sectionLength;
        }

        private static void ReadLayersAndMasks(PsdDocument psdDocument, BigEndianBinaryReader reader)
        {
            // TODO: Add Support for PSB (length is 8 bytes)
            uint sectionLength = reader.ReadUInt32();
            long startPosition = reader.BaseStream.Position;


            ReadLayers(psdDocument, reader);
            // TODO: Read masks


            // Pad to make sure the next section starts at the correct position
            reader.BaseStream.Position = startPosition + sectionLength;
        }

        private static void ReadImageResources(PsdDocument psdDocument, BigEndianBinaryReader reader)
        {
            uint sectionLength = reader.ReadUInt32();
            long startPosition = reader.BaseStream.Position;

            psdDocument.ImageResources = new Dictionary<ImageResourceId, ImageResource>();

            while (reader.BaseStream.Position - startPosition < sectionLength)
            {
                ImageResource.LoadIntoDocument(psdDocument, reader);
            }

            // Pad to make sure the next section starts at the correct position
            reader.BaseStream.Position = startPosition + sectionLength;
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

        public static PsdDocument Load(Stream stream)
        {
            PsdDocument psdDocument = new PsdDocument();

            BigEndianBinaryReader reader = new BigEndianBinaryReader(stream);

            ReadFileHeader(psdDocument, reader);
            ReadColorModeData(psdDocument, reader);
            ReadImageResources(psdDocument, reader);
            ReadLayersAndMasks(psdDocument, reader);

            return psdDocument;
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