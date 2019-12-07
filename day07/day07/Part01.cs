﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace day07
{
    public class Part01
    {
        public string Run()
        {
            var input = "input.txt";
            var lines = File.ReadAllLines(input).ToList();
            var opcodes = new List<int>();

            foreach (var line in lines)
            {
                foreach (var opcode in line.Split(new[] { "," }, StringSplitOptions.None))
                {
                    opcodes.Add(int.Parse(opcode));
                }
            }

            var strongestSignal = int.MinValue;

            var permutations = new Stack<int[]>();

            CreatePermutations(permutations);

            while (permutations.Count > 0)
            {
                var permutation = permutations.Pop();

                var amplifiers = AmplifiersSetup(opcodes);
                var amplifier = amplifiers[0];

                var signal = SendSignal(amplifier, permutation);

                if (signal > strongestSignal)
                {
                    strongestSignal = signal;
                }
            }

            return strongestSignal.ToString();
        }

        private List<Amplifier> AmplifiersSetup(List<int> opcodes)
        {
            var amplifiers = new List<Amplifier>();

            for (int i = 0; i < 5; i++)
            {
                amplifiers.Add(new Amplifier(opcodes));
                if (i > 0)
                {
                    amplifiers[i - 1].Next = amplifiers[i];
                }
            }

            return amplifiers;
        }

        private void CreatePermutations(Stack<int[]> permutations)
        {
            var permutation = new[] { 0, 1, 2, 3, 4 };

            permutations.Push(permutation);

            int run = 0;

            Action<int[]> switch34 = new Action<int[]>(p => { permutations.Push(Switch(p, 3, 4)); });
            Action<int[]> switch23 = new Action<int[]>(p => { permutations.Push(Switch(p, 2, 3)); });
            Action<int[]> switch12 = new Action<int[]>(p => { permutations.Push(Switch(p, 1, 2)); });
            Action<int[]> switch01 = new Action<int[]>(p => { permutations.Push(Switch(p, 0, 1)); });

            while (run < 4)
            {
                switch (run)
                {
                    case 0:
                    {
                        switch34(permutations.Peek());
                    }
                    break;
                    case 1:
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            switch23(permutations.Peek());
                            switch34(permutations.Peek());
                        }
                    }
                    break;
                    case 2:
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            switch12(permutations.Peek());

                            for (int j = 0; j < 2; j++)
                            {
                                switch23(permutations.Peek());
                                switch34(permutations.Peek());
                            }
                        }
                    }
                    break;
                    case 3:
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            switch01(permutations.Peek());

                            for (int j = 0; j < 3; j++)
                            {
                                switch12(permutations.Peek());

                                for (int k = 0; k < 2; k++)
                                {
                                    switch23(permutations.Peek());
                                    switch34(permutations.Peek());
                                }
                            }
                        }
                    }
                    break;
                }

                run++;
            }
        }

        private int[] Switch(int[] parameters, int indexA, int indexB)
        {
            int[] values = parameters.ToArray();
            int value = values[indexA];
            values[indexA] = values[indexB];
            values[indexB] = value;
            return values;
        }

        private int SendSignal(Amplifier amplifier, int[] phaseSettings)
        {
            var settings = new Queue<int>(phaseSettings);
            int value = 0;

            do
            {
                amplifier.Reset();
                amplifier.AddInput(settings.Dequeue());
                amplifier.AddInput(value);
                value = amplifier.Run();
                amplifier = amplifier.Next;
            } while (amplifier != null);

            return value;
        }
    }
}
