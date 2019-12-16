using System;
using System.Collections.Generic;
using System.Linq;

namespace day16
{
    public class SecondStar
    {
        public static string Run()
        {
            var digits = InputParser.Parse("input.txt");

            int phases = 100;
            var repeatedDigits = new List<int>();

            for (int r = 0; r < 10_000; r++)
            {
                repeatedDigits.AddRange(digits);
            }

            digits = repeatedDigits.ToArray();

            for (int phase = phases; phases > 0; phases--)
            {
                var newDigitsList = new List<int>();

                for (int outputPhase = 0; outputPhase < digits.Length; outputPhase++)
                {
                    var digitsToAddTogether = new List<int>();
                    int[] outputPattern = CreateOutputPattern(outputPhase, digits.Length);

                    var interestingIndices = CreateInterestingIndices(outputPhase, digits.Length);

                    for (int c = 0; c < interestingIndices.Length; c++)
                    {
                        //if (c < outputPhase)
                        //{
                        //    continue;
                        //}
                        //else if (c > Math.Pow(outputPhase + 1, 2) + 1)
                        //{
                        //    string.Join("", new[] { 1, 2 });
                        //}

                        int interestingIndex = interestingIndices[c];

                        int number = digits[interestingIndex] * outputPattern[interestingIndex];
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

                if (phase % 10 == 0 && phase < 100)
                {
                    Console.WriteLine(" " + phase);
                }
                else
                {
                    Console.Write("*");
                }

                //Console.WriteLine(string.Join("", digits.Take(8)));
            }

            var lookInto = int.Parse("0" + string.Join("", digits.Take(7)));

            return ("0" + string.Join("", digits)).Substring(lookInto, 8);
            //return "Done";
        }

        private static int[] CreateInterestingIndices(int outputPhase, int length)
        {
            // special for output phase 0

            // 0: 0, 2, 4, 6, 8, 10, 12, 14, 16, 18
            // 1: 1, 2, 5, 6, 9, 10, 13, 14, 17, 18

            var list = new List<int>();

            bool skip = false;
            int playhead = outputPhase;

            while (playhead < length)
            {
                if (!skip)
                {
                    list.Add(playhead++);

                    for (int i = 0; i < outputPhase; i++)
                    {
                        if (playhead < length)
                        {
                            list.Add(playhead++);
                        }
                    }
                }
                else
                {
                    playhead += outputPhase + 1;
                }

                skip = !skip;
            }

            return list.ToArray();
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
