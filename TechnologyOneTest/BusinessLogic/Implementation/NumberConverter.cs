using TechnologyOneTest.Interfaces;

namespace TechnologyOneTest.BusinessLogic.Implementation
{
    /// <summary>
    /// This class contains a single public method ConvertCurrencyToText that allows input of a double (>= 0 and < 1,000,000,000,000) and ret
    /// </summary>
    public class NumberConverter : INumberConverter
    {
        private const double oneTrillion = 1000000000000;

        private readonly string[] _ones = { "ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE" };
        private readonly string[] _tens = { "TEN", "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN", "SIXTEEN", "SEVENTEEN", "EIGHTEEN", "NINETEEN" };
        private readonly string[] _tensMultiple = { "", "", "TWENTY", "THIRTY", "FORTY", "FIFTY", "SIXTY", "SEVENTY", "EIGHTY", "NINETY" };
        private readonly string[] _group = { "", "THOUSAND", "MILLION", "BILLION" };

        const string dollarsText = "DOLLARS";
        const string centsText = "CENTS";

        const string dollarsTextSingular = "DOLLAR";
        const string centsTextSingular = "CENT";

        const string andText = "AND";

        const string hundredText = "HUNDRED";

        /// <summary>
        /// This method allows input of a double (>= 0 and < 1,000,000,000,000) and returns the currency text representation of that number.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public string ConvertCurrencyToText(double number)
        {
            // Throw error if number is out of bounds for the listed text conversion strings (less than one trillion dollars)
            if (number >= oneTrillion || number < 0)
            {
                throw new ArgumentOutOfRangeException("This converter only handles numbers from zero to less than one trillion");
            }

            // I was not sure what to display for a value of 0 so just have a special case for it here
            if (number == 0)
            {
                return $"{_ones[0]} {dollarsText}";
            }

            long wholeNumber = Convert.ToInt64(Math.Floor(number));
            string wholeNumberString = ConvertLongToText(wholeNumber);
            string dollarsString = $"{wholeNumberString} {(wholeNumber == 1 ? dollarsTextSingular : dollarsText)}";

            // round off to two decimal places if there are more than two
            double fractionalNumber = Math.Round(number - Math.Truncate(number), 2);

            // if there is no fraction (cents) then just return the whole number text
            if (fractionalNumber == 0)
            {
                return dollarsString;
            }

            string fractionalNumberString = ConvertFractionToText(fractionalNumber);
            string centsString = $"{fractionalNumberString} {(fractionalNumber == 0.01 ? centsTextSingular : centsText)}";

            // if there is no whole number (dollars) then just return the fractional number (cents)
            if (wholeNumber == 0)
            {
                return centsString;
            }

            return $"{dollarsString} {andText} {centsString}";
        }

        /// <summary>
        /// Convert a partial number from the overall number into its text representation (uses recursive call to break up the operation into 'thousands' groups
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private string ConvertLongToText(long number)
        {
            // return ones
            if (number < 10)
            {
                return _ones[number];
            }

            // return teens
            if (number < 20)
            {
                return _tens[number - 10];
            }

            // return < 100
            if (number < 100)
            {
                return GetLessThan100Text(number);
            }

            // return < 1000
            if (number < 1000)
            {
                return $"{_ones[number / 100]} {hundredText} {andText} {ConvertLongToText(number % 100)}".Trim();
            }

            // return all numbers >= 1000
            return GetGreaterThanOrEqualTo1000Text(number);
        }

        /// <summary>
        /// Logic to return text for integral numbers between 20 and 99
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private string GetLessThan100Text(long number)
        {
            string text = _tensMultiple[number / 10];

            if (number % 10 > 0)
            {
                text += "-" + _ones[number % 10];
            }

            text = text.Trim();
            return text;
        }

        /// <summary>
        /// Logic to return text for integral numbers >= 1000
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private string GetGreaterThanOrEqualTo1000Text(long number)
        {
            string groupText = string.Empty;
            bool lessThanOneHundredAndUsed = false;

            for (int i = 0; number > 0; i++)
            {
                long groupNumber = number % 1000;

                if (groupNumber != 0)
                {
                    bool lessThanOneHundredAndRequired = i == 0 && !lessThanOneHundredAndUsed && groupNumber > 0 && groupNumber < 99;

                    string groupTextTemp = $"{(lessThanOneHundredAndRequired ? $"{andText} " : string.Empty)}{ConvertLongToText(groupNumber)} {_group[i]} ";

                    if (lessThanOneHundredAndRequired)
                    {
                        lessThanOneHundredAndUsed = true;
                    }

                    groupText = groupTextTemp + groupText;
                }

                number /= 1000;
            }

            return groupText.Trim();
        }

        /// <summary>
        /// Convert the fractional (cents) component to its text representation
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private string ConvertFractionToText(double number)
        {
            // use this string format to ensure we are working with two digits so for instance 0.3 is 30 cents rather than 3 cents
            string fractionalString = number.ToString("F2");
            // convert cents string to an int by removing decimal place and numbers before it
            int fractionalPart = int.Parse(fractionalString.Substring(fractionalString.IndexOf('.') + 1));
            return ConvertLongToText(fractionalPart);
        }
    }
}