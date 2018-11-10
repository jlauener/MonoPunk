using System;
using System.Collections.Generic;

namespace MonoPunk
{
    public static class Rand
    {
        private static readonly Random _random = new Random();

        public static float NextFloat()
        {
            return (float)_random.NextDouble();
        }

        public static float NextFloat(float max)
        {
            return NextFloat() * max;
        }

        public static float NextFloat(float min, float max)
        {
            return min + NextFloat() * (max - min);
        }

        public static int NextInt(int max)
        {
            return (int)NextFloat(max);
        }

        public static int NextInt(int min, int max)
        {
            return (int)NextFloat(min, max);
        }

        public static bool NextBool()
        {
            return NextFloat() < 0.5f;
        }

        public static void Shuffle<T>(T[] array)
        {
            for(var i = 0; i < array.Length; ++i)
            {
                var other = NextInt(array.Length);
                T temp = array[i];
                array[i] = array[other];
                array[other] = temp;
            }
        }    

        public static void Shuffle<T>(List<T> list)
        {
            for(var i = 0; i < list.Count; ++i)
            {
                var other = NextInt(list.Count);
                T temp = list[i];
                list[i] = list[other];
                list[other] = temp;
            }
        }

        public static T GetRandomElement<T>(List<T> list, T defaultValue = default(T))
        {
            if(list.Count == 0)
            {
                return defaultValue;
            }

            return list[NextInt(list.Count)];
        }

        public static T GetRandomElement<T>(T[] array)
        {
            if(array.Length == 0)
            {
                return default(T);

            }
            return array[NextInt(array.Length)];
        }

        public static T RemoveRandomElement<T>(List<T> list)
        {
            if(list.Count == 0)
            {
                return default(T);
            }

            var index = NextInt(list.Count);
            var result = list[index];
            list.RemoveAt(index);
            return result;
        }       
    }
}
