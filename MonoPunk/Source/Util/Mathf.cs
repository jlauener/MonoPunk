using Microsoft.Xna.Framework;
using System;

namespace MonoPunk
{
    public static class Mathf
    {
        public const float Pi = (float)Math.PI;
        public const float Pi2 = Pi * 2.0f;
        public const float HalfPi = Pi * 0.5f;
        public const float Epsilon = 0.0000001f;

        public static float ToRadians(float degrees)
        {
            return degrees * Pi / 180.0f;
        }

        public static float ToDegrees(float radians)
        {
            return radians * 180.0f / Pi;
        }

        public static bool IntersectRect(float x1, float y1, float width1, float height1, float x2, float y2, float width2, float height2)
        {
            return x1 + width1 > x2 && y1 + height1 > y2 && x1 < x2 + width2 && y1 < y2 + height2;
        }
        public static bool IntersectPoint(float x, float y, float width, float height, float px, float py)
        {
            return x + width > px && y + height > py && x < px && y < py;
        }

        public static int Round(float value)
        {
            return (int)Math.Round(value);
        }

        public static float Min(float val1, float val2)
        {
            return Math.Min(val1, val2);
        }

        public static float Max(float val1, float val2)
        {
            return Math.Max(val1, val2);
        }

        public static float Clamp(float val, float min, float max)
        {
			if (min > max)
			{
				if (val > min) return min;
				if (val < max) return max;
			}
			else
			{
				if (val < min) return min;
				if (val > max) return max;
			}
            return val;
        }

        public static int Min(int val1, int val2)
        {
            return Math.Min(val1, val2);
        }

        public static int Max(int val1, int val2)
        {
            return Math.Max(val1, val2);
        }

        public static int Clamp(int val, int min, int max)
        {
            if (val < min) return min;
            if (val > max) return max;
            return val;
        }

        public static int Floor(float val)
        {
            return (int)Math.Floor(val);
        }

        public static int Ceiling(float val)
        {
            return (int)Math.Ceiling(val);
        }

        public static int Sign(float value)
        {
            return value < 0.0f ? -1 : 1;
        }

        public static float Abs(float value)
        {
            return (float)Math.Abs(value);
        }

        public static float Diff(float val1, float val2)
        {
            return (float)Math.Abs(val1 - val2);
        }

        public static void Swap(ref float a, ref float b)
        {
            var temp = a;
            a = b;
            b = temp;
        }

        public static float Cos(float value)
        {
            return (float)Math.Cos(value);
        }

        public static float Sin(float value)
        {
            return (float)Math.Sin(value);
        }

        public static float Tan(float value)
        {
            return (float)Math.Tan(value);
        }

        public static float Acos(float value)
        {
            return (float)Math.Acos(value);
        }

        public static float Asin(float value)
        {
            return (float)Math.Asin(value);
        }

        public static float Atan(float value)
        {
            return (float)Math.Atan(value);
        }

        public static float Lerp(float pct, float min, float max)
        {
            return min + pct * (max - min);
        }
    }
}
