using Sahara.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Globalization;
using System.Windows.Media;

namespace Sahara
{
    public class TestResultStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is TestResultStatus))
            {
                throw new NotImplementedException("TestResultStatusConverter can only convert TestResultStatus");
            }
            var status = (TestResultStatus)value;
            string path = String.Empty;
            switch (status.Name)
            {
                case "PASS":
                    path = "Images/tr_pass.png";
                    break;
                case "FAIL":
                    path = "Images/tr_fail.png";
                    break;
                case "INCONCLUSIVE":
                    path = "Images/tr_inc.png";
                    break;
                default:
                    throw new NotSupportedException();
            }
            return new BitmapImage(new Uri("/Sahara;component/" + path, UriKind.Relative));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    public class TestResultStatusToSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TestResultStatus))
            {
                throw new ArgumentException("TestResultStatusToColorConverter can only convert TestResultStatus");
            }
            var status = value as TestResultStatus;
            switch (status.Name)
            {
                case "PASS":
                    return new SolidColorBrush(Colors.Green);
                case "FAIL":
                    return new SolidColorBrush(Colors.Red);
                default:
                    return new SolidColorBrush(Colors.Blue);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
