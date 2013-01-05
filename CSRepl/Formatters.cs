using System;
using System.Collections;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CSRepl
{
    public static class Formatters
    {
        public static string FormatOutput(this object result)
        {
            if (result == null)
                return "null";

            if (result is string)
                return FormatOutput(result as string);

            if (result is Int16
                || result is Int32
                || result is Int64
                || result is double
                || result is float
                || result is bool
                || result.GetType().Name.Contains("AnonymousType")
                )
            {
                return result.ToString();
            }

            if (result is IDictionary)
                return FormatOutput(result as IDictionary);

            if (result is Array)
                return FormatOutput(result as Array);

            if (result is IXPathNavigable)
            {
                var navigable = result as IXPathNavigable;
                var navigator = navigable.CreateNavigator();
                return XDocument.Parse(navigator.OuterXml).ToString();
            }
            
            if (result is XDocument)
                return result.ToString();
            
            if (result is IEnumerable)
                return FormatOutput(result as IEnumerable);


            MethodInfo toString = result.GetType().GetMethod("ToString", Type.EmptyTypes); //GetMethod("ToString", BindingFlags.OptionalParamBinding);
            if (toString != null && toString.DeclaringType != typeof(object))
            {
                return result.ToString();
            }

            if (result.GetType() != typeof(object))
            {
                var output = new StringBuilder();

                var type = result.GetType();

                output.Append(type.Name);
                output.Append(" {");

                var i = 0;
                foreach (var member in type.GetProperties())
                {
                    if (member.MemberType == MemberTypes.Property)
                    {
                        output.Append(" ");
                        output.Append(member.Name);
                        output.Append(" = ");
                        output.Append(member.GetValue(result, null).FormatOutput());

                        if (i < type.GetProperties().Length - 1)
                        {
                            output.Append(", ");
                        }
                    }
                    i++;
                }

                output.Append(" }");
                return output.ToString();
            }

            return result.ToString();
        }

        private static string FormatOutput(Array array)
        {
            var builder = new StringBuilder("[");
            var i = 0;
            foreach (var item in array)
            {
                builder.Append(item.FormatOutput());
                if (i < array.Length - 1)
                {
                    builder.Append(",");
                }
                i++;
            }
            builder.Append("]");
            return builder.ToString();
        }

        private static string FormatOutput(string value)
        {
            return "\"" + value + "\"";
        }

        private static string FormatOutput(IEnumerable enumerable)
        {
            var builder = new StringBuilder("[");
            var enumerator = enumerable.GetEnumerator();
            var i = 0;
            while (enumerator.MoveNext())
            {
                builder.Append(enumerator.Current.FormatOutput());
                builder.Append(",");
                i++;
            }
            if (i > 0)
            {
                builder.Remove(builder.Length - 1, 1);
            }
            builder.Append("]");
            return builder.ToString();
        }

        private static string FormatOutput(IDictionary dictionary)
        {
            var builder = new StringBuilder("[");

            IDictionaryEnumerator enumerator = dictionary.GetEnumerator();
            var i = 0;
            while (enumerator.MoveNext())
            {
                builder.Append("[");
                builder.Append(enumerator.Key.FormatOutput());
                builder.Append(", ");
                builder.Append(enumerator.Value.FormatOutput());
                builder.Append("]");
                builder.Append(",");
                i++;
            }
            if (i > 0)
            {
                builder.Remove(builder.Length - 1, 1);
            }
            
            builder.Append("]");
            return builder.ToString();
        }
    }
}