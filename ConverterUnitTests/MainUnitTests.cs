using TechnologyOneTest.BusinessLogic.Implementation;
using TechnologyOneTest.Interfaces;

namespace ConverterUnitTests
{
    public class Tests
    {
        private class NumberAndExpectedText
        {
            public double Number { get; set; }
            public string ExpectedText { get; set; }

            public NumberAndExpectedText(double number, string expectedText)
            {
                Number = number;
                ExpectedText = expectedText;
            }
        }

        private INumberConverter _numberConverter;

        [SetUp]
        public void Setup()
        {
            _numberConverter = new NumberConverter();
        }

        [Test]
        public void TestValidNumbers()
        {
            List<NumberAndExpectedText> allTestValues = GetValidTestNumbersAndExpectedStrings();

            foreach (NumberAndExpectedText testValue in allTestValues)
            {
                Assert.That(_numberConverter.ConvertCurrencyToText(testValue.Number), Is.EqualTo(testValue.ExpectedText));
            }
        }

        [Test]
        public void TestInvalidNumberBelowZero()
        {
            const double negativeNumber = -1;
            Assert.Throws<ArgumentOutOfRangeException>(() => _numberConverter.ConvertCurrencyToText(negativeNumber));
        }

        [Test]
        public void TestInvalidNumberAboveMax()
        {
            const double oneTrillion = 1000000000000;
            Assert.Throws<ArgumentOutOfRangeException>(() => _numberConverter.ConvertCurrencyToText(oneTrillion));
        }

        private List<NumberAndExpectedText> GetValidTestNumbersAndExpectedStrings()
        {
            return new List<NumberAndExpectedText>
            {
                CreateTestValues(99876.54, "NINETY-NINE THOUSAND EIGHT HUNDRED AND SEVENTY-SIX DOLLARS AND FIFTY-FOUR CENTS"),
                CreateTestValues(991876.54, "NINE HUNDRED AND NINETY-ONE THOUSAND EIGHT HUNDRED AND SEVENTY-SIX DOLLARS AND FIFTY-FOUR CENTS"),
                CreateTestValues(9876.54, "NINE THOUSAND EIGHT HUNDRED AND SEVENTY-SIX DOLLARS AND FIFTY-FOUR CENTS"),
                CreateTestValues(1012, "ONE THOUSAND AND TWELVE DOLLARS"),
                CreateTestValues(1112, "ONE THOUSAND ONE HUNDRED AND TWELVE DOLLARS"),
                CreateTestValues(1000, "ONE THOUSAND DOLLARS"),
                CreateTestValues(1000.06, "ONE THOUSAND DOLLARS AND SIX CENTS"),
                CreateTestValues(1000.16, "ONE THOUSAND DOLLARS AND SIXTEEN CENTS"),
                CreateTestValues(0, "ZERO DOLLARS"),
                CreateTestValues(10.3, "TEN DOLLARS AND THIRTY CENTS"),
                CreateTestValues(10.30, "TEN DOLLARS AND THIRTY CENTS"),
                CreateTestValues(10.3000000000000, "TEN DOLLARS AND THIRTY CENTS"),
                CreateTestValues(000000000010.30, "TEN DOLLARS AND THIRTY CENTS"),
                CreateTestValues(123.45, "ONE HUNDRED AND TWENTY-THREE DOLLARS AND FORTY-FIVE CENTS"),
                CreateTestValues(0.01, "ONE CENT"),
                CreateTestValues(1.01, "ONE DOLLAR AND ONE CENT"),
                CreateTestValues(112, "ONE HUNDRED AND TWELVE DOLLARS"),
                CreateTestValues(1.11, "ONE DOLLAR AND ELEVEN CENTS"),
                CreateTestValues(11.90, "ELEVEN DOLLARS AND NINETY CENTS"),
                CreateTestValues(987654321012.34, "NINE HUNDRED AND EIGHTY-SEVEN BILLION SIX HUNDRED AND FIFTY-FOUR MILLION THREE HUNDRED AND TWENTY-ONE THOUSAND AND TWELVE DOLLARS AND THIRTY-FOUR CENTS"),
            };
        }

        private NumberAndExpectedText CreateTestValues(double number, string expectedText)
        {
            return new NumberAndExpectedText(number, expectedText);
        }
    }
}