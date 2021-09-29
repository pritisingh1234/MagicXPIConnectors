using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MagicSoftware.MQTT.Helpers;
using System.Xml.Serialization;

namespace MagicSoftware.MQTT
{
    #region Qos Enum
    [Serializable]
    public enum QosEnum
    {
        [XmlEnum("0")]
        [DisplayName("0")]
        Qos0,
        [XmlEnum("1")]
        [DisplayName("1")]
        Qos1,
        [XmlEnum("2")]
        [DisplayName("2")]
        Qos2
    }

    #endregion
}
