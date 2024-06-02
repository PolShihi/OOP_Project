using MyModel.Models.DTOs;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientSideApp.Converters
{
    public class SelectedColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is UserSession session && parameter is UserSession session1 && session.Id == session1.Id)
            {
                return Color.FromRgb(200, 200, 200);
            }

            return Color.FromRgb(255, 255, 255);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
