using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace day08
{
    class Program
    {
        private class Layer
        {
            public int Zeros { get; set; }
            public string Buffer { get; set; }
            public byte[,] Bytes { get; set; }
        }

        private enum Color : int
        {
            Black = 0,
            White = 1,
            Transparant = 2
        };

        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("input.txt");

            var image = lines[0];

            Size imageSize = new Size(25, 6);
            var layers = new List<Layer>();

            

            int numberofzeros = 0;

            for (int i = 0; i < image.Length; i+= imageSize.Width * imageSize.Height)
            {
                string buffer = image.Substring(i, imageSize.Width * imageSize.Height);
                byte[,] bytes = new byte[imageSize.Height, imageSize.Width];

                int zeros = 0;
                Layer layer = new Layer();

                for (int y = 0; y < imageSize.Height; y++)
                {
                    for (int x = 0; x < imageSize.Width; x++)
                    {
                        var data = byte.Parse(buffer[y * imageSize.Width + x].ToString());
                        bytes[y, x] = data;

                        if (data == 0)
                        {
                            zeros++;
                        }
                    }
                }

                layer.Buffer = buffer;
                layer.Zeros = zeros;
                layer.Bytes = bytes;

                layers.Add(layer);
            }

            var layerMax = layers.Min(l => l.Zeros);
            var zeroLayer = layers.Where(l => l.Zeros == layerMax).Single();

            int n1 = 0;
            int n2 = 0;

            for (int b = 0; b < zeroLayer.Buffer.Length; b++)
            {
                if (zeroLayer.Buffer[b] == '1')
                {
                    n1++;
                }
                else if ( zeroLayer.Buffer[b] == '2')
                {
                    n2++;
                }
            }

            Console.Clear();

            for (int l = layers.Count - 1; l >= 0; l--)
            {
                var layer = layers[l];

                for (int pixel = 0; pixel < imageSize.Width * imageSize.Height; pixel++)
                {
                    Console.SetCursorPosition(pixel % imageSize.Width, pixel / imageSize.Width);
                    
                    if (layer.Buffer[pixel] == '0')
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("X");
                    }
                    else if (layer.Buffer[pixel] == '1')
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write("X");
                    }
                    
                }
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(0, 8);
            Console.WriteLine(n1 * n2);

            Console.ReadKey(true);
        }
    }
}
