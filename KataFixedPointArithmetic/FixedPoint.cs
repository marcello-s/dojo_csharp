#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataFixedPointArithmetic
{
    public struct FixedPoint
    {
        /*
         * Q Number Format
         * ---------------
         * source: http://en.wikipedia.org/wiki/Q_(number_format)
         *
         * f: number of fractional bits
         * m: number of magnitude
         * s: number of sign bits
         * b: total number of bits
         *
         * example: Qf, Qm.f
         */

        // Q15.16
        private const int Scale = 16;
        private const int One = 1 << Scale;
        private long Value;
        private const int Size = sizeof(long);
        private const int M = (Size * 2) - 1;
        private const int N = Size * 2;

        public FixedPoint(int value, bool scaleUp = true)
        {
            Value = value;
            if (scaleUp)
            {
                Value <<= Scale;
            }
        }

        public FixedPoint(double value)
            : this()
        {
            value *= One;
            Value = (int)Math.Round(value);
        }

        public static int MaxValue
        {
            get { return Convert.ToInt32(Math.Pow(2, M) - Math.Pow(2, -N)); }
        }

        public static int MinValue
        {
            get { return Convert.ToInt32(-Math.Pow(2, M)); }
        }

        public int ToInt()
        {
            return (int)Value >> Scale;
        }

        public double ToDouble()
        {
            return Value / (double)One;
        }

        public FixedPoint Inverse
        {
            get { return new FixedPoint(-(int)Value, false); }
        }

        // conversion operators

        public static explicit operator double(FixedPoint a)
        {
            return a.ToDouble();
        }

        public static implicit operator FixedPoint(double value)
        {
            return new FixedPoint(value);
        }

        // first order operators

        public static FixedPoint operator +(FixedPoint a)
        {
            return a;
        }

        public static FixedPoint operator +(FixedPoint a, FixedPoint b)
        {
            FixedPoint f;
            f.Value = a.Value + b.Value;
            return f;
        }

        public static FixedPoint operator -(FixedPoint a)
        {
            return a.Inverse;
        }

        public static FixedPoint operator -(FixedPoint a, FixedPoint b)
        {
            FixedPoint f;
            f.Value = a.Value - b.Value;
            return f;
        }

        // second order operators

        public static FixedPoint operator *(FixedPoint a, FixedPoint b)
        {
            FixedPoint f;
            f.Value = (a.Value * b.Value) >> Scale;
            return f;
        }

        public static FixedPoint operator /(FixedPoint a, FixedPoint b)
        {
            FixedPoint f;
            f.Value = (a.Value << Scale) / b.Value;
            return f;
        }

        // modulo operator

        public static FixedPoint operator %(FixedPoint a, FixedPoint b)
        {
            FixedPoint f;
            f.Value = a.Value % b.Value;
            return f;
        }

        // equality

        public static bool operator ==(FixedPoint a, FixedPoint b)
        {
            return a.Value == b.Value;
        }

        public static bool operator !=(FixedPoint a, FixedPoint b)
        {
            return a.Value != b.Value;
        }

        // comparison

        public static bool operator >(FixedPoint a, FixedPoint b)
        {
            return a.Value > b.Value;
        }

        public static bool operator <(FixedPoint a, FixedPoint b)
        {
            return a.Value < b.Value;
        }

        public static bool operator >=(FixedPoint a, FixedPoint b)
        {
            return a.Value >= b.Value;
        }

        public static bool operator <=(FixedPoint a, FixedPoint b)
        {
            return a.Value <= b.Value;
        }

        // overide object methods
        public override bool Equals(object obj)
        {
            if (obj is FixedPoint)
            {
                return ((FixedPoint)obj).Value == Value;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
