using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		public void Ctor_WithValidParameters_DoesNotThrowException()
		{
			Action act = () => new NumberValidator(3, 1, true);

			act.Should().NotThrow(CreateTestMessage(3, 1, true));
		}

		[TestCase(0, TestName = "PrecisionEqualZero")]
		[TestCase(-1, TestName = "PrecisionLessThanZero")]
		public void Ctor_PrecisionLessOrEqualZero_ThrowException(int precision)
		{
			CtorThrowArgumentException(precision);
		}

		[Test]
		public void Ctor_ScaleLessThanZero_ThrowException()
		{
			CtorThrowArgumentException(2, -1);
		}

		[TestCase(2, 3, TestName = "ScaleGreaterThanPrecision")]
		[TestCase(2, 2, TestName = "ScaleEqualPrecision")]
		public void Ctor_ScaleGreaterOrEqualPrecision_ThrowException(int precision, int scale)
		{
			CtorThrowArgumentException(precision, scale);
		}

		[TestCase(null, TestName = "GotNull")]
		[TestCase("", TestName = "GotEmptyString")]
		public void IsValidNumber_GotNullOrEmpty_ReturnFalse(string isValidNumberParameter)
		{
			IsValidNumberTest(17, 0, false, isValidNumberParameter, false);
		}

		[TestCase("a.bc", TestName = "LettersInsteadNumbers")]
		[TestCase("1.", TestName = "ParameterHasSeparatorButNoDecimalPart")]
		[TestCase(".54", TestName = "ParameterHasSeparatorButNoIntegerPart")]
		[TestCase("+", TestName = "ParameterIsJustSignOfNumber")]
		[TestCase("1,23,4", TestName = "ParameterHasMoreThanOneSeparator")]
		[TestCase("++1.23", TestName = "ParameterHasMoreThanOneSignOfNumber")]
		[TestCase("1;23", TestName = "SeparatorIsNotDotOrComma")]
		public void IsValidNumber_ParameterHasNotValidForm_ReturnFalse(string isValidNumberParameter)
		{
			IsValidNumberTest(10, 4, false, isValidNumberParameter, false);
		}

		[Test]
		public void IsValidNumber_DecimalPartOfParameterGreaterThanScale_ReturnFalse()
		{
			IsValidNumberTest(10, 1, false, "1.23", false);
		}

		[Test]
		public void IsValidNumber_SignCountOfParameterGreaterThanPrecision_ReturnFalse()
		{
			IsValidNumberTest(3, 2, false, "12.34", false);
		}

		[TestCase(".", TestName = "SeparatorIsDot")]
		[TestCase(",", TestName = "SeparatorIsComma")]
		public void IsValidNumber_SeparatorIsNotIncludeInPrecision(string separator)
		{
			var isValidNumberParameter = $"0{separator}0";
			IsValidNumberTest(2, 1, false, isValidNumberParameter, true);
		}

		[TestCase(".", TestName = "SeparatorIsDot")]
		[TestCase(",", TestName = "SeparatorIsComma")]
		public void IsValidNumber_SeparatorCanBeDotOrComma(string separator)
		{
			var isValidNumberParameter = $"0{separator}0";
			IsValidNumberTest(5, 2, false, isValidNumberParameter, true);
		}

		[TestCase("+", TestName = "PlusIsCounted")]
		[TestCase("-", TestName = "MinusIsCounted")]
		public void IsValidNumber_SignOfNumberIncludeInPrecision(string sign)
		{
			var isValidNumberParameter = sign + "1.23";
			IsValidNumberTest(3, 2, false, isValidNumberParameter, false);
		}

		[Test]
		public void IsValidNumber_GotNegativeNumberWhenExpectOnlyPositive_ReturnFalse()
		{
			IsValidNumberTest(5, 0, true, "-12", false);
		}

		[Test]
		public void IsValidNumber_WhenScaleIsNotZero_ParameterCanBeWithoutDecimalPart()
		{
			IsValidNumberTest(10, 2, false, "123", true);
		}

		[Test]
		public void IsValidNumber_WhenGotValidNumber_ReturnTrue()
		{
			IsValidNumberTest(17, 2, true, "0.0", true);
		}

		private static void IsValidNumberTest(int precision, int scale, bool onlyPositive,
			string isValidNumberParameter, bool expectedResult)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);

			var actualResult = validator.IsValidNumber(isValidNumberParameter);

			actualResult.Should().Be(expectedResult,
				because: CreateTestMessage(precision, scale, onlyPositive, isValidNumberParameter));
		}

		private static void CtorThrowArgumentException(int precision, int scale = 0)
		{
			Action act = () => new NumberValidator(precision, scale);

			act.Should().Throw<ArgumentException>(because: CreateTestMessage(precision, scale));
		}

		private static string CreateTestMessage(int precision, int scale = 0,
			bool onlyPositive = false, string? isValidNumberParameter = null)
		{
			var message = "these parameters were passed:\n" +
			              $"precision = {precision}\n" +
			              $"scale = {scale}\n" +
			              $"onlyPositive = {onlyPositive}\n";
			if (isValidNumberParameter != null)
				message += $"IsValidNumber parameter = {isValidNumberParameter}\n";
			return message;
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