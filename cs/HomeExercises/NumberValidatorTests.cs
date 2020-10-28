using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-1, 0, TestName = "Constructor throws when precision < 0 and  scale == 0")]
		[TestCase(1, 2, TestName = "Constructor throws when scale > precision")]
		[TestCase(1, -1, TestName = "Constructor throws when scale < 0")]
		[TestCase(1, 1, TestName = "Constructor throws when scale == precision")]
		[TestCase(0, 0, TestName = "Constructor throws when precision == 0 ")]
		public void Constructor_ThrowArgumentException(int precision, int scale)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale));
		}

		[TestCase(2, 0, TestName = "Constructor throws when precision > 0 and scale == 0")]
		[TestCase(2, 1, TestName = "Constructor throws when precision > 0 and scale > 0 and precision > scale")]
		public void Constructor_NotThrowArgumentException(int precision, int scale)
		{
			Assert.DoesNotThrow(() => new NumberValidator(precision, scale, true));
		}

		[TestCase("2.4", TestName = "Number is valid when string")]
		[TestCase("2,4", TestName = "Number is valid when separator is ','")]
		[TestCase("0.1", TestName = "Number is valid when string less then 1")]
		[TestCase("2", TestName = "Number is valid when string without dot")]
		[TestCase("-12.12", TestName = "Number is valid when string with negative number")]
		[TestCase("+2", TestName = "Number is valid when string with '+'")]
		[TestCase("+12.12", TestName = "Number is valid when string length with '+' = precision")]
		[TestCase("214,14", TestName = "Number is valid when separator is ',' and length = precision")]
		[TestCase("00.000", TestName = "Number is valid when 00.000")]
		public void IsValidNumber_Valid_WhenNotOnlyPositive(string line)
		{
			new NumberValidator(5, 3).IsValidNumber(line).Should().BeTrue();
		}

		[TestCase("12.1", TestName = "Number is valid when string with positive number when only positive")]
		[TestCase("2", TestName = "Number is valid when string without dot when only positive")]
		[TestCase("0.1", TestName = "Number is valid when string less then 0 when only positive")]
		public void IsValidNumber_Valid_WhenOnlyPositive(string line)
		{
			new NumberValidator(5, 3, true).IsValidNumber(line).Should().BeTrue();
		}

		[TestCase("11.1111", TestName = "Number is not valid when to long string")]
		[TestCase("+1111.1", TestName = "Number is not valid when length more at plus sign")]
		[TestCase("1.000000000", TestName = "Number is not valid when length more at non significant zeros")]
		[TestCase("a.sd", TestName = "Number is not valid when only letters string")]
		[TestCase("4.44d", TestName = "Number is not valid when string with letters")]
		[TestCase(null, TestName = "Number is not valid when Null")]
		[TestCase("", TestName = "Number is not valid when empty string")]
		[TestCase("1 2", TestName = "Number is not valid when string with space ")]
		public void IsValidNumber_NotValid_WhenNotOnlyPositive(string line)
		{
			new NumberValidator(5, 3).IsValidNumber(line).Should().BeFalse();
		}

		[TestCase("-0.00", TestName = "Number is not valid when negative value when only positive")]
		[TestCase("-11.1111", TestName = "Number is not valid when to long string when only positive")]
		[TestCase("-1.000000000", TestName = "Number is not valid when length more at non significant zeros when only positive")]
		[TestCase("a.sd", TestName = "Number is not valid when only letters string when only positive")]
		[TestCase("4.44d", TestName = "Number is not valid when string with letters when only positive")]
		[TestCase(null, TestName = "Number is not valid when Null when only positive")]
		[TestCase("", TestName = "Number is not valid when empty string when only positive")]
		public void IsValidNumber_NotValid_WhenOnlyPositive(string line)
		{
			new NumberValidator(5, 3, true).IsValidNumber(line).Should().BeFalse();
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