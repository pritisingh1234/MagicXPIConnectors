using System;
using MagicSoftware.Integration.UserComponents.Interfaces;
using MagicSoftware.Integration.UserComponents;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.IO;
using System.Xml;


namespace MagicSoftware.MQTT.Trigger
{
    /// <summary>
    /// This is the MQTT trigger's ViewModel class.
    /// </summary>
    class MQTTTriggerViewModel : MQTTViewModel
    {        
        private MQTTTriggerModel TriggerWorkData { get { return (MQTTTriggerModel) WorkData; } }

        public ObservableCollection<TopicAndQoSItemViewModel> Topics { get; private set; }

        private ZoomableTriggerFieldViewModel messagePayload;
        public ZoomableTriggerFieldViewModel MessagePayload
        {
            get
            {
                return messagePayload;
            }
            set
            {
                messagePayload = value;
                OnPropertyChanged("MessagePayload");
            }
        }

        private ZoomableTriggerFieldViewModel messageTopic;
        public ZoomableTriggerFieldViewModel MessageTopic
        {
            get
            {
                return messageTopic;
            }
            set
            {
                messageTopic = value;
                OnPropertyChanged("MessageTopic");
            }
        }

        private TopicAndQoSItemViewModel selectedLine;
        public TopicAndQoSItemViewModel SelectedLine
        {
            get
            {
                return selectedLine;
            }
            set
            {
                selectedLine = value;
                OnPropertyChanged("SelectedLine");
            }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="model"></param>
        /// <param name="utils"></param>
        /// <param name="serviceData"></param>
        public MQTTTriggerViewModel(MQTTTriggerModel model, ISDKStudioUtils utils, IReadOnlyServiceConfiguration serviceData)
            : base(model, utils, serviceData)
        {
            // This initializes the trigger fields with a value according to the value in the data class.
            MessageTopic = new MessageTopicVariableFieldViewModel(TriggerWorkData);
            MessagePayload = new MessagePayloadVariableFieldViewModel(TriggerWorkData);          
            Topics = new ObservableCollection<TopicAndQoSItemViewModel>();
            InitTopics();
        }

        // This class gets the topic's data in XML format and deserializes it to the Topics collection.
        private void InitTopics()
        {
            var originalData = (MQTTTriggerModel)OriginalData;
            var topicsXML = originalData.ListOfTopicsAndQoS.GetAlpha();

            if (topicsXML != string.Empty)
            {
                Topics = DeserializeTopics(topicsXML);
            }

            UpdateIndexes();
        }

        internal void Prompt(object p)
        {
            var promptParameters = (string)p;
            if (!string.IsNullOrEmpty(promptParameters))
            {
               var parameters = promptParameters.Split(';');
               switch (parameters[0])
               {
                  case "MessageTopic":
                     MessageTopic.OpenFieldEditor(Utils, DataType.Alpha);
                     break;
                  case "MessagePayload":
                     MessagePayload.OpenFieldEditor(Utils, DataType.Blob);
                     break;
               }
            }
        }

        /// <summary>
        /// This commits the user's changes.
        /// </summary>
        public override void CommitChanges()
        {
            var originalData = (MQTTTriggerModel)OriginalData;
            var newData = (MQTTTriggerModel)WorkData;

            originalData.ListOfTopicsAndQoS.SetAlpha(SerializeTopics());
            originalData.message = newData.message;
            originalData.topic = newData.topic;
        }

        /// <summary>
        /// This method is called when the user clicks the New button.
        /// </summary>
        internal void AddEmptyRow()
        {
            if (ValidateLastTopic())
            {
                Topics.Add(new TopicAndQoSItemViewModel(Topics.Count + 1));
            }     
        }

        /// <summary>
        /// This method checks the last topic value after the user enters it.
        /// </summary>
        /// <returns></returns>
        public bool ValidateLastTopic()
        {
            if (Topics.Count > 0)
            {
                var topic = Topics[Topics.Count - 1];
                return topic.ValidateTopic();
            }
            return true;
        }

        /// <summary>
        /// This method is called when the user clicks the Delete button.
        /// </summary>
        internal void RemoveRow()
        {
            Topics.Remove(SelectedLine);
            UpdateIndexes();
        }

        private void UpdateIndexes()
        {
            for (var i = 0; i < Topics.Count; i++)
            {
                Topics[i].Index = i + 1;
            }
        }

        private string SerializeTopics()
        {
            string xml;
            var serializer = new XmlSerializer(typeof(ObservableCollection<TopicAndQoSItemViewModel>));
            var subReq = Topics;
            using (var sww = new StringWriter())
            using (var writer = XmlWriter.Create(sww))
            {
                serializer.Serialize(writer, subReq);
                xml = sww.ToString(); // Your XML
            }

            return xml;
        }

        private ObservableCollection<TopicAndQoSItemViewModel> DeserializeTopics(string topics)
        {
            ObservableCollection<TopicAndQoSItemViewModel> result;
            var serializer = new XmlSerializer(typeof(ObservableCollection<TopicAndQoSItemViewModel>));
            using (TextReader reader = new StringReader(topics))
            {
                result = (ObservableCollection<TopicAndQoSItemViewModel>)serializer.Deserialize(reader);
            }

            return result;
        }
    }
}
