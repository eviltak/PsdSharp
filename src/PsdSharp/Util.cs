using System;

namespace PsdSharp
{
    public static class Util
    {
        /// <summary>
        /// Converts the blend mode key string into a <see cref="Layer.BlendModes"/> object for storage.
        /// </summary>
        /// <param name="s">The blend mode key string.</param>
        /// <returns>The converted <see cref="Layer.BlendModes"/> object.</returns>
        public static Layer.BlendModes ToBlendModeEnum(string s)
        {
            s = s.Trim();

            s = char.ToUpper(s[0]) + s.Substring(1);

            Layer.BlendModes result;
            Enum.TryParse(s, out result);

            return result;
        }

        /// <summary>
        /// Converts the <see cref="Layer.BlendModes"/> enum object into a writable 4 character long key string.
        /// </summary>
        /// <param name="blendMode">The <see cref="Layer.BlendModes"/> object.</param>
        /// <returns>The converted key string.</returns>
        public static string ToBlendModeString(this Layer.BlendModes blendMode)
        {
            string result = blendMode.ToString();

            result = result.PadRight(4);

            result = char.ToLower(result[0]) + result.Substring(1);

            return result;
        }

        // The following SwapBytes methods have been taken from System.Drawing.PSD by Darren Horrocks.
        // https://github.com/bizzehdee/System.Drawing.PSD/blob/master/Utilities.cs

        public static ushort SwapBytes(ushort x)
        {
            return (ushort) (((x & 0xff) << 8) | ((x >> 8) & 0xff));
        }

        public static uint SwapBytes(uint x)
        {
            // swap adjacent 16-bit blocks
            x = (x >> 16) | (x << 16);
            // swap adjacent 8-bit blocks
            return ((x & 0xFF00FF00) >> 8) | ((x & 0x00FF00FF) << 8);
        }

        public static ulong SwapBytes(ulong x)
        {
            // swap adjacent 32-bit blocks
            x = (x >> 32) | (x << 32);
            // swap adjacent 16-bit blocks
            x = ((x & 0xFFFF0000FFFF0000) >> 16) | ((x & 0x0000FFFF0000FFFF) << 16);
            // swap adjacent 8-bit blocks
            return ((x & 0xFF00FF00FF00FF00) >> 8) | ((x & 0x00FF00FF00FF00FF) << 8);
        }

        public static short SwapBytes(short x)
        {
            return (short) SwapBytes((ushort) x);
        }

        public static int SwapBytes(int x)
        {
            return (int) SwapBytes((uint) x);
        }

        public static long SwapBytes(long x)
        {
            return (long) SwapBytes((ulong) x);
        }
    }
}