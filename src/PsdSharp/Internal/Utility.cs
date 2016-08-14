using System;
using CallerMemberNameAttribute = System.Runtime.CompilerServices.CallerMemberNameAttribute;

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