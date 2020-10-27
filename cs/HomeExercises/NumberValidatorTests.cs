using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-1, 2, true, "precision must be a positive number", TestName = "NegativePrecision")]
		[TestCase(0, 2, true, "precision must be a positive number", TestName = "ZeroPrecision")]
		[TestCase(1, -1, true, "scale must be a non-negative number less than precision", TestName = "NegativeScale")]
		[TestCase(1, 2, true, "scale must be a non-negative number less than precision", TestName = "ScaleMoreThenPrecision")]
		[TestCase(1, 1, true, "scale must be a non-negative number less than precision", TestName = "ScaleEqualPrecision")]
		public void Creating_ThrowsArgumentException_OnWrongData(int precision, int scale, bool onlyPositive, string message)
		{
			Action act = () => new NumberValidator(precision, scale, onlyPositive);
			act.Should().Throw<ArgumentException>()
				.WithMessage(message);
		}

		[TestCase("a", TestName = "OneLetter")]
		[TestCase("-a", TestName = "NegativeOneLetter")]
		[TestCase("a.", TestName = "OneLetterBeforeDot")]
		[TestCase(".a", TestName = "OneLetterAfterDot")]
		[TestCase("a.a", TestName = "TwoLettersWithDot")]
		[TestCase("a,", TestName = "OneLetterBeforeComma")]
		[TestCase(",a", TestName = "OneLetterAfterComma")]
		[TestCase("a,a", TestName = "TwoLettersWithComma")]
		[TestCase(",", TestName = "Comma")]
		[TestCase(".", TestName = "Dot")]
		[TestCase("", TestName = "Empty")]
		[TestCase(" ", TestName = "WhiteSpace")]
		[TestCase("\n", TestName = "NewLine")]
		[TestCase("\0", TestName = "ZeroSymbol")]
		[TestCase(null, TestName = "null")]
		[TestCase("++1", TestName = "DoublePlus")]
		[TestCase("--1", TestName = "DoubleMinus")]
		[TestCase("+-1", TestName = "PlusMinus")]
		[TestCase("1..5", TestName = "TwoDotsInNumber")]
		[TestCase("1,,5", TestName = "TwoCommaInNumber")]
		[TestCase("6,02E23", TestName = "BigInteger")]
		public void IsValidNumber_ReturnFalseNotOnNumbers(string checkedString)
		{
			var numberValidator = new NumberValidator(17, 2, true);
			numberValidator.IsValidNumber(checkedString).Should().BeFalse();
		}

		[Test]
		public void OnlyPositiveValidator_IsValidNumber_ShouldBeFalse_OnNegativeNumber()
		{
			new NumberValidator(17, 2, true).IsValidNumber("-1");
		}

		[TestCase("-100", TestName = "NegativeInteger")]
		[TestCase("1000", TestName = "PositiveInteger")]
		[TestCase("-1.00", TestName = "NegativeNumberWithTwoDigitsAfterDot")]
		[TestCase("-1,00", TestName = "NegativeNumberWithTwoDigitsAfterComma")]
		[TestCase("10.00", TestName = "TwoDigitsAfterDot")]
		[TestCase("10,00", TestName = "TwoDigitsAfterComma")]
		public void IsValidNumber_ShouldBeFalseOnTooLongPrecisionNumbers(string checkedNumber)
		{
			var numberValidator = new NumberValidator(3, 2, false);
			numberValidator.IsValidNumber(checkedNumber).Should().BeFalse();
		}

		[TestCase("1.000", TestName = "WithDot")]
		[TestCase("1,000", TestName = "WithComma")]
		[TestCase("-1.000", TestName = "NegativeWithDot")]
		[TestCase("-1,000", TestName = "NegativeWithComma")]
		public void IsValidNumber_ShouldBeFalseOnTooLongScaleNumbers(string checkedNumber)
		{
			var numberValidator = new NumberValidator(10, 2, false);
			numberValidator.IsValidNumber(checkedNumber).Should().BeFalse();
		}
		
		[TestCase("1.5", TestName = "OneDigitAfterDot")]
		[TestCase("1.05", TestName = "TwoDigitsAfterDot")]
		[TestCase("1", TestName = "Integer")]
		[TestCase("1,5", TestName = "OneDigitAfterComma")]
		[TestCase("1,05", TestName = "TwoDigitsAfterComma")]
		[TestCase("+1.5", TestName = "OneDigitAfterDotWithPlus")]
		[TestCase("+1.05", TestName = "TwoDigitsAfterDotWithPlus")]
		[TestCase("+1", TestName = "IntegerWithPlus")]
		[TestCase("+1,5", TestName = "OneDigitAfterCommaWithPlus")]
		[TestCase("+1,05", TestName = "TwoDigitsAfterCommaWithPlus")]
		public void OnlyPositiveValidator_ShouldValidate(string checkedNumber)
		{
			var numberValidator = new NumberValidator(17, 2, true);
			numberValidator.IsValidNumber(checkedNumber).Should().BeTrue();
		}
		
		[TestCase("1.05", TestName = "TwoDigitsAfterDot")]
		[TestCase("100", TestName = "Integer")]
		[TestCase("+100", TestName = "IntegerWithPlus")]
		[TestCase("+10,0", TestName = "OneDigitAfterComma_WithPlus")]
		[TestCase("+10.0", TestName = "OneDigitAfterDot_WithPlus")]
		[TestCase("1,05", TestName = "TwoDigitsAfterComma")]
		[TestCase("-1.5", TestName = "NegativeWithOneDigitAfterDot")]
		[TestCase("-10", TestName = "NegativeInteger")]
		public void NotOnlyPositiveValidator_ShouldValidate(string checkedNumber)
		{
			var numberValidator = new NumberValidator(3, 2, false);
			numberValidator.IsValidNumber(checkedNumber).Should().BeTrue();
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
				throw new ArgumentException("scale must be a non-negative number less than precision");
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