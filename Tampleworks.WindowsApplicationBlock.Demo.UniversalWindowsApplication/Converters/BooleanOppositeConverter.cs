using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Tampleworks.WindowsApplicationBlock.Demo.UwpApp.Converters
{
    public sealed class BooleanOppositeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => ConvertCommon(value);

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => ConvertCommon(value);

        private static bool ConvertCommon(object value)
        {
            if (!(value is bool)) throw new ArgumentException("Only boolean argument supported.");
            bool bValue = (bool)value;
            return !bValue;
        }
    }
}
