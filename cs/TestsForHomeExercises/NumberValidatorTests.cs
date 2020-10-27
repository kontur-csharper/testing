using System;
using FluentAssertions;
using HomeExercises;
using NUnit.Framework;

namespace TestsForHomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		public void Ctor_ThrowArgumentException_WhenPrecisionNotPositive()
		{
			var exception = Assert.Throws<ArgumentException>(() => new NumberValidator(0, 2, true));
			exception.Message.Should().Be("precision must be a positive number");
		}

		[TestCase(1, 2, true, TestName = "Ctor_ThrowArgumentException_ScaleMoreThenPrecision")]
		[TestCase(1, -1, true, TestName = "Ctor_ThrowArgumentException_ScaleNegative")]
		public void CtorTest(int precision, int scale, bool onlyPositive)
		{
			Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, onlyPositive));
		}

		[Test]
		public void Ctor_NotThrowException_WhenDataIsCorrect()
		{
			Assert.DoesNotThrow(() => new NumberValidator(5, 3, true));
		}

		[TestCase("0.0", TestName = "IsValidNumber_True_PositiveFloatNumberWithDot")]
		[TestCase("0,0", TestName = "IsValidNumber_True_PositiveFloatNumberWithComma")]
		[TestCase("+0.0", TestName = "IsValidNumber_True_PositiveFloatNumberWhithPlus")]
		[TestCase("0", TestName = "IsValidNumber_True_PositiveIntNumber")]
		[TestCase("+0", TestName = "IsValidNumber_True_PositiveIntNumberWhithPlus")]
		public void IsValidNumber_True_Positive(string numberToValidate)
		{
			new NumberValidator(17, 2, true).IsValidNumber(numberToValidate).Should().BeTrue();
		}


		[TestCase("-0.0", TestName = "IsValidNumber_True_NegativeFloatNumber")]
		[TestCase("-0", TestName = "IsValidNumber_True_NegativeIntNumber")]
		public void IsValidNumber_True_Negative(string numberToValidate)
		{
			new NumberValidator(17, 2).IsValidNumber(numberToValidate).Should().BeTrue();
		}

		[TestCase("0.0.0", TestName = "IsValidNumber_False_WhenMultipleDots")]
		[TestCase("abc", TestName = "IsValidNumber_False_WhenText")]
		[TestCase(null, TestName = "IsValidNumber_False_Null")]
		[TestCase("", TestName = "IsValidNumber_False_EmptyString")]
		[TestCase("0.", TestName = "IsValidNumber_False_PositiveFloatNumberWhithoutFracPart")]
		public void IsValidNumber_False_NotNumber(string objectToValidate)
		{
			new NumberValidator(17, 2).IsValidNumber(objectToValidate).Should().BeFalse();
		}

		[TestCase("-1.0", TestName = "IsValidNumber_False_NegativeFloat_WhenOnlyPositive")]
		[TestCase("-1", TestName = "IsValidNumber_False_NegativeInt_WhenOnlyPositive")]
		public void IsValidNumber_False_Negative_WhenOnlyPositive(string numberToValidate)
		{
			new NumberValidator(17, 5, true).IsValidNumber(numberToValidate).Should().BeFalse();
		}

		[TestCase(3, 2, true, "+0.00", TestName = "IsValidNumber_False_WhenLengthMoreThenPrecision")]
		[TestCase(17, 2, true, "0.000", TestName = "IsValidNumber_False_WhenFracPartMoreThenScale")]
		public void IsValidNumber_False_BecauseOfLength(int precision, int scale, bool onlyPositive,
			string numberToValidate)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(numberToValidate).Should().BeFalse();
		}
	}
}