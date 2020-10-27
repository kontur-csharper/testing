using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
        private NumberValidator numberValidator;

        [OneTimeSetUp]
        public void SetUp()
        {
            numberValidator = new NumberValidator(6, 2);
        }

        [TestCase(0, 2, TestName = "when precision is zero")]
        [TestCase(-1, 2, TestName = "when precision is negative")]
        [TestCase(2, -1, TestName = "when scale is negative")]
        [TestCase(2, 2, TestName = "when scale equals precision")]
        [TestCase(2, 3, TestName = "when scale is more than precision")]
        public void Constructor_ThrowsArgumentException(int precision, int scale)
        {
            Action act = () => new NumberValidator(precision, scale);

            act.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Constructor_NotThrowsException_IfArgumentsIsCorrect()
        {
            Action act = () => new NumberValidator(3, 2, true);

            act.Should().NotThrow();
        }

        [Test]
        public void IsValidNumber_IsFalse_IfNumberIsNegativeWhenOnlyPositive()
        {
            var onlyPositiveNumberValidator = new NumberValidator(6, 2, true);

            onlyPositiveNumberValidator.IsValidNumber("-0.1").Should().BeFalse();
        }

        [TestCase(".05")]
        [TestCase(",05")]
        [TestCase("-+5")]
        [TestCase("5:2")]
        [TestCase("1.1e1")]
        [TestCase("A,5")]
        [TestCase("5,A")]
        [TestCase("  ")]
        [TestCase("55.")]
        [TestCase("55,")]
        [TestCase("5..5")]
        [TestCase("5,,5")]
        public void IsValidNumber_IsFalse_IfFormatIsIncorrect(string number)
        {
            numberValidator.IsValidNumber(number).Should().BeFalse();
        }

        [TestCase(null, TestName = "when string is null")]
        [TestCase("", TestName = "when string is empty")]
        [TestCase("13.123", TestName = "when fractal part length is more than scale")]
        public void IsValidNumber_IsFalse(string number)
        {
            numberValidator.IsValidNumber(number).Should().BeFalse();
        }

        [TestCase("1234567")]
        [TestCase("12345.67")]
        [TestCase("-123456")]
        [TestCase("-1234.56")]
        [TestCase("+123456")]
        [TestCase("+1234.56")]
        public void IsValidNumber_IsFalse_IfNumberLengthMoreThanPrecision(string number)
        {
            numberValidator.IsValidNumber(number).Should().BeFalse();
        }

        [TestCase("-123.5", TestName = "when number is negative when not only-positive")]
        [TestCase("123,5", TestName = "when number is positive when not only-positive")]
        [TestCase("+123.5", TestName = "when number has plus")]
        [TestCase("123,12", TestName = "when fractal part length equals scale")]
        [TestCase("123", TestName = "when fractal part is empty")]
        public void IsValidNumber_IsTrue(string number)
        {
            numberValidator.IsValidNumber(number).Should().BeTrue();
        }

        [Test]
        public void IsValidNumber_IsTrue_IfNumberIsPositiveWhenOnlyPositive()
        {
            var onlyPositiveNumberValidator = new NumberValidator(6, 2, true);

            onlyPositiveNumberValidator.IsValidNumber("123.5").Should().BeTrue();
        }

        [TestCase("0000")]
        [TestCase("0000.0")]
        [TestCase("+0000")]
        [TestCase("-0000")]
        public void IsValidNumber_IsTrue_IfNumberContainsOnlyZeros(string number)
        {
            numberValidator.IsValidNumber(number).Should().BeTrue();
        }

        [TestCase("123456")]
        [TestCase("12345.6")]
        [TestCase("-12345")]
        [TestCase("-1234,5")]
        [TestCase("+12345")]
        [TestCase("+1234,5")]
        public void IsValidNumber_IsTrue_IfNumberLengthEqualsPrecision(string number)
        {
            numberValidator.IsValidNumber(number).Should().BeTrue();
        }

        [Test]
        public void IsValidNumber_IsTrue_IfScaleIsZero()
        {
            var numberValidatorWithZeroScale = new NumberValidator(5);

            numberValidatorWithZeroScale.IsValidNumber("123");
        }
    }

    public class NumberValidator
    {
        private readonly Regex numberRegex;
        private readonly bool onlyPositive;
        private readonly int precision;
        private readonly int scale;

        public NumberValidator(int precision, int scale = 0, bool onlyPositive = false)
        {
            this.precision = precision;
            this.scale = scale;
            this.onlyPositive = onlyPositive;
            if (precision <= 0)
                throw new ArgumentException("precision must be a positive number");
            if (scale < 0 || scale >= precision)
                throw new ArgumentException("precision must be a non-negative number less or equal than precision");
            numberRegex = new Regex(@"^([+-]?)(\d+)([.,](\d+))?$", RegexOptions.IgnoreCase);
        }

        public bool IsValidNumber(string value)
        {
            // Проверяем соответствие входного значения формату N(m,k), в соответствии с правилом, 
            // описанным в Формате описи документов, направляемых в налоговый орган в электронном виде по телекоммуникационным каналам связи:
            // Формат числового значения указывается в виде N(m.к), где m – максимальное количество знаков в числе, включая знак (для отрицательного числа), 
            // целую и дробную часть числа без разделяющей десятичной точки, k – максимальное число знаков дробной части числа. 
            // Если число знаков дробной части числа равно 0 (т.е. число целое), то формат числового значения имеет вид N(m).

            if (string.IsNullOrEmpty(value))
                return false;

            var match = numberRegex.Match(value);
            if (!match.Success)
                return false;

            // Знак и целая часть
            var intPart = match.Groups[1].Value.Length + match.Groups[2].Value.Length;
            // Дробная часть
            var fracPart = match.Groups[4].Value.Length;

            if (intPart + fracPart > precision || fracPart > scale)
                return false;

            if (onlyPositive && match.Groups[1].Value == "-")
                return false;
            return true;
        }
    }
}