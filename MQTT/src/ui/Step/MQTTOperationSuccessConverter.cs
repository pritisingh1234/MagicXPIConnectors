using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace MagicSoftware.MQTT.Step
{
    public class MQTTOperationSuccessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string returnValue = string.Empty;

            var itemValue = value as KeyValuePair<string, VariableFieldViewModel>?;

            if (itemValue.HasValue)
            {
                if (itemValue.Value.Key == "Variable")
                {
                    returnValue = MQTTResourceManager.MQTTOperationSuccessVariable;
                }
                if (itemValue.Value.Key == "None")
                {
                    returnValue = MQTTResourceManager.MQTTOperationSuccessNone;
                }
            }

            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}