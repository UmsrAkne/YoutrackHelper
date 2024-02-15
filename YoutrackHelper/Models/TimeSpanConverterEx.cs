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
                var dayStr = ts.TotalDays >= 1 ? $"{(int)ts.TotalDays}day " : string.Empty;
                return ts != TimeSpan.Zero
                    ? $"{dayStr}{ts.Hours}h {ts.Minutes}min"
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