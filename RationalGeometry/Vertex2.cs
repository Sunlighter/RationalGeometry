using System;
using System.Collections.Generic;
using System.Text;

namespace Sunlighter.RationalGeometry
{
    public class Vertex2 : IEquatable<Vertex2>
    {
        private BigRational x;
        private BigRational y;

        public Vertex2(BigRational x, BigRational y)
        {
            this.x = x;
            this.y = y;
        }

        public BigRational X { get { return x; } }
        public BigRational Y { get { return y; } }

        public static Vertex2 operator +(Vertex2 a, Vector2 b)
        {
            return new Vertex2(a.x + b.X, a.y + b.Y);
        }

        public static Vertex2 operator +(Vector2 a, Vertex2 b)
        {
            return new Vertex2(a.X + b.x, a.Y + b.y);
        }

        public static Vertex2 operator -(Vertex2 a, Vector2 b)
        {
            return new Vertex2(a.x - b.X, a.y - b.Y);
        }

        public static Vector2 operator -(Vertex2 a, Vertex2 b)
        {
            return new Vector2(a.x - b.x, a.y - b.y);
        }

        private static Vertex2 origin = null;

        public static Vertex2 Origin
        {
            get
            {
                if (origin == null)
                {
                    lock (typeof(Vertex2))
                    {
                        if (origin == null)
                        {
                            origin = new Vertex2(BigRational.Zero, BigRational.Zero);
                        }
                    }
                }
                return origin;
            }
        }

        public static bool operator ==(Vertex2 a, Vertex2 b)
        {
            if (object.ReferenceEquals(a, null) && object.ReferenceEquals(b, null)) return true;
            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null)) return false;
            return (a.x == b.x) && (a.y == b.y);
        }

        public static bool operator !=(Vertex2 a, Vertex2 b)
        {
            if (object.ReferenceEquals(a, null) && object.ReferenceEquals(b, null)) return false;
            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null)) return true;
            return (a.x != b.x) || (a.y != b.y);
        }

        public bool Equals(Vertex2 other)
        {
            return (this == other);
        }

        public override bool Equals(object obj)
        {
            if (obj is Vertex2) return Equals((Vertex2)obj);
            else return false;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"[{x}, {y}]";
        }
    }
}
