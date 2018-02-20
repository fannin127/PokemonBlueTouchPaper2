using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Globalization;
using System.IO.IsolatedStorage;

namespace SilverlightApplication3
{
    public class VisibilitySaveContinueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            using (var folder = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if(folder.FileExists("save.xml"))
                {
                    return Visibility.Visible;
                } else
                {
                    return Visibility.Collapsed;
                }
            }
                
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
