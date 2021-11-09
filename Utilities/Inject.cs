using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace cugoj_ng_server.Utilities
{
    public static class Inject
    {
        const BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public static void SetProperties(Type type, object obj, Dictionary<Type, object> rules, BindingFlags bindingFlags = bindingAttr)
        {
            Array.ForEach(type.GetProperties(bindingFlags), propInfo =>
            {
                if (rules.TryGetValue(propInfo.PropertyType, out object propValue))
                    propInfo.SetValue(obj, propValue);
            });
        }
        public static void SetProperties<T>(T obj, Dictionary<Type, object> rules, BindingFlags bindingFlags = bindingAttr) =>
            SetProperties(typeof(T), obj, rules, bindingFlags);

        public static void SetPropertiesByType<T>(Dictionary<Type, object> rules, BindingFlags bindingFlags = bindingAttr) =>
            SetProperties(typeof(T), null, rules, bindingFlags);

    }
}
