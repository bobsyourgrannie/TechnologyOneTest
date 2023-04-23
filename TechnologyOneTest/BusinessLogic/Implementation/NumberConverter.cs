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

        private readonly string[] _lessThanTwentyNumberStrings = { "ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE", "TEN", "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN", "SIXTEEN", "SEVENTEEN", "EIGHTEEN", "NINETEEN" };
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
                return $"{_lessThanTwentyNumberStrings[0]} {dollarsText}";
            }

            long wholeNumber = Convert.ToInt64(Math.Floor(number));
            string wholeNumberString = ConvertNumberToText(wholeNumber);
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
        /// Convert a partial number from the overall number into its text representation (this class uses recursive calls to this method to get text for each 'thousand' groups to shard code for thousands and millions (and above if we want the numbers to go higher later with a minor modification)
        /// </summary>
        /// <param name="partialNumber"></param>
        /// <returns></returns>
        private string ConvertNumberToText(long partialNumber)
        {
            // handle up to 19
            if (partialNumber < 20)
            {
                return _lessThanTwentyNumberStrings[partialNumber];
            }

            // handle 20 to 99
            if (partialNumber < 100)
            {
                return Get21To99Text(partialNumber);
            }

            // handle 100 to 999
            if (partialNumber < 1000)
            {
                return Get100To999Text(partialNumber);
            }

            // handle all numbers >= 1000
            return Get1000AndAboveText(partialNumber);
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
                text += "-" + _lessThanTwentyNumberStrings[number % 10];
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
            string val = $" {_lessThanTwentyNumberStrings[number / 100]} {hundredText} ";
            if (number % 100 > 0)
            {
                val += $"{andText} {ConvertNumberToText(number % 100)}".Trim();
            }
            return val.Trim();
        }

        /// <summary>
        /// Logic to return text for integral numbers >= 1000
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private string Get1000AndAboveText(long number)
        {
            string textRepresentation = string.Empty;
            bool lessThanOneHundredAndUsed = false;

            for (int i = 0; number > 0; i++)
            {
                // iterate through the number in 'thousand' groups
                long thousandsGroupNumber = number % 1000;

                if (thousandsGroupNumber != 0)
                {
                    // this is to determine where to add the 'and' between a hundreds value and the 1 to 99 value text below it
                    bool lessThanOneHundredJoiningTextRequired = i == 0 && !lessThanOneHundredAndUsed && thousandsGroupNumber > 0 && thousandsGroupNumber < 99;

                    string thousandsGroupTextRepresentation = $"{(lessThanOneHundredJoiningTextRequired ? $"{andText} " : string.Empty)}{ConvertNumberToText(thousandsGroupNumber)} {_thousandsNumberStrings[i]} ";

                    if (lessThanOneHundredJoiningTextRequired)
                    {
                        lessThanOneHundredAndUsed = true;
                    }

                    textRepresentation = thousandsGroupTextRepresentation + textRepresentation;
                }

                number = number / 1000;
            }

            return textRepresentation.Trim();
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
            return ConvertNumberToText(fractionalPart);
        }
    }
}