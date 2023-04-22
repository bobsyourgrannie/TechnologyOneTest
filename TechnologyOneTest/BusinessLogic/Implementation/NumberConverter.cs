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

            //if (wholeNumber == 1)
            //{
            //    dollarsString = dollarsString.Replace(dollarsText, dollarsTextSingular);
            //}

            double fractionalNumber = Math.Round(number - Math.Truncate(number), 2);

            if (fractionalNumber == 0)
            {
                return dollarsString;
            }

            string fractionalNumberString = ConvertFractionToText(fractionalNumber);
            string centsString = $"{fractionalNumberString} {centsText}";

            if (fractionalNumber == 0.01)
            {
                centsString = centsString.Replace(centsText, centsTextSingular);
            }

            if (wholeNumber == 0)
            {
                return centsString;
            }

            return $"{dollarsString} {andText} {centsString}";
        }

        private string ConvertLongToText(long number)
        {
            if (number < 10)
            {
                return _ones[number];
            }

            if (number < 20)
            {
                return _tens[number - 10];
            }

            if (number < 100)
            {
                string text =  _tensMultiple[number / 10];

                if(number % 10 > 0)
                {
                    text += "-" + _ones[number % 10];
                }

                text = text.Trim();
                return text;
            }

            if (number < 1000)
            {
                return $"{_ones[number / 100]} {hundredText} {andText} {ConvertLongToText(number % 100)}".Trim();
            }

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

        private string ConvertFractionToText(double number)
        {
            string fractionalString = number.ToString("F2");
            int fractionalPart = int.Parse(fractionalString.Substring(fractionalString.IndexOf('.') + 1));

            string[] fractionalParts = ConvertLongToText(fractionalPart).Split(' ');

            if (fractionalParts.Length == 1)
            {
                return $"{fractionalParts[0]}";
            }

            string tensPart = fractionalParts[fractionalParts.Length - 2];
            string onesPart = fractionalParts[fractionalParts.Length - 1];

            return $"{tensPart}{(onesPart == _ones[0] ? string.Empty : "-" + onesPart)}";
        }
    }
}