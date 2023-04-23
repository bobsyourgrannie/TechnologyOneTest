using TechnologyOneTest.Interfaces;

namespace TechnologyOneTest.BusinessLogic.Implementation
{
    /// <summary>
    /// This class contains a single public method ConvertCurrencyToText that allows input of a double (>= 0 and < 1,000,000,000) and ret
    /// </summary>
    public class NumberConverter : INumberConverter
    {
        private const double oneBillion = 1000000000;

        const string dollarsText = "DOLLARS";
        const string centsText = "CENTS";

        const string dollarsTextSingular = "DOLLAR";
        const string centsTextSingular = "CENT";

        const string andText = "AND";

        const string hundredText = "HUNDRED";

        private readonly string[] _lessThanTensNumberStrings = { "ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE" };
        private readonly string[] _teensNumberStrings = { "TEN", "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN", "SIXTEEN", "SEVENTEEN", "EIGHTEEN", "NINETEEN" };
        private readonly string[] _tensMultiplesNumberStrings = { "", "", "TWENTY", "THIRTY", "FORTY", "FIFTY", "SIXTY", "SEVENTY", "EIGHTY", "NINETY" };
        private readonly string[] _thousandsNumberStrings = { "", "THOUSAND", "MILLION" }; // could keep adding to this list depending on how high we need to go

        /// <summary>
        /// This method allows input of a double (>= 0 and < 1,000,000,000) and returns the currency text representation of that number.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public string ConvertCurrencyToText(double number)
        {
            // Throw error if number is out of bounds for the listed text conversion strings
            if (number >= oneBillion || number < 0)
            {
                throw new ArgumentOutOfRangeException("This converter only handles numbers from zero to less than one billion");
            }

            // I was not sure what to display for a value of 0 so just have a special case for it here
            if (number == 0)
            {
                return $"{_lessThanTensNumberStrings[0]} {dollarsText}";
            }

            long wholeNumber = Convert.ToInt64(Math.Floor(number));
            string wholeNumberString = ConvertLongToText(wholeNumber);
            string dollarsString = $"{wholeNumberString} {(wholeNumber == 1 ? dollarsTextSingular : dollarsText)}";

            // round off to two decimal places if there are more than two (UI also does this operation before send)
            double fractionalNumber = Math.Round(number - Math.Truncate(number), 2);

            // if there is no fraction (cents) then just return the whole number (dollars) string
            if (fractionalNumber == 0)
            {
                return dollarsString;
            }

            string fractionalNumberString = ConvertFractionToText(fractionalNumber);
            string centsString = $"{fractionalNumberString} {(fractionalNumber == 0.01 ? centsTextSingular : centsText)}";

            // if there is no whole number (dollars) then just return the fractional number (cents) string
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
            // handle < 10
            if (number < 10)
            {
                return _lessThanTensNumberStrings[number];
            }

            // handle 10 to 19
            if (number < 20)
            {
                return _teensNumberStrings[number - 10];
            }

            // handle 20 to 99
            if (number < 100)
            {
                return Get21To99Text(number);
            }

            // handle 100 to 999
            if (number < 1000)
            {
                return Get100To999Text(number);
            }

            // handle all numbers >= 1000
            return GetGreaterThanOrEqualTo1000Text(number);
        }

        /// <summary>
        /// Logic to return text for integral numbers between 20 and 99
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private string Get21To99Text(long number)
        {
            string text = _tensMultiplesNumberStrings[number / 10];

            if (number % 10 > 0)
            {
                text += "-" + _lessThanTensNumberStrings[number % 10];
            }

            text = text.Trim();
            return text;
        }

        /// <summary>
        /// Logic to return text for integral numbers between 100 and 999
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private string Get100To999Text(long number)
        {
            string val = $" {_lessThanTensNumberStrings[number / 100]} {hundredText} ";
            if (number % 100 > 0)
            {
                val += $"{andText} {ConvertLongToText(number % 100)}".Trim();
            }
            return val.Trim();
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
                // iterate through the number in 'thousand' groups
                long groupNumber = number % 1000;

                if (groupNumber != 0)
                {
                    // this is to determine where to add the 'and' between a hundreds value and the 1 to 99 value text below it
                    bool lessThanOneHundredJoiningTextRequired = i == 0 && !lessThanOneHundredAndUsed && groupNumber > 0 && groupNumber < 99;

                    string groupTextTemp = $"{(lessThanOneHundredJoiningTextRequired ? $"{andText} " : string.Empty)}{ConvertLongToText(groupNumber)} {_thousandsNumberStrings[i]} ";

                    if (lessThanOneHundredJoiningTextRequired)
                    {
                        lessThanOneHundredAndUsed = true;
                    }

                    groupText = groupTextTemp + groupText;
                }

                number = number / 1000;
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