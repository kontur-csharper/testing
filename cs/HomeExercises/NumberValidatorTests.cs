using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[TestFixture]
	public class NumberValidatorTests
	{
		[TestCase(true)]
		[TestCase(false)]
		public void Construction_Throws_NegativePrecision(bool onlyPositive)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, onlyPositive));
		}
		
		[TestCase(true)]
		[TestCase(false)]
		public void Construction_Throws_NegativeScale(bool onlyPositive)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(2, -1, onlyPositive));
		}

		[TestCase(true)]
		[TestCase(false)]
		public void Constructor_Throws_ScaleBiggerThanPrecision(bool onlyPositive)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(2, 4, onlyPositive));
		}
		
		[TestCase(true)]
		[TestCase(false)]
		public void Constructor_Throws_PrecisionIsZero(bool onlyPositive)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(0, 0, onlyPositive));
		}
		
		public void Construction_DoesntThrows_CorrectInit()
		{
			Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
		}

		[TestCase("abc")]
		[TestCase("-abc")]
		[TestCase("ab.c")]
		[TestCase("aaaaaa")]
		[TestCase("aaaa.bbbb")]
		public void IsValidNumber_ReturnsFalse_NaN(string input)
		{
			var validator = new NumberValidator(5, 2, true);
			input.Should().Match(input => !validator.IsValidNumber(input));
		}
		
		[TestCase("0")]
		[TestCase("0.0")]
		[TestCase("00.00")]
		public void IsValidNumber_ReturnsTrue_PrecisionBiggerThanDigitsCount(string input)
		{
			var validator = new NumberValidator(17, 2, true);
			input.Should().Match(input => validator.IsValidNumber(input));
		}

		[TestCase("00.00")]
		[TestCase("0000")]
		[TestCase("0.000")]
		public void IsValidNumber_ReturnsFalse_PrecisionLessThanDigitsCount(string input)
		{
			var validator = new NumberValidator(3, 2, true);
			input.Should().Match(input => !validator.IsValidNumber(input));
		}
		
		[TestCase("-0.00")]
		[TestCase("+0.00")]
		public void IsValidNumber_ReturnsFalse_PrecisionLessThanDigitsCountWithSign(string input)
		{
			var validator = new NumberValidator(3, 2, true);
			input.Should().Match(input => !validator.IsValidNumber(input));
		}
		
		[TestCase("00.00")]
		[TestCase("0.000")]
		[TestCase("0000")]
		public void IsValidNumber_ReturnsTrue_PrecisionEqualDigitsCount(string input)
		{
			var validator = new NumberValidator(4, 3, true);
			input.Should().Match(input => validator.IsValidNumber(input));
		}
		
		[TestCase("-0.00")]
		[TestCase("+0.00")]
		[TestCase("+000")]
		[TestCase("-000")]
		public void IsValidNumber_ReturnsTrue_PrecisionEqualDigitsCountWithSign(string input)
		{
			var validator = new NumberValidator(4, 2, false);
			input.Should().Match(input => validator.IsValidNumber(input));
		}
		
		[TestCase("0.000")]
		[TestCase("000.000")]
		public void IsValidNumber_ReturnsFalse_ScaleLessThanFracPartCount(string input)
		{
			var validator = new NumberValidator(17, 2, true);
			input.Should().Match(input => !validator.IsValidNumber(input));
		}
		
		[Test]
		public void IsValidNumber_ReturnsTrue_ScaleIsZeroInputInteger()
		{
			var validator = new NumberValidator(17, 0, true);
			"0".Should().Match(input => validator.IsValidNumber(input));
		}
		
		[TestCase("0.0")]
		[TestCase("00.")]
		public void IsValidNumber_ReturnsFalse_ScaleIsZeroInputNotInteger(string input)
		{
			var validator = new NumberValidator(17, 0, true);
			input.Should().Match(input => !validator.IsValidNumber(input));
		}

		[TestCase("")]
		[TestCase("  ")]
		[TestCase("   \t")]
		public void IsValidNumber_ReturnsFalse_EmptyString(string input)
		{
			var validator = new NumberValidator(17, 0, true);
			input.Should().Match(input => !validator.IsValidNumber(input));
		}
		
		[TestCase("-1.1")]
		[TestCase("-1")]
		[TestCase("-1.111")]
		[TestCase("-1111.1")]
		[TestCase("-1111")]
		public void IsValidNumber_ReturnsFalse_OnlyPositiveWithNegativeInput(string input)
		{
			var validator = new NumberValidator(4, 2, true);
			input.Should().Match(input => !validator.IsValidNumber(input));
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