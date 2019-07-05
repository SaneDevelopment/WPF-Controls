// -----------------------------------------------------------------------
// <copyright file="DependencyPropertyUtilTests.cs" company="Sane Development">
//
// Sane Development WPF Controls Library Unit Tests.
//
// The BSD 3-Clause License.
//
// Copyright (c) Sane Development.
// All rights reserved.
//
// See LICENSE file for full license information.
//
// </copyright>
// -----------------------------------------------------------------------

namespace SaneDevelopment.WPF.Controls.Tests
{
    using System;
    using Xunit;

    public class DependencyPropertyUtilTests
    {
        #region IsValidDoubleValue

        [Fact]
        public void IsValidDoubleValue_WhenNull_ReturnsFalse()
        {
            var res = DependencyPropertyUtil.IsValidDoubleValue(null);

            Assert.False(res);
        }

        [Fact]
        public void IsValidDoubleValue_WhenNaN_ReturnsFalse()
        {
            var res = DependencyPropertyUtil.IsValidDoubleValue(double.NaN);

            Assert.False(res);
        }

        [Fact]
        public void IsValidDoubleValue_WhenNegativeInfinity_ReturnsFalse()
        {
            var res = DependencyPropertyUtil.IsValidDoubleValue(double.NegativeInfinity);

            Assert.False(res);
        }

        [Fact]
        public void IsValidDoubleValue_WhenPositiveInfinity_ReturnsFalse()
        {
            var res = DependencyPropertyUtil.IsValidDoubleValue(double.PositiveInfinity);

            Assert.False(res);
        }

        [Theory]
        [InlineData(double.MinValue)]
        [InlineData(double.MaxValue)]
        [InlineData(double.Epsilon)]
        [InlineData(0)]
        public void IsValidDoubleValue_WhenDouble_ReturnsTrue(double valueToValidate)
        {
            var res = DependencyPropertyUtil.IsValidDoubleValue(valueToValidate);

            Assert.True(res);
        }

        #endregion IsValidDoubleValue

        #region IsValidDateTimeValue

        [Fact]
        public void IsValidDateTimeValue_WhenNull_ReturnsFalse()
        {
            var res = DependencyPropertyUtil.IsValidDateTimeValue(null);

            Assert.False(res);
        }

        [Fact]
        public void IsValidDateTimeValue_WhenIsDateTime_ReturnsTrue()
        {
            var valueToValidate = DateTime.MinValue;

            var res = DependencyPropertyUtil.IsValidDateTimeValue(valueToValidate);

            Assert.True(res);
        }

        [Theory]
        [InlineData(0d)]
        [InlineData(true)]
        [InlineData(false)]
        [InlineData(0)]
        [InlineData("")]
        [InlineData('\0')]
        public void IsValidDateTimeValue_WhenIsNotDateTime_ReturnsFalse(object valueToValidate)
        {
            var res = DependencyPropertyUtil.IsValidDateTimeValue(valueToValidate);

            Assert.False(res);
        }

        #endregion IsValidDateTimeValue

        #region IsValidBoolValue

        [Fact]
        public void IsValidBoolValue_WhenNull_ReturnsFalse()
        {
            var res = DependencyPropertyUtil.IsValidBoolValue(null);

            Assert.False(res);
        }

        [Fact]
        public void IsValidBoolValue_WhenIsBool_ReturnsTrue()
        {
            var valueToValidate = true;

            var res = DependencyPropertyUtil.IsValidBoolValue(valueToValidate);

            Assert.True(res);
        }

        [Theory]
        [InlineData(0d)]
        [InlineData(0)]
        [InlineData("")]
        [InlineData('\0')]
        public void IsValidBoolValue_WhenIsNotBool_ReturnsFalse(object valueToValidate)
        {
            var res = DependencyPropertyUtil.IsValidBoolValue(valueToValidate);

            Assert.False(res);
        }

        #endregion IsValidBoolValue

        #region IsValidTimeSpanValue

        [Fact]
        public void IsValidTimeSpanValue_WhenNull_ReturnsFalse()
        {
            var res = DependencyPropertyUtil.IsValidTimeSpanValue(null);

            Assert.False(res);
        }

        [Fact]
        public void IsValidTimeSpanValue_WhenIsTimeSpan_ReturnsTrue()
        {
            var valueToValidate = TimeSpan.MinValue;

            var res = DependencyPropertyUtil.IsValidTimeSpanValue(valueToValidate);

            Assert.True(res);
        }

        [Theory]
        [InlineData(0d)]
        [InlineData(true)]
        [InlineData(false)]
        [InlineData(0)]
        [InlineData("")]
        [InlineData('\0')]
        public void IsValidTimeSpanValue_WhenIsNotTimeSpan_ReturnsFalse(object valueToValidate)
        {
            var res = DependencyPropertyUtil.IsValidTimeSpanValue(valueToValidate);

            Assert.False(res);
        }

        #endregion IsValidTimeSpanValue
    }
}
