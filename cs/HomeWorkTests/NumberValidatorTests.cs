using System;
using HomeExercises;
using NUnit.Framework;

namespace HomeWorkTests
{
	public class NumberValidatorTests
	{
		[TestCase(-1, TestName = "When_Precision_Is_Negative")]
		[TestCase(0, TestName = "When_Precision_Is_Zero")]
		[TestCase(1, 2, TestName = "When_Scale_Is_Bigger_Than_Precision")]
		public void NumberValidatorConstructor_ThrowsArgumentException(int precision, int scale = 0)
		{
			Assert.Throws<ArgumentException>(() => NumberValidator.AreFieldsCorrect(precision, scale));
		}

		[TestCase("0", 1, ExpectedResult = true, TestName = "Return_True_When_Number_Match_Precision")]
		[TestCase("-1.23", 3, 2, ExpectedResult = false, TestName = "Returns_False_When_Number_Is_Negative_And_Float")]
		[TestCase("-1", 1, ExpectedResult = false, TestName = "Returns_False_When_Number_Is_Negative_And_Int")]
		[TestCase("-10.68", 5, 2, ExpectedResult = true, TestName = "Return_True_When_Number_Is_Negative_And_Float")]
		[TestCase("+1.23", 3, 2, ExpectedResult = true, TestName = "Return_True_If_Do_Not_Consider_Plus_And_Number_Is_Float")]
		[TestCase("+1", 1, ExpectedResult = true, TestName = "Return_True_If_Do_Not_Consider_Plus_And_Number_Is_Int")]
		[TestCase("-1.23", 4, 2, true, ExpectedResult = false,
			TestName = "Return_True_When_Number_Is_Negative_Float_And_OnlyPositive_Is_True")]
		[TestCase("-1", 2, 0, true, ExpectedResult = false,
			TestName = "Return_True_When_Number_Is_Negative_Int_And_OnlyPositive_Is_True")]
		[TestCase("a.sd", 3, ExpectedResult = false, TestName = "Returns_False_When_Letters_Are_In_Int_Part_And_Frac_Part")]
		[TestCase("", 1, ExpectedResult = false, TestName = "Returns_False_When_Input_Is_Empty")]
		[TestCase(null, 1, ExpectedResult = false, TestName = "Returns_False_When_Input_Is_Null")]
		[TestCase("4a", 3, ExpectedResult = false, TestName = "Returns_False_When_Number_And_Letter_Are_In_Int_Part")]
		[TestCase("4.a", 3, ExpectedResult = false, TestName = "Returns_False_When_Number_In_Int_Part_And_Letter_In_Frac_Part")]
		[TestCase(".5", 2, ExpectedResult = false, TestName = "Returns_False_When_Number_Without_Int_Part")]
		[TestCase("      ", 10, ExpectedResult = false, TestName = "Returns_False_When_Spaces_In_Number")]
		[TestCase("923456787654323456765434567654345676543234567654334", int.MaxValue, ExpectedResult = true,
			TestName = "Returns_True_When_Number_Is_Big")]
		public bool IsValidNumber(string number, int precision,
			int scale = 0, bool onlyPositive = false)
		{
			return NumberValidator.Create(precision, scale, onlyPositive).IsValidNumber(number);
		}
	}
}