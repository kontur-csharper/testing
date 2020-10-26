using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidator_Should
	{
		[TestCase(2, 1)]
		[TestCase(1, 0)]
		public void NotThrow_OnCreation_IfScaleNonNegativeAndLessThanPrecision(int precision, int scale)
		{
			Assert.DoesNotThrow(() => new NumberValidator(precision, scale));
		}
		
		[Test]
		public void NotThrow_OnCreation_OnlyPositiveNumberValidator()
		{
			Assert.DoesNotThrow(() => new NumberValidator(2, 1, true));
		}
		
		[Test]
		public void ThrowArgumentException_OnCreation_IfScaleNegative()
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(2, -1));
		}
		
		[TestCase(0, -1)]
		[TestCase(-1, -2)]
		public void ThrowArgumentException_OnCreation_IfPrecisionNonPositive(int precision, int scale)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale));
		}
		
		[TestCase(2, 2)]
		[TestCase(2, 3)]
		public void ThrowArgumentException_OnCreation_IfScaleNotLessThanPrecision(int precision, int scale)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale));
		}
		
		[TestCase("123")]
		[TestCase("12.3")]
		[TestCase("-123")]
		[TestCase("+123")]
		public void BeTrue_IsValidNumber_IfNumberLengthNotGreaterThanPrecision(string number)
		{
			var validator = new NumberValidator(4, 3);
			
			validator.IsValidNumber(number).Should().BeTrue();
		}
		
		[TestCase("12.3")]
		[TestCase("12.34")]
		public void BeTrue_IsValidNumber_IfFractionalPartLengthNotGreaterThanScale(string number)
		{
			var validator = new NumberValidator(5, 2);
			
			validator.IsValidNumber(number).Should().BeTrue();
		}
		
		[TestCase("123")]
		[TestCase("12.3")]
		[TestCase("+123")]
		[TestCase("+12.3")]
		public void BeTrue_IsValidNumber_IfPositiveNumber_WithOnlyPositiveNumberValidator(string number)
		{
			var validator = new NumberValidator(10, 9, true);
			
			validator.IsValidNumber(number).Should().BeTrue();
		}
		
		[TestCase(null)]
		[TestCase("")]
		[TestCase("a.cd")]
		[TestCase("1.2.3")]
		public void BeFalse_IsValidNumber_IfStringIsIncorrect(string number)
		{
			var validator = new NumberValidator(10, 9);
			
			validator.IsValidNumber(number).Should().BeFalse();
		}
		
		[TestCase("1234")]
		[TestCase("123.4")]
		[TestCase("-123")]
		[TestCase("+123")]
		[TestCase("+12.3")]
		public void BeFalse_IsValidNumber_IfNumberLengthGreaterThanPrecision(string number)
		{
			var validator = new NumberValidator(3, 2);
			
			validator.IsValidNumber(number).Should().BeFalse();
		}
		
		[Test]
		public void BeFalse_IsValidNumber_IfFractionalPartLengthGreaterThanScale()
		{
			var validator = new NumberValidator(10, 2);
			
			validator.IsValidNumber("1.234").Should().BeFalse();
		}
		
		[Test]
		public void BeFalse_IsValidNumber_IfNegativeNumber_WithOnlyPositiveNumberValidator()
		{
			var validator = new NumberValidator(10, 2, true);
			
			validator.IsValidNumber("-123").Should().BeFalse();
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