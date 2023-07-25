using UnityEngine;

namespace tonigames.battleship.Util
{
    public static class ArrayRandomizer
    {
        public static void RandomizeArray<T>(T[] array)
        {
            var n = array.Length;
            while (n > 1)
            {
                n--;
                var k = Random.Range(0, n + 1);
                (array[k], array[n]) = (array[n], array[k]);
            }
        }
    }
}