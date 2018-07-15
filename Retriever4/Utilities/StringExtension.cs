using System;
using System.Collections.Generic;
using System.Linq;

namespace Retriever4.Utilities
{
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

        public static string JoinArray(this string[] arr)
        {
            string merged = "";
            for (int i = 0; i < arr.Length; i++)
                merged += arr[i] + " ";
            return merged;
        }

        public static string PadBoth(this string str, int length)
        {
            int spaces = length - str.Length;
            int padLeft = spaces / 2 + str.Length;
            return str.PadLeft(padLeft).PadRight(length, ' ');
        }
    }
}
