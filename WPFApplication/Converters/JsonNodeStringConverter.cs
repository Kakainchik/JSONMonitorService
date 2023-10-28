using FileMonitorService.JsonService;
using System;
using System.Globalization;
using System.Windows.Data;

namespace WPFApplication.Converters
{
    public class JsonNodeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is not JsonNodeTree)
                throw new ArgumentException();

            JsonNodeTree node = (JsonNodeTree)value;
            if(parameter.ToString() == "PROPERTY")
            {
                if(node.IsArrayItem)
                    return node.ArrayIndex!;
                else if(node.IsMainRoot)
                    return "JSON";
                else
                    return node.PropertyName ?? string.Empty;
            }
            else if(parameter.ToString() == "VALUE")
            {
                if(node.IsArray)
                    return "[]";
                else if(node.IsComplex || node.IsMainRoot || node.IsArrayItem)
                    return string.Empty;
                else if(node.Value is string str)
                    return string.Concat("\"", str, "\"");
                else
                    return node.Value?.ToString() ?? "null";
            }
            else
                return new object();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}