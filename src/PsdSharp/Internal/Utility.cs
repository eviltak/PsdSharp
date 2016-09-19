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
using System.Runtime.CompilerServices;

namespace PsdSharp.Internal
{
    /// <summary>
    /// Static class containing some utility methods.
    /// </summary>
    internal static class Utility
    {
        /// <summary>
        /// Validates that the value falls in (inclusive) the given range.
        /// </summary>
        /// <param name="value">The value to check the range of.</param>
        /// <param name="min">The minimum (inclusive) possible value of the property.</param>
        /// <param name="max">The maximum (inclusive) possible value of the property.</param>
        /// <param name="propertyName">The name of the property that called this method. Any value passed to this
        ///  parameter will be replaced with the caller's name.</param>
        public static void ValidatePropertyRange(int value, int min, int max,
                                                 [CallerMemberName] string propertyName = "")
        {
            if (value < min || value > max)
                throw new ArgumentOutOfRangeException(
                    propertyName,
                    $"Value of {propertyName} can only be between (inclusive) {min} and {max}.");
        }
    }
}