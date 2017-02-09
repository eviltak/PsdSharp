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

using PsdSharp.IO;

namespace PsdSharp.Layers
{
    public class Channel
    {
        public short Id { get; set; }

        public int ImageDataLength { get; set; }

        internal static Channel Load(BinaryReader reader)
        {
            Channel channel = new Channel();

            channel.Id = reader.ReadInt16();

            // TODO: Add support for PSB (PSB channel image data length is 8 bytes)
            channel.ImageDataLength = reader.ReadInt32();

            return channel;
        }
    }
}