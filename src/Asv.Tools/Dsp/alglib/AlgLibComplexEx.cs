using System;

namespace Asv.Tools
{
    public static class AlgLibComplexEx
    {
        private static readonly alglib.complex Zero = new alglib.complex(0.0, 0.0);

        private static readonly alglib.complex One = new alglib.complex(1.0, 0.0);

        private static readonly alglib.complex ImaginaryOne = new alglib.complex(0.0, 1.0);

        /// <summary>Creates a complex number from a point's polar coordinates.</summary>
        /// <param name="magnitude">The magnitude, which is the distance from the origin (the intersection of the x-axis and the y-axis) to the number.</param>
        /// <param name="phase">The phase, which is the angle from the line to the horizontal axis, measured in radians.</param>
        /// <returns>A complex number.</returns>
        private static alglib.complex FromPolarCoordinates(double magnitude, double phase) => new alglib.complex(magnitude * Math.Cos(phase), magnitude * Math.Sin(phase));

        /// <summary>Gets the phase of a complex number.</summary>
        /// <returns>The phase of a complex number, in radians.</returns>
        public static double Phase(this alglib.complex value)
        {
            return Math.Atan2(value.y, value.x);
        }

        /// <summary>Gets the magnitude (or absolute value) of a complex number.</summary>
        /// <returns>The magnitude of the current instance.</returns>
        public static double Magnitude(this alglib.complex value)
        {
            return value.Abs();
        }

        /// <summary>Gets the absolute value (or magnitude) of a complex number.</summary>
        /// <param name="value">A complex number.</param>
        /// <returns>The absolute value of <paramref name="value" />.</returns>
        private static double Abs(this alglib.complex value)
        {
            if (double.IsInfinity(value.x) || double.IsInfinity(value.y))
                return double.PositiveInfinity;
            var num1 = Math.Abs(value.x);
            var num2 = Math.Abs(value.y);
            if (num1 > num2)
            {
                var num3 = num2 / num1;
                return num1 * Math.Sqrt(1.0 + num3 * num3);
            }
            if (num2 == 0.0)
                return num1;
            var num4 = num1 / num2;
            return num2 * Math.Sqrt(1.0 + num4 * num4);
        }

        /// <summary>Returns the sine of the specified complex number.</summary>
        /// <param name="value">A complex number.</param>
        /// <returns>The sine of <paramref name="value" />.</returns>
        public static alglib.complex Sin(this alglib.complex value)
        {
            double real = value.x;
            double imaginary = value.y;
            return new alglib.complex(Math.Sin(real) * Math.Cosh(imaginary), Math.Cos(real) * Math.Sinh(imaginary));
        }

        /// <summary>Returns the hyperbolic sine of the specified complex number.</summary>
        /// <param name="value">A complex number.</param>
        /// <returns>The hyperbolic sine of <paramref name="value" />.</returns>
        public static alglib.complex Sinh(this alglib.complex value)
        {
            double real = value.x;
            double imaginary = value.y;
            return new alglib.complex(Math.Sinh(real) * Math.Cos(imaginary), Math.Cosh(real) * Math.Sin(imaginary));
        }

        /// <summary>Returns the angle that is the arc sine of the specified complex number.</summary>
        /// <param name="value">A complex number.</param>
        /// <returns>The angle which is the arc sine of <paramref name="value" />.</returns>
        public static alglib.complex Asin(this alglib.complex value) => -ImaginaryOne * (ImaginaryOne * value + (One - value * value).Sqrt()).Log();

        /// <summary>Returns the cosine of the specified complex number.</summary>
        /// <param name="value">A complex number.</param>
        /// <returns>The cosine of <paramref name="value" />.</returns>
        public static alglib.complex Cos(this alglib.complex value)
        {
            double real = value.x;
            double imaginary = value.y;
            return new alglib.complex(Math.Cos(real) * Math.Cosh(imaginary), -(Math.Sin(real) * Math.Sinh(imaginary)));
        }

        /// <summary>Returns the hyperbolic cosine of the specified complex number.</summary>
        /// <param name="value">A complex number.</param>
        /// <returns>The hyperbolic cosine of <paramref name="value" />.</returns>
        public static alglib.complex Cosh(this alglib.complex value)
        {
            double real = value.x;
            double imaginary = value.y;
            return new alglib.complex(Math.Cosh(real) * Math.Cos(imaginary), Math.Sinh(real) * Math.Sin(imaginary));
        }

        /// <summary>Returns the angle that is the arc cosine of the specified complex number.</summary>
        /// <param name="value">A complex number that represents a cosine.</param>
        /// <returns>The angle, measured in radians, which is the arc cosine of <paramref name="value" />.</returns>
        public static alglib.complex Acos(this alglib.complex value) => -ImaginaryOne * (value + ImaginaryOne * (One - value * value).Sqrt()).Log();

        /// <summary>Returns the tangent of the specified complex number.</summary>
        /// <param name="value">A complex number.</param>
        /// <returns>The tangent of <paramref name="value" />.</returns>
        public static alglib.complex Tan(alglib.complex value) => value.Sin() / value.Cos();

        /// <summary>Returns the hyperbolic tangent of the specified complex number.</summary>
        /// <param name="value">A complex number.</param>
        /// <returns>The hyperbolic tangent of <paramref name="value" />.</returns>
        public static alglib.complex Tanh(alglib.complex value) => value.Sinh() / value.Cosh();

        /// <summary>Returns the angle that is the arc tangent of the specified complex number.</summary>
        /// <param name="value">A complex number.</param>
        /// <returns>The angle that is the arc tangent of <paramref name="value" />.</returns>
        public static alglib.complex Atan(alglib.complex value)
        {
            alglib.complex complex = new alglib.complex(2.0, 0.0);
            return ImaginaryOne / complex * ((One - ImaginaryOne * value).Log() - (One + ImaginaryOne * value).Log());
        }

        /// <summary>Returns the natural (base <see langword="e" />) logarithm of a specified complex number.</summary>
        /// <param name="value">A complex number.</param>
        /// <returns>The natural (base <see langword="e" />) logarithm of <paramref name="value" />.</returns>
        public static alglib.complex Log(this alglib.complex value) => new alglib.complex(Math.Log(value.Abs()), Math.Atan2(value.y, value.x));

        /// <summary>Returns the logarithm of a specified complex number in a specified base.</summary>
        /// <param name="value">A complex number.</param>
        /// <param name="baseValue">The base of the logarithm.</param>
        /// <returns>The logarithm of <paramref name="value" /> in base <paramref name="baseValue" />.</returns>
        public static alglib.complex Log(this alglib.complex value, double baseValue) => value.Log() / ((alglib.complex)baseValue).Log();

        /// <summary>Returns the base-10 logarithm of a specified complex number.</summary>
        /// <param name="value">A complex number.</param>
        /// <returns>The base-10 logarithm of <paramref name="value" />.</returns>
        public static alglib.complex Log10(this alglib.complex value) => (value.Log()).Scale(0.43429448190325);

        /// <summary>Returns <see langword="e" /> raised to the power specified by a complex number.</summary>
        /// <param name="value">A complex number that specifies a power.</param>
        /// <returns>The number <see langword="e" /> raised to the power <paramref name="value" />.</returns>
        public static alglib.complex Exp(this alglib.complex value)
        {
            double num = Math.Exp(value.x);
            return new alglib.complex(num * Math.Cos(value.y), num * Math.Sin(value.y));
        }

        /// <summary>Returns the square root of a specified complex number.</summary>
        /// <param name="value">A complex number.</param>
        /// <returns>The square root of <paramref name="value" />.</returns>
        public static alglib.complex Sqrt(this alglib.complex value) => FromPolarCoordinates(Math.Sqrt(value.Magnitude()), value.Phase() / 2.0);

        /// <summary>Returns a specified complex number raised to a power specified by a complex number.</summary>
        /// <param name="value">A complex number to be raised to a power.</param>
        /// <param name="power">A complex number that specifies a power.</param>
        /// <returns>The complex number <paramref name="value" /> raised to the power <paramref name="power" />.</returns>
        public static alglib.complex Pow(this alglib.complex value, alglib.complex power)
        {
            if (power == Zero)
                return One;
            if (value == Zero)
                return Zero;
            double real1 = value.x;
            double imaginary1 = value.y;
            double real2 = power.x;
            double imaginary2 = power.y;
            double num1 = value.Abs();
            double num2 = Math.Atan2(imaginary1, real1);
            double num3 = real2 * num2 + imaginary2 * Math.Log(num1);
            double num4 = Math.Pow(num1, real2) * Math.Pow(Math.E, -imaginary2 * num2);
            return new alglib.complex(num4 * Math.Cos(num3), num4 * Math.Sin(num3));
        }

        /// <summary>Returns a specified complex number raised to a power specified by a double-precision floating-point number.</summary>
        /// <param name="value">A complex number to be raised to a power.</param>
        /// <param name="power">A double-precision floating-point number that specifies a power.</param>
        /// <returns>The complex number <paramref name="value" /> raised to the power <paramref name="power" />.</returns>
        public static alglib.complex Pow(this alglib.complex value, double power) => value.Pow(new alglib.complex(power, 0.0));

        private static alglib.complex Scale(this alglib.complex value, double factor) => new alglib.complex(factor * value.x, factor * value.y);
    }
}
