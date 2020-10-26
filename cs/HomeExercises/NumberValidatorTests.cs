using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	
	public class CreationNumberValidatorTests
	{

		[TestCase(3,2,true,TestName = "PrecisionIsPositive_NotThrowsArgumentException")]
		public void NoExceptionThrow(int precision,int scale, bool onlyPositive = true)
		{
			Action act = () => new NumberValidator(precision, scale);

			act.Should().NotThrow<ArgumentException>();
		}

		[TestCase(2, 2, TestName = "PrecisionEqualScale_ThrowsArgumentException")]
		[TestCase(-1, -3, TestName = "PrecisionIsNegative_ThrowsArgumentException")]
		[TestCase(0, -3, TestName = "PrecisionIsZero_ThrowsArgumentException")]
		[TestCase(1, 2, TestName = "PrecisionLessScale_ThrowsArgumentException")]
		[TestCase(1, -3, TestName = "ScaleIsNegative_ThrowsArgumentException")]
		public void ExceptionThrow(int precision, int scale)
		{
			Action act = () => new NumberValidator(precision, scale);

			act.Should().Throw<ArgumentException>();
		}
	}

	[TestFixture]
	public class IsValidNumberTests 
	{
		[TestCase(false, "", TestName = "EmptyLine_False")]
		[TestCase(false, null, TestName = "NullLine_False")]
		[TestCase(false, "+111.00", true, 3, 2, TestName = "IntPartPlusFracPartMoreThanPrecision_False")]
		[TestCase(false, "-111.00", true, 3, 2, TestName = " IsPositiveButLineWithMinus_False")]
		[TestCase(false, "-11.0", TestName = "FracPartMoreThanScale_False")]
		[TestCase(true, "+1", TestName = "RegexLineWithMinus_True")]
		[TestCase(true, "-1", false, TestName = "RegexLineWithPlus_True")]//
		[TestCase(true, "1.0", TestName = "RegexLineWithDot_True")]
		[TestCase(true, "1,0", TestName = "RegexLineWithComma_True")]
		[TestCase(false, "1.,0", TestName = "RegexLineWithCommaAndDot_False")]
		[TestCase(false, "aa1.aa0aa", TestName = "RegexLineWithLetters_False")]
		public void IsValidNumber(bool expectedResult, string value, bool onlyPositive = true, int precision = 17, int scale = 2)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);

			var actualResult = validator.IsValidNumber(value);

			actualResult.Should().Be(expectedResult);
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