using System;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
        [TestCase(-1, 0, TestName = "PrecisionIsNegative")]
        [TestCase(0, 0, TestName = "PrecisionIsZero")]
        [TestCase(1, -1, TestName = "ScaleIsNegative")]
        [TestCase(1, 2, TestName = "ScaleGreaterPrecision")]
        public void NumberValidatorConstructor_OnIncorrectInputs_ThrowArgumentException(int precision, int scale)
        {
            Assert.Throws<ArgumentException>(() => { _ = new NumberValidator(precision, scale); });
        }

        [TestCase(1, 0, TestName = "WhenScaleIsZero")]
        [TestCase(3, 2, TestName = "WhenScaleLessPrecision")]
        [TestCase(1000, 999, TestName = "WhenScalePrecisionBigAndCorrect")]
        public void NumberValidatorConstructor_OnCorrectInputs_DoesNotThrow(int precision, int scale)
        {
            Assert.DoesNotThrow(() => { _ = new NumberValidator(precision, scale); });
        }


        [TestCase(null, TestName = "NullReference")]
        [TestCase("", TestName = "EmptyString")]
        [TestCase(" ", TestName = "Space")]
        [TestCase(" 1", TestName = "StartsWithSpace")]
        [TestCase("1 ", TestName = "EndsWithSpace")]
        [TestCase("abc", TestName = "ValueIsLettersOnly")]
        [TestCase("1vc", TestName = "IntegerContainsLetters")]
        [TestCase("1.2c", TestName = "FractContainsLetters")]
        [TestCase("a.bc", TestName = "FractIsLettersOnly")]
        [TestCase("+a.b", TestName = "LettersWithSign")]
        [TestCase("+1.c", TestName = "SignedLettersAndNumbers")]
        [TestCase("1.", TestName = "MissingFractionalPart")]
        [TestCase("+-1", TestName = "TwiceSigns")]
        [TestCase("9999", TestName = "TooBigValue")]
        [TestCase("0.000", TestName = "TooBigFractionalPart")]
        public void IsValidNumber_OnIncorrectInputs_ReturnFalse(string value)
        {
            Assert.IsFalse(new NumberValidator(3, 2).IsValidNumber(value));
        }

        [TestCase("0", TestName = "IntegerOnly")]
        [TestCase("999", TestName = "MaxPossibleCorrectInteger")]
        [TestCase("+10", TestName = "PositiveSignedInteger")]
        [TestCase("-10", TestName = "NegativeSignedInteger")]
        [TestCase("0.0", TestName = "SmallCorrectValue")]
        [TestCase("0,0", TestName = "SeparatorIsComma")]
        [TestCase("9.99", TestName = "MaxPossibleCorrectDecimal")]
        [TestCase("+9.9", TestName = "PositiveSignedDecimal")]
        [TestCase("-9.9", TestName = "NegativeSignedDecimal")]
        public void IsValidNumber_OnCorrectInputsAndSignDoesntMatter_ReturnTrue(string value)
        {
            Assert.IsTrue(new NumberValidator(3, 2).IsValidNumber(value));
        }

        [TestCase("100", TestName = "PositiveIntWithoutSign")]
        [TestCase("+1", TestName = "PositiveInt")]
        [TestCase("+99", TestName = "MaxPositiveInt")]
        [TestCase("+1.0", TestName = "PositiveDecimal")]
        [TestCase("+9.9", TestName = "MaxPositiveDecimal")]
        public void IsValidNumber_OnPositiveInputsWithOnlyPositiveValidator_ReturnTrue(string value)
        {
            Assert.IsTrue(new NumberValidator(3, 2, true).IsValidNumber(value));
        }

        [TestCase("-1", TestName = "NegativeInt")]
        [TestCase("-99", TestName = "MaxNegativeInt")]
        [TestCase("-1.0", TestName = "NegativeDecimal")]
        [TestCase("-9.9", TestName = "MaxNegativeDecimal")]
        public void IsValidNumber_OnNegativeInputsWithOnlyPositiveValidator_ReturnFalse(string value)
        {
            Assert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber(value));
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