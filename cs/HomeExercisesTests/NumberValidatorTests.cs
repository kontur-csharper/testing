using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using HomeExercises;

namespace HomeExercisesTests
{
	public class NumberValidatorTests
	{
		[TestCase(-1, 1, TestName = "Negative precision")]
		[TestCase(0, 1, TestName = "Zero precision")]
		[TestCase(1, 2, TestName = "Precision below scale")]
		[TestCase(10, 10, TestName = "Scale same as precision")]
		[TestCase(1, 0, true, false, TestName = "Valid does not throw")]
		public void ConstructorTests(
			int precision,
			int scale,
			bool onlyPositive = false,
			bool shouldThrow = true)
		{
			void Construct() => _ = new NumberValidator(precision, scale, onlyPositive);
			if (shouldThrow)
				Assert.Throws<ArgumentException>(Construct);
			else
				Assert.DoesNotThrow(Construct);
		}

		private static IEnumerable<TestCaseData> PrepareTestCases()
		{
			(NumberValidator validator, (string number, bool isValidNumberExpected, string testName)[] testData)[] tests = {
				(ValidatorLittleFraction, new[]
				{
					("0.000", false, "Excess fraction"),
					("0", true, "No fraction"),
					("0.0", true, "Less fraction"),
				}),
				(ValidatorSmallPrecision, new[]
				{
					("00.00", false, "Excess overall length"),
					("+0.00", false, "Excess overall including positive sign"),
					("-0.00", false, "Excess overall including negative sign"),
					("0.00", true, "Exclude sign"),
					("a.sd", false, "Illegal characters"),
				}),
				(ValidatorPositiveOnly, new[]
				{
					("+1.23", true, "Positive number"),
					("-1.23", false, "Negative when only positive"),
				}),
				(ValidatorAllSigns, new[]
				{
					("-1.23", true, "Negative number allowed"),
				}),
				(ValidatorSmallPrecision, new[]
				{
					("", false, "Empty"),
				}),
			};
			return tests.SelectMany(testCases 
				=> testCases.testData.Select(test
					=> new TestCaseData(testCases.validator, test.number, test.isValidNumberExpected) {TestName = test.testName}));
		}

		private static readonly NumberValidator ValidatorSmallPrecision 
			= new NumberValidator(3, 2, true);
		
		private static readonly NumberValidator ValidatorLittleFraction
			= new NumberValidator(17, 2, true);
		
		private static readonly NumberValidator ValidatorPositiveOnly
			= new NumberValidator(4, 2, true);
		
		private static readonly NumberValidator ValidatorAllSigns
			= new NumberValidator(4, 2);


		[TestCaseSource(nameof(PrepareTestCases))]
		public void TestNumberValidation(NumberValidator validator, string input, bool expected)
		{
			var actual = validator.IsValidNumber(input);

			Assert.That(actual, Is.EqualTo(expected), $"Validation of '{input}' was not correct");
		}
	}
}