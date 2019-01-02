using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace MarkdownMaker.Extensions
{
    public static class ReflectionExtensions
    {
        public static bool IsNumeric(this Type type)
        {
            return type.IsWholeNumber() || type.IsFloatingPointNumber();
        }

        public static bool IsFloatingPointNumber(this Type type)
        {
            if (type == null)
            {
                return false;
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                case TypeCode.Object:
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return Nullable.GetUnderlyingType(type).IsFloatingPointNumber();
                    }
                    return false;
            }
            return false;
        }

        public static bool IsWholeNumber(this Type type)
        {
            if (type == null)
            {
                return false;
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                case TypeCode.Object:
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return Nullable.GetUnderlyingType(type).IsWholeNumber();
                    }
                    return false;
            }
            return false;
        }

        public static bool IsEnumerable(this Type type)
        {
            var isGenericEnumerable = typeof(IEnumerable<>).IsAssignableFrom(type);
            var legacyEnumerable = typeof(IEnumerable).IsAssignableFrom(type);

            return isGenericEnumerable ||
                   legacyEnumerable;
        }
    }
}