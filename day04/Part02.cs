using System;
using System.Collections.Generic;
using System.Linq;

namespace day04
{
    public class Part02
    {
        public string Run(string input)
        {
            // 760 is not right
            // 497 is not right
            // 639 is not right

            var z = Grouped("133345");

            var split = input.Split(new string[] { "-" }, StringSplitOptions.None);
            var min = int.Parse(split[0]);
            var max = int.Parse(split[1]);

            int skipped = 0;
            int valid = 0;

            for (int n = min; n <= max; n++)
            {
                string s = n.ToString();

                int g = 0;
                int d = 0;
                int dec = 0;

                for (int si = 0; si < s.Length - 1; si++)
                {
                    if (s[si] == s[si + 1])
                    {
                        d++;
                    }
                    if (int.Parse(s[si].ToString()) > int.Parse(s[si + 1].ToString())) dec++;
                }

                if (d > 0)
                {

                }

                if (d == 0 || dec > 0 || g > 0)
                {
                    skipped++;
                    continue;
                }
                else
                {
                    if (Grouped("z" + s + "z"))
                    {
                        skipped++;
                        continue;
                    }

                    valid++;
                }
            }

            return (valid).ToString();
        }

        private static bool Grouped(string s)
        {
            var groupedIntegers = new Dictionary<int, int>();
            for (int x = 0; x < s.Length; x++)
            {
                int theSame = 1;
                for (int playhead = 1; x + playhead < s.Length; playhead++)
                {
                    if (s.Substring(x, 1) != s.Substring(x + playhead, 1))
                    {
                        break;
                    }
                    theSame++;
                }

                if (theSame >= 2)
                {
                    var key = int.Parse(s.Substring(x, 1));
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
