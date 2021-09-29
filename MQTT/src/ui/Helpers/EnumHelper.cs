using System;
using System.Collections.Generic;
using System.Reflection;

namespace MagicSoftware.MQTT.Helpers
{
    static class EnumHelper
    {
        public static IEnumerable<Tuple<string, object>> GetDisplayNames(Type enumType)
        {
            var result = new List<Tuple<string, object>>();

            var enumFields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var field in enumFields)
            {
                var attrs = field.GetCustomAttributes(typeof(DisplayNameAttribute), false);
                var name = field.Name;
                if (attrs.Length > 0)
                {
                    name = ((DisplayNameAttribute)attrs[0]).DisplayName;
                }
                result.Add(new Tuple<string, object>(name, field.GetValue(enumType)));
            }

            return result;
        }
    }
}
