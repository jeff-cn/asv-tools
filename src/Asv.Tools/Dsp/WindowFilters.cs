﻿using System;

namespace Asv.Tmda.Core
{
    public enum WindowFilterEnum
    {
        None,
        Hamming,
        Hann,
        Cosine
    }

    public static class WindowFilters
    {


        public static double[] Create(WindowFilterEnum filter, int width)
        {
            switch (filter)
            {
                case WindowFilterEnum.None:
                    return None(width);
                case WindowFilterEnum.Hamming:
                    return Hamming(width);
                case WindowFilterEnum.Hann:
                    return Hann(width);
                case WindowFilterEnum.Cosine:
                    return Cosine(width);
                default:
                    throw new ArgumentOutOfRangeException(nameof(filter), filter, null);
            }
        }

        public static double[] None(int width)
        {
            var numArray = new double[width];
            for (var index = 0; index < numArray.Length; ++index)
                numArray[index] = 1;
            return numArray;
        }

        public static double[] Hamming(int width)
        {
            var num = 2.0 * Math.PI / (double)width;
            var numArray = new double[width];
            for (var index = 0; index < numArray.Length; ++index)
                numArray[index] = 0.53836 + -0.46164 * Math.Cos((double)index * num);
            return numArray;
        }
        public static double[] Hann(int width)
        {
            var num = 2.0 * Math.PI / (double)width;
            var numArray = new double[width];
            for (var index = 0; index < numArray.Length; ++index)
                numArray[index] = 0.5 - 0.5 * Math.Cos((double)index * num);
            return numArray;
        }
        public static double[] Cosine(int width)
        {
            var num = Math.PI / (double)width;
            var numArray = new double[width];
            for (var index = 0; index < numArray.Length; ++index)
                numArray[index] = Math.Sin((double)index * num);
            return numArray;
        }
    }
}