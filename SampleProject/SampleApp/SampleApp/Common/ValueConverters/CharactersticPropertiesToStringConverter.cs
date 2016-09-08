using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveBluetooth.Core;
using Xamarin.Forms;

namespace SampleApp.Common.ValueConverters
{
    public class CharactersticPropertiesToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CharacteristicProperty propeties = (CharacteristicProperty) value;
            string text = "";

            CharacteristicProperty[] propertyValues = (CharacteristicProperty[])Enum.GetValues(typeof(CharacteristicProperty));
            foreach (var characteristicProperty in propertyValues)
            {
                if (propeties.HasFlag(characteristicProperty))
                {
                    if (text != "")
                    {
                        text += ", ";
                    }
                    text += $"{Enum.GetName(typeof(CharacteristicProperty), characteristicProperty)}";
                }
            }
            return text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
