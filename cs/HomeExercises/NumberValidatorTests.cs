using System;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		[Test]
		public void NumberValidatorConstructorMultipleTest()
		{
			Assert.Multiple(() =>
			{
				Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true),
					"NumberValidatorConstructor_PrecisionLessThan0_ArgumentExceptionThrown");

				Assert.Throws<ArgumentException>(() => new NumberValidator(2, -1),
					"NumberValidatorConstructor_ScaleLessThan0_ArgumentExceptionThrown");
				Assert.Throws<ArgumentException>(() => new NumberValidator(2, 3, true),
					"NumberValidatorConstructor_ScaleMoreThanPrecision_ArgumentExceptionThrown");

				Assert.Throws<ArgumentException>(() => new NumberValidator(2, 2, true),
					"NumberValidatorConstructor_ScaleEqualPrecision_ArgumentExceptionThrown");


				Assert.DoesNotThrow(() => new NumberValidator(1, 0, true),
					"NumberValidatorConstructor_OnlyPositiveNumbersScaleSet0_CreatedNumberValidator");
				Assert.DoesNotThrow(() => new NumberValidator(1, 0),
					"NumberValidatorConstructor_ScalesSet0_CreatedNumberValidator");
			});
		}

		[Test]
		public void NumberValidationMultiTest()
		{
			Assert.Multiple(() =>
			{
				Assert.IsTrue(new NumberValidator(4, 2, true).IsValidNumber("+1.23"),
					"IsValidNumber_NumberWith3DigitsAndInsignificantSign_True");


				var validator = new NumberValidator(17, 2, true);

				Assert.IsFalse(validator.IsValidNumber(string.Empty), "IsValidNumber_EmptyString_False");
				Assert.IsFalse(validator.IsValidNumber(null), "IsValidNumber_Null_False");
				Assert.IsFalse(validator.IsValidNumber("     "), "IsValidNumber_Spaces_False");
				Assert.True(validator.IsValidNumber("0"), "IsValidNumber_NumberWithoutFractionalPart_True");
				Assert.IsFalse(validator.IsValidNumber("0.000"), "IsValidNumber_FractionalPartMoreThanScale_False");

				Assert.IsTrue(validator.IsValidNumber("0.0"), "IsValidNumber_FractionalPartIs0_True");
				Assert.IsTrue(validator.IsValidNumber("1"),
					"IsValidNumber_PositiveIntegerWithOnlyPositiveValidator_True");
				Assert.IsTrue(new NumberValidator(4, 1, false).IsValidNumber("1"),
					"IsValidNumber_PositiveIntegerWhenValidatorNotOnlyPositive_True");
				Assert.IsFalse(validator.IsValidNumber("-1"),
					"IsValidNumber_NegativeIntegerWhenValidatorOnlyPositive_False");
				Assert.IsFalse(validator.IsValidNumber(".1"), "IsValidNumber_BeginWithDot_False");
				Assert.IsFalse(validator.IsValidNumber("1."),
					"IsValidNumber_NumberWithDotButWithoutFractionalPart_False");
				Assert.IsTrue(validator.IsValidNumber("1,2"), "IsValidNumber_NumberWithComma_True");
				Assert.IsTrue(validator.IsValidNumber("౦౧౨"), "IsValidNumber_TeluguNumbers_True");
				Assert.IsTrue(validator.IsValidNumber("๑๒๓"), "IsValidNumber_ThaiNumbers_True");
				Assert.IsTrue(validator.IsValidNumber("٦٧"), "IsValidNumber_ArabicNumbers_True");
				Assert.IsTrue(validator.IsValidNumber("१०"), "IsValidNumber_DevanagariNumbers_True");

				validator = new NumberValidator(3, 2, true);


				Assert.IsFalse(validator.IsValidNumber("+0.00"),
					"IsValidNumber_DigitsAndSignLengthMoreThanPrecision_False");

				Assert.IsFalse(validator.IsValidNumber("-0.00"),
					"IsValidNumber_DigitsAndSignLengthMoreThanPrecision_False");

				Assert.IsFalse(validator.IsValidNumber("00.00"), "IsValidNumber_InsignificantZerosWithDot_False");
				Assert.IsFalse(validator.IsValidNumber("-a.sd"), "IsValidNumber_LettersWithSignAndDot_False");
				Assert.IsFalse(validator.IsValidNumber("1.a"), "IsValidNumber_DigitWithDotAndLetter_False");
				Assert.IsFalse(validator.IsValidNumber(" 1"), "IsValidNumber_NumberWithSpaceInBegin_False");
				Assert.IsFalse(validator.IsValidNumber("1 "), "IsValidNumber_SpaceAfterNumber_False");
				Assert.IsFalse(validator.IsValidNumber("-+1"), "IsValidNumber_PlusAndMinusBeforeNumber_False");
			});
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