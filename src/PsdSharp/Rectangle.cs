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
        public static Rectangle Empty => new Rectangle();

        public int X { get; set; }
        public int Y { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public int Left => X;
        public int Right => X + Width;

        public int Top => Y;
        public int Bottom => Y + Height;

        public bool IsEmpty => Height == 0 && Width == 0 && X == 0 && Y == 0;

        public override bool Equals(object obj)
        {
            if (!(obj is Rectangle))
                return false;

            Rectangle comp = (Rectangle) obj;

            return comp == this;
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