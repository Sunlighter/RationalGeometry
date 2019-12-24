using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Sunlighter.RationalGeometry
{
    public class Vector3 : IEquatable<Vector3>
    {
        private BigRational x;
        private BigRational y;
        private BigRational z;

        public Vector3(BigRational x, BigRational y, BigRational z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public BigRational X { get { return x; } }
        public BigRational Y { get { return y; } }
        public BigRational Z { get { return z; } }

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3 operator -(Vector3 a)
        {
            return new Vector3(-a.x, -a.y, -a.z);
        }

        public BigRational Dot(Vector3 other)
        {
            return (x * other.x) + (y * other.y) + (z * other.z);
        }

        public Vector3 Cross(Vector3 other)
        {
            return new Vector3
            (
                y * other.z - z * other.y,
                z * other.x - x * other.z,
                x * other.y - y * other.x
            );
        }

        public bool IsParallelTo(Vector3 other)
        {
            return this.Cross(other) == Vector3.Zero;
        }

        public bool IsPerpendicularTo(Vector3 other)
        {
            return this.Dot(other) == BigRational.Zero;
        }

        public BigRational LengthSquared { get { return this.Dot(this); } }

        public BigRational ScaledLengthAlong(Vector3 axis)
        {
            return this.Dot(axis) / axis.Dot(axis);
        }

        public Vector3 ComponentAlong(Vector3 axis)
        {
            return axis * ScaledLengthAlong(axis);
        }

        public Vector3 ComponentOrtho(Vector3 axis)
        {
            return this - ComponentAlong(axis);
        }

        public static Vector3 operator *(Vector3 a, BigInteger b)
        {
            BigRational bb = new BigRational(b, BigInteger.One);
            return new Vector3(a.x * bb, a.y * bb, a.z * bb);
        }

        public static Vector3 operator *(Vector3 a, BigRational b)
        {
            return new Vector3(a.x * b, a.y * b, a.z * b);
        }

        public static Vector3 operator *(BigInteger a, Vector3 b)
        {
            BigRational aa = new BigRational(a, BigInteger.One);
            return new Vector3(aa * b.x, aa * b.y, aa * b.z);
        }

        public static Vector3 operator *(BigRational a, Vector3 b)
        {
            return new Vector3(a * b.x, a * b.y, a * b.z);
        }

        public static Vector3 operator /(Vector3 a, BigInteger b)
        {
            BigRational bb = new BigRational(b, BigInteger.One);
            return new Vector3(a.x / bb, a.y / bb, a.z / bb);
        }

        public static Vector3 operator /(Vector3 a, BigRational b)
        {
            return new Vector3(a.x / b, a.y / b, a.z / b);
        }

        private static Vector3 zero = null;

        public static Vector3 Zero
        {
            get
            {
                if (zero == null)
                {
                    lock (typeof(Vector3))
                    {
                        if (zero == null)
                        {
                            zero = new Vector3(BigRational.Zero, BigRational.Zero, BigRational.Zero);
                        }
                    }
                }
                return zero;
            }
        }

        public static bool operator ==(Vector3 a, Vector3 b)
        {
            if (object.ReferenceEquals(a, null) && object.ReferenceEquals(b, null)) return true;
            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null)) return false;
            return (a.x == b.x) && (a.y == b.y) && (a.z == b.z);
        }

        public static bool operator !=(Vector3 a, Vector3 b)
        {
            if (object.ReferenceEquals(a, null) && object.ReferenceEquals(b, null)) return false;
            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null)) return true;
            return (a.x != b.x) || (a.y != b.y) || (a.z != b.z);
        }

        public bool Equals(Vector3 other)
        {
            return (this == other);
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector3) return Equals((Vector3)obj);
            else return false;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"<{x}, {y}, {z}>";
        }
    }
}
