using System;
using System.Collections.Generic;
using System.Linq;

namespace day04
{
    public class SecondStar
    {
        public static string Run(string input)
        {
            var inputValues = input.Split(new string[] { "-" }, StringSplitOptions.None);

            var min = int.Parse(inputValues[0]);
            var max = int.Parse(inputValues[1]);

            int skipped = 0;
            int valid = 0;

            for (int number = min; number <= max; number++)
            {
                string numberString = number.ToString();
                bool decreasing = false;
                int duplicate = 0;

                for (int si = 0; si < numberString.Length - 1; si++)
                {
                    if (numberString[si] == numberString[si + 1])
                    {
                        duplicate++;
                    }

                    if (int.Parse(numberString[si].ToString()) > int.Parse(numberString[si + 1].ToString()))
                    {
                        decreasing = true;
                        break;
                    }
                }

                if (duplicate == 0 || decreasing)
                {
                    skipped++;
                    continue;
                }

                if (Grouped(numberString))
                {
                    skipped++;
                    continue;
                }

                valid++;
            }

            return (valid).ToString();
        }

        private static bool Grouped(string numberString)
        {
            var paddedNumberString = $"z{numberString}z";
            var groupedIntegers = new Dictionary<int, int>();

            for (int x = 0; x < paddedNumberString.Length; x++)
            {
                int theSame = 1;
                for (int playhead = 1; x + playhead < paddedNumberString.Length; playhead++)
                {
                    if (paddedNumberString.Substring(x, 1) != paddedNumberString.Substring(x + playhead, 1))
                    {
                        break;
                    }
                    theSame++;
                }

                if (theSame >= 2)
                {
                    var key = int.Parse(paddedNumberString.Substring(x, 1));
                    if (!groupedIntegers.ContainsKey(key))
                    {
                        groupedIntegers.Add(key, theSame);
                    }
                    else if (groupedIntegers[key] < theSame)
                    {
                        groupedIntegers[key] = theSame;
                    }
                }
            }

            int biggest = -1;
            foreach (var kvp in groupedIntegers)
            {
                if (kvp.Key > biggest)
                {
                    biggest = kvp.Key;
                }
            }

			if (groupedIntegers.Count > 0)
			{
				if (groupedIntegers.Values.Contains(2))
				{
					return false;
				}
            }

            return true;
        }
    }
}
