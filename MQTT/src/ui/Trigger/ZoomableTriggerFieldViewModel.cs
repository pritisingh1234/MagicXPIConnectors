using System.ComponentModel;
using MagicSoftware.Integration.UserComponents;
using MagicSoftware.Integration.UserComponents.Interfaces;

namespace MagicSoftware.MQTT.Trigger
{
    abstract class ZoomableTriggerFieldViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected MQTTTriggerModel WorkData { get; private set; }

        protected ZoomableTriggerFieldViewModel(MQTTTriggerModel workData)
        {
            WorkData = workData;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public abstract void OpenFieldEditor(ISDKStudioUtils utils, DataType datatype);
        public abstract void OpenExpressionEditor(ISDKStudioUtils utils, DataType datatype);

        public abstract void ClearValue();
    }

    abstract class VariableFieldViewModel : ZoomableTriggerFieldViewModel
    {
        public VariableFieldViewModel(MQTTTriggerModel workData) : base(workData) 
        {        
        }

        public abstract Variable FieldVariable { get; set; }

        public override void OpenFieldEditor(ISDKStudioUtils utils, DataType datatype)
        {
            FieldVariable = utils.OpenVariablePicklist(FieldVariable, VariableFilter.ALLVariables, datatype);
        }


        public override void OpenExpressionEditor(ISDKStudioUtils utils, DataType datatype)
        {
            // This is not supported...
        }

        public override void ClearValue()
        {
            FieldVariable.SetValue(null);
        }
    }

    /// <summary>
    /// This class represents the Message Payload field in the MQTT trigger's Configuration window.
    /// </summary>
    class MessagePayloadVariableFieldViewModel : VariableFieldViewModel
    {
        public MessagePayloadVariableFieldViewModel(MQTTTriggerModel workData) : base(workData)
        {
            if (workData.message.GetValue() == string.Empty)
                workData.message.SetValue(Utils.DEFAULT_BLOB_VARIABLE);
        }

        public override Variable FieldVariable
        {
            get { return WorkData.message; }
            set
            {
                WorkData.message = value;
                OnPropertyChanged("FieldVariable");
            }
        }
    }

    /// <summary>
    /// This class represents the Message Topic field in the MQTT trigger's Configuration window.
    /// </summary>
    class MessageTopicVariableFieldViewModel : VariableFieldViewModel
    {
        public MessageTopicVariableFieldViewModel(MQTTTriggerModel workData) : base(workData)
        {
            if (workData.topic.GetValue() == string.Empty)
                workData.topic.SetValue(Utils.DEFAULT_ALPHA_VARIABLE);
        }

        public override Variable FieldVariable
        {
            get { return WorkData.topic; }
            set
            {
                WorkData.topic = value;
                OnPropertyChanged("FieldVariable");
            }
        }
    }

}
