using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace cugoj_ng_server.Utilities
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    sealed class LabelsAttribute : Attribute
    {
        private readonly HashSet<string> LabelSet = null;


        public LabelsAttribute(params string[] labels)
        {
            LabelSet = new(labels);
        }
        public bool HasLabel(string label) => LabelSet.Contains(label);
    }
    static class LabelExtension
    {
        public static bool HasLabel(this PropertyInfo propertyInfo, string label) =>
            propertyInfo.GetCustomAttributes<LabelsAttribute>().Any(attr => attr.HasLabel(label));
    }

}
