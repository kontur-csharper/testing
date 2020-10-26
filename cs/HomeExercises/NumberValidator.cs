using System;
using System.Text.RegularExpressions;

namespace HomeExercises
{
	public class NumberValidator
	{
		private static readonly Regex NumberRegex = new Regex(@"^(?<sign>[+-]?)(?<intPart>\d+)([.,](?<fracPart>\d+))?$",
			RegexOptions.IgnoreCase);

		private readonly bool onlyPositive;
		private readonly int precision;
		private readonly int scale;

		private NumberValidator(int precision, int scale = 0, bool onlyPositive = false)
		{
			this.precision = precision;
			this.scale = scale;
			this.onlyPositive = onlyPositive;
		}

		public static NumberValidator Create(int precision, int scale = 0, bool onlyPositive = false)
		{
			AreFieldsCorrect(precision, scale);
			return new NumberValidator(precision, scale, onlyPositive);
		}

		public static void AreFieldsCorrect(int precision, int scale)
		{
			if (precision <= 0)
				throw new ArgumentException("precision must be a positive number");
			if (scale < 0 || scale > precision)
				throw new ArgumentException("scale must be a non-negative number less or equal than precision");
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

			var match = NumberRegex.Match(value);
			if (!match.Success)
				return false;

			// Знак и целая часть
			var intPart = match.Groups["sign"].Value == "+"
				? match.Groups["intPart"].Value.Length
				: match.Groups["sign"].Value.Length + match.Groups["intPart"].Value.Length;
			// Дробная часть
			var fracPart = match.Groups["fracPart"].Value.Length;

			if (intPart + fracPart > precision || fracPart > scale)
				return false;

			return !onlyPositive || match.Groups["sign"].Value != "-";
		}
	}
}