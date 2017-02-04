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
using PsdSharp.ImageResources;
using PsdSharp.Internal;
using PsdSharp.Internal.Loaders;
using PsdSharp.Layers;

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

        public List<Layer> Layers { get; set; }

        public static PsdDocument Load(string fileName)
        {
            return PsdDocumentLoader.Load(fileName);
        }
    }
}