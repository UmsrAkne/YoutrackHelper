using System;
using System.Globalization;
using System.Windows.Data;

namespace YoutrackHelper.Models
{
    public class TimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var ts = (TimeSpan)value;
                return ts != TimeSpan.Zero
                    ? TimeSpan.FromSeconds(Math.Floor(((TimeSpan)value).TotalSeconds)).ToString(@"hh\:mm\:ss")
                    : string.Empty;
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}