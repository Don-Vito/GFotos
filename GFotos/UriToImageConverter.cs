using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace GFotos
{
    public class UriToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (value is string)
            {
                value = new Uri((string)value);
            }

            var uri = value as Uri;
            if (uri != null)
            {
                var bi = new BitmapImage();

                bi.BeginInit();
                bi.DecodePixelWidth = 120;
                bi.DecodePixelHeight = 120; 
                bi.UriSource = uri;
                bi.EndInit();
                return bi;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}