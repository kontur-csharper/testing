using System;
using NUnit.Framework;

namespace HomeExercises.NumberValidator_Task
{
	public class NumberValidatorTests
	{
		[TestCase(-1, 0, TestName = "When precision < 0")]
		[TestCase(0, TestName = "When precision == 0")]
		[TestCase(2, -1, TestName = "When scale < 0")]
		[TestCase(2, 2, TestName = "When scale == precision")]
		[TestCase(2, 3, TestName = "When scale > precision")]
		public void СreateValidator_ThrowArgumentException_FlagOnlyPositiveDoesNotMatter(int precision, int scale = 0,
			bool onlyPositive = false)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, onlyPositive),
				$"The exception was not thrown");
		}

		[TestCase(2, 0, TestName = "When precision > 0 and scale == 0")]
		[TestCase(3, 2, TestName = "When precision > 0, scale > 0 and scale < precision")]
		public void СreateValidator_NotException_FlagOnlyPositiveDoesNotMatter(int precision, int scale = 0,
			bool onlyPositive = false)
		{
			Assert.DoesNotThrow(() => new NumberValidator(precision, scale, onlyPositive),
				$"The exception was thrown");
		}

		[TestCase("-1.0", 5, 2,
			TestName = "Normal negative value, between int and fracPart '.'")]
		[TestCase("+1.0", 5, 2,
			TestName = "Normal positive value, between int and fracPart '.'")]
		[TestCase("-1,0", 5, 2,
			TestName = "Normal negative value, between int and fracPart ','")]
		[TestCase("+55555.00", 17, 2, true,
			TestName = "Normal positive value and onlyPositives are allowed")]
		[TestCase("55555.00", 17, 2, true,
			TestName = "Absolute value and onlyPositive")]
		[TestCase("55555.00", 17, 2,
			TestName = "Absolute value")]
		[TestCase("-55555.00", 17, 2,
			TestName = "Normal negative value")]
		[TestCase("+0.12", 17, 2,
			TestName = "Simple positive value when int+fracPart <= precision")]
		[TestCase("+55.010", 17, 3, true,
			TestName = "Simple positive value when int+fracPart < precision and only positives are allowed")]
		public void Check_ValidNumber(string value, int precision, int scale = 0, bool onlyPositive = false)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);

			var actualResult = validator.IsValidNumber(value);

			Assert.IsTrue(actualResult,
				"Incorrect value");
		}

		[TestCase("", 17, 2,
			TestName = "Empty value")]
		[TestCase(null, 2, 1,
			TestName = "Value is null")]
		[TestCase("aaa", 5, 1,
			TestName = "Not a number")]
		[TestCase("\n\t\r", 5, 1,
			TestName = "Special symbols")]
		[TestCase("+aa", 5, 2,
			TestName = "Value contains not number")]
		[TestCase("-aa", 5, 2,
			TestName = "Value contains not number")]
		[TestCase("+1.0aa", 5, 2,
			TestName = "Value contains letters at the end")]
		[TestCase("ff+1.0", 5, 2,
			TestName = "Value contains letters at the beginning")]
		[TestCase("., -1.0.", 5, 2,
			TestName = "Value contains white space or other symbols at the beginning")]
		[TestCase("+-1.0.", 5, 2,
			TestName = "Incorrect format of value")]
		[TestCase("--1.0.", 5, 2,
			TestName = "Incorrect format of negative value")]
		[TestCase("++1.0.", 5, 2,
			TestName = "Incorrect format of positive value")]
		[TestCase("+12.00", 3, 2,
			TestName = "Incorrect positive value when int+fracPart > precision")]
		[TestCase("-14.00", 4, 1,
			TestName = "Incorrect negative value when fracPart > scale")]
		[TestCase("-55555.00", 17, 2, true,
			TestName = "Incorrect negative value only positives are allowed.")]
		public void Check_InvalidNumber(string value, int precision, int scale = 0, bool onlyPositive = false)
		{
			var validator = new NumberValidator(precision, scale, onlyPositive);

			var actualResult = validator.IsValidNumber(value);

			Assert.IsFalse(actualResult,
				"Value is valid, but expected: invalid value");
		}
	}
}