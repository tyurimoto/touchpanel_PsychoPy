using System;

namespace Compartment
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumDisplayNameAttribute : Attribute
    {
        /// <summary>表示名</summary>
        public string Name { get; set; }

        /// <summary>enum表示名属性</summary>
        /// <param name="name">表示名</param>
        public EnumDisplayNameAttribute(string name)
        {
            Name = name;
        }
    }

    public static class EnumExtensions
    {
        public static string GetEnumDisplayName<T>(this T enumValue)
        {
            var field = typeof(T).GetField(enumValue.ToString());
            var attrType = typeof(EnumDisplayNameAttribute);
            var attribute = Attribute.GetCustomAttribute(field, attrType);
            return (attribute as EnumDisplayNameAttribute)?.Name;
        }
    }
}
