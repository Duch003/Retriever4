﻿using System;
using System.Linq;
using System.Reflection;

namespace Retriever4.Validation
{
    public static class ObjectsValidation
    {
        public static bool CheckFieldsForNulls(object myObject, string[] ignoredFields)
        {
            if (myObject == null)
            {
                string message = $"Nie można sprawdzić pól obiektu. Argument wejściowy jest null. " +
                    $"Metoda: {nameof(CheckFieldsForNulls)}, klasa: ObjectTests.cs.";
                throw new ArgumentNullException(nameof(myObject), message);
            }
            if (ignoredFields.Any(z => z == null))
            {
                string message = $"Nie można sprawdzić pól obiektu. Jeden z obiektów tablicy wejściowej jest null. " +
                    $"Metoda: {nameof(CheckFieldsForNulls)}, klasa: ObjectTests.cs.";
                throw new ArgumentNullException(nameof(ignoredFields), message);
            }

            foreach (PropertyInfo pi in myObject.GetType().GetProperties())
            {
                if (ignoredFields.Any(z => z == pi.Name))
                    continue;
                string value = (string)pi.GetValue(myObject, null);
                if (value == null)
                {
                    return false;
                }
            }
            return true;

        }
    }
}
