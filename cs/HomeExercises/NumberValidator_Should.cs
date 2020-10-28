using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidator_Should
	{
		[TestCase(-1)]
		[TestCase(0)]
		public void Throw_OnCreation_WhenPrecisionIsNegativeOrZero(int precision)
		{
			FluentActions.Invoking(() => new NumberValidator(precision))
				.Should().Throw<ArgumentException>();
		}

		[TestCase(3, 5)]
		[TestCase(3, 3)]
		public void Throw_OnCreation_WhenScaleGraterOrEqualsPrecision(int precision, int scale)
		{
			FluentActions.Invoking(() => new NumberValidator(precision, scale))
				.Should().Throw<ArgumentException>();
		}

		[Test]
		public void Throw_OnCreation_WhenScaleIsNegative()
		{
			FluentActions.Invoking(() => new NumberValidator(1, -2))
				.Should().Throw<ArgumentException>();
		}

		[TestCase(3, 2)]
		[TestCase(3, 1)]
		[TestCase(1, 0)]
		public void NotThrow_OnCreation_WithCorrectData(int precision, int scale)
		{
			FluentActions.Invoking(() => new NumberValidator(precision, scale))
				.Should().NotThrow<ArgumentException>();
		}


		[TestCase("")]
		[TestCase(null)]
		public void ReturnFalse_IfNumberIsNullOrEmpty(string number)
		{
			new NumberValidator(1, 0).IsValidNumber(number).Should()
				.BeFalse();
		}

		[TestCase(" ")]
		[TestCase("asdk")]
		[TestCase("w.dk")]
		[TestCase("++-")]
		[TestCase("++1")]
		[TestCase("0.0.0")]
		public void ReturnFalse_IfNumberDoesNotMatchFormat(string number)
		{
			new NumberValidator(1, 0).IsValidNumber(number).Should()
				.BeFalse();
		}

		[TestCase(1, 0, "12")]
		[TestCase(1, 0, "-1")]
		[TestCase(1, 0, "+1")]
		[TestCase(2, 1, "-0.0")]
		[TestCase(3, 2, "123.0")]
		public void ReturnFalse_IfNumberScaleGreaterThanPrecision(int precision, int scale,
			string number)
		{
			new NumberValidator(precision).IsValidNumber(number).Should()
				.BeFalse();
		}

		[TestCase(3, 1, "0.00")]
		[TestCase(4, 2, "0.000")]
		[TestCase(5, 2, "-0.000")]
		public void ReturnFalse_IfFractionalPartScaleGraterThenScale(int precision, int scale,
			string number)
		{
			new NumberValidator(precision, scale).IsValidNumber(number).Should()
				.BeFalse();
		}

		[TestCase(2, 0, "-1")]
		[TestCase(3, 1, "-1.5")]
		[TestCase(4, 2, "-1.55")]
		[TestCase(5, 2, "-10.55")]
		public void ReturnFalse_IfOnlyPositive_ButGivenNegativeNumber(int precision, int scale,
			string number)
		{
			new NumberValidator(precision, scale, true).IsValidNumber(number).Should()
				.BeFalse();
		}

		[TestCase(3, 1, "-12")]
		[TestCase(3, 1, "-1.2")]
		public void ReturnTrue_IfDefaultOnlyPositive_AndGivenNegativeNumber(int precision,
			int scale, string number)
		{
			new NumberValidator(precision, scale).IsValidNumber(number).Should()
				.BeTrue();
		}

		[TestCase(3, 1, "-1.0")]
		[TestCase(3, 1, "+1,0")]
		public void SupportCommasAndDotsWithinFormat(int precision, int scale, string number)
		{
			new NumberValidator(precision, scale).IsValidNumber(number).Should()
				.BeTrue();
		}

		[TestCase(17, 2, true, "0")]
		[TestCase(17, 2, true, "0.00")]
		[TestCase(17, 2, true, "+0.00")]
		[TestCase(17, 2, false, "-0.00")]
		[TestCase(17, 3, false, "-0.005")]
		public void ReturnTrue_WithCorrectData(int precision, int scale, bool onlyPositive,
			string number)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number).Should()
				.BeTrue();
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
				throw new ArgumentException("precision must be a positive number greater then zero");
			if (scale < 0 || scale >= precision)
				throw new ArgumentException("scale must be a non-negative number less than precision");
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