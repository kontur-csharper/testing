using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
        [Test]
        [TestCase(-1, 0, TestName = "WhenNonPositivePrecision")]
        [TestCase(3, -2, TestName = "WhenNegativeScale")]
        [TestCase(1, 3, TestName = "WhenScaleOverPrecision")]
        public void NumberValidatorThrowExceptionOnWrongParameters(int precision, int scale)
        {
            ((Action)(() => new NumberValidator(precision, scale))).Should().Throw<ArgumentException>();
        }

        [Test]
        [TestCase(2, 1, TestName = "WhenPrecisionPositive")]
        [TestCase(2, 0, TestName = "WhenScaleZero")]
        public void NumberValidatorNotThrowExceptionOnCorrectParameters(int precision, int scale)
        {
            ((Action)(() => new NumberValidator(precision, scale))).Should().NotThrow();
        }

        [Test]
        [TestCase(17, 2, "0.0", true, TestName = "WhenValueCorrect")]
        [TestCase(17, 0, "2", true, TestName = "WhenValueInt")]
        [TestCase(17, 2, "+2.3", true, TestName = "WhenValuePositive")]
        [TestCase(17, 2, "-2.3", true, TestName = "WhenValueNegative")]
        [TestCase(17, 2, "2,3", true, TestName = "WhenCommaIsSeparatorInValue")]
        [TestCase(4, 2, "12,12", true, TestName = "SeparatorNotAffectOnCountNumberInPrecision")]
        [TestCase(3, 2, "12,12", false, TestName = "WhenNumbersInValueOverPrecision")]
        [TestCase(5, 1, "12,123", false, TestName = "WhenFractionalPartInValueOverScale")]
        [TestCase(4, 2, "-123,1", false, TestName = "SignAffectOnCountNumberInPrecision")]
        [TestCase(4, 2, "asfa", false, TestName = "WhenValueWrong")]
        [TestCase(4, 2, "12,", false, TestName = "WhenValueNotContainFractionalPart")]
        [TestCase(4, 2, ",12", false, TestName = "WhenValueNotContainIntegerPart")]
        [TestCase(4, 2, " ", false, TestName = "WhenValueEmpty")]
        [TestCase(4, 2, null, false, TestName = "WhenValueNull")]
        [TestCase(4, 2, " 12.1", false, TestName = "WhenValueNotAtBeginningOfLine")]
        [TestCase(4, 2, "12.1 ", false, TestName = "WhenValueNotAtEndOfLine")]
        public void ValidNumberOnValues(int precision, int scale, string value, bool expected)
        {
            var validator = new NumberValidator(precision, scale);
            validator.IsValidNumber(value).Should().Be(expected);
        }

        [Test]
        public void ValidNumberOnValuesWhenValueNegativeAndOnlyPositiveIsTrue()
        {
            var validator = new NumberValidator(4, 2, true);
            validator.IsValidNumber("-1,1").Should().BeFalse();
        }
    }

    public class NumberValidator
    {
        private readonly static Regex numberRegex;
        private readonly bool onlyPositive;
        private readonly int precision;
        private readonly int scale;

        static NumberValidator()
        {
            numberRegex = new Regex(@"^([+-]?)(\d+)([.,](\d+))?$", RegexOptions.IgnoreCase);
        }

        public NumberValidator(int precision, int scale = 0, bool onlyPositive = false)
        {
            this.precision = precision;
            this.scale = scale;
            this.onlyPositive = onlyPositive;
            if (precision <= 0)
                throw new ArgumentException("precision must be a positive number");
            if (scale < 0 || scale >= precision)
                throw new ArgumentException("precision must be a non-negative number less or equal than precision");
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