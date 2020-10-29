using System;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises.NumberValidator
{
    public class NumberValidatorTests
    {
        [TestCase(2, 1, true,
            TestName = "Precision more than scale and they doesn't equal zero, onlyPositive equal true")]
        [TestCase(1, 0, true,
            TestName = "Precision more than scale and scale equal zero, onlyPositive equal true")]
        [TestCase(2, 1,
            TestName = "Precision more than scale and they doesn't equal zero, onlyPositive equal false")]
        public void CreateNumberValidator_DoesNotThrow(int precision, int scale = 0, bool onlyPositive = false)
        {
            Assert.DoesNotThrow(() => new NumberValidator(precision, scale, onlyPositive), 
	            GetErrorString("", precision, scale, onlyPositive, "constructor arguments must be correct"));
        }


        [TestCase(0, TestName = "Precision equal zero")]
        [TestCase(-1, TestName = "Precision is negative ")]
        [TestCase(2, 4, TestName = "Scale more than precision")]
        [TestCase(2, 2, TestName = "Scale equal precision")]
        [TestCase(2, -1, TestName = "Scale is negative")]
        public void CreateNumberValidator_ThrowException(int precision, int scale = 2, bool onlyPositive = false)
        {
	        Assert.Throws<ArgumentException>(() => new NumberValidator(precision, scale, onlyPositive),
		        GetErrorString("", precision, scale, onlyPositive, "constructor arguments must be incorrect"));
        }


        [TestCase(null, TestName = "Value is null")]
        [TestCase("", TestName = "Value is empty")]
        [TestCase(" ", TestName = "Value is whitespace")]
        [TestCase("aaa", TestName = "Value consists letter only")]
        [TestCase("12.3a", TestName = "Value contains letter")]
        [TestCase("\n\t\r", TestName = "Value consists special symbols only")]
        [TestCase("--123", TestName = "Value contains too many signs")]
        [TestCase("1..23", TestName = "Value contains too many dots")]
        [TestCase("1,,23", TestName = "Value contains too many comma")]
        [TestCase("1,", TestName = "No any symbols after comma")]
        [TestCase(".1", TestName = "No any symbols before dot")]
        public void IsValidNumber_ReturnFalse_IncorrectValueFormat(string value, int precision = 17, int scale = 2, bool onlyPositive = true)
        {
            new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeFalse(
                GetErrorString(value, precision, scale, onlyPositive, "value must be incorrect"));
        }


        [TestCase("00.00",
            TestName = "Frac and int parts of length more than precision without sign")]
        [TestCase("-0.00",
            TestName = "Frac and int parts of length more than precision with sign minus")]
        [TestCase("+0.00",
            TestName = "Frac and int parts of length more than precision with sign plus")]
        [TestCase("0.000",
            TestName = "Fractional part of length more than scale")]
        [TestCase("4444",
            TestName = "Integer part of length more than precision without frac part")]
        [TestCase("-1.23",
            TestName = "Negative number when onlyPositive flag enabled")]
        [TestCase("-0.00",
            TestName = "Negative number zero when onlyPositive flag enabled")]
        public void IsValidNumber_ReturnFalse_InvalidValue(string value, int precision = 3, int scale = 2, bool onlyPositive = true)
        {
            new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeFalse(
                GetErrorString(value, precision, scale, onlyPositive, "value must be invalid"));
        }

        
        [Test]
        public void IsValidNumber_ReturnTrue_WhenOneNumberValidatorAndOnlyPositiveFlagDisabled()
        {
            var numberValidator = new NumberValidator(4, 2, false);
            var positiveNumber = 1.23;
            var negativeNumber = -positiveNumber;

            numberValidator.IsValidNumber(positiveNumber.ToString())
                .Should().BeTrue("because one number validator with onlyPositive flag must work with numbers regardless of their sign");
            numberValidator.IsValidNumber(negativeNumber.ToString())
                .Should().BeTrue("because number validator with onlyPositive flag must work with numbers regardless of their sign");
        }


        [TestCase("0.0", 17, 2, true,
            TestName = "Int part of length less than precision without sign")]
        [TestCase("0", 17, 2, true,
            TestName = "Int part of length less than precision without frac part")]
        [TestCase("0", 1, 0, true,
            TestName = "Int part of length equal precision without frac part")]
        [TestCase("+1.23", 4, 2, true,
            TestName = "Int part, frac part and symbol plus of length equal precision (dot separator)")]
        [TestCase("-1.23", 4, 2, false,
            TestName = "Value length equal precision with disabled onlyPositive (dot separator)")]
        [TestCase("+1,23", 4, 2, true,
            TestName = "Int part, frac part and symbol plus of length equal precision (comma separator)")]
        [TestCase("-1,23", 4, 2, false,
            TestName = "Value length equal precision with disabled onlyPositive (comma separator)")]
        [TestCase("999999999", 10, 2, false,
            TestName = "Big number")]
        public void IsValidNumber_ReturnTrue_ValidValue(string value, int precision, int scale, bool onlyPositive)
        {
            new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value).Should().BeTrue(
                GetErrorString(value, precision, scale, onlyPositive, "value must be valid"));
        }

        private string GetErrorString(string value, int precision, int scale, bool onlyPositive, string startText = "")
        {
            return $"{startText} (your data: value={value}, precision={precision}, scale={scale}, onlyPositive={onlyPositive})";
        }
    }
}