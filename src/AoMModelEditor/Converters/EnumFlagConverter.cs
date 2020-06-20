using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace AoMModelEditor.Converters
{
    public class EnumFlagConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var flagValue = values[0] as Enum;
            var propertyValue = values[1] as Enum;

            if (flagValue == null) return false;
            if (propertyValue == null) return false;

            return propertyValue.HasFlag(flagValue);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
