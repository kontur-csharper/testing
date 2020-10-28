using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	[SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
	public class NumberValidatorTests
	{
		[TestCase(-1, TestName = "Negative precision")]
		[TestCase(0, TestName = "Zero precision")]
		public void Ctor_WrongPrecision_ShouldThrow(int precision)
		{
			Action test = () => new NumberValidator(precision);
			test.Should()
				.Throw<ArgumentException>();
		}

		[TestCase(1, -1, TestName = "Negative scale")]
		[TestCase(1, 2, TestName = "Scale greater than precision")]
		[TestCase(1, 1, TestName = "Scale equal to precision")]
		public void Ctor_WrongScale_ShouldThrow(int precision, int scale)
		{
			Action test = () => new NumberValidator(precision, scale);
			test.Should()
				.Throw<ArgumentException>();
		}

		[TestCase(2, 1, TestName = "Scale is positive")]
		[TestCase(1, 0, TestName = "Scale is zero")]
		public void Ctor_CorrectScale_ShouldNotThrow(int precision, int scale)
		{
			Action test = () => new NumberValidator(precision, scale);
			test.Should().NotThrow();
		}

		[TestCase(null, TestName = "Value is null")]
		[TestCase("", TestName = "Value is empty")]
		[TestCase(".9", TestName = "Missing integer part")]
		[TestCase("0.", TestName = "Missing fracture part")]
		[TestCase(".", TestName = "Missing everything")]
		[TestCase("+-0.9", TestName = "Two different signs")]
		[TestCase("--0.9", TestName = "Two minus signs")]
		[TestCase("++0.9", TestName = "Two plus signs")]
		[TestCase(" 0.9", TestName = "Not at start of a line")]
		[TestCase("0.9 ", TestName = "Not at The End Of The Line")]
		[TestCase("0.a01", TestName = "Contains non-digits")]
		public void IsValidNumber_InvalidNumberFormat_ReturnsFalse(string value)
		{
			var validator = new NumberValidator(7, 4);
			validator.IsValidNumber(value)
				.Should()
				.BeFalse();
		}

		[TestCaseSource(nameof(negativeNumbersCases))]
		public void IsValidNumber_NegativeExcluded_ReturnsFalseOnNegative(int precision, int scale, string value)
		{
			var validator = new NumberValidator(precision, scale, true);
			validator.IsValidNumber(value)
				.Should()
				.BeFalse();
		}

		[TestCaseSource(nameof(alwaysFalseCases))]
		public void IsValidNumber_NotValidNumber_ReturnsFalse(int precision, int scale, string value)
		{
			var validator = new NumberValidator(precision, scale, false);
			validator.IsValidNumber(value)
				.Should()
				.BeFalse();
		}

		[TestCaseSource(nameof(alwaysTrueCases))]
		public void IsValidNumber_CorrectNumber_ReturnsTrue(int precision, int scale, string value)
		{
			var validator = new NumberValidator(precision, scale, true);
			validator.IsValidNumber(value)
				.Should()
				.BeTrue();
		}

		[TestCaseSource(nameof(negativeNumbersCases))]
		public void IsValidNumber_WithNegative_ReturnsTrueOnNegative(int precision, int scale, string value)
		{
			var validator = new NumberValidator(precision, scale, false);
			validator.IsValidNumber(value)
				.Should()
				.BeTrue();
		}

		private static IEnumerable<TestCaseData> negativeNumbersCases = new[]
		{
			new TestCaseData(5, 4, "-0.01") {TestName = "Negative fracture number"},
			new TestCaseData(5, 4, "-10") {TestName = "Negative integer number"},
		};

		private static IEnumerable<TestCaseData> alwaysTrueCases = new[]
		{
			new TestCaseData(5, 3, "12.34") {TestName = "Number shorter than precision"},
			new TestCaseData(5, 3, "123.45") {TestName = "Number have same size with precision"},
			new TestCaseData(5, 3, "1234") {TestName = "Int number shorter than precision"},
			new TestCaseData(5, 3, "12345") {TestName = "Int number is same size with precision"},
			new TestCaseData(5, 3, "+1234") {TestName = "Int number is same size with precision (Sign affect length)"},
			new TestCaseData(10, 4, "+0.1234") {TestName = "Fracture part is same size with scale (Sign not affect)"},
			new TestCaseData(10, 4, "0.1234") {TestName = "Fracture part have same size with scale"},
			new TestCaseData(10, 4, "0.123") {TestName = "Fracture part shorter than scale"},
		};

		private static IEnumerable<TestCaseData> alwaysFalseCases = new[]
		{
			new TestCaseData(5, 4, "123456") {TestName = "Int number longer than precision"},
			new TestCaseData(5, 4, "123.456") {TestName = "Number longer than precision"},
			new TestCaseData(5, 4, "-123.45") {TestName = "Number longer than precision (Sign affect length)"},
			new TestCaseData(10, 4, "0.12345") {TestName = "Fracture longer than scale"}
		};
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
				throw new ArgumentException("scale must be a non-negative number less than precision");
			numberRegex = new Regex(@"^([+-]?)(\d+)([.,](\d+))?$", RegexOptions.IgnoreCase);
		}

		public bool IsValidNumber(string value)
		{
			// Проверяем соответствие входного значения формату N(m,k), в соответствии с правилом, 
			// описанным в Формате описи документов, направляемых в налоговый орган в электронном виде по телекоммуникационным каналам связи:
			// Формат числового значения указывается в виде N(m.к), где m – максимальное количество знаков в числе, включая знак (для положительного и отрицательного числа), 
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