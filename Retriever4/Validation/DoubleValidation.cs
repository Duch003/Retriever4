using Retriever4.Utilities;
using System;

namespace Retriever4.Validation
{
    public static class DoubleValidation
    {
        /// <summary>
        /// Check if realValue is lower or equal dbValue.
        /// </summary>
        /// <param name="staticValue">Static wearlevel.</param>
        /// <param name="realValue">Battery real wearlevel.</param>
        /// <returns>True if wear level is below or eqal acceptable value.</returns>
        public static bool CompareWearLevel(double staticValue, double realValue)
        {
            return staticValue >= realValue;
        }

        /// <summary>
        /// Check if is realRawValue[B] eqals dbRawValue[GB] with two-way 14% tolerance.
        /// </summary>
        /// <param name="dbValue">Raw string from database.</param>
        /// <param name="realRawValue">Raw value from computer in B.</param>
        /// <returns>True if realValue eqals dbValue with two-way 14% tolerance.</returns>
        public static bool CompareStorages(string dbRawValue, double realRawValue)
        {
            if(string.IsNullOrEmpty(dbRawValue))
            {
                var message = $"Podano pusty string. Metoda: {nameof(CompareStorages)}, klasa: DoubleValidation.cs.";
                throw new ArgumentException(message, nameof(dbRawValue));
            }
            if(realRawValue < 0)
            {
                var message = $"Rzeczywsita pojemność dysku nie może być mniejsza od zera. Metoda: {nameof(CompareStorages)}, klasa: DoubleValidation.cs.";
                throw new ArgumentException(message, nameof(realRawValue));
            }

            //Converting realValue [B] to [GB]
            double gigabyte = 1000000000;//1073741824;//[B]
            var realValue = realRawValue / gigabyte;
            //Checking dbValue unit (true if [TB])
            var tb = dbRawValue.ToLower().Contains("tb");
            //Remove unwanted chars from dbRawValue, only numbers stay
            dbRawValue = dbRawValue.RemoveLetters().Replace(',', '.').RemoveWhiteSpaces();
            //Attepmt to parsing
            if(!double.TryParse(dbRawValue, out var dbValue))
            {
                var message = $"Nie można przekonwertować ciągu <<{dbRawValue}>> na double. Metoda: {nameof(CompareStorages)}, klasa: DoubleValidation.cs.";
                throw new Exception(message);
            }
            //Unit correct
            if (tb)
            {
                dbValue *= 1000;
            }
            //Calculate bounds
            var upperEdge = dbValue * 1.14;
            var lowerEdge = dbValue * 0.86;
            //Return result
            return (upperEdge >= realValue) && (lowerEdge <= realValue);  
        }

        /// <summary>
        /// Check if are numbers equals.
        /// </summary>
        /// <param name="a">First number.</param>
        /// <param name="b">Second number.</param>
        /// <returns>True if equals.</returns>
        public static bool AreNumbersEquals(double a, double b)
        {
            return a == b;
        }
    }
}
