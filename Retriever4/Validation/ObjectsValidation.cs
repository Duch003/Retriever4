using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Retriever4.Validation
{
    public static class ObjectsValidation
    {
        /// <summary>
        /// Check object fields for nulls. Auto ignore static fields;
        /// </summary>
        /// <param name="myObject">Object to check.</param>
        /// <param name="ignoredFields">Additional fields names which will be ignored.</param>
        /// <returns>False if object contains nulls, true if clear.</returns>
        public static bool CheckFieldsForNulls(object myObject, string[] ignoredFields)
        {
            if (myObject == null)
            {
                var message = $"Nie można sprawdzić pól obiektu. Argument wejściowy jest null. " +
                    $"Metoda: {nameof(CheckFieldsForNulls)}, klasa: ObjectTests.cs.";
                throw new ArgumentNullException(nameof(myObject), message);
            }

            if (ignoredFields == null)
                ignoredFields = new string[0];

            if (ignoredFields.Any(z => z == null))
            {
                var message = $"Nie można sprawdzić pól obiektu. Jeden z obiektów tablicy wejściowej jest null. " +
                    $"Metoda: {nameof(CheckFieldsForNulls)}, klasa: ObjectTests.cs.";
                throw new ArgumentNullException(nameof(ignoredFields), message);
            }

            foreach (var fi in myObject.GetType().GetFields())
            {
                if (ignoredFields.Any(z => z == fi.Name) || fi.IsStatic)
                    continue;
                var value = fi.GetValue(myObject) == null ? null : fi.GetValue(myObject).ToString();
                if (value == null)
                {
                    return false;
                }
            }
            return true;

        }

        /// <summary>
        /// Check object fields for negative numbers. Auto ignore static fields;
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="ignoredFields"></param>
        /// <returns>False is there are negative assigned negative numbers. True if not.</returns>
        public static bool CheckFieldsForNegativeNumbers(object myObject, string[] ignoredFields)
        {
            if (myObject == null)
            {
                var message = $"Nie można sprawdzić pól obiektu. Argument wejściowy jest null. " +
                    $"Metoda: {nameof(CheckFieldsForNegativeNumbers)}, klasa: ObjectTests.cs.";
                throw new ArgumentNullException(nameof(myObject), message);
            }

            if (ignoredFields == null)
                ignoredFields = new string[0];

            if (ignoredFields.Any(z => z == null))
            {
                var message = $"Nie można sprawdzić pól obiektu. Jeden z obiektów tablicy wejściowej jest null. " +
                    $"Metoda: {nameof(CheckFieldsForNegativeNumbers)}, klasa: ObjectTests.cs.";
                throw new ArgumentNullException(nameof(ignoredFields), message);
            }

            foreach (var fi in myObject.GetType().GetFields())
            {
                if (ignoredFields.Any(z => z == fi.Name) || fi.IsStatic)
                    continue;
                var value = fi.GetValue(myObject) == null ? null : fi.GetValue(myObject).ToString();
                if (int.Parse(value) < 0)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Gather evey field without assigned value. Autoignore static fields.
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="ignoredFields"></param>
        /// <returns>List of fields names.</returns>
        public static List<string> TakeFieldsWithNulls(object myObject, string[] ignoredFields)
        {
            if (myObject == null)
            {
                var message = $"Nie można sprawdzić pól obiektu. Argument wejściowy jest null. " +
                    $"Metoda: {nameof(TakeFieldsWithNulls)}, klasa: ObjectTests.cs.";
                throw new ArgumentNullException(nameof(myObject), message);
            }

            if (ignoredFields == null)
                ignoredFields = new string[0];

            if (ignoredFields.Any(z => z == null))
            {
                var message = $"Nie można sprawdzić pól obiektu. Jeden z obiektów tablicy wejściowej jest null. " +
                    $"Metoda: {nameof(TakeFieldsWithNulls)}, klasa: ObjectTests.cs.";
                throw new ArgumentNullException(nameof(ignoredFields), message);
            }

            List<string> nullFields = new List<string>();
            foreach (var fi in myObject.GetType().GetFields())
            {
                if (ignoredFields.Any(z => z == fi.Name) || fi.IsStatic)
                    continue;
                var value = fi.GetValue(myObject) == null ? null : fi.GetValue(myObject).ToString();
                if (value == null)
                {
                    nullFields.Add(fi.Name);
                }
            }
            return nullFields;
        }

        /// <summary>
        /// Gather evey field with negative value. Autoignore static fields.
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="ignoredFields"></param>
        /// <returns>Dictionary with field names as keys, and field values as </returns>
        public static Dictionary<string, dynamic> TakeFieldsWithNegativeNumbers(object myObject, string[] ignoredFields)
        {
            if (myObject == null)
            {
                var message = $"Nie można sprawdzić pól obiektu. Argument wejściowy jest null. " +
                    $"Metoda: {nameof(TakeFieldsWithNulls)}, klasa: ObjectTests.cs.";
                throw new ArgumentNullException(nameof(myObject), message);
            }

            if (ignoredFields == null)
                ignoredFields = new string[0];

            if (ignoredFields.Any(z => z == null))
            {
                var message = $"Nie można sprawdzić pól obiektu. Jeden z obiektów tablicy wejściowej jest null. " +
                    $"Metoda: {nameof(TakeFieldsWithNulls)}, klasa: ObjectTests.cs.";
                throw new ArgumentNullException(nameof(ignoredFields), message);
            }

            Dictionary<string, dynamic> negativeNumbers = new Dictionary<string, dynamic>();
            foreach (var fi in myObject.GetType().GetFields())
            {
                if (ignoredFields.Any(z => z == fi.Name) || fi.IsStatic)
                    continue;
                if (fi.IsNumericType() && fi.IsNullable() && (double)fi.GetValue(myObject) < 0)
                {
                    negativeNumbers.Add(fi.Name, fi.GetValue(myObject));
                }
            }
            return negativeNumbers;
        }
    }
}
