using System;
using System.Collections.Generic;
using System.Text;

namespace Sunlighter.RationalGeometry
{
    public class Vertex3 : IEquatable<Vertex3>
    {
        private BigRational x;
        private BigRational y;
        private BigRational z;

        public Vertex3(BigRational x, BigRational y, BigRational z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public BigRational X { get { return x; } }
        public BigRational Y { get { return y; } }
        public BigRational Z { get { return z; } }

        public static Vertex3 operator +(Vertex3 a, Vector3 b)
        {
            return new Vertex3(a.x + b.X, a.y + b.Y, a.z + b.Z);
        }

        public static Vertex3 operator +(Vector3 a, Vertex3 b)
        {
            return new Vertex3(a.X + b.x, a.Y + b.y, a.Z + b.z);
        }

        public static Vertex3 operator -(Vertex3 a, Vector3 b)
        {
            return new Vertex3(a.x - b.X, a.y - b.Y, a.z - b.Z);
        }

        public static Vector3 operator -(Vertex3 a, Vertex3 b)
        {
            return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        private static Vertex3 origin = null;

        public static Vertex3 Origin
        {
            get
            {
                if (origin == null)
                {
                    lock (typeof(Vertex3))
                    {
                        if (origin == null)
                        {
                            origin = new Vertex3(BigRational.Zero, BigRational.Zero, BigRational.Zero);
                        }
                    }
                }
                return origin;
            }
        }

        public static bool operator ==(Vertex3 a, Vertex3 b)
        {
            if (object.ReferenceEquals(a, null) && object.ReferenceEquals(b, null)) return true;
            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null)) return false;
            return (a.x == b.x) && (a.y == b.y) && (a.z == b.z);
        }

        public static bool operator !=(Vertex3 a, Vertex3 b)
        {
            if (object.ReferenceEquals(a, null) && object.ReferenceEquals(b, null)) return false;
            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null)) return true;
            return (a.x != b.x) || (a.y != b.y) || (a.z != b.z);
        }

        public bool Equals(Vertex3 other)
        {
            return (this == other);
        }

        public override bool Equals(object obj)
        {
            if (obj is Vertex3) return Equals((Vertex3)obj);
            else return false;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"[{x}, {y}, {z}]";
        }
    }


}
