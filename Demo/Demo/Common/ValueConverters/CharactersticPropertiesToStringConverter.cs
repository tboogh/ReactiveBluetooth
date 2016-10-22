using System;
using System.Globalization;
using ReactiveBluetooth.Core.Types;
using Xamarin.Forms;

namespace Demo.Common.ValueConverters
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
