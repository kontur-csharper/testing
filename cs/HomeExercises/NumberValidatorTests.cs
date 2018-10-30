﻿using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
        [TestCase(-1, 1, TestName = "NegativePrecision")]
        [TestCase(0, 1, TestName = "PrecisionIsZero")]
        [TestCase(1, -1, TestName = "NegativeScale")]
        [TestCase(1, 2, TestName = "ScaleMoreThanPrecision")]
        public void Constructor_ShouldThrowArgumentException_OnInvalidParams(int precision, int scale)
        {
            Action action = () => new NumberValidator(precision, scale);
            action.ShouldThrow<ArgumentException>();
        }

        [TestCase("1234567890", 10, TestName = "NumberCanContainAnyDigit")]
        [TestCase("0.00", 17, 2, TestName = "LessDigitsThanPrecision")]
        [TestCase("0.0", 3, 2, TestName = "LessDigitsThanScale")]
        [TestCase("0", 3, 2, TestName = "NoDigitsInScale")]
        [TestCase("0,00", 3, 2, TestName = "UsingComma")]
        [TestCase("-0", 2, TestName = "UsingMinus")]
        [TestCase("+0", 2, 0, true, TestName = "UsePlusInOnlyPositive")]
        public void IsValid_ShouldReturnTrue_OnRightNumberFormat(string number, int precision, int scale = 0, bool onlyPositive = false) =>
            new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number).Should().BeTrue();

        [TestCase("a.aa", 3, 2, TestName = "NumberIsLowercaseLetters")]
        [TestCase("A.AA", 3, 2, TestName = "NumberIsUppercaseLetters")]
        [TestCase("ё.яы", 3, 2, TestName = "NumberIsRussianLetters")]
        [TestCase(null, 1, TestName = "NumberIsNull")]
        [TestCase("", 1, TestName = "NumberIsEmpty")]
        [TestCase(" ", 1, TestName = "NumberIsSpace")]
        [TestCase("00.00", 3, 2, TestName = "MoreDigitsThanPrecision")]
        [TestCase("0.00", 3, 1, TestName = "MoreDigitsThanScale")]
        [TestCase("-0", 2, 0, true, TestName = "UsingMinusInOnlyPositive")]
        [TestCase("+0", 1, TestName = "PlusCountsAsDigit")]
		[TestCase("++0", 3, TestName = "MoreThanOnePlus")]
		[TestCase("+ 0", 3, TestName = "SpaceAfterPlus")]
		[TestCase("+-0", 3, TestName = "BothSigns")]
		[TestCase("–0", 2, TestName = "DashInsteadOfMinus")]
        [TestCase(" 0.0", 3, 2, TestName = "SymbolsBeforeNumber")]
        [TestCase("0.0 ", 3, 2, TestName = "SymbolsAfterNumber")]
        [TestCase("0.", 1, TestName = "NoDigitsAfterDot")]
        [TestCase(".0", 2, 1, TestName = "NoDigitsBeforeDot")]
        public void IsValid_ShouldReturnFalse_OnWrongNumberFormat(string number, int precision, int scale = 0, bool onlyPositive = false) =>
            new NumberValidator(precision, scale, onlyPositive).IsValidNumber(number).Should().BeFalse();
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