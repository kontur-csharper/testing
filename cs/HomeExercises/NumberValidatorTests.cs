using System;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		private NumberValidator GetNumberValidator(int precision, int scale, bool onlyPositive)
		{
			return new NumberValidator(precision, scale, onlyPositive);
		}

		[TestCase(-1, 2, true, TestName = "WhenNotPositivePrecision")]
		[TestCase(0, 2, true, TestName = "WhenZeroPrecision")]
		[TestCase(0, -1, true, TestName = "WhenNegativeScale")]
		[TestCase(5, 5, true, TestName = "WhenScaleEqualPrecision")]
		[TestCase(4, 5, true, TestName = "WhenScaleBiggerThenPrecision")]
		public void ThrowException(int precision, int scale, bool onlyPositive)
		{
			Assert.Throws<ArgumentException>(() => GetNumberValidator(precision, scale, onlyPositive));
		}

		[TestCase(4, 2, false, "0,1", ExpectedResult = true, TestName = "ReturnTrue_WhenFractNumberWithСomma")]
		[TestCase(4, 2, false, "0.1", ExpectedResult = true, TestName = "ReturnTrue_WhenFractNumberWithDot")]
		[TestCase(4, 2, true, "+0.0", ExpectedResult = true, TestName = "ReturnTrue_WhenPositiveFractNumber")]
		[TestCase(4, 2, true, "+0", ExpectedResult = true, TestName = "ReturnTrue_WhenPositiveIntNumber")]
		[TestCase(4, 2, false, "-0.0", ExpectedResult = true, TestName = "ReturnTrue_WhenNegativeFractNumber")]
		[TestCase(4, 2, false, "-0", ExpectedResult = true, TestName = "ReturnTrue_WhenNegativeIntNumber")]
		[TestCase(17, 2, false, "99999999999999", ExpectedResult = true, TestName = "ReturnTrue_WhenLargeIntNumber")]
		[TestCase(30, 17, false, "99999999999999,9999999999999", ExpectedResult = true,
			TestName = "ReturnTrue_WhenLargeFractNumber")]
		[TestCase(6, 2, true, "1\n", ExpectedResult = true, TestName = "ReturnTrue_WhenInputEndWithNewLine")]
		[TestCase(3, 2, true, "", ExpectedResult = false, TestName = "ReturnFalse_WhenEmptyInput")]
		[TestCase(3, 2, true, " ", ExpectedResult = false, TestName = "ReturnFalse_WhenEmptySpaceInput")]
		[TestCase(3, 2, true, null, ExpectedResult = false, TestName = "ReturnFalse_WhenNullInput")]
		[TestCase(3, 2, true, "   0.1", ExpectedResult = false,
			TestName = "ReturnFalse_WhenEmptySpaceBeforeNumber")]
		[TestCase(3, 2, true, "0.1   ", ExpectedResult = false,
			TestName = "ReturnFalse_WhenEmptySpaceAfterNumber")]
		[TestCase(3, 2, true, "+ 0.1", ExpectedResult = false,
			TestName = "ReturnFalse_WhenEmptySpaceBetweenSignAndNumber")]
		[TestCase(3, 2, true, "0.1af", ExpectedResult = false,
			TestName = "ReturnFalse_WhenInputContainsCharacters")]
		[TestCase(3, 2, true, "ABCdeeeeeeffFFFff", ExpectedResult = false, TestName = "ReturnFalse_WhenTextInput")]
		[TestCase(3, 1, true, "+0.00", ExpectedResult = false,
			TestName = "ReturnFalse_WhenInputBiggerThenPrecision")]
		[TestCase(6, 2, true, "00.000", ExpectedResult = false,
			TestName = "ReturnFalse_WhenFracPartBiggerThenScale")]
		[TestCase(6, 2, true, "-0.00", ExpectedResult = false,
			TestName = "ReturnFalse_WhenOnlyPositiveButInputIsNegativeNumber")]
		[TestCase(6, 2, true, "++1.23", ExpectedResult = false, TestName = "ReturnFalse_WhenMoreThanOneSign")]
		[TestCase(6, 2, true, "1. 3", ExpectedResult = false,
			TestName = "ReturnFalse_WhenEmptySpaceAfterSeparater")]
		[TestCase(6, 2, true, "1 .3", ExpectedResult = false,
			TestName = "ReturnFalse_WhenEmptySpaceBeforeSeparater")]
		[TestCase(6, 2, true, "\n1", ExpectedResult = false, TestName = "ReturnFalse_WhenInputStartWithNewLine")]
		[TestCase(6, 2, true, "1.\n0", ExpectedResult = false, TestName = "ReturnFalse_WhenNewLineAfterDot")]
		[TestCase(6, 2, true, "1\n.0", ExpectedResult = false, TestName = "ReturnFalse_WhenNewLineBeforeDot")]
		[TestCase(6, 2, true, "1.23.23", ExpectedResult = false, TestName = "ReturnFalse_WhenMoreThanOneDot")]
		[TestCase(6, 2, true, "1.", ExpectedResult = false, TestName = "ReturnFalse_WhenIntNumberWithDot")]
		[TestCase(6, 2, true, "1,", ExpectedResult = false, TestName = "ReturnFalse_WhenIntNumberWithComma")]
		public bool IsValidNumber(int precision, int scale, bool onlyPositive, string input)
		{
			return GetNumberValidator(precision, scale, onlyPositive).IsValidNumber(input);
		}
	}
}