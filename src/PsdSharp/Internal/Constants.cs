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

namespace PsdSharp.Internal
{
    /// <summary>
    /// A class containing all constants used in code. All "magic" numbers and strings 
    /// that are be used only once also go here.
    /// </summary>
    internal static class Constants
    {
        public const string ImageResourceSignature = "8BIM";

        /// <summary>
        /// The fixed signature that each document must have.
        /// </summary>
        public const string FileSignature = "8BPS";

        /// <summary>
        /// The minimum possible number of channels in the document.
        /// </summary>
        public const int MinChannelCount = 1;

        public const int MaxChannelCount = 56;

        public const int MinDocumentWidth = 1;
        public const int MaxDocumentWidth = 30000;

        public const int MinDocumentHeight = 1;
        public const int MaxDocumentHeight = 30000;
    }
}