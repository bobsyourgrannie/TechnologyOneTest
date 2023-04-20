using TechnologyOneTest.BusinessLogic.Implementation;
using TechnologyOneTest.Interfaces;

namespace TechnologyOneTest.Data
{
    public class NumberToTextService
    {
        private readonly INumberConverter _numberConverter;

        public NumberToTextService()
        {
            //_numberConverter = converter;
            _numberConverter = new NumberConverter();
        }

        public Task<string> GetNumberAsTextAsync(double inputNumber)
        {
            var text = _numberConverter.ConvertNumberToText(inputNumber);
            return Task.FromResult(text);
        }
    }
}