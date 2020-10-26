using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-1, 2, true, TestName = "Negative precision")]
		[TestCase(0, 2, false, TestName = "Zero precision")]
		[TestCase(2, -1, false, TestName = "Negative scale")]
		public void ThrowException_When(int precision, int scale, bool onlyPositive)
		{
			Func<NumberValidator> validator = () => new NumberValidator(precision, scale, onlyPositive);

			validator.Should().Throw<ArgumentException>();
		}

		[TestCase(3, 0, true, TestName = "Zero scale")]
		[TestCase(10, 5, false, TestName = "Natural scale")]
		[TestCase(1, 0, true, TestName = "Positive precision")]
		public void NotThrowException_When(int precision, int scale, bool onlyPositive)
		{
			Func<NumberValidator> validator = () => new NumberValidator(precision, scale, onlyPositive);

			validator.Should().NotThrow<ArgumentException>();
		}

		[Timeout(100)]
		[TestCase("+1337", 4, 2, false, false, TestName = "Integer is bigger than precision")]
		[TestCase("-12,34", 4, 2, false, false,TestName = "Double is bigger than precision")]
		[TestCase("123+", 4, 2, false, false, TestName = "Sign after number")]
		[TestCase("72:00", 4, 2, false, false, TestName = "Wrong separator")]
		[TestCase("++2", 4, 2, false, false, TestName = "More then one sign")]
		[TestCase("-21.00A", 4, 2, false, false, TestName = "Letters after number")]
		[TestCase("M123", 4, 2, true, false, TestName = "Letters before number")]
		[TestCase("    2", 4, 2, false, false, TestName = "Empty space before number")]
		[TestCase("3    ", 4, 2, false, false, TestName = "Empty space after number")]
		[TestCase("   9.45   ", 4, 2, false, false, TestName = "Number between empty space")]
		[TestCase("- 2", 4, 2, false, false, TestName = "Empty space after sign")]
		[TestCase("-", 4, 2, false, false, TestName = "Only minus sign")]
		[TestCase("+", 4, 2, false, false, TestName = "Only plus sign")]
		[TestCase("-5,003", 4, 2, false, false, TestName = "Fractional part is bigger than scale")]
		[TestCase("-1.0", 4, 2, true, false, TestName = "Negative double when only positive")]
		[TestCase("-100", 4, 2, true, false, TestName = "Negative integer when only positive")]
		[TestCase("abc", 4, 2, false, false, TestName = "Only letters")]
		[TestCase(null, 4, 2, false, false, TestName = "String is null")]
		[TestCase(" ", 4, 2, false, false, TestName = "String is empty")]
		[TestCase("+3,", 4, 2, false, false, TestName = "No digits after separator")]
		[TestCase("1.2,3", 6, 3, false, false, TestName = "More than one separator")]
		[TestCase(".", 4, 3, false, false, TestName = "Only separator")]
		[TestCase("6..9", 4, 3, false, false, TestName = "Two separators in a row")]
		[TestCase("13,10", 4, 2, false, true, TestName = "Scale equals fractional part")]
		[TestCase("-5,1", 20, 10, false, true, TestName = "Scale is bigger than fractional part")]
		[TestCase("+12", 4, 2, false, true, TestName = "Positive integer")]
		[TestCase("20,0", 4, 2, false, true, TestName = "Positive double")]
		[TestCase("-454", 4, 2, false, true, TestName = "Negative integer")]
		[TestCase("-20,0", 4, 2, false, true, TestName = "Negative double")]
		[TestCase("2,3", 4, 2, false, true, TestName = "Decimal comma")]
		[TestCase("3.2", 4, 2, false, true, TestName = "Decimal point")]
		[TestCase("1337", 4, 2, false, true, TestName = "Number equals precision length")]
		[TestCase("4,990", 50, 6, false, true, TestName = "Number length is smaller then precision")]
		[TestCase("1941.4", 1000000, 2, false, true, TestName = "Big precision")]
		[TestCase("-191939.291945", 1000000, 10000, false, true, TestName = "Big scale")]
		[TestCase("0", 4, 2, false, true, TestName = "Zero integer")]
		[TestCase("0.0", 4, 2, false, true, TestName = "Zero double")]
		[TestCase("-0", 4, 2, false, true, TestName = "Signed zero")]
		[TestCase("001.2", 4, 2, false, true, TestName = "Integer part starts with zero")]
		[TestCase("2.100", 6, 4, false, true, TestName = "Fractional part ends with zero")]
		public void IsValidNumber_ReturnResult_When(
			string number, int precision, int scale, bool onlyPositive, bool testResult)
		{
			var numberValidator = new NumberValidator(precision, scale, onlyPositive);

			numberValidator.IsValidNumber(number).Should().Be(testResult);
		}
	}
}