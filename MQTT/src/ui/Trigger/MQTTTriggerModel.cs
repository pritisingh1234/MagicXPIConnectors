using MagicSoftware.Integration.UserComponents;

namespace MagicSoftware.MQTT.Trigger
{
    /// <summary>
    /// This is the MQTT trigger's data class.
    /// </summary>
    class MQTTTriggerModel : MQTTModelBase
    {
        
        [Id(1)]
        [UseForConfiguration]
        public Alpha ListOfTopicsAndQoS { get; set; }
        
        [Id(2)]
        [PrimitiveDataTypes(DataType.Alpha)]
        [MagicSoftware.Integration.UserComponents.Encoding(EncodingType.Unicode)]
        [TriggerIn]
        [DisplayPropertyName("MQTT Topic")]
        public Variable topic { get; set; }
      
        [Id(3)]
        [PrimitiveDataTypes(DataType.Blob)]
        [MagicSoftware.Integration.UserComponents.Encoding(EncodingType.Binary)]
        [TriggerIn]
        [DisplayPropertyName("MQTT Message")]
        public Variable message { get; set; }


        /// <summary>
        /// Ctor
        /// </summary>
		public MQTTTriggerModel()
		{
            InitDataMembers();
		}

        /// <summary>
        /// Initializing 
        /// </summary>
        private void InitDataMembers()
        {
            topic = new Variable();
            topic.SetValue(string.Empty);
            message = new Variable();
            message.SetValue(string.Empty);
            ListOfTopicsAndQoS = new Alpha();
            ListOfTopicsAndQoS.SetAlpha(string.Empty);
        }
    }
}
