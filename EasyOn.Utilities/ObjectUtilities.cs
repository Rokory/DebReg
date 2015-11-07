using System;
using System.Reflection;

namespace EasyOn.Utilities
{
    public static class ObjectUtilities
    {
        public static void CopyProperties<T>(this T source, T destination)
        {

            PropertyInfo[] properties = source.GetType().GetProperties();

            if (source.GetType().IsAssignableFrom(destination.GetType()))
            {
                foreach (var property in properties)
                {
                    if
                    (
                        property.CanWrite
                        && property.CanRead
                        && (
                            property.PropertyType.IsPrimitive
                            || property.PropertyType.Equals(typeof(string))
                            || property.PropertyType.Equals(typeof(String))
                        )
                    )
                    {
                        property.SetValue(destination, property.GetValue(source));
                    }
                }
            }
            else
            {
                throw new ArgumentException();
            }

        }
    }
}
