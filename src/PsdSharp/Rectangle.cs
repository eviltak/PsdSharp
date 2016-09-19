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

namespace PsdSharp
{
    public struct Rectangle
    {
        /// <summary>
        /// Returns an empty <see cref="Rectangle"/>.
        /// </summary>
        public static Rectangle Empty => new Rectangle();

        /// <summary>
        /// The left coordinate of the <see cref="Rectangle"/>.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// The top coordinate of the <see cref="Rectangle"/>.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// The width of the <see cref="Rectangle"/>.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The height of the <see cref="Rectangle"/>.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The left coordinate of the <see cref="Rectangle"/>.
        /// </summary>
        public int Left => X;

        /// <summary>
        /// The right coordinate of the <see cref="Rectangle"/>.
        /// </summary>
        public int Right => X + Width;

        /// <summary>
        /// The top coordinate of the <see cref="Rectangle"/>.
        /// </summary>
        public int Top => Y;

        /// <summary>
        /// The bottom coordinate of the <see cref="Rectangle"/>.
        /// </summary>
        public int Bottom => Y + Height;

        /// <summary>
        /// Indicates whether the <see cref="Rectangle"/> is empty or not. An empty <see cref="Rectangle"/> has all of the top, left, bottom and right coordinates equal to zero.
        /// </summary>
        public bool IsEmpty => Height == 0 && Width == 0 && X == 0 && Y == 0;

        /// <summary>
        /// Creates a <see cref="Rectangle"/> from the specified left, top, right and bottom coordinates.</summary>
        /// <param name="left">The left coordinate of the <see cref="Rectangle"/>.</param>
        /// <param name="top">The top coordinate of the <see cref="Rectangle"/>.</param>
        /// <param name="right">The right coordinate of the <see cref="Rectangle"/>.</param>
        /// <param name="bottom">The bottom coordinate of the <see cref="Rectangle"/>.</param>
        public Rectangle(int left, int top, int right, int bottom)
        {
            X = left;
            Y = top;
            Width = right - left;
            Height = top - bottom;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Rectangle))
                return false;

            return (Rectangle) obj == this;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Rectangle left, Rectangle right)
        {
            return left.X == right.X
                   && left.Y == right.Y
                   && left.Width == right.Width
                   && left.Height == right.Height;
        }

        public static bool operator !=(Rectangle left, Rectangle right)
        {
            return !(left == right);
        }
    }
}