using TechnologyOneTest.Interfaces;

namespace TechnologyOneTest.BusinessLogic.Implementation
{
    public class NumberConverter : INumberConverter
    {
        private readonly string[] _ones = { "ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE" };
        private readonly string[] _tens = { "TEN", "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN", "SIXTEEN", "SEVENTEEN", "EIGHTEEN", "NINETEEN" };
        private readonly string[] _tensMultiple = { "", "", "TWENTY", "THIRTY", "FORTY", "FIFTY", "SIXTY", "SEVENTY", "EIGHTY", "NINETY" };
        private readonly string[] _group = { "", "THOUSAND", "MILLION", "BILLION" };

        public string ConvertNumberToText(double number)
        {
            if (number == 0)
            {
                return "ZERO DOLLARS AND ZERO CENTS";
            }

            int wholeNumber = Convert.ToInt32(Math.Floor(number));
            double fractionalNumber = Math.Round(number - Math.Truncate(number), 2);

            string wholeNumberString = ConvertToText(wholeNumber);
            string fractionalNumberString = ConvertFractionalToText(fractionalNumber);

            string dollarsString = $"{wholeNumberString} DOLLARS";
            string centsString = $"{fractionalNumberString} CENTS";

            if (wholeNumber == 1)
            {
                dollarsString = dollarsString.Replace("DOLLARS", "DOLLAR");
            }

            if (fractionalNumber == 0)
            {
                return dollarsString;
            }

            if (fractionalNumber == 1)
            {
                centsString = centsString.Replace("CENTS", "CENT");
            }

            if (wholeNumber == 0)
            {
                return centsString;
            }

            return $"{dollarsString} AND {centsString}";
        }

        private string ConvertToText(long number)
        {
            if (number < 0)
            {
                return "MINUS " + ConvertToText(Math.Abs(number));
            }

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
                return $"{_tensMultiple[number / 10]} {_ones[number % 10]}".Trim();
            }

            if (number < 1000)
            {
                return $"{_ones[number / 100]} HUNDRED {ConvertToText(number % 100)}".Trim();
            }

            string groupText = string.Empty;

            for (int i = 0; number > 0; i++)
            {
                long groupNumber = number % 1000;

                if (groupNumber != 0)
                {
                    string groupTextTemp = $"{ConvertToText(groupNumber)} {_group[i]} ";

                    groupText = groupTextTemp + groupText;
                }

                number /= 1000;
            }

            return groupText.Trim();
        }

        private string ConvertFractionalToText(double number)
        {
            if (number == 0)
            {
                return "ZERO";
            }

            string fractionalString = number.ToString("F2");
            int fractionalPart = int.Parse(fractionalString.Substring(fractionalString.IndexOf('.') + 1));

            if (fractionalPart == 0)
            {
                return "ZERO";
            }

            string[] fractionalParts = ConvertToText(fractionalPart).Split(' ');

            if (fractionalParts.Length == 1)
            {
                return $"{fractionalParts[0]}";
            }

            string tensPart = fractionalParts[fractionalParts.Length - 2];
            string onesPart = fractionalParts[fractionalParts.Length - 1];

            return $"{tensPart} {(onesPart == "ZERO" ? string.Empty : onesPart)}";
        }
    }
}
