using System;
using System.Linq;
using System.Text.RegularExpressions;
using Retriever4.Utilities;

namespace Retriever4.Validation
{
    public static class StringValidation
    {
        /// <summary>
        /// Compare two string with each other.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>True if first string contains second or second contains first.</returns>
        public static bool CompareStrings(string left, string right)
        {
            //Validation
            if (string.IsNullOrEmpty(left))
            {
                var message = $"Parametr jest null lub jest pusty. Metoda: {nameof(CompareStrings)}, klasa: StringValidation.cs.";
                throw new ArgumentException(message, nameof(left));
            }
            if (string.IsNullOrEmpty(right))
            {
                var message = $"Parametr jest null lub jest pusty. Metoda: {nameof(CompareStrings)}, klasa: StringValidation.cs.";
                throw new ArgumentException(message, nameof(right));
            }
            return left.Contains(right) || right.Contains(left);
        }

        /// <summary>
        /// Compare method especially for cpu's.
        /// </summary>
        /// <param name="dbCpu">Raw value from database.</param>
        /// <param name="realCpu">Raw value from computer.</param>
        /// <returns>True if realCPU contains dbCPU.</returns>
        public static bool CompareCpu(string dbCpu, string realCpu)
        {
            //Validation
            if (string.IsNullOrEmpty(dbCpu))
            {
                var message = $"Parametr jest null lub jest pusty. Metoda: {nameof(CompareCpu)}, klasa: StringValidation.cs.";
                throw new ArgumentException(message, nameof(dbCpu));
            }
            if (string.IsNullOrEmpty(realCpu))
            {
                var message = $"Parametr jest null lub jest pusty. Metoda: {nameof(CompareCpu)}, klasa: StringValidation.cs.";
                throw new ArgumentException(message, nameof(realCpu));
            }

            //Convert all letters to lower and split string on whitespaces
            var patterns = dbCpu.ToLower().Split(' ');

            //Linq query that realise AND logic. If realCPU does NOT contains all patterns, then false.
            return patterns.Any(z => realCpu.ToLower().Contains(z));
        }

        /// <summary>
        /// Compare method especially for mainboard models. Autosplit into pattenrs array on semicolon.
        /// </summary>
        /// <param name="dbModel">Raw value form database.</param>
        /// <param name="realModel">Raw value from computer.</param>
        /// <returns>True if realModel contains dbModel patterns.</returns>
        public static bool CompareMainboardModel(string dbModel, string realModel)
        {
            //Validation
            if (string.IsNullOrEmpty(dbModel))
            {
                var message = $"Parametr jest null lub jest pusty. Metoda: {nameof(CompareMainboardModel)}, klasa: StringValidation.cs.";
                throw new ArgumentException(message, nameof(dbModel));
            }
            if (string.IsNullOrEmpty(realModel))
            {
                var message = $"Parametr jest null lub jest pusty. Metoda: {nameof(CompareMainboardModel)}, klasa: StringValidation.cs.";
                throw new ArgumentException(message, nameof(realModel));
            }

            //TODO Czy da radę zmienić mały x na gwiazdkę?
            var patterns = dbModel.Split(';');
            var matched = true;
            for(var i = 0; i < patterns.Length; i++)
            {
                patterns[i] = patterns[i].RemoveSymbols().Replace("*", @"\w?").ToLower();
                var match = Regex.Match(realModel.ToLower(), patterns[i]);
                matched = matched && match.Success;
            }
            return matched; 
        }
    }
}
