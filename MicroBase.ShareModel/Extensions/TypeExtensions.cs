using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MicroBase.Share.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsCollection(this Type type)
        {
            // string implements IEnumerable, but for our purposes we don't consider it a collection.
            if (type == typeof(string)) return false;

            var interfaces = from inf in type.GetTypeInfo().GetInterfaces()
                             where inf == typeof(IEnumerable) ||
                                   (inf.GetTypeInfo().IsGenericType && inf.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                             select inf;

            return interfaces.Count() != 0;
        }

        public static bool IsNullable(this Type type)
        {
            if (type == null)
            {
                return false;
            }

            return type.GetTypeInfo().IsGenericType &&
                   type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool IsNumericType(this object o)
        {
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

        public static bool IsBooleanType(this object o)
        {
            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Boolean:
                    return true;
                default:
                    return false;
            }
        }

        public static PropertyInfo GetProperty<T>(this T obj, string name) where T : class
        {
            var fullName = obj.JsonSerialize().Replace("\"", "");
            var t = Type.GetType(fullName);
            var val = System.Reflection.TypeExtensions.GetProperty(t, name);

            return val;
        }

        public static Guid GetValueOrDefault(Guid? uuid)
        {
            if (uuid.HasValue)
            {
                return uuid.Value;
            }

            return Guid.Empty;
        }
    }
}