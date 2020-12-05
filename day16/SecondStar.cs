using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace day16
{
    public class SecondStar
    {
        public static string Run()
        {
            var timesRepeat = 10_000;
            var digits = InputParser.ParseAsString("input.txt");

            var digitsIntegers = new int[digits.Length];

            for (int digitIndex = 0; digitIndex < digits.Length; digitIndex++)
                digitsIntegers[digitIndex] = int.Parse(digits[digitIndex].ToString());

            var offset = int.Parse(digits.Substring(0, 7));

            var virtualMax = digitsIntegers.Length * timesRepeat;
            var virtualWorkspaceLength = virtualMax - offset;

            var signalInput = new int[virtualWorkspaceLength];

            for (int signalIndex = 0; signalIndex < virtualWorkspaceLength; signalIndex++)
                signalInput[signalIndex] = digitsIntegers[(offset + signalIndex) % digitsIntegers.Length];
            
            int[] stairs = new int[signalInput.Length];

            for (int phase = 0; phase < 100; phase++)
            {
                int startValue = signalInput.Sum();

                for (int element = 0; element < signalInput.Length; element++)
                {
                    if (element == 0)
                        stairs[element] = startValue;
                    else
                        stairs[element] = stairs[element - 1] - signalInput[element - 1];
                }

                for (int element = 0; element < stairs.Length; element++)
                    stairs[element] = stairs[element] % 10;

                Array.Copy(stairs, signalInput, stairs.Length);
                Array.Clear(stairs, 0, stairs.Length);
            }

            var result = string.Empty;
            for (int n = 0; n < 8; n++)
            {
                result += signalInput[n].ToString();
            }

            return result;
        }

#if olderstuffisenabled
        private static readonly int _phasesToComplete = 100;

        public class IntCollection
        {
            private int[] _values;
            private int _size;

            public IntCollection(int size)
            {
                _values = new int[size];
                _size = size;
            }

            public int Get(int index)
                => _values[index];

            public int Set(int index, int value)
                => _values[index] = value;

            public int Size => _size;
        }

        public static string Run()
        {
            int inputRepeatedTimes = 10_000;
            var digits = InputParser.ParseAsString("input.txt");

            //digits = "03036732577212944063491565474664";
            //inputRepeatedTimes = 1;

            //var digitsIntegers = new int[digits.Length * inputRepeatedTimes];


            // skip the 0s

            // 1 = meaningfulPlayhead+0,2,4,6,8
            // 2 = meaningfulPlayhead+0,2,4,6,8

            var nextInputA = new IntCollection(digits.Length * inputRepeatedTimes);
            var nextInputB = new IntCollection(digits.Length * inputRepeatedTimes);
            
            IntCollection read = nextInputA;
            IntCollection write = nextInputB;
            bool isA = true;

            for (int digitIndex = 0; digitIndex < digits.Length; digitIndex++)
                read.Set(digitIndex, int.Parse(digits[digitIndex].ToString()));

            Thread[] threads = new Thread[8];

            for (int phase = 1; phase <= _phasesToComplete; phase++)
            {
                int nextInputIndex = 0;


                for (int signalIndex = 0; signalIndex < read.Size; signalIndex++)
                {
                    int playheadAdvance = 0;
                    bool isPositive = true;
                    int sum = 0;

                    int signalIndexPlusOne = signalIndex + 1;

                    var currentPosition = signalIndex + playheadAdvance; // + innerPhase;

                    do
                    {
                        var signalProductFlag = (int)Math.Floor((currentPosition - signalIndexPlusOne) / (signalIndexPlusOne * 4.0));
                        
                        if (signalProductFlag == 0) isPositive = true;
                        else if (signalProductFlag == 2) isPositive = false;
                        else
                            continue;

                        var product = (isPositive ? 1 : -1) * read.Get(currentPosition);

                        sum += product;

                        playheadAdvance++;

                        currentPosition = signalIndex + playheadAdvance;

                        if (currentPosition >= read.Size)
                            break;
                    } while (true);

                    sum = Math.Abs(sum % (sum >= 0 ? 10 : -10));

                    write.Set(nextInputIndex++, sum);
                }

                if (isA)
                {
                    read = nextInputB;
                    write = nextInputA;
                }
                else
                {
                    read = nextInputA;
                    write = nextInputB;
                }

                isA = !isA;
            }

            int offset = 5_977_709;
            string result = string.Empty;

            for (int offsetNumberIndex = 0; offsetNumberIndex < 8; offsetNumberIndex++)
            {
                int wrappedOffset = (offset + offsetNumberIndex) % read.Size;
                int digit = read.Get(wrappedOffset);

                result += digit.ToString();
            }

            return result;
        }

        public class SignalInputEnumerable : IEnumerable<int>
        {
            private class SignalInputEnumerator : IEnumerator<int>
            {
                private int[] _digits;
                private int _repeats;
                private int _step;
                private int _currentRepeat;

                public SignalInputEnumerator(string digits, int repeats)
                {
                    _digits = new int[digits.Length];

                    for (int digitIndex = 0; digitIndex < digits.Length; digitIndex++)
                        _digits[digitIndex] = int.Parse(digits[digitIndex].ToString());

                    _repeats = repeats;

                    Reset();
                }

                public int Current
                    => _digits[_step];

                object IEnumerator.Current
                    => Current;

                public void Dispose()
                {
                }

                public bool MoveNext()
                {
                    _step++;

                    if (_step >= _digits.Length)
                    {
                        if (_currentRepeat == _repeats)
                            return false;

                        _currentRepeat++;
                        _step = 0;
                        return true;
                    }

                    return true;
                }

                public void Reset()
                {
                    _step = -1;
                    _currentRepeat = 1;
                }
            }

            private SignalInputEnumerator _enumerator;

            public SignalInputEnumerable(string digits, int repeats)
            {
                _enumerator = new SignalInputEnumerator(digits, repeats);
            }

            public void Reset()
            {
                _enumerator.Reset();
            }

            public IEnumerator<int> GetEnumerator()
                => _enumerator;

            IEnumerator IEnumerable.GetEnumerator()
                => GetEnumerator();
        }
#endif

        #region the previous stuff
#if oldstuffisenabled
        public static string Run()
        {
            var digits = InputParser.ParseAsString("input.txt");

            int phases = 100;
            //var repeatedDigits = new List<int>();

            //for (int r = 0; r < 10_000; r++)
            //{
            //    repeatedDigits.AddRange(digits);
            //}

            //digits = repeatedDigits.ToArray();

            for (int phase = phases; phases > 0; phases--)
            {
                string newDigits = "";

                for (int outputPhase = 0; outputPhase < digits.Length; outputPhase++)
                {
                    var digitsToAddTogether = new List<int>();
                    int[] outputPattern = CreateOutputPattern(outputPhase, digits.Length);

                    var interestingIndices = CreateInterestingIndices(outputPhase, digits.Length);

                    int runningTotal = 0;
                    for (int c = 0; c < digits.Length; c++)
                    {
                        int m = MathIt(outputPhase, c);
                        if (m == 0)
                        {
                            runningTotal += int.Parse(digits[c].ToString());
                        }
                        else if (m == 2)
                        {
                            runningTotal -= int.Parse(digits[c].ToString());
                        }
                    }

                    //for (int c = 0; c < interestingIndices.Length; c++)
                    //{
                    //    //if (c < outputPhase)
                    //    //{
                    //    //    continue;
                    //    //}
                    //    //else if (c > Math.Pow(outputPhase + 1, 2) + 1)
                    //    //{
                    //    //    string.Join("", new[] { 1, 2 });
                    //    //}

                    //    int interestingIndex = interestingIndices[c];

                    //    int number = int.Parse(digits[interestingIndex].ToString()) * outputPattern[interestingIndex];
                    //    digitsToAddTogether.Add(TakeOnesDigit(number));
                    //}

                    //int sum = 0;
                    //foreach (var digit in digitsToAddTogether)
                    //{
                    //    sum += digit;
                    //}

                    newDigits += Math.Abs(TakeOnesDigit(runningTotal)).ToString();
                }

                digits = newDigits;

                //if (phase % 10 == 0 && phase < 100)
                //{
                //    Console.WriteLine(" " + phase);
                //}
                //else
                //{
                //    Console.Write("*");
                //}

                //Console.WriteLine(string.Join("", digits.Take(8)));
            }

            //var lookInto = int.Parse("0" + string.Join("", digits.Take(7)));

            //return ("0" + string.Join("", digits)).Substring(lookInto, 8);

            return digits.Substring(0, 7);
        }

        private static int MathIt(int outputPhase, int playhead)
        {
            return (playhead - outputPhase) % ((outputPhase + 1) * 4);
        }

        private static int[] CreateInterestingIndices(int outputPhase, int length)
        {
            // special for output phase 0

            // 0: 0, 2, 4, 6,  8, 10, 12, 14, 16, 18 -- 4, 8   (4)
            // 1: 1, 2, 5, 6,  9, 10, 13, 14, 17, 18 -- 9, 17  (8)
            // 2: 2, 3, 4, 8,  9, 10, 14, 15, 16, 20 -- 14, 26 (12)
            // 3: 3, 4, 5, 6, 11, 12, 13, 14, 19, 20 -- 19, 35 (16)

            // (i-n) % (n+1)*4
            // (n+1)*4+n

            // n = outputPhase
            // i = playhead

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
#endif
        #endregion
    }
}
