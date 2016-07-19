using System;

namespace PsdSharp
{
    public struct Rectangle
    {
        public static Rectangle Empty
        {
            get { return new Rectangle(); }
        }

        public int X { get; set; }
        public int Y { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public int Left { get { return X; } }
        public int Right { get { return X + Width; } }

        public int Top { get { return Y; } }
        public int Bottom { get { return Y + Height; } }

        public bool IsEmpty
        {
            get { return Height == 0 && Width == 0 && X == 0 && Y == 0; }
        }

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
            return (left.X == right.X
                    && left.Y == right.Y
                    && left.Width == right.Width
                    && left.Height == right.Height);
        }

        public static bool operator !=(Rectangle left, Rectangle right)
        {
            return !(left == right);
        }
    }
}

