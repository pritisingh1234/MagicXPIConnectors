using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using MagicSoftware.MQTT.Step;
using MagicSoftware.MQTT.Trigger;
using MagicSoftware.Integration.UserComponents;
using MagicSoftware.Integration.UserComponents.Interfaces;
using System.Windows;

namespace MagicSoftware.MQTT
{
    /// <summary>
    /// This is the parent view model of the MQTT step and trigger.
    /// </summary>
    class MQTTViewModel : INotifyPropertyChanged
    {
        #region Fields

        public event PropertyChangedEventHandler PropertyChanged;
        private String resourceName;
        protected MQTTModelBase WorkData { get; private set; }
        protected MQTTModelBase OriginalData { get; private set; }
        protected ISDKStudioUtils Utils { get; private set; }
        private String isDesign;
        #endregion

        #region Properties

        public String ResourceName
        {
            get { return resourceName; }
            set
            {
                resourceName = value; 
                OnPropertyChanged("ResourceName");
            }
        }

        public String IsDesign
        {
            get { return isDesign; }
            set
            {
                isDesign = value;
                OnPropertyChanged("IsDesign");
            }
        }

        #endregion

        /// <summary>
        /// This is the ctor for the MQTT step.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="utils"></param>
        /// <param name="resourceData"></param>
        public MQTTViewModel(MQTTModelBase model, ISDKStudioUtils utils, IReadOnlyResourceConfiguration resourceData)
        {
            OriginalData = model;
            WorkData = model.Clone();
            this.Utils = utils;

            if (resourceData != null)
            {
                this.ResourceName = resourceData.ResourceName;
            }
            else
            {
                ResourceName = MagicSoftware.MQTT.Utils.UNKNOWN_RESOURCE_NAME;
            }

            UpdateDesignMode();            
        }

        /// <summary>
        /// This is the ctor for the MQTT trigger.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="utils"></param>
        /// <param name="servicesData"></param>
        public MQTTViewModel(MQTTModelBase model, ISDKStudioUtils utils, IReadOnlyServiceConfiguration servicesData)
        {
            OriginalData = model;
            WorkData = model.Clone();
            this.Utils = utils;

            try
            {
                this.ResourceName = servicesData.resourceConfiguration.ResourceName;
            }
            catch
            {
                ResourceName = MagicSoftware.MQTT.Utils.UNKNOWN_RESOURCE_NAME;
            }

            UpdateDesignMode();
        }

        private void UpdateDesignMode()
        {
            // Is it Debug or Design state - return 'True' / 'False'
            string isDesign = this.Utils.GetSystemProperty("IsStudioInDesignMode");

            if (isDesign.ToLower() == true.ToString().ToLower())
                IsDesign = Visibility.Visible.ToString();
            else
                IsDesign = Visibility.Hidden.ToString();
        }


        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        
        // This method checks if the user replaced the old data with the new data.
        public bool DataWasChanged()
        {
            return !WorkData.Equals(OriginalData);
        }

        public virtual void CommitChanges()
        {
        }

    }
}
