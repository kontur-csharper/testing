using NUnit.Framework;
using System;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		public class ConstuctorTest
		{
			[TestCase(1, 0)]
			[TestCase(3, 1)]
			[TestCase(15, 14)]
			[TestCase(15, 7)]
			[TestCase(4, 2)]
			public void Test_ScaleMustBeBetweenZeroAndPrecision(int precision, int scale)
			{
				Assert.DoesNotThrow(() => new NumberValidator(precision, scale));
			}

			[TestCase(-1, 3)]
			[TestCase(9, -5)]
			[TestCase(-2, -1)]
			public void Test_ScaleAndPrecisionMustBePositive(int precision, int scale)
            {
				Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale));
            }

			[TestCase(4, 6)]
			[TestCase(3, 3)]
			public void Test_ScaleMustBeLessThanPrecision(int precision, int scale)
            {
				Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale));
            }
		}

		public class CorrectNumberFormTest
		{
			private readonly NumberValidator standartValidator = new NumberValidator(10, 9);

			[TestCase("a.bc")]
			[TestCase("12a.bc")]
			[TestCase("12.c")]
			[TestCase("43.b43c")]
			[TestCase("a")]
			[TestCase("a.104")]
			[TestCase("-2#")]
			public void Test_InputNumberContainsSignAndSeparator_IsValid(string str)
			{
				Assert.IsFalse(standartValidator.IsValidNumber(str));
			}

			[Test]
			public void Test_NullString_IsNotValid()
			{
				Assert.IsFalse(standartValidator.IsValidNumber(null));	
			}

			[Test]
			public void Test_EmptyString_IsNotValid()
			{
				Assert.IsFalse(standartValidator.IsValidNumber(string.Empty));	
			}
		
			[TestCase("1.0", ExpectedResult = true)]
			[TestCase("1,0", ExpectedResult = true)]
			[TestCase("-7.15", ExpectedResult = true)]
			[TestCase("-0,15", ExpectedResult = true)]
			[TestCase("1:0", ExpectedResult = false)]
			[TestCase("9-3", ExpectedResult = false)]
			public bool Test_DotOrCommaSeparator_IsValid(string str)
			{
				return standartValidator.IsValidNumber(str);
			}

			[TestCase("1.0")]
			[TestCase("15,15")]
			[TestCase("6")]
			[TestCase("-8")]
			[TestCase("-3.2")]
			[TestCase("-7,16")]
			public void Test_OneOrNothingSeparator_IsValid(string str)
			{
				Assert.IsTrue(standartValidator.IsValidNumber(str));
			}

			[TestCase("1.0.0")]
			[TestCase("15,,15")]
			[TestCase("6,0.5")]
			[TestCase("-8.5,6")]
			[TestCase("-3.,2")]
			[TestCase("-7,.16")]
			[TestCase("1.1.1.1")]
			public void Test_MoreThanOneSeparator_IsNotValid(string str)
			{
				Assert.IsFalse(standartValidator.IsValidNumber(str));
			}

			[TestCase("15.")]
			[TestCase("15,")]
			[TestCase(".50")]
			[TestCase(",6")]
			[TestCase("+7.")]
			[TestCase("+.15")]
			[TestCase("-,0")]
			[TestCase("+.")]
			[TestCase("-,")]
			[TestCase(".")]
			[TestCase(",")]
			public void Test_SeparatorBetweenNumbers_IsCorrect(string str)
			{
				Assert.IsFalse(standartValidator.IsValidNumber(str));
			}

			[TestCase("+")]
			[TestCase("-")]
			[TestCase("+.")]
			[TestCase("-,")]
			[TestCase(".")]
			[TestCase(",")]
			public void Test_StringWithoutNumber_IsNotValid(string str)
			{
				Assert.IsFalse(standartValidator.IsValidNumber(str));
			}

			[TestCase("+9.5")]
			[TestCase("-8.6")]
			[TestCase("+9,0")]
			[TestCase("-7,3")]
			[TestCase("+913")]
			[TestCase("-54")]
			public void Test_StringStartesWithSign_IsValid(string str)
			{
				Assert.IsTrue(standartValidator.IsValidNumber(str));
			}

			[TestCase("0.0")]
			[TestCase("1,1")]
			[TestCase("12")]
			public void Test_StringWithoutSign_IsValid(string str)
			{
				Assert.IsTrue(standartValidator.IsValidNumber(str));
			}

			[TestCase("++0")]
			[TestCase("+-1")]
			[TestCase("--0.0")]
			[TestCase("-+1,1")]
			[TestCase("++++12")]
			[TestCase("-+-+-+-6")]
			public void Test_StringWithMoreThanOneSign_IsNotValid(string str)
			{
				Assert.IsFalse(standartValidator.IsValidNumber(str));
			}

		}

		public class ValidatorScaleAndPrecisionTeste
		{
			[TestCase(13, 1, "1.0")]
			[TestCase(1, 0, "7")]
			[TestCase(2, 1, "3.9")]
			[TestCase(3, 2, "-3.0")]
			[TestCase(2, 0, "-9")]
			public void Test_LengthNumberLessOrEqualThanProcision_IsValid(int procising, int scale, string str)
			{
				Assert.IsTrue(new NumberValidator(procising, scale).IsValidNumber(str));
			}

			[TestCase(2, 1, "4.45")]
			[TestCase(2, 1, "+5.0")]
			[TestCase(1, 0, "+7")]
			public void Test_LengthNumberMoreThanProcision_IsNotValid(int procising, int scale, string str)
			{
				Assert.IsFalse(new NumberValidator(procising, scale).IsValidNumber(str));
			}

			[TestCase(1, "0.0")]
			[TestCase(2, "1.0")]
			[TestCase(1, "0")]
			[TestCase(1, "+5.6")]
			[TestCase(3, "-7")]
			[TestCase(3, "13.666")]
			public void Test_FractionalPartLessOrEqualThanScale_IsValid(int scale, string str)
			{
				Assert.IsTrue(new NumberValidator(10, scale).IsValidNumber(str));
			}

			[TestCase(1, "0.13")]
			[TestCase(2, "0.137")]
			[TestCase(0, "0.0")]
			public void Test_FractionalPartMoreThanScale_IsNotValid(int scale, string str)
			{
				Assert.IsFalse(new NumberValidator(10, scale).IsValidNumber(str));
			}

			[TestCase("0,15")]
			[TestCase("+0,15")]
			[TestCase("+0")]
			[TestCase("+0.0")]
			[TestCase("16")]
			public void Test_PositiveValidatorProcessesPositiveNumbers(string str)
			{
				Assert.IsTrue(new NumberValidator(10, 9, true).IsValidNumber(str));
			}

			[TestCase("-0,15")]
			[TestCase("-0")]
			[TestCase("-0.0")]
			[TestCase("-16")]
			public void Test_PositiveValidatorDoNotProcessesNotPositiveNumbers(string str)
			{
				Assert.IsFalse(new NumberValidator(10, 9, true).IsValidNumber(str));
			}

			[TestCase("0,15")]
			[TestCase("-0,15")]
			[TestCase("0")]
			[TestCase("+0.0")]
			[TestCase("-16")]
			public void Test_NotOnlyPositiveValidatorProcessNumbersWithAnySign(string str)
			{
				Assert.IsTrue(new NumberValidator(10, 9, false).IsValidNumber(str));
			}
		}
	}
}