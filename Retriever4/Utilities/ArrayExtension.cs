using System;

namespace Retriever4.Utilities
{
    public static class ArrayExtension
    {
        //TODO Użyć LinkedList
        public static T[] Expand<T>(this T[] arr)
        {
            var temp = arr;
            arr = new T[temp.Length + 1];
            for (var i = 0; i < temp.Length; i++)
            {
                arr[i] = temp[i];
            }
            return arr;
        }

        public static T[] RemoveIndexAndShrink<T>(this T[] arr, int index)
        {
            if (index >= arr.Length || index < 0)
                throw new ArgumentException("Zmienna index jest nieprawidlowa. Musi byc wieksza od 0, ale mniejsza od dlugosci tablicy wejsciowe.", "index");
            var temp = arr;
            arr = new T[temp.Length - 1];

            for (int i = 0, j = 0; i < arr.Length; i++, j++)
            {
                if (j == index)
                {
                    i--;
                    continue;
                }
                arr[i] = temp[j];
            }
            return arr;
        }
    }
}
