using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;

namespace Sunlighter.RationalGeometry
{
    public static partial class Extensions
    {
        public static bool IsOdd(this BigInteger b)
        {
            return (b & BigInteger.One) != BigInteger.Zero;
        }

        public static BigInteger LeastCommonMultiple(BigInteger a, BigInteger b)
        {
            return a * (b / BigInteger.GreatestCommonDivisor(a, b));
        }

        public static int GetInt32Value(this BigInteger b, OverflowBehavior overflowBehavior)
        {
            if (b >= int.MinValue && b <= int.MaxValue)
            {
                return (int)b;
            }
            else if (b < int.MinValue)
            {
                switch(overflowBehavior)
                {
                    case OverflowBehavior.Wraparound:
                        return unchecked((int)(b & 0xFFFFFFFF));
                    case OverflowBehavior.Saturate:
                    default:
                        return int.MinValue;
                    case OverflowBehavior.ThrowException:
                        throw new OverflowException();
                }
            }
            else
            {
                switch(overflowBehavior)
                {
                    case OverflowBehavior.Wraparound:
                        return unchecked((int)(b & uint.MaxValue));
                    case OverflowBehavior.Saturate:
                    default:
                        return int.MaxValue;
                    case OverflowBehavior.ThrowException:
                        throw new OverflowException();
                }
            }
        }

        public static long GetInt64Value(this BigInteger b, OverflowBehavior overflowBehavior)
        {
            if (b >= long.MinValue && b <= long.MaxValue)
            {
                return (long)b;
            }
            else if (b < long.MinValue)
            {
                switch (overflowBehavior)
                {
                    case OverflowBehavior.Wraparound:
                        return unchecked((long)(b & ulong.MaxValue));
                    case OverflowBehavior.Saturate:
                    default:
                        return long.MinValue;
                    case OverflowBehavior.ThrowException:
                        throw new OverflowException();
                }
            }
            else
            {
                switch (overflowBehavior)
                {
                    case OverflowBehavior.Wraparound:
                        return unchecked((long)(b & ulong.MaxValue));
                    case OverflowBehavior.Saturate:
                    default:
                        return int.MaxValue;
                    case OverflowBehavior.ThrowException:
                        throw new OverflowException();
                }
            }
        }
    }

    public enum OverflowBehavior
    {
        Wraparound,
        Saturate,
        ThrowException
    }

    public enum FloatingOverflowBehavior
    {
        SaturateToInfinity,
        ThrowException
    }

    public enum RoundingMode
    {
        Floor,
        Round,
        Ceiling,
        TruncateTowardZero
    }

    public class BigRational
    {
        private readonly BigInteger numerator;
        private readonly BigInteger denominator;

        public BigRational(BigInteger numerator, BigInteger denominator)
        {
            if (denominator.IsZero) throw new DivideByZeroException("Denominator of a rational number cannot be zero");

            if (denominator < BigInteger.Zero)
            {
                numerator = -numerator;
                denominator = -denominator;
            }

            BigInteger gcd = BigInteger.GreatestCommonDivisor(numerator, denominator);

            if (gcd != BigInteger.One)
            {
                numerator = numerator / gcd;
                denominator = denominator / gcd;
            }

            this.numerator = numerator;
            this.denominator = denominator;
        }

        public bool IsNegative => numerator < BigInteger.Zero;
        public bool IsZero => numerator.IsZero;

        public BigInteger Numerator => numerator;
        public BigInteger Denominator => denominator;

        public static BigRational operator +(BigRational a, BigRational b)
        {
            return new BigRational(a.numerator * b.denominator + b.numerator * a.denominator, a.denominator * b.denominator);
        }

        public static BigRational operator -(BigRational a)
        {
            return new BigRational(-a.numerator, a.denominator);
        }

        public static BigRational operator -(BigRational a, BigRational b)
        {
            return new BigRational(a.numerator * b.denominator - b.numerator * a.denominator, a.denominator * b.denominator);
        }

        public static BigRational operator *(BigRational a, BigRational b)
        {
            return new BigRational(a.numerator * b.numerator, a.denominator * b.denominator);
        }

        public static BigRational operator /(BigRational a, BigRational b)
        {
            return new BigRational(a.numerator * b.denominator, a.denominator * b.numerator);
        }

        public static BigRational operator +(BigRational a, BigInteger b)
        {
            return new BigRational(a.numerator + b * a.denominator, a.denominator);
        }

        public static BigRational operator +(BigInteger a, BigRational b)
        {
            return new BigRational(a * b.denominator + b.numerator, b.denominator);
        }

        public static BigRational operator -(BigRational a, BigInteger b)
        {
            return new BigRational(a.numerator - b * a.denominator, a.denominator);
        }

        public static BigRational operator -(BigInteger a, BigRational b)
        {
            return new BigRational(a * b.denominator - b.numerator, b.denominator);
        }

        public static BigRational operator *(BigRational a, BigInteger b)
        {
            return new BigRational(a.numerator * b, a.denominator);
        }

        public static BigRational operator *(BigInteger a, BigRational b)
        {
            return new BigRational(a * b.numerator, b.denominator);
        }

        public static BigRational operator /(BigRational a, BigInteger b)
        {
            return new BigRational(a.numerator, a.denominator * b);
        }

        public static BigRational operator /(BigInteger a, BigRational b)
        {
            return new BigRational(a * b.denominator, b.numerator);
        }

        public static implicit operator BigRational(BigInteger i)
        {
            return new BigRational(i, BigInteger.One);
        }

        public static explicit operator double(BigRational a)
        {
            return a.GetDoubleValue(RoundingMode.Round);
        }

        public static explicit operator float(BigRational a)
        {
            return a.GetSingleValue(RoundingMode.Round);
        }

        public BigInteger Floor()
        {
            BigInteger b = numerator / denominator;
            if (numerator < BigInteger.Zero) b = b - BigInteger.One;
            return b;
        }

        public BigInteger Round()
        {
            BigRational r = this + new BigRational(BigInteger.One, new BigInteger(2));
            if (r.Denominator == BigInteger.One)
            {
                if ((r.Numerator & BigInteger.One) != BigInteger.Zero)
                {
                    return r.Numerator - BigInteger.One;
                }
                else return r.Numerator;
            }
            else return r.Floor();
        }

        public BigInteger Ceiling()
        {
            BigInteger b = numerator / denominator;
            if (numerator >= BigInteger.Zero) b = b + BigInteger.One;
            return b;
        }

        public BigInteger TruncateTowardZero()
        {
            BigInteger b = numerator / denominator;
            return b;
        }

        public BigInteger RoundingOp(RoundingMode m)
        {
            switch (m)
            {
                case RoundingMode.Ceiling: return Ceiling();
                case RoundingMode.Floor: return Floor();
                case RoundingMode.Round: return Round();
                case RoundingMode.TruncateTowardZero: return TruncateTowardZero();
                default: goto case RoundingMode.Round;
            }
        }

        public static bool operator <(BigRational a, BigRational b)
        {
            return a.numerator * b.denominator < b.numerator * a.denominator;
        }

        public static bool operator >(BigRational a, BigRational b)
        {
            return a.numerator * b.denominator > b.numerator * a.denominator;
        }

        public static bool operator <=(BigRational a, BigRational b)
        {
            return a.numerator * b.denominator <= b.numerator * a.denominator;
        }

        public static bool operator >=(BigRational a, BigRational b)
        {
            return a.numerator * b.denominator >= b.numerator * a.denominator;
        }

        public static bool operator ==(BigRational a, BigRational b)
        {
            if (object.ReferenceEquals(a, null) && object.ReferenceEquals(b, null)) return true;
            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null)) return false;
            return (a.numerator == b.numerator) && (a.denominator == b.denominator);
        }

        public static bool operator !=(BigRational a, BigRational b)
        {
            if (object.ReferenceEquals(a, null) && object.ReferenceEquals(b, null)) return false;
            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null)) return true;
            return (a.numerator != b.numerator) || (a.denominator != b.denominator);
        }

        public override bool Equals(object obj)
        {
            return (obj is BigRational) && (this == (BigRational)obj);
        }

        public override int GetHashCode()
        {
            return $"{numerator}/{denominator}".GetHashCode();
        }

        public static BigRational Min(BigRational a, BigRational b)
        {
            return (a < b) ? a : b;
        }

        public static BigRational Max(BigRational a, BigRational b)
        {
            return (a > b) ? a : b;
        }

        public BigRational Reciprocal()
        {
            return new BigRational(denominator, numerator);
        }

        public static BigRational Gcd(BigRational a, BigRational b)
        {
            return new BigRational
            (
                BigInteger.GreatestCommonDivisor(a.Numerator * b.Denominator, b.Numerator * a.Denominator),
                a.Denominator * b.Denominator
            );
        }

        public static BigRational Lcm(BigRational a, BigRational b)
        {
            return new BigRational
            (
                Extensions.LeastCommonMultiple(a.Numerator * b.Denominator, b.Numerator * a.Denominator),
                a.Denominator * b.Denominator
            );
        }

        public static BigRational Pow(BigRational @base, int expt)
        {
            if (expt < 0) return Pow(@base.Reciprocal(), -expt);
            return new BigRational(BigInteger.Pow(@base.Numerator, expt), BigInteger.Pow(@base.Denominator, expt));
        }

        private static readonly Lazy<BigRational> zero = new Lazy<BigRational>(() => new BigRational(BigInteger.Zero, BigInteger.One), LazyThreadSafetyMode.ExecutionAndPublication);

        public static BigRational Zero => zero.Value;

        private static readonly Lazy<BigRational> one = new Lazy<BigRational>(() => new BigRational(BigInteger.One, BigInteger.One), LazyThreadSafetyMode.ExecutionAndPublication);

        public static BigRational One => one.Value;

        private static readonly Lazy<BigRational> two = new Lazy<BigRational>(() => new BigRational(2, BigInteger.One), LazyThreadSafetyMode.ExecutionAndPublication);

        public static BigRational Two => two.Value;

        private static readonly Lazy<BigRational> oneHalf = new Lazy<BigRational>(() => new BigRational(BigInteger.One, 2), LazyThreadSafetyMode.ExecutionAndPublication);

        public static BigRational OneHalf => oneHalf.Value;

        private static readonly Lazy<BigRational> minusOne = new Lazy<BigRational>(() => new BigRational(BigInteger.MinusOne, BigInteger.One), LazyThreadSafetyMode.ExecutionAndPublication);

        public static BigRational MinusOne => minusOne.Value;

        public static Tuple<BigRational, int> Normalize(BigRational r)
        {
            if (r.IsNegative)
            {
                Tuple<BigRational, int> result = Normalize(-r);
                return new Tuple<BigRational, int>(-result.Item1, result.Item2);
            }

            Stack<BigRational> powers = new Stack<BigRational>();
            Stack<int> exponents = new Stack<int>();

            BigRational currentPower;
            int currentExponent;

            int finalExponent = 0;

            if (r < One)
            {
                currentPower = OneHalf;
                currentExponent = -1;

                while (r < currentPower)
                {
                    powers.Push(currentPower);
                    exponents.Push(currentExponent);
                    currentPower *= currentPower;
                    currentExponent *= 2;
                }

                while (powers.Count > 0)
                {
                    currentPower = powers.Pop();
                    currentExponent = exponents.Pop();
                    if (r < currentPower)
                    {
                        r /= currentPower;
                        finalExponent += currentExponent;
                    }
                }
            }
            else
            {
                currentPower = Two;
                currentExponent = 1;

                while (r > currentPower)
                {
                    powers.Push(currentPower);
                    exponents.Push(currentExponent);
                    currentPower *= currentPower;
                    currentExponent *= 2;
                }

                while (powers.Count > 0)
                {
                    currentPower = powers.Pop();
                    currentExponent = exponents.Pop();
                    if (r > currentPower)
                    {
                        r /= currentPower;
                        finalExponent += currentExponent;
                    }
                }
            }

            while (r >= Two)
            {
                r /= Two;
                finalExponent += 1;
            }

            while (r < One)
            {
                r *= Two;
                finalExponent -= 1;
            }

            return new Tuple<BigRational, int>(r, finalExponent);
        }


        private static BigRational doubleFractionScale = new BigRational(new BigInteger(0x10000000000000), BigInteger.One);

        public double GetDoubleValue(RoundingMode m)
        {
            if (this.IsZero) return 0.0;
            Tuple<BigRational, int> normalized = Normalize(this);
            BigRational frac = normalized.Item1;
            int expt = normalized.Item2;

            expt += 1023;
            int loops = 53;
            while (expt < 0 && loops > 0)
            {
                frac /= Two;
                expt += 1;
                loops -= 1;
            }

            if (expt <= 0) { expt = 0; frac /= Two; }

            if (expt > 2046) return (frac < Zero) ? double.NegativeInfinity : double.PositiveInfinity;

            long bits = (frac * doubleFractionScale).RoundingOp(m).GetInt64Value(OverflowBehavior.Saturate);
            if (bits < 0) bits = (-bits) | unchecked((long)0x8000000000000000L);
            bits &= unchecked((long)0x800FFFFFFFFFFFFFL);
            bits |= (long)expt << 52;

            return BitConverter.Int64BitsToDouble(bits);
        }

        private static BigRational singleFractionScale = new BigRational(new BigInteger(0x800000), BigInteger.One);

        public float GetSingleValue(RoundingMode m)
        {
            if (this.IsZero) return 0.0f;
            Tuple<BigRational, int> normalized = Normalize(this);
            BigRational frac = normalized.Item1;
            int expt = normalized.Item2;

            expt += 127;
            int loops = 24;
            while (expt < 0 && loops > 0)
            {
                frac /= Two;
                expt += 1;
                loops -= 1;
            }

            if (expt <= 0) { expt = 0; frac /= Two; }

            if (expt > 254) return (frac < Zero) ? float.NegativeInfinity : float.PositiveInfinity;

            int bits = (frac * singleFractionScale).RoundingOp(m).GetInt32Value(OverflowBehavior.Saturate);
            if (bits < 0) bits = (-bits) | unchecked((int)0x80000000L);
            bits &= unchecked((int)0x807FFFFFL);
            bits |= expt << 23;

            return BitConverter.ToSingle(BitConverter.GetBytes(bits), 0);
        }

        private static void DecomposeSingle(float f, out bool isNegative, out int exponent, out int fraction)
        {
            int i = BitConverter.ToInt32(BitConverter.GetBytes(f), 0);
            fraction = i & 0x7FFFFF;
            exponent = ((i >> 23) & 0xFF) - 127;
            if (exponent == 128) throw new Exception("Float not representable as a rational");
            if (exponent == -127) { exponent = -126; } else { fraction |= 0x800000; }
            isNegative = ((i >> 31) != 0);
        }

        private static void DecomposeDouble(double d, out bool isNegative, out long exponent, out long fraction)
        {
            long l = BitConverter.DoubleToInt64Bits(d);
            fraction = l & 0xFFFFFFFFFFFFFL;
            exponent = ((l >> 52) & 0x7FFL) - 1023;
            if (exponent == 1024) throw new Exception("Double not representable as a rational");
            if (exponent == -1023) { exponent = -1022; } else { fraction |= 0x10000000000000L; }
            isNegative = ((l >> 63) != 0);
        }

        public static BigRational GetRationalValue(float f)
        {
            bool isNegative;
            int exponent;
            int fraction;
            DecomposeSingle(f, out isNegative, out exponent, out fraction);
            BigRational val0 = Pow(Two, exponent - 23) * new BigRational(fraction, BigInteger.One);
            return isNegative ? -val0 : val0;
        }

        public static BigRational GetRationalValue(double d)
        {
            bool isNegative;
            long exponent;
            long fraction;
            DecomposeDouble(d, out isNegative, out exponent, out fraction);
            BigRational val0 = Pow(Two, unchecked((int)(exponent - 52))) * new BigRational(fraction, BigInteger.One);
            return isNegative ? -val0 : val0;
        }
    }
}
