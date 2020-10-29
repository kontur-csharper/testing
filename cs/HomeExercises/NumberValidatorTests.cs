using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase("0.0", 3, 2, true, TestName = "When zero value and dot")]
		[TestCase("12.3", 4, 3, true, TestName = "When positive value and dot")]
		[TestCase("0,0", 3, 2, true, TestName = "When zero value and comma")]
		[TestCase("12,3", 4, 3, true, TestName = "When positive value and comma")]
		[TestCase("0", 3, 2, true, TestName = "When zero without fraction")]
		[TestCase("12", 3, 2, true, TestName = "When without fraction")]
		[TestCase("000", 3, 2, true, TestName = "When Int length equals precision")]
		[TestCase("12.3", 3, 2, true, TestName = "When Int and fraction lengths equals precision")]
		[TestCase("0.12", 3, 2, true, TestName = "When fraction length equals Scale")]
		public void IsValidNumber_BeTrue_WhenValidValue(string number, int precision, int scale, bool onlyPositive)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);

			numberValidator.IsValidNumber(number).Should().BeTrue();
		}

		[TestCase("+99", 3, 2, true, TestName = "When Int with positive sign length equals precision")]
		[TestCase("-100", 4, 2, false, TestName = "When Int with negative sign length equals precision")]
		[TestCase("+12.3", 4, 3, true, TestName = "When positive value with sign and dot")]
		[TestCase("-12,3", 5, 4, false, TestName = "When negative value and comma")]
		[TestCase("+12,3", 4, 3, true, TestName = "When positive value with sign and comma")]
		[TestCase("-1.2", 3, 2, false, TestName = "When Int and fraction lengths with negative sign equals precision")]
		[TestCase("+1.2", 3, 2, true, TestName = "When Int and fraction lengths with positive sign equals precision")]
		[TestCase("-12.3", 5, 4, false, TestName = "When negative value and dot")]
		public void IsValidNumber_BeTrue_WhenValidValueWithSign(string number, int precision, int scale, bool onlyPositive)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);

			numberValidator.IsValidNumber(number).Should().BeTrue();
		}
		
		[TestCase("-2147483648.9", 21, 20, false, TestName = "When value more than Integer with negative sign")]
		[TestCase("-783439742833543434339.35354395084358", 51, 50, false, TestName = "When value much more than Integer with negative sign")]
		[TestCase("+2147483648.9", 21, 20, true, TestName = "When value more than Integer with positive sign")]
		[TestCase("+7777777777777777777777777.6666666666", 51, 50, true, TestName = "When value much more than Integer with positive sign")]
		[TestCase("2147483648.9", 21, 20, true, TestName = "When value more than Integer")]
		[TestCase("9999999999999999999999999999.9", 51, 50, true, TestName = "When value much more than Integer")]

		public void IsValidNumber_BeTrue_WhenValidWithLargeValue(string number, int precision, int scale, bool onlyPositive)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);

			numberValidator.IsValidNumber(number).Should().BeTrue();
		}

		[TestCase("+ . ", 4, 3, true, TestName = "When space instead numbers")]
		[TestCase(" 2", 3, 2, true, TestName = "When value start with space")]
		[TestCase("-", 3, 2, false, TestName = "When only negative sign symbol")]
		[TestCase("+", 3, 2, true, TestName = "When only positive sign symbol")]
		[TestCase("a.2", 3, 2, true, TestName = "When Int part not correct")]
		[TestCase("a", 3, 2, true, TestName = "When Int part not correct without fraction")]
		[TestCase("0.b", 3, 2, true, TestName = "When fraction part not correct")]
		[TestCase("a.b", 3, 2, true, TestName = "When Int and fraction not correct")]
		[TestCase("=1.2", 4, 3, true, TestName = "When sign not correct")]
		[TestCase("12-", 3, 2, true, TestName = "When negative sign in wrong space")]
		[TestCase(".2", 3, 2, true, TestName = "When without Int part")]
		public void IsValidNumber_BeFalse_WhenInvalidValueSymbols(string number, int precision, int scale, bool onlyPositive)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);

			numberValidator.IsValidNumber(number).Should().BeFalse();
		}
		
		[TestCase("2 3", 10, 9, true, TestName = "When space instead separation")]
		[TestCase("1@2", 10, 9, true, TestName = "When separation sign not correct")]
		[TestCase("1.,2", 10, 9, true, TestName = "When both separation sign in a row")]
		[TestCase("1..2", 10, 9, true, TestName = "When two dots in a row")]
		[TestCase("1.2.3", 10, 9, true, TestName = "When many dots")]
		[TestCase("1,2,3", 10, 9, true, TestName = "When many commas")]
		[TestCase("1.2,34", 10, 9, true, TestName = "When many separation sign")]
		[TestCase("1 2", 10, 9, true, TestName = "When without separation sign")]
		public void IsValidNumber_BeFalse_WhenInvalidValueSeparates(string number, int precision, int scale, bool onlyPositive)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);

			numberValidator.IsValidNumber(number).Should().BeFalse();
		}
		
		[TestCase("\r\n", 5, 4, true, TestName = "When value is new line character")]
		[TestCase(" ", 3, 2, true, TestName = "When value is space character")]
		[TestCase(null, 6, 5, true, TestName = "When value is null")]
		[TestCase("-1,2", 4, 3, true, TestName = "When OnlyPositive true and negative sign")]
		[TestCase("", 3, 2, true, TestName = "When empty value")]
		[TestCase("0000", 3, 2, true, TestName = "When Int length more than precision")]
		[TestCase("12.34", 3, 2, true, TestName = "When Int and fraction lengths more than precision")]
		[TestCase("-1.23", 3, 2, false, TestName = "When Int and fraction lengths with negative sign more precision")]
		[TestCase("1234567890", 3, 2, true, TestName = "When Int length much more than precision")]
		[TestCase("111111111111.222", 5, 4, true, TestName = "When Int and fraction more lengths than precision")]
		[TestCase("0.123", 3, 2, true, TestName = "When fraction length more than Scale")]
		[TestCase("0.1234567890000000000000000000", 3, 2, true, TestName = "When fraction length much more than Scale")]
		public void IsValidNumber_BeFalse_WhenInvalidValue(string number, int precision, int scale, bool onlyPositive)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);

			numberValidator.IsValidNumber(number).Should().BeFalse();
		}

		[TestCase(0, 0, true, TestName = "When precision is zero")]
		[TestCase(-1, -2, true, TestName = "When precision is negative")]
		[TestCase(2, -2, true, TestName = "When scale is negative")]
		[TestCase(2, 2, true, TestName = "When scale equals precision")]
		[TestCase(2, 10, true, TestName = "When scale more precision")]
		public void NumberValidatorConstructor_ThrowsArgumentException_WhenInvalidValue(int precision, int scale, bool onlyPositive)
		{
			Action act = () => new NumberValidator(precision, scale, onlyPositive);

			act.Should().Throw<ArgumentException>();
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
			if (string.IsNullOrEmpty(value))
				return false;

			var match = numberRegex.Match(value);
			if (!match.Success)
				return false;
			
			var intPart = match.Groups[1].Value.Length + match.Groups[2].Value.Length;
			
			var fracPart = match.Groups[4].Value.Length;

			if (intPart + fracPart > precision || fracPart > scale)
				return false;

			if (onlyPositive && match.Groups[1].Value == "-")
				return false;
			return true;
		}
	}
}