using System;
using System.Collections.Generic;
using System.Linq;

namespace day16
{
    public class FirstStar
    {
        public static string Run()
        {
            var digits = InputParser.Parse("input.txt");

            int phases = 100;

            for (int phase = phases; phases > 0; phases--)
            {
                var newDigitsList = new List<int>();

                for (int outputPhase = 0; outputPhase < digits.Length; outputPhase++)
                {
                    var digitsToAddTogether = new List<int>();
                    int[] outputPattern = CreateOutputPattern(outputPhase, digits.Length);


                    for (int c = 0; c < digits.Length; c++)
                    {
                        int number = digits[c] * outputPattern[c];
                        digitsToAddTogether.Add(TakeOnesDigit(number));
                    }

                    int sum = 0;
                    foreach (var digit in digitsToAddTogether)
                    {
                        sum += digit;
                    }

                    newDigitsList.Add(Math.Abs(TakeOnesDigit(sum)));
                }

                digits = newDigitsList.ToArray();
            }

            return string.Join("", digits.Take(8));
        }

        private static int TakeOnesDigit(int number)
        {
            string numberString = number.ToString();
            if (number > 9)
            {
                numberString = numberString.Substring(numberString.Length - 1);
            }
            else if (number < -9)
            {
                numberString = "-" + numberString.Substring(numberString.Length - 1);
            }

            return int.Parse(numberString);
        }

        private static int[] CreateOutputPattern(int outputPhase, int length)
        {
            var basePattern = new[] { 0, 1, 0, -1 };
            var outputValues = new List<int>();

            int playhead = 0;

            while (outputValues.Count <= length)
            {
                outputValues.Add(basePattern[playhead]);

                for (var r = 0; r < outputPhase; r++)
                {
                    outputValues.Add(basePattern[playhead]);
                }

                playhead = ++playhead % 4;
            }

            return outputValues.Skip(1).Take(length).ToArray();
        }
    }
}
