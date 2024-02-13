using System;
using System.Globalization;
using System.Windows.Data;

namespace YoutrackHelper.Models
{
    public class TimeSpanConverterEx : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var ts = (TimeSpan)value;
                return ts != TimeSpan.Zero
                    ? $"{(int)ts.TotalDays} day {ts:hh\\:mm}"
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