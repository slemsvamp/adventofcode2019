using System;
using System.Collections.Generic;
using System.Text;

namespace day15
{
    public class Randomizer
    {
        public static bool ShouldAttemptToReinitialize = true;

        private int _depth, _quantityBuffered, _quantityBufferedDepth, _count;
        private int? _seed;
        private ulong _max;
        private byte[] _byteBuffer;
        private Random _baseRandom;

        public Randomizer(int quantityBuffered = 1000, int depth = 4, int? seed = null)
        {
            _depth = depth < 1 ? 1 : depth > 8 ? 8 : depth;
            _quantityBuffered = quantityBuffered;
            _quantityBufferedDepth = quantityBuffered * depth;
            _count = 0;
            _byteBuffer = new byte[_quantityBufferedDepth];
            _seed = seed;

            if (seed.HasValue)
            {
                _baseRandom = new Random(seed.Value);
            }
            else
            {
                _baseRandom = new Random();
            }

            _baseRandom.NextBytes(_byteBuffer);

            ulong newMax = 0;
            for (int i = 0; i < this._depth; i++)
                newMax += (ulong)0xff << (8 * i);
            _max = newMax;
        }

        public void Reinitialize(int pQuantityBuffered, int pDepth)
        {
            _depth = pDepth;
            _quantityBuffered = pQuantityBuffered;
            _quantityBufferedDepth = pQuantityBuffered * pDepth;
            _count = 0;
            _byteBuffer = new byte[_quantityBufferedDepth];
            _baseRandom.NextBytes(_byteBuffer);

            ulong newMax = 0;
            for (int i = 0; i < _depth; i++)
                newMax += (ulong)0xff << (8 * i);
            _max = newMax;
        }

        public float GetNext()
        {
            byte[] bytes = GetBytes();

            ulong value = 0, max = 0;

            for (int count = 0; count < _depth; count++)
            {
                int leftshift = 8 * count;
                value += (ulong)bytes[count] << leftshift;
                max += (ulong)0xff << leftshift;
            }

            return (float)value / max;
        }

        public bool FlipCoin()
        {
            return GetNext(1) == 1;
        }

        public int GetNext(int pMax)
        {
            if (Randomizer.ShouldAttemptToReinitialize && (ulong)pMax > _max && _depth < 8)
                Reinitialize(8, _quantityBuffered);

            return GetNext(0, pMax);
        }

        public int GetNext(int pMin, int pMax)
        {
            if (Randomizer.ShouldAttemptToReinitialize && (ulong)pMax > _max && _depth < 8)
                Reinitialize(8, _quantityBuffered);

            int num = pMin + (int)(GetNext() * (pMax - pMin + 1));

            return num < pMin ? pMin : num > pMax ? pMax : num;
        }

        public byte[] NextBytes(byte[] pBytes)
        {
            _baseRandom.NextBytes(pBytes);
            return pBytes;
        }

        private byte[] GetBytes()
        {
            byte[] bytes = new byte[_depth];

            int startingpoint = _count * _depth;

            if (startingpoint + _depth > _byteBuffer.Length - 1)
                RefreshBuffer();

            for (int i = 0; i < _depth; i++)
                bytes[i] = _byteBuffer[startingpoint + i];

            _count++;

            return bytes;
        }

        private void RefreshBuffer()
        {
            _baseRandom.NextBytes(_byteBuffer);
            _count = 0;
        }
    }
}
