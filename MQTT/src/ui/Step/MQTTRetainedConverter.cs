using System;
using System.Globalization;
using System.Windows.Data;

namespace MagicSoftware.MQTT.Step
{
    public class MQTTRetainedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            RetainedEnum enumValue = (RetainedEnum)value;

            string returnValue = string.Empty;

            switch (enumValue)
            {
                case RetainedEnum.Yes:
                    returnValue = MQTTResourceManager.MQTTRetainedYes;
                    break;
                case RetainedEnum.No:
                    returnValue = MQTTResourceManager.MQTTRetainedNo;
                    break;
            }

            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}