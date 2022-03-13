using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("WkHtmlWrapper.UnitTests")]
[assembly: InternalsVisibleTo("WkHtmlWrapper.Image")]
[assembly: InternalsVisibleTo("WkHtmlWrapper.Pdf")]
namespace WkHtmlWrapper.Core.Attributes
{
    internal class ConsoleLineParameterAttribute : Attribute
    {
        public string ParameterName { get; }

        public ConsoleLineParameterAttribute(string parameterName)
        {
            ParameterName = parameterName;
        }
    }
}