using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Tampleworks.WindowsApplicationBlock.Demo.UniversalWindowsApplication.Converters
{
    public sealed class TrueToCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is bool)) throw new ArgumentException("Only boolean argument supported.");
            bool bValue = (bool)value;
            return bValue ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotSupportedException();
    }
}
