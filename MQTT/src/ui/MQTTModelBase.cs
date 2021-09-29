namespace MagicSoftware.MQTT
{
    /// <summary>
    /// This is the parent class of a model class for the MQTT step and trigger.
    /// </summary>
    class MQTTModelBase
    {
        public MQTTModelBase Clone()
        {
            return (MQTTModelBase)MemberwiseClone();
        }

        public virtual void CopyFrom(MQTTModelBase WorkData) { }
    }
}
