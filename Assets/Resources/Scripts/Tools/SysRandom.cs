using System;
using UnityEngine;

public static class SysRandom
{
    private static System.Random _random = new System.Random();

    public static void Seed(int seed)
    {
        _random = new System.Random(seed);
    }

    public static int Range(int min, int max)
    {
        return _random.Next(min, max);
    }

    public static float Range(float min, float max)
    {
        return (float)_random.NextDouble() * (max - min) + min;
    }

    public static float Value
    {
        get { return (float)_random.NextDouble(); }
    }

    public static int Next(int max)
    {
        return _random.Next(max);
    }

    public static Vector2 InsideUnitCircle
    {
        get
        {
            double angle = _random.NextDouble() * Math.PI * 2;
            double radius = Math.Sqrt(_random.NextDouble());
            return new Vector2((float)(Math.Cos(angle) * radius), (float)(Math.Sin(angle) * radius));
        }
    }
}
