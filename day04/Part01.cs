using System;
using System.Collections.Generic;
using System.Text;

namespace day04
{
    public class Part01
    {
        public string Run(string input)
        {
            // 1661 not right
            // 278 not right
            // 109 not right
            var result = "";

            var split = input.Split(new string[] { "-" }, StringSplitOptions.None);
            var min = int.Parse(split[0]);
            var max = int.Parse(split[1]);

            int skipped = 0;
            int valid = 0;

            for (int n = min; n <= max; n++)
            {
                string s = n.ToString();

                int d = 0;
                int dec = 0;
                for (int si = 0; si < s.Length - 1; si++)
                {
                    if (s[si] == s[si + 1]) d++;
                    if (int.Parse(s[si].ToString()) > int.Parse(s[si + 1].ToString())) dec++;
                }
                if (d == 0 || dec > 0)
                {
                    skipped++;
                    continue;
                }
                else
                {
                    valid++;
                }
            }

            return (valid).ToString();
        }
    }
}
