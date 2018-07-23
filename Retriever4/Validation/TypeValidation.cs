using System;

namespace Retriever4.Validation
{
    public static class TypeValidation
    {
        public static bool IsNumericType(this object o)
        {
            if (o == null)
            {
                string message = $"Parametr wejściowy jest null. Metoda: {nameof(IsNumericType)}, klasa: TypeValidation.cs.";
                throw new ArgumentNullException(nameof(o), message);
            }
            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsNullable<T>(this T obj)
        {
            if (obj == null) return true; // obvious
            Type type = typeof(T);
            if (!type.IsValueType) return true; // ref-type
            if (Nullable.GetUnderlyingType(type) != null) return true; // Nullable<T>
            return false; // value-type
        }
    }
}
