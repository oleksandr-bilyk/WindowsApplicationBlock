using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Tampleworks.WindowsApplicationBlock.Demo.UwpApp.Converters
{
    public sealed class TrueToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is bool)) throw new ArgumentException("Only boolean argument supported.");
            bool bValue = (bool)value;
            return bValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotSupportedException();
    }
}
