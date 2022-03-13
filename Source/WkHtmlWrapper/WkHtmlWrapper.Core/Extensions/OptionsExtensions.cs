using System.Linq;
using System.Reflection;
using WkHtmlWrapper.Core.Attributes;
using WkHtmlWrapper.Core.Options.Interfaces;

namespace WkHtmlWrapper.Core.Extensions
{
    internal static class OptionsExtensions
    {
        public static string OptionsToCommandLineParameters(this IOptions options)
        {
            var parameters = options.GetType().GetProperties()
                .Select(p => OptionsPropertyToCommandLineParameter(options, p));

            return string.Join(" ", parameters);
        }

        private static string OptionsPropertyToCommandLineParameter(IOptions options, PropertyInfo property)
        {
            var propertyName = property.GetCustomAttribute<ConsoleLineParameterAttribute>().ParameterName;
            var (propertyValue, skipProperty) = OptionsPropertyValueToString(options, property);
            return skipProperty 
                ? string.Empty
                : $"{propertyName}{(string.IsNullOrEmpty(propertyValue) ? string.Empty : $" {propertyValue}")}";
        }

        private static (string val, bool skip) OptionsPropertyValueToString(IOptions options, PropertyInfo property)
        {
            switch (property.GetValue(options))
            {
                case int value:
                    return value == default ? (string.Empty, true) : (value.ToString(), false);
                case bool value:
                    return (string.Empty, !value);
                case object value:
                    return string.IsNullOrWhiteSpace(value.ToString())
                        ? (string.Empty, true)
                        : (value.ToString(), false);
            }
            return (string.Empty, true);
        }
    }
}