using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Retriever4
{
    public static class ExpandArr
    {
        public static T[] Expand<T>(T[] arr)
        {
            var temp = arr;
            arr = new T[temp.Length + 1];
            for (var i = 0; i < temp.Length; i++)
            {
                arr[i] = temp[i];
            }
            return arr;
        }

        public static T[] RemoveIndexAndShrink<T>(T[] arr, int index)
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
    public static class StringExtension
    {
        public static IEnumerable<String> SplitInParts(this String s, int partLength)
        {
            if (s == null)
                throw new ArgumentNullException("Wartość wejściowa jest null");
            if (partLength <= 0)
                throw new ArgumentException("Zmienna partLength musi być nieujemna", "partLength");

            for (var i = 0; i < s.Length; i += partLength)
                yield return s.Substring(i, Math.Min(partLength, s.Length - i));
        }

        public static string RemoveLetters(this String s)
        {
            if (s == null)
                throw new ArgumentNullException("Wartość wejściowa jest null");

            for (int i = 0; i < s.Length; i++)
                if (char.IsLetter(s[i]) && i >= 0)
                {
                    s = s.Remove(i, 1);
                    i--;
                }
            return s;
        }

        public static string RemoveSymbols(this String s, char[] except = null)
        {
            if (s == null)
                throw new ArgumentNullException("Wartość wejściowa jest null");
            if (except == null)
                except = new char[0];
            for (int i = 0; i < s.Length; i++)
                if ((char.IsSymbol(s[i]) || char.IsPunctuation(s[i])) && i >= 0 && !except.Any(z => z == s[i]))
                {
                    s = s.Remove(i, 1);
                    i--;
                }
            return s;
        }

        public static string RemoveWhiteSpaces(this String s)
        {
            if (s == null)
                throw new ArgumentNullException("Wartość wejściowa jest null");

            for (int i = 0; i < s.Length; i++)
                if (char.IsWhiteSpace(s[i]) && i >= 0)
                {
                    s = s.Remove(i, 1);
                    i--;
                }
            return s;
        }
    }
}

