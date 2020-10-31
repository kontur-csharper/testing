using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[TestCase(-1, 2, true)]
		[TestCase(-1, 2, false)]
		[TestCase(3, -2, true)]
		[TestCase(3, -2, false)]
		[TestCase(1, 2, true)]
		[TestCase(1, 2, false)]
		[TestCase(0, 0, true)]
		[TestCase(0, 0, false)]
		public void NumberValidator_ThrowsException(int precision, int scale, bool onlyPositive)
		{
			Action create = () => new NumberValidator(precision, scale, onlyPositive);
			create.Should().Throw<ArgumentException>();
		}

		[TestCase(1, 0, true)]
		[TestCase(1, 0, false)]
		[TestCase(4, 2, true)]
		[TestCase(4, 2, false)]
		public void NumberValidator_DoesNotThrowException(int precision, int scale, bool onlyPositive)
		{
			Action create = () => new NumberValidator(precision, scale, onlyPositive);
			create.Should().NotThrow<ArgumentException>();
		}

		[TestCase(17, 2, true, "0.0")]
		[TestCase(17, 2, false, "-0.0")]
		[TestCase(17, 0, true, "0")]
		[TestCase(17, 2, true, "0")]
		[TestCase(4, 2, false, "-0.00")]
		[TestCase(3, 2, true, "0,00")]
		public void IsValidNumber_ReturnsTrue_OnValidInput(int precision, int scale, bool onlyPositive, string number)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number).Should().BeTrue();
		}

		[TestCase(17, 2, true, "0.")]
		[TestCase(17, 2, true, ".0")]
		[TestCase(17, 2, true, "0   ")]
		[TestCase(17, 2, true, "   0")]
		[TestCase(17, 2, true, null)]
		[TestCase(17, 2, true, "")]
		[TestCase(17, 2, true, " ")]
		[TestCase(3, 2, true, "12.12")]
		[TestCase(3, 2, false, "-0.00")]
		[TestCase(3, 2, true, "+0.00")]
		[TestCase(3, 2, true, "+-0.00")]
		[TestCase(3, 2, true, "++0.00")]
		[TestCase(3, 2, true, "--0.00")]
		[TestCase(10, 5, true, "0.0.0")]
		[TestCase(3, 2, true, "0012")]
		[TestCase(17, 2, true, "0.000")]
		[TestCase(3, 2, true, "a.sd")]

		public void IsValidNumber_ReturnsFalse_OnValidInput(int precision, int scale, bool onlyPositive, string number)
		{
			new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number).Should().BeFalse();
		}

		[TestCase("0.00")]
		[TestCase("0.000")]
		public void IsValidNumber_ReturnsSameResult_OnSecondCall(string number)
		{
			var numberValidator = new NumberValidator(17, 2, true);
			var result1 = numberValidator.IsValidNumber(number);
			var result2 = numberValidator.IsValidNumber(number);
			Assert.AreEqual(result1, result2);
		}

		[Test]
		public void IsValidNumber_SupportsAllDigits()
		{
			var allCasesAreValid = true;
			var numberValidator = new NumberValidator(17, 5, true);
			for (var i = 0; i < 10; i++)
				if (!numberValidator.IsValidNumber($"{i}{i}.{9 - i}{9 - i}"))
				{
					allCasesAreValid = false;
					break;
				}

			allCasesAreValid.Should().BeTrue();
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
				throw new ArgumentException("precision must be a positive number");
			if (scale < 0 || scale >= precision)
				throw new ArgumentException("precision must be a non-negative number less or equal than precision");
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