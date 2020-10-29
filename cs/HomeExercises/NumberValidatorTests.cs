using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		private NumberValidator defaultValidator = new NumberValidator(4, 2);

		[Test]
		public void NumberValidator_ThrowsException_WithNegativeScale()
		{
			Action getNewValidator = () => new NumberValidator(-1, 2);
			getNewValidator.Should().Throw<ArgumentException>("with negative scale");
		}

		[Test]
		public void NumberValidator_ThrowsException_WhenScaleIsLessThanPrecision()
		{
			Action getNewValidator = () => new NumberValidator(1, 2);
			getNewValidator.Should().Throw<ArgumentException>("with precision that is bigger than scale");
		}

		[Test]
		public void NumberValidator_Pass_DefaultNumber()
		{
			ValidatorShouldPass(4, 2, false, "1234");
		}

		[Test]
		public void NumberValidator_Pass_NumbersWithSeparators()
		{
			ValidatorShouldPass(4, 2, false, "12.34");
			ValidatorShouldPass(4, 2, false, "12,34");
		}

		[Test]
		public void NumberValidator_Pass_PositiveNumberWithPlus()
		{
			ValidatorShouldPass(4, 2, false, "+5");
		}

		[Test]
		public void NumberValidator_Pass_NegativeNumbers()
		{
			ValidatorShouldPass(4, 2, false, "-5");
		}

		[Test]
		public void NumberValidator_DoesntPass_LongNumbers()
		{
			ValidatorShouldNotPass(4, 2, false, "12345");
			ValidatorShouldNotPass(4, 2, false, "123.45");
		}

		[Test]
		public void NumberValidator_DoesntPass_NumbersWithBigPrecision()
		{
			ValidatorShouldNotPass(4, 3, false, "123.4567");
		}

		[Test]
		public void NumberValidator_DoesntPass_NotNumber()
		{
			ValidatorShouldNotPass(4, 2, false, "abc.d");
		}

		[Test]
		public void OnlyPositiveNumberValidator_DoesntPass_NegativeNumbers()
		{
			ValidatorShouldNotPass(4, 2, true, "-5");
		}

		private void ValidatorShouldPass(int scale, int precision, bool onlyPositive, string numberToCheck)
		{
			new NumberValidator(scale, precision, onlyPositive)
				.IsValidNumber(numberToCheck).Should()
				.BeTrue($"because {(onlyPositive ? "only positive" : "")}validator with " +
				        $"scale: {scale}, precision: {precision} should pass number: {numberToCheck}");
		}

		private void ValidatorShouldNotPass(int scale, int precision, bool onlyPositive, string numberToCheck)
		{
			new NumberValidator(scale, precision, onlyPositive)
				.IsValidNumber(numberToCheck).Should()
				.BeFalse($"because {(onlyPositive ? "only positive" : "")} validator with " +
				        $"scale: {scale}, precision: {precision} should NOT pass number: {numberToCheck}");
		}
	}

	public class NumberValidator
	{
		private readonly Regex numberRegex;
		private readonly bool onlyPositive;
		private readonly int scale;
		private readonly int precision;

		public NumberValidator(int scale, int precision = 0, bool onlyPositive = false)
		{
			this.scale = scale;
			this.precision = precision;
			this.onlyPositive = onlyPositive;
			if (scale <= 0)
				throw new ArgumentException("precision must be a positive number");
			if (precision < 0 || precision >= scale)
				throw new ArgumentException("precision must be a non-negative number less or equal than scale");
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

			if (intPart + fracPart > scale || fracPart > precision)
				return false;

			if (onlyPositive && match.Groups[1].Value == "-")
				return false;
			return true;
		}
	}
}