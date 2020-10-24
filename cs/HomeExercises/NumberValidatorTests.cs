using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
        [Test]
        [TestCase(3, 2, true, "0", TestName = "IntegerNumber")]
        [TestCase(3, 1, true, "000", TestName = "CorrectPrecisionSchema")]
        [TestCase(3, 2, true, "0.00", TestName = "CorrectScaleSchema")]
        [TestCase(4, 2, true, "+1.23", TestName = "PositiveNumber")]
        [TestCase(4, 2, false, "-1.23", TestName = "NegativeNumberWithNotOnlyPositiveValidator")]
        [TestCase(3, 2, true, "0,00", TestName = "CommaSeparator")]
        public void IsValidNumber_ReturnsTrue(
                int precision, int scale, bool onlyPositive, string numberValue)
        {
            var validationResult = new NumberValidator(precision, scale, onlyPositive)
                .IsValidNumber(numberValue);

            validationResult.Should().BeTrue();
        }

        [Test]
        [TestCase(3, 2, true, null, TestName = "NullString")]
        [TestCase(3, 2, true, "", TestName = "StringIsEmpty")]
        [TestCase(3, 2, true, "a.sd", TestName = "NotNumber")]
        [TestCase(17, 2, true, ".0", TestName = "StartsWithSeparator")]
        [TestCase(17, 2, true, "0.", TestName = "EndsWithSeparator")]
        [TestCase(17, 2, true, "0|0", TestName = "UnexpectedSeparator")]
        [TestCase(17, 2, true, "0..0", TestName = "SeveralSeparators")]
        [TestCase(3, 2, true, "++1.0", TestName = "SeveralSigns")]
        [TestCase(4, 2, true, "-1.23", TestName = "NegativeNumberWithOnlyPositiveValidator")]
        [TestCase(2, 1, true, "+1.0", TestName = "SingAffectsDigitsAmount")]
        [TestCase(5, 2, true, "0.000", TestName = "ScaleBiggerThanValidatorScale")]
        [TestCase(3, 2, true, "00.00", TestName = "PrecisionBiggerThanValidatorPrecision")]
        public void IsValidNumber_ReturnsFalse(
                int precision, int scale, bool onlyPositive, string numberValue)
        {
            var validationResult = new NumberValidator(precision, scale, onlyPositive)
                .IsValidNumber(numberValue);

            validationResult.Should().BeFalse();
        }

        [Test]
        [TestCase(-1, 0, true, TestName = "NegativePrecision")]
        [TestCase(0, 0, true, TestName = "ZeroPrecision")]
        [TestCase(1, -1, true, TestName = "NegativeScale")]
        [TestCase(1, 1, true, TestName = "PrecisionEqualToScale")]
        [TestCase(1, 2, true, TestName = "PrecisionLessThanScale")]
        public void NumberValidatorConstructor_ShouldThrowArgumentException(
            int precision, int scale, bool onlyPositive)
        {
            Assert.Throws<ArgumentException>(() =>
                new NumberValidator(precision, scale, onlyPositive));
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