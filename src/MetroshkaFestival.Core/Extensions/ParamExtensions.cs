using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace MetroshkaFestival.Core.Extensions
{
    public static class ParamExtensions
    {
        public static IEnumerable<TValue> GetEnumerableObjectPenetration<TObject, TValue>(this TObject @object,
                                                                                          Func<ObjectInfo, TValue> generateValue)
        {
            return GetEnumerableObjectPenetration(@object, generateValue, null);
        }

        private static readonly HashSet<Type> IgnoreTypes = new HashSet<Type>
        {
            typeof(string),
            typeof(IFormFile),
            typeof(HttpContext),
            typeof(DateTime)
        };

        private static IEnumerable<TValue> GetEnumerableObjectPenetration<TObject, TValue>(TObject @object,
                                                                                           Func<ObjectInfo, TValue> generateValue,
                                                                                           string startName)
        {
            var propertiesInfo = @object.GetType()
                                        .GetProperties();

            foreach (var propertyInfo in propertiesInfo)
            {
                var objectInfo = new ObjectInfo
                {
                    Name = $"{startName}{propertyInfo.Name}",
                    Value = propertyInfo.GetValue(@object),
                    ValueType = propertyInfo.PropertyType,
                    PropertyInfo = propertyInfo
                };

                if (propertyInfo.GetCustomAttributesData().Any(x => x.AttributeType == typeof(JsonIgnoreAttribute)))
                {
                    continue;
                }

                if (objectInfo.ValueType.IsGenericType && (objectInfo.ValueType.Name.StartsWith("IEnumerable") || objectInfo.ValueType.Name.StartsWith("ICollection")) ||
                    (objectInfo.Value?.GetType().IsArray ?? false) &&
                    objectInfo.Value != null)
                {
                    foreach (var propertyModel in GetEnumerableParamProperties(objectInfo, generateValue))
                    {
                        yield return propertyModel;
                    }
                }
                else if (objectInfo.Value != null &&
                         !IgnoreTypes.Contains(objectInfo.ValueType) &&
                         !objectInfo.ValueType.IsValueType)
                {
                    var paramProperties = GetEnumerableObjectPenetration(objectInfo.Value, generateValue, $"{objectInfo.Name}.");
                    foreach (var paramProperty in paramProperties)
                    {
                        yield return paramProperty;
                    }
                }
                else
                {
                    yield return generateValue(objectInfo);
                }
            }
        }

        private static IEnumerable<TValue> GetEnumerableParamProperties<TValue>(ObjectInfo objectInfo,
                                                                                Func<ObjectInfo, TValue> generateValue)
        {
            var genericArguments = objectInfo.ValueType.GetGenericArguments();

            if (objectInfo.ValueType.IsGenericType)
            {
                if (genericArguments.Length != 1)
                {
                    throw new ArgumentException($"Не поддерживается generic-тип с {genericArguments.Length} аргументами");
                }

                if (genericArguments[0] != typeof(string) &&
                    !genericArguments[0].IsPrimitive)
                {
                    throw new ArgumentException($"Не поддерживается generic-параметр {genericArguments[0].FullName}");
                }
            }

            var valueType = objectInfo.ValueType.IsArray
                ? objectInfo.ValueType.GetElementType()
                : genericArguments[0];
            var isPrimitive = valueType == typeof(string) || valueType.IsPrimitive;

            var counter = 0;
            foreach (var item in (IEnumerable) objectInfo.Value)
            {
                var itemObjectInfo = new ObjectInfo
                {
                    Name = $"{objectInfo.Name}[{counter++}]",
                    Value = item,
                    PropertyInfo = objectInfo.PropertyInfo,
                    ValueType = item.GetType()
                };

                if (isPrimitive)
                {
                    yield return generateValue(itemObjectInfo);
                }
                else
                {
                    var paramProperties = GetEnumerableObjectPenetration(item, generateValue, $"{itemObjectInfo.Name}.");
                    foreach (var paramProperty in paramProperties)
                    {
                        yield return paramProperty;
                    }
                }
            }
        }

        public class ObjectInfo
        {
            public string Name { get; set; }
            public object Value { get; set; }
            public Type ValueType { get; set; }
            public PropertyInfo PropertyInfo { get; set; }
        }
    }
}