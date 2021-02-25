﻿// Exocortex.DSP.Fourier
using System;
using Exocortex.DSP;

public class Fourier
{
    private const int cMaxLength = 4096;

    private const int cMinLength = 1;

    private const int cMaxBits = 12;

    private const int cMinBits = 0;

    private static int[][] _reversedBits = new int[12][];

    private static int[][] _reverseBits = null;

    private static int _lookupTabletLength = -1;

    private static double[,][] _uRLookup = null;

    private static double[,][] _uILookup = null;

    private static float[,][] _uRLookupF = null;

    private static float[,][] _uILookupF = null;

    private static bool _bufferFLocked = false;

    private static float[] _bufferF = new float[0];

    private static bool _bufferCFLocked = false;

    private static bool _bufferCLocked = false;

    private Fourier()
    {
    }

    private static void Swap(ref float a, ref float b)
    {
        float num = a;
        a = b;
        b = num;
    }

    private static void Swap(ref double a, ref double b)
    {
        double num = a;
        a = b;
        b = num;
    }

    public static bool IsPowerOf2(int x)
    {
        return (x & (x - 1)) == 0;
    }

    private static int Pow2(int exponent)
    {
        if (exponent >= 0 && exponent < 31)
        {
            return 1 << exponent;
        }
        return 0;
    }

    private static int Log2(int x)
    {
        if (x <= 65536)
        {
            if (x <= 256)
            {
                if (x <= 16)
                {
                    if (x <= 4)
                    {
                        if (x <= 2)
                        {
                            if (x <= 1)
                            {
                                return 0;
                            }
                            return 1;
                        }
                        return 2;
                    }
                    if (x <= 8)
                    {
                        return 3;
                    }
                    return 4;
                }
                if (x <= 64)
                {
                    if (x <= 32)
                    {
                        return 5;
                    }
                    return 6;
                }
                if (x <= 128)
                {
                    return 7;
                }
                return 8;
            }
            if (x <= 4096)
            {
                if (x <= 1024)
                {
                    if (x <= 512)
                    {
                        return 9;
                    }
                    return 10;
                }
                if (x <= 2048)
                {
                    return 11;
                }
                return 12;
            }
            if (x <= 16384)
            {
                if (x <= 8192)
                {
                    return 13;
                }
                return 14;
            }
            if (x <= 32768)
            {
                return 15;
            }
            return 16;
        }
        if (x <= 16777216)
        {
            if (x <= 1048576)
            {
                if (x <= 262144)
                {
                    if (x <= 131072)
                    {
                        return 17;
                    }
                    return 18;
                }
                if (x <= 524288)
                {
                    return 19;
                }
                return 20;
            }
            if (x <= 4194304)
            {
                if (x <= 2097152)
                {
                    return 21;
                }
                return 22;
            }
            if (x <= 8388608)
            {
                return 23;
            }
            return 24;
        }
        if (x <= 268435456)
        {
            if (x <= 67108864)
            {
                if (x <= 33554432)
                {
                    return 25;
                }
                return 26;
            }
            if (x <= 134217728)
            {
                return 27;
            }
            return 28;
        }
        if (x <= 1073741824)
        {
            if (x <= 536870912)
            {
                return 29;
            }
            return 30;
        }
        return 31;
    }

    private static int ReverseBits(int index, int numberOfBits)
    {
        int num = 0;
        for (int i = 0; i < numberOfBits; i++)
        {
            num = (num << 1) | (index & 1);
            index >>= 1;
        }
        return num;
    }

    private static int[] GetReversedBits(int numberOfBits)
    {
        if (_reversedBits[numberOfBits - 1] == null)
        {
            int num = Pow2(numberOfBits);
            int[] array = new int[num];
            for (int i = 0; i < num; i++)
            {
                int num2 = i;
                int num3 = 0;
                for (int j = 0; j < numberOfBits; j++)
                {
                    num3 = (num3 << 1) | (num2 & 1);
                    num2 >>= 1;
                }
                array[i] = num3;
            }
            _reversedBits[numberOfBits - 1] = array;
        }
        return _reversedBits[numberOfBits - 1];
    }

    private static void ReorderArray(float[] data)
    {
        int num = data.Length / 2;
        int[] reversedBits = GetReversedBits(Log2(num));
        for (int i = 0; i < num; i++)
        {
            int num2 = reversedBits[i];
            if (num2 > i)
            {
                Swap(ref data[i << 1], ref data[num2 << 1]);
                Swap(ref data[(i << 1) + 1], ref data[(num2 << 1) + 1]);
            }
        }
    }

    private static void ReorderArray(double[] data)
    {
        int num = data.Length / 2;
        int[] reversedBits = GetReversedBits(Log2(num));
        for (int i = 0; i < num; i++)
        {
            int num2 = reversedBits[i];
            if (num2 > i)
            {
                Swap(ref data[i << 1], ref data[num2 << 1]);
                Swap(ref data[i << 2], ref data[num2 << 2]);
            }
        }
    }

    private static int _ReverseBits(int bits, int n)
    {
        int num = 0;
        for (int i = 0; i < n; i++)
        {
            num = (num << 1) | (bits & 1);
            bits >>= 1;
        }
        return num;
    }

    private static void InitializeReverseBits(int levels)
    {
        _reverseBits = new int[levels + 1][];
        for (int i = 0; i < levels + 1; i++)
        {
            int num = (int)Math.Pow(2.0, i);
            _reverseBits[i] = new int[num];
            for (int j = 0; j < num; j++)
            {
                _reverseBits[i][j] = _ReverseBits(j, i);
            }
        }
    }

    private static void SyncLookupTableLength(int length)
    {
        if (length > _lookupTabletLength)
        {
            int levels = (int)Math.Ceiling(Math.Log(length, 2.0));
            InitializeReverseBits(levels);
            InitializeComplexRotations(levels);
            _lookupTabletLength = length;
        }
    }

    private static int GetLookupTableLength()
    {
        return _lookupTabletLength;
    }

    private static void ClearLookupTables()
    {
        _uRLookup = null;
        _uILookup = null;
        _uRLookupF = null;
        _uILookupF = null;
        _lookupTabletLength = -1;
    }

    private static void InitializeComplexRotations(int levels)
    {
        _uRLookup = new double[levels + 1, 2][];
        _uILookup = new double[levels + 1, 2][];
        _uRLookupF = new float[levels + 1, 2][];
        _uILookupF = new float[levels + 1, 2][];
        int num = 1;
        for (int i = 1; i <= levels; i++)
        {
            int num2 = num;
            num <<= 1;
            double num3 = 1.0;
            double num4 = 0.0;
            double num5 = Math.PI / (double)num2 * 1.0;
            double num6 = Math.Cos(num5);
            double num7 = Math.Sin(num5);
            _uRLookup[i, 0] = new double[num2];
            _uILookup[i, 0] = new double[num2];
            _uRLookupF[i, 0] = new float[num2];
            _uILookupF[i, 0] = new float[num2];
            for (int j = 0; j < num2; j++)
            {
                _uRLookupF[i, 0][j] = (float)(_uRLookup[i, 0][j] = num3);
                _uILookupF[i, 0][j] = (float)(_uILookup[i, 0][j] = num4);
                double num8 = num3 * num7 + num4 * num6;
                num3 = num3 * num6 - num4 * num7;
                num4 = num8;
            }
            double num9 = 1.0;
            double num10 = 0.0;
            double num11 = Math.PI / (double)num2 * -1.0;
            double num12 = Math.Cos(num11);
            double num13 = Math.Sin(num11);
            _uRLookup[i, 1] = new double[num2];
            _uILookup[i, 1] = new double[num2];
            _uRLookupF[i, 1] = new float[num2];
            _uILookupF[i, 1] = new float[num2];
            for (int k = 0; k < num2; k++)
            {
                _uRLookupF[i, 1][k] = (float)(_uRLookup[i, 1][k] = num9);
                _uILookupF[i, 1][k] = (float)(_uILookup[i, 1][k] = num10);
                double num14 = num9 * num13 + num10 * num12;
                num9 = num9 * num12 - num10 * num13;
                num10 = num14;
            }
        }
    }

    private static void LockBufferF(int length, ref float[] buffer)
    {
        _bufferFLocked = true;
        if (length >= _bufferF.Length)
        {
            _bufferF = new float[length];
        }
        buffer = _bufferF;
    }

    private static void UnlockBufferF(ref float[] buffer)
    {
        _bufferFLocked = false;
        buffer = null;
    }

    private static void LinearFFT(float[] data, int start, int inc, int length, FourierDirection direction)
    {
        float[] buffer = null;
        LockBufferF(length * 2, ref buffer);
        int num = start;
        for (int i = 0; i < length * 2; i++)
        {
            buffer[i] = data[num];
            num += inc;
        }
        FFT(buffer, length, direction);
        num = start;
        for (int j = 0; j < length; j++)
        {
            data[num] = buffer[j];
            num += inc;
        }
        UnlockBufferF(ref buffer);
    }

    private static void LinearFFT_Quick(float[] data, int start, int inc, int length, FourierDirection direction)
    {
        float[] buffer = null;
        LockBufferF(length * 2, ref buffer);
        int num = start;
        for (int i = 0; i < length * 2; i++)
        {
            buffer[i] = data[num];
            num += inc;
        }
        FFT_Quick(buffer, length, direction);
        num = start;
        for (int j = 0; j < length; j++)
        {
            data[num] = buffer[j];
            num += inc;
        }
        UnlockBufferF(ref buffer);
    }

    /// <summary>
    /// 傅里叶转换
    /// </summary>
    /// <param name="data"></param>
    /// <param name="length"></param>
    /// <param name="direction"></param>
    public static void FFT(float[] data, int length, FourierDirection direction)
    {
        SyncLookupTableLength(length);
        int num = Log2(length);
        ReorderArray(data);
        int num2 = 1;
        int num3 = ((direction != FourierDirection.Forward) ? 1 : 0);
        for (int i = 1; i <= num; i++)
        {
            int num4 = num2;
            num2 <<= 1;
            float[] array = _uRLookupF[i, num3];
            float[] array2 = _uILookupF[i, num3];
            for (int j = 0; j < num4; j++)
            {
                float num5 = array[j];
                float num6 = array2[j];
                for (int k = j; k < length; k += num2)
                {
                    int num7 = k << 1;
                    int num8 = k + num4 << 1;
                    float num9 = data[num8];
                    float num10 = data[num8 + 1];
                    float num11 = num9 * num5 - num10 * num6;
                    float num12 = num9 * num6 + num10 * num5;
                    num9 = data[num7];
                    num10 = data[num7 + 1];
                    data[num7] = num9 + num11;
                    data[num7 + 1] = num10 + num12;
                    data[num8] = num9 - num11;
                    data[num8 + 1] = num10 - num12;
                }
            }
        }
    }

    public static void FFT_Quick(float[] data, int length, FourierDirection direction)
    {
        int num = Log2(length);
        ReorderArray(data);
        int num2 = 1;
        int num3 = ((direction != FourierDirection.Forward) ? 1 : 0);
        for (int i = 1; i <= num; i++)
        {
            int num4 = num2;
            num2 <<= 1;
            float[] array = _uRLookupF[i, num3];
            float[] array2 = _uILookupF[i, num3];
            for (int j = 0; j < num4; j++)
            {
                float num5 = array[j];
                float num6 = array2[j];
                for (int k = j; k < length; k += num2)
                {
                    int num7 = k << 1;
                    int num8 = k + num4 << 1;
                    float num9 = data[num8];
                    float num10 = data[num8 + 1];
                    float num11 = num9 * num5 - num10 * num6;
                    float num12 = num9 * num6 + num10 * num5;
                    num9 = data[num7];
                    num10 = data[num7 + 1];
                    data[num7] = num9 + num11;
                    data[num7 + 1] = num10 + num12;
                    data[num8] = num9 - num11;
                    data[num8 + 1] = num10 - num12;
                }
            }
        }
    }

    public static void RFFT(float[] data, FourierDirection direction)
    {
        if (data == null)
        {
            throw new ArgumentNullException("data");
        }
        RFFT(data, data.Length, direction);
    }

    public static void RFFT(float[] data, int length, FourierDirection direction)
    {
        if (data == null)
        {
            throw new ArgumentNullException("data");
        }
        if (data.Length < length)
        {
            throw new ArgumentOutOfRangeException("length", length, "must be at least as large as 'data.Length' parameter");
        }
        if (!IsPowerOf2(length))
        {
            throw new ArgumentOutOfRangeException("length", length, "must be a power of 2");
        }
        float num = 0.5f;
        float num2 = (float)Math.PI / (float)(length / 2);
        float num3;
        if (direction == FourierDirection.Forward)
        {
            num3 = -0.5f;
            FFT(data, length / 2, direction);
        }
        else
        {
            num3 = 0.5f;
            num2 = 0f - num2;
        }
        float num4 = (float)Math.Sin(0.5 * (double)num2);
        float num5 = -2f * num4 * num4;
        float num6 = (float)Math.Sin(num2);
        float num7 = 1f + num5;
        float num8 = num6;
        for (int i = 1; i < length / 4; i++)
        {
            int num9 = 2 * i;
            int num10 = length - 2 * i;
            float num11 = num * (data[num9] + data[num10]);
            float num12 = num * (data[num9 + 1] - data[num10 + 1]);
            float num13 = (0f - num3) * (data[num9 + 1] + data[num10 + 1]);
            float num14 = num3 * (data[num9] - data[num10]);
            data[num9] = num11 + num7 * num13 - num8 * num14;
            data[num9 + 1] = num12 + num7 * num14 + num8 * num13;
            data[num10] = num11 - num7 * num13 + num8 * num14;
            data[num10 + 1] = 0f - num12 + num7 * num14 + num8 * num13;
            num7 = (num4 = num7) * num5 - num8 * num6 + num7;
            num8 = num8 * num5 + num4 * num6 + num8;
        }
        if (direction == FourierDirection.Forward)
        {
            float num15 = data[0];
            data[0] = num15 + data[1];
            data[1] = num15 - data[1];
        }
        else
        {
            float num16 = data[0];
            data[0] = num * (num16 + data[1]);
            data[1] = num * (num16 - data[1]);
            FFT(data, length / 2, direction);
        }
    }

    public static void FFT2(float[] data, int xLength, int yLength, FourierDirection direction)
    {
        if (data == null)
        {
            throw new ArgumentNullException("data");
        }
        if (data.Length < xLength * yLength * 2)
        {
            throw new ArgumentOutOfRangeException("data.Length", data.Length, "must be at least as large as 'xLength * yLength * 2' parameter");
        }
        if (!IsPowerOf2(xLength))
        {
            throw new ArgumentOutOfRangeException("xLength", xLength, "must be a power of 2");
        }
        if (!IsPowerOf2(yLength))
        {
            throw new ArgumentOutOfRangeException("yLength", yLength, "must be a power of 2");
        }
        int num = 1;
        if (xLength > 1)
        {
            SyncLookupTableLength(xLength);
            for (int i = 0; i < yLength; i++)
            {
                int start = i * xLength;
                LinearFFT_Quick(data, start, num, xLength, direction);
            }
        }
        if (yLength > 1)
        {
            SyncLookupTableLength(yLength);
            for (int j = 0; j < xLength; j++)
            {
                int start2 = j * num;
                LinearFFT_Quick(data, start2, xLength, yLength, direction);
            }
        }
    }
}
