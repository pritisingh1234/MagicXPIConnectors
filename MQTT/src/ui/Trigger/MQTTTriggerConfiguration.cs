using System;
using MagicSoftware.Integration.UserComponents.Interfaces;
using System.ComponentModel.Composition;
using System.Windows.Forms;

namespace MagicSoftware.MQTT.Trigger
{
    /// <summary>
    /// This class manages the MQTT trigger's configuration actions.
    /// For example: validate a service, open the configuration window, etc.
    /// </summary>
    [Export(typeof(IUserTriggerComponent))] 
    public class MQTTTriggerConfiguration : IUserTriggerComponent
    {
        const string ID_CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        const int    ID_SIZE = 23;

        private bool res = false;
        private ISDKStudioUtils utils;
        public ICheckerResult Check(ref object data, IReadOnlyServiceConfiguration serviceData)
        {
            return null;
        }

        /// <summary>
        /// This method is called when the user opens the step configuration.
        /// </summary>
        /// <param name="dataObject"></param>
        /// <param name="utils"></param>
        /// <param name="serviceData"></param>
        /// <param name="NavigateTo"></param>
        /// <param name="configurationChanged"></param>
        /// <returns></returns>
        public bool? Configure(ref object dataObject, ISDKStudioUtils utils, IReadOnlyServiceConfiguration serviceData, object NavigateTo, out bool configurationChanged)
        {
            this.utils = utils;
            var triggerViewModel = new MQTTTriggerViewModel((MQTTTriggerModel)dataObject, utils, serviceData);
           var window = new MQTTTriggerView
           {
              Owner = System.Windows.Application.Current.MainWindow,
              DataContext = triggerViewModel
           };

           var windowResult = window.ShowDialog();

            configurationChanged = false;

            if (windowResult != null && (bool)windowResult)
            {
                if (triggerViewModel.DataWasChanged())
                {
                    triggerViewModel.CommitChanges();
                    configurationChanged = true;
                }
            }

            res = true;
            return windowResult;
        }

        public object CreateDataObject()
        {
            return new MQTTTriggerModel();
        }

        /// <summary>
        /// This method receives an updatable Service object and the name of the helper button that was clicked. 
        /// </summary>
        /// <param name="helperID"></param>
        /// <param name="serviceData"></param>
        public void InvokeServiceHelper(string helperID, IServiceConfiguration serviceData)
        {
            // This takes the current generated ID.
            var currentGeneratedId = serviceData.GetPropertyValue("Client ID");
            var result = DialogResult.Yes;

            // This checks if the user wants to overwrite the current value, if it exists.
            if (!string.IsNullOrEmpty(currentGeneratedId))
                result = System.Windows.Forms.MessageBox.Show(MQTTResourceManager.MQTTOverwriteClientIdValueMsg, MQTTResourceManager.MQTTGenerateIDCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                var newGeneratedId = Utils.RandomString(ID_SIZE, ID_CHARACTERS);
                using (var writer = serviceData.BeginEditing())
                {
                    writer.SetPropertyValue("Client ID", newGeneratedId);
                    writer.AcceptChanges();
                }
            }
        }

        /// <summary>
        /// This method is called when the user clicks the MQTT service's Validate button (if it exists).
        /// </summary>
        /// <param name="serviceData"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public bool ValidateService(IReadOnlyServiceConfiguration serviceData, out string errorMsg)
        {
            throw new NotImplementedException();
        }
    }
}
