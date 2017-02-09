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

using System.IO;
using PsdSharp.Internal;
using PsdSharp.IO;

namespace PsdSharp.ImageResources
{
    public class ImageResource
    {
        public byte[] data;
        private PsdDocument document;

        public ImageResourceId Id { get; set; }

        public string Name { get; set; }

        private ImageResource(ImageResourceId id, string name, byte[] data)
            : this(null, id, name, data)
        {
        }

        internal ImageResource(PsdDocument document, ImageResourceId id, string name, byte[] data)
        {
            this.document = document;
            this.data = data;
            Id = id;
            Name = name;
        }

        internal static ImageResource Load(BinaryReader reader)
        {
            // Read an image resource

            string signature = new string(reader.ReadChars(4));

            if (!signature.Equals(Constants.ImageResourceSignature))
                throw new IOException("Invalid image resource.");

            ImageResourceId id = (ImageResourceId)reader.ReadInt16();

            string name = reader.ReadPaddedPascalString();

            int dataLength = reader.ReadInt32();
            // If odd, pad to make it even
            dataLength += dataLength % 2;

            byte[] data = reader.ReadBytes(dataLength);

            ImageResource imageResource = new ImageResource(id, name, data);

            return imageResource;
        }

        internal static void LoadIntoDocument(PsdDocument psdDocument, BinaryReader reader)
        {
            ImageResource imageResource = ImageResource.Load(reader);
            psdDocument.ImageResources[imageResource.Id] = imageResource;
        }

    }
}