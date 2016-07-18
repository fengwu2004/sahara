using Sahara.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Sahara
{
    public class ResultViewModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }

            if (!(value is ResultViewMode))
            {
                throw new NotImplementedException("ResultViewModeConverter can only convert ResultViewMode");
            }

            var mode = (ResultViewMode)value;
            return mode.Name == parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isChecked = (bool)value;
            if (!isChecked)
            {
                return null;
            }

            switch (parameter.ToString())
            {
                case "simple":
                    return ResultViewMode.Simple;
                case "advanced":
                    return ResultViewMode.Advanced;
                default:
                    break;
            }

            return null;
        }
    }
}
