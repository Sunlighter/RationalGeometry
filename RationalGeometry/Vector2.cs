using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Sunlighter.RationalGeometry
{
    public class Vector2 : IEquatable<Vector2>
    {
        private BigRational x;
        private BigRational y;

        public Vector2(BigRational x, BigRational y)
        {
            this.x = x;
            this.y = y;
        }

        public BigRational X { get { return x; } }
        public BigRational Y { get { return y; } }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x + b.x, a.y + b.y);
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x - b.x, a.y - b.y);
        }

        public static Vector2 operator -(Vector2 a)
        {
            return new Vector2(-a.x, -a.y);
        }

        public BigRational Dot(Vector2 other)
        {
            return (x * other.x) + (y * other.y);
        }

        public BigRational Cross(Vector2 other)
        {
            return (x * other.y) - (y * other.x);
        }

        public Vector2 R90()
        {
            return new Vector2(y, -x);
        }

        public bool IsParallelTo(Vector2 other)
        {
            return this.Cross(other) == BigRational.Zero;
        }

        public bool IsPerpendicularTo(Vector2 other)
        {
            return this.Dot(other) == BigRational.Zero;
        }

        public BigRational LengthSquared { get { return this.Dot(this); } }

        public BigRational ScaledLengthAlong(Vector2 axis)
        {
            return this.Dot(axis) / axis.Dot(axis);
        }

        public Vector2 ComponentAlong(Vector2 axis)
        {
            return axis * ScaledLengthAlong(axis);
        }

        public Vector2 ComponentOrtho(Vector2 axis)
        {
            return this - ComponentAlong(axis);
        }

        public static Vector2 operator *(Vector2 a, BigInteger b)
        {
            BigRational bb = new BigRational(b, BigInteger.One);
            return new Vector2(a.x * bb, a.y * bb);
        }

        public static Vector2 operator *(Vector2 a, BigRational b)
        {
            return new Vector2(a.x * b, a.y * b);
        }

        public static Vector2 operator *(BigInteger a, Vector2 b)
        {
            BigRational aa = new BigRational(a, BigInteger.One);
            return new Vector2(aa * b.x, aa * b.y);
        }

        public static Vector2 operator *(BigRational a, Vector2 b)
        {
            return new Vector2(a * b.x, a * b.y);
        }

        public static Vector2 operator /(Vector2 a, BigInteger b)
        {
            BigRational bb = new BigRational(b, BigInteger.One);
            return new Vector2(a.x / bb, a.y / bb);
        }

        public static Vector2 operator /(Vector2 a, BigRational b)
        {
            return new Vector2(a.x / b, a.y / b);
        }

        private static Vector2 zero = null;

        public static Vector2 Zero
        {
            get
            {
                if (zero == null)
                {
                    lock (typeof(Vector2))
                    {
                        if (zero == null)
                        {
                            zero = new Vector2(BigRational.Zero, BigRational.Zero);
                        }
                    }
                }
                return zero;
            }
        }

        public static bool operator ==(Vector2 a, Vector2 b)
        {
            if (object.ReferenceEquals(a, null) && object.ReferenceEquals(b, null)) return true;
            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null)) return false;
            return (a.x == b.x) && (a.y == b.y);
        }

        public static bool operator !=(Vector2 a, Vector2 b)
        {
            if (object.ReferenceEquals(a, null) && object.ReferenceEquals(b, null)) return false;
            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null)) return true;
            return (a.x != b.x) || (a.y != b.y);
        }

        public bool Equals(Vector2 other)
        {
            return (this == other);
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector2) return Equals((Vector2)obj);
            else return false;
        }

        public override int GetHashCode()
        {
            return $"<{x}, {y}>".GetHashCode();
        }

        public override string ToString()
        {
            return $"<{x}, {y}>";
        }
    }
}
