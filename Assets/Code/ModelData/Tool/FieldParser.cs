using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldParser
{
    public static int IntParser(string obj)
    {
        return int.Parse(obj);
    }

    public static float FloatParser(string obj)
    {
        return float.Parse(obj);
    }

    public static long LongParser(string obj)
    {
        return long.Parse(obj);
    }

    public static double DoubleParser(string obj)
    {
        return double.Parse(obj);
    }

    public static int[] IntArrayParser(string obj)
    {
        var orignArr = obj.Split(',');

        var arr = new int[orignArr.Length];

        for(var i = 0; i < orignArr.Length; i++)
        {
            arr[i] = IntParser(orignArr[i]);
        }

        return arr;
    }

    public static int[][] IntArraysParser(string obj)
    {
        var orignArr = obj.Split(';');

        var arrs = new int[orignArr.Length][];

        for (var i = 0; i < orignArr.Length; i++)
        {
            arrs[i] = new int[orignArr[i].Length];

            for (var j = 0; j < orignArr[i].Length; j++)
            {
                arrs[i][j] = IntParser(orignArr[i]);
            }
        }

        return arrs;
    }

    public static float[] FloatArrayParser(string obj)
    {
        var orignArr = obj.Split(',');

        var arr = new float[orignArr.Length];

        for (var i = 0; i < orignArr.Length; i++)
        {
            arr[i] = FloatParser(orignArr[i]);
        }

        return arr;
    }

    public static float[][] FloatArraysParser(string obj)
    {
        var orignArr = obj.Split(';');

        var arrs = new float[orignArr.Length][];

        for (var i = 0; i < orignArr.Length; i++)
        {
            arrs[i] = new float[orignArr[i].Length];

            for (var j = 0; j < orignArr[i].Length; j++)
            {
                arrs[i][j] = FloatParser(orignArr[i]);
            }
        }

        return arrs;
    }

    public static long[] LongArrayParser(string obj)
    {
        var orignArr = obj.Split(',');

        var arr = new long[orignArr.Length];

        for (var i = 0; i < orignArr.Length; i++)
        {
            arr[i] = LongParser(orignArr[i]);
        }

        return arr;
    }

    public static long[][] LongArraysParser(string obj)
    {
        var orignArr = obj.Split(';');

        var arrs = new long[orignArr.Length][];

        for (var i = 0; i < orignArr.Length; i++)
        {
            arrs[i] = new long[orignArr[i].Length];

            for (var j = 0; j < orignArr[i].Length; j++)
            {
                arrs[i][j] = LongParser(orignArr[i]);
            }
        }

        return arrs;
    }

    public static double[] DoubleArrayParser(string obj)
    {
        var orignArr = obj.Split(',');

        var arr = new double[orignArr.Length];

        for (var i = 0; i < orignArr.Length; i++)
        {
            arr[i] = DoubleParser(orignArr[i]);
        }

        return arr;
    }

    public static double[][] DoubleArraysParser(string obj)
    {
        var orignArr = obj.Split(';');

        var arrs = new double[orignArr.Length][];

        for (var i = 0; i < orignArr.Length; i++)
        {
            arrs[i] = new double[orignArr[i].Length];

            for (var j = 0; j < orignArr[i].Length; j++)
            {
                arrs[i][j] = DoubleParser(orignArr[i]);
            }
        }

        return arrs;
    }
}
