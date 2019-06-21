// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoubleUtil.cs" company="Sane Development">
//
//   Sane Development WPF Controls Library
//
//   The BSD 3-Clause License
//
//   Copyright (c) Sane Development
//   All rights reserved.
//
//   Redistribution and use in source and binary forms, with or without modification,
//   are permitted provided that the following conditions are met:
//
//   - Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//   - Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//   - Neither the name of the Sane Development nor the names of its contributors
//     may be used to endorse or promote products derived from this software
//     without specific prior written permission.
//
//   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
//   AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
//   THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
//   ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS
//   BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY,
//   OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
//   PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
//   OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
//   WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
//   ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
//   EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.Contracts;

namespace SaneDevelopment.WPF4.Controls
{
    /// <summary>
    /// Code extracted from Framework's dll via Reflector
    /// </summary>
    [Pure]
    internal static class DoubleUtil
    {
// ReSharper disable InconsistentNaming
        internal const double DBL_EPSILON = 2.2204460492503131E-16;
        internal const float FLT_MIN = 1.175494E-38f;
// ReSharper restore InconsistentNaming

        [Pure]
        public static bool AreClose(double value1, double value2)
        {
// ReSharper disable CompareOfFloatsByEqualityOperator
            Contract.Ensures(value1 != value2 || Contract.Result<bool>());

            if (value1 == value2) return true;
// ReSharper restore CompareOfFloatsByEqualityOperator
            double num = ((Math.Abs(value1) + Math.Abs(value2)) + 10.0) * DBL_EPSILON;
            double num2 = value1 - value2;
            return ((-num < num2) && (num > num2));
        }

        //public static bool AreClose(Point point1, Point point2)
        //{
        //    return (AreClose(point1.X, point2.X) && AreClose(point1.Y, point2.Y));
        //}

        //public static bool AreClose(Rect rect1, Rect rect2)
        //{
        //    if (rect1.IsEmpty)
        //    {
        //        return rect2.IsEmpty;
        //    }
        //    return (((!rect2.IsEmpty && AreClose(rect1.X, rect2.X)) && (AreClose(rect1.Y, rect2.Y) && AreClose(rect1.Height, rect2.Height))) && AreClose(rect1.Width, rect2.Width));
        //}

        //public static bool AreClose(Size size1, Size size2)
        //{
        //    return (AreClose(size1.Width, size2.Width) && AreClose(size1.Height, size2.Height));
        //}

        //public static bool AreClose(Vector vector1, Vector vector2)
        //{
        //    return (AreClose(vector1.X, vector2.X) && AreClose(vector1.Y, vector2.Y));
        //}

        //public static int DoubleToInt(double val)
        //{
        //    if (0.0 >= val)
        //    {
        //        return (int)(val - 0.5);
        //    }
        //    return (int)(val + 0.5);
        //}

        [Pure]
        public static bool GreaterThan(double value1, double value2)
        {
            return ((value1 > value2) && !AreClose(value1, value2));
        }

        [Pure]
        public static bool GreaterThanOrClose(double value1, double value2)
        {
            Contract.Ensures(value1 <= value2 || Contract.Result<bool>());
            if (value1 <= value2)
            {
                return AreClose(value1, value2);
            }
            return true;
        }

        //public static bool IsBetweenZeroAndOne(double val)
        //{
        //    return (GreaterThanOrClose(val, 0.0) && LessThanOrClose(val, 1.0));
        //}

        //public static bool IsOne(double value)
        //{
        //    return (Math.Abs((double)(value - 1.0)) < 2.2204460492503131E-15);
        //}

        //public static bool IsZero(double value)
        //{
        //    return (Math.Abs(value) < 2.2204460492503131E-15);
        //}

        [Pure]
        public static bool LessThan(double value1, double value2)
        {
            return value1 < value2 && !AreClose(value1, value2);
        }

        [Pure]
        public static bool LessThanOrClose(double value1, double value2)
        {
            Contract.Ensures(Contract.Result<bool>() == value1 < value2 || AreClose(value1, value2));

            return value1 < value2 || AreClose(value1, value2);
        }

        //public static bool RectHasNaN(Rect r)
        //{
        //    if ((!double.IsNaN(r.X) && !double.IsNaN(r.Y)) && (!double.IsNaN(r.Height) && !double.IsNaN(r.Width)))
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        internal static bool IsDoubleFinite(double d)
        {
            return (!double.IsInfinity(d) && !double.IsNaN(d));
        }
    }
}
