using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	
	public class NumberValidatorTests
	{
		[TestCase("1", 4, 2, TestName = "When unsigned number without fractional part")]
		[TestCase("+1", 4, 2, TestName = "When positive number without fractional part")]
		[TestCase("-1", 4, 2, TestName = "When negative number without fractional part")]
		[TestCase("1.1", 4, 2, TestName = "When unsigned number with fractional part length 1")]
		[TestCase("1.11", 4, 2, TestName = "When unsigned number with fractional part length greater than 1")]
		[TestCase("1,11", 4, 2, TestName = "When unsigned number with separator comma")]
		[TestCase("+1.11", 4, 2, TestName = "When positive number with fractional part")]
		[TestCase("-1.11", 4, 2, TestName = "When negative number with fractional part")]
		[TestCase("010", 4, 2, TestName = "When unsigned number starts with 0 without fractional part")]
		[TestCase("+010", 4, 2, TestName = "When positive number starts with 0 without fractional part")]
		[TestCase("-010", 4, 2, TestName = "When negative number starts with 0 without fractional part")]
		[TestCase("01.1", 4, 2, TestName = "When unsigned number starts with 0 with fractional part")]
		[TestCase("-01.1", 4, 2, TestName = "When negative number starts with 0 with fractional part")]
		[TestCase("+01.0", 4, 2, TestName = "When positive number starts with 0 with fractional part")]
		[TestCase("൨", 4, 2, TestName = "When Malayalam numerals")]
		[TestCase("۴", 4, 2, TestName = "When Eastern Arabic numerals")]
		[TestCase("٥", 4, 2, TestName = "When Arabic numerals")]
		[TestCase("000", 4, 2, TestName = "When number consists of only zeros without fractional part")]
		[TestCase("1.00000", 10, 9, TestName = "When fractional part consists only of zeros")]
		[TestCase("9223372036854775807", 100, 9, TestName = "When number is long.MaxValue")]
		[TestCase("55", 2, 1, TestName = "When precision of number is equal to specified precision")]
		[TestCase("11.11", 5, 2, TestName = "When scale of number is equal to specified scale")]
		[TestCase("11.11", 5, 3, TestName = "When scale of number less than specified scale")]
		[TestCase("+11", 5, 3, true, TestName = "When positive number and only positive on")]
		[TestCase("11", 5, 3, true, TestName = "When unsigned number and only positive on")]
		[TestCase("0", 5, 3, true, TestName = "When number is zero and only positive on")]
		// Нет поддержки римских цифр.

		public void IsValidNumber_Validates(string number, int precision, int scale = 0, bool onlyPositive = false)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number).Should().BeTrue();
		}

		[TestCase(".0", 4, 2, TestName = "When number starts with dot")]
		[TestCase(",0", 4, 2, TestName = "When number starts with comma")]
		[TestCase("0.", 4, 2, TestName = "Number ends with dot")]
		[TestCase("0,", 4, 2, TestName = "Number ends with comma")]
		[TestCase("++0", 4, 2, TestName = "When number starts with double sign")]
		[TestCase("0+", 4, 2, TestName = "When number ends with sign")]
		[TestCase(null, 4, 2, TestName = "When number is null")]
		[TestCase("", 4, 2, TestName = "When number is empty string")]
		[TestCase("Danil", 4, 2, TestName = "When number is word without dot")]
		[TestCase("Da.nil", 4, 2, TestName = "When number is word with dot")]
		[TestCase("0!0", 4, 2, TestName = "When number has different separator")]
		[TestCase("1..001", 4, 2, TestName = "When number contains more than 1 dots next to each other")]
		[TestCase("1.00.1", 4, 2, TestName = "When number contains more than 1 dot in different locations")]
		[TestCase("1,,001", 4, 2, TestName = "When number contains more than 1 commas next to each other")]
		[TestCase("1,00,1", 4, 2, TestName = "When number contains more than 1 comma in different locations")]
		[TestCase("@1.1", 4, 2, TestName = "When number starts with not digit or sign")]
		[TestCase("1.@1", 4, 2, TestName = "When number contains symbol not digit, dot or comma")]
		[TestCase("1.g1", 4, 2, TestName = "When number contains letter")]
		[TestCase("1. 1", 4, 2, TestName = "When number contains space")]
		[TestCase("1.⁣1", 4, 2, TestName = "When valid number contains empty symbol")]
		[TestCase("1.\n⁣1", 4, 2, TestName = "When valid number contains control char \\n in middle")]
		[TestCase("1.1\n⁣", 4, 2, TestName = "When valid number contains control char \\n at the end")]
		public void IsValidNumber_Invalidates(string number, int precision, int scale = 0, bool onlyPositive = false)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number).Should().BeFalse(null, number);
		}
		
		[TestCase(0, 0, "precision must be a positive number", TestName = "When precision is zero")]
		[TestCase(-5, 0, "precision must be a positive number", TestName = "When precision is negative number")]
		[TestCase(1, 2, "precision must be a non-negative number less or equal than precision", TestName = "When scale greater than precision")]
		[TestCase(5, 5, "precision must be a non-negative number less or equal than precision", TestName = "When scale equal to precision")]
		[TestCase(5, -4, "precision must be a non-negative number less or equal than precision", TestName = "When scale is negative number")]
		public void NumberValidatorConstructor_ThrowException(int precision, int scale, string message)
		{
			Action action = () => new NumberValidator(precision, scale);
			action.Should().Throw<ArgumentException>().WithMessage(message);
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