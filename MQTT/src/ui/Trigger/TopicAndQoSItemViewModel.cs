using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using MagicSoftware.Integration.UserComponents;
using System.Xml.Serialization;
using System.Windows;
using System.Text.RegularExpressions;

namespace MagicSoftware.MQTT.Trigger
{
    /// <summary>
    /// This class represents each row in the Topics table.
    /// </summary>
    [XmlType(TypeName = "TopicAndQoS")]
    [Serializable]
    public class TopicAndQoSItemViewModel : INotifyPropertyChanged, IEditableObject
    {
        const string TOPIC_PATTERNS = "([^\\/][#\\+])|(#.)";
        public bool isNewRow = true;
        private string topic;     
        public string Topic
        {
            get
            {
                return topic;
            }
            set
            {
                if (!isNewRow)
                    ValidateTopic(value);
                topic = value;
                OnPropertyChanged("Topic");
                isNewRow = false;
            }
        }

        /// <summary>
        /// This method validates the Topic value according to MQTT requirements. 
        /// </summary>
        /// <param name="value"></param>
        private void ValidateTopic(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                MessageBox.Show(MQTTResourceManager.MQTTTopicFieldCantBeEmptyMsg, MQTTResourceManager.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }

           if (value != null)
           {
              var m = Regex.Match(value, TOPIC_PATTERNS);

              if (m.Success)
              {
                 MessageBox.Show(MQTTResourceManager.MQTTTopicIsNotValidMsg, MQTTResourceManager.Error, MessageBoxButton.OK, MessageBoxImage.Error);
              }
           }
        }

        private QosEnum qos;
        public QosEnum QoS
        {
            get
            {
                return qos;
            }
            set
            {
                qos = value;
                OnPropertyChanged("QoS");
            }
        }
        
        private int index;       

        [XmlIgnore]
        public int Index
        {
            get
            {
                return index;
            }
            set
            {
                index = value;
                OnPropertyChanged("Index");
            }
        }

        private String oldTopic;
        private QosEnum oldQoS;
        private int oldIndex;

        public TopicAndQoSItemViewModel()
        {
        }

        public TopicAndQoSItemViewModel(int index)
        {
            this.index = index;
            QoS = QosEnum.Qos0;
            Topic = string.Empty;
        }

        public TopicAndQoSItemViewModel(Alpha topic, QosEnum qos, int index)
        {
            Index = index;
            QoS = qos;
            Topic = topic.GetAlpha();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void BeginEdit()
        {
            oldTopic = Topic;
            oldQoS = QoS;
            oldIndex = Index;
        }

        public void CancelEdit()
        {
            Topic = oldTopic;
            QoS = oldQoS;
            Index = oldIndex;
        }

        public void EndEdit()
        {
            //ValidateTopic();
        }

        internal bool ValidateTopic()
        {           
            if (string.IsNullOrWhiteSpace(Topic))
            {
               MessageBox.Show(MQTTResourceManager.MQTTTopicFieldCantBeEmptyMsg, MQTTResourceManager.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            var m = Regex.Match(Topic, TOPIC_PATTERNS);
            
            if (m.Success)
            {
               MessageBox.Show(MQTTResourceManager.MQTTTopicIsNotValidMsg, MQTTResourceManager.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }
    }
}
