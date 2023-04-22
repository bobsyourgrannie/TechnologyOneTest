using TechnologyOneTest.BusinessLogic.Implementation;
using TechnologyOneTest.Interfaces;

namespace TechnologyOneTest.Data
{
    public class NumberToTextService
    {
        private readonly INumberConverter _numberConverter;

        /// <summary>
        /// I have used a dependency-injected INumberConverter here to improve decoupling.
        /// </summary>
        /// <param name="numberConverted"></param>
        public NumberToTextService(INumberConverter numberConverted)
        {
            _numberConverter = numberConverted;
        }

        /// <summary>
        /// This method accepts a number and then checks that it is within a reasonable range for what might be expect in this solution. I have limited the inputs to be >= 0 and < 10,000. The actual number converter can handle up to less than one trillion.
        /// </summary>
        /// <param name="inputNumber"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Task<string> GetNumberAsTextAsync(double inputNumber)
        {
            if (inputNumber < 0 || inputNumber > 10000)
            {
                throw new ArgumentOutOfRangeException("Value must be greater than or equal to $0, and less than $10,000");
            }
            string text = _numberConverter.ConvertCurrencyToText(inputNumber);
            return Task.FromResult(text);
        }
    }
}