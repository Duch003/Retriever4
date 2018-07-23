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
    }
}
