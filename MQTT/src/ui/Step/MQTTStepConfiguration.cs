using System;
using MagicSoftware.Integration.UserComponents.Interfaces;
using MagicSoftware.Integration.UserComponents;
using System.ComponentModel.Composition;
using System.Windows;
using Magicsoftware.iBolt.Common;
using Magicsoftware.iBolt.Common.Resources;
using Magicsoftware.iBolt.Common.Utilities;

namespace MagicSoftware.MQTT.Step
{
    /// <summary>
    /// This class manages the MQTT step configuration actions.
    /// validate resource, open the configuration window and etc.
    /// </summary>
    [Export(typeof(IUserComponent))]
    public class MQTTStepConfiguration : IUserComponent
    {
        private bool res = false;
        private ISDKStudioUtils utils;
        private MQTTStepViewModel stepViewModel = null;
        public MQTTStepConfiguration()
        {
        }

        #region IUserComponent implementation

        public object CreateDataObject()
        {
            return new MQTTStepModel();
        }

        /// <summary>
        /// This method called when user opens the step configuration.
        /// </summary>
        /// <param name="dataObject"></param>
        /// <param name="utils"></param>
        /// <param name="resourceData"></param>
        /// <param name="NavigateTo"></param>
        /// <param name="configurationChanged"></param>
        /// <returns></returns>
        public bool? Configure(ref object dataObject, ISDKStudioUtils utils, IReadOnlyResourceConfiguration resourceData, object NavigateTo, out bool configurationChanged)
        {
            this.utils = utils;
            stepViewModel = new MQTTStepViewModel((MQTTStepModel)dataObject, utils, resourceData);
            var window = new MQTTStepView
            {
                Owner = Application.Current.MainWindow,
                DataContext = stepViewModel
            };


            var windowResult = window.ShowDialog();

            configurationChanged = false;

            if ((bool)windowResult)
            {
                if (stepViewModel.DataWasChanged())
                {
                    stepViewModel.CommitChanges();
                    configurationChanged = true;
                }
            }

            res = true;
            return windowResult;
        }

        /// <summary>
        /// Returns the schema object created during configurations
        /// </summary>
        /// <returns></returns>
        public SchemaInfo GetSchema()
        {
            return GetXMLSchemaConfiguration();
        }

        /// <summary>
        /// This method called by the checker during build action
        /// </summary>
        /// <param name="data"></param>
        /// <param name="resourceData"></param>
        /// <returns></returns>
        public ICheckerResult Check(ref object data, IReadOnlyResourceConfiguration resourceData)
        {
            /* int keepAliveInterval; 
             if (!int.TryParse(resourceData.GetPropertyValue("MQTT Keep Alive Interval"), out keepAliveInterval))
             {
                 CheckerMessage cm = new CheckerMessage();
                 cm.ErrorMessage = "MQTT Keep Alive Interval property is invalid.";
                 cm.PropertyDisplayName = "MQTT Keep Alive Interval";
                 cm.PropertyID = 20;
                 cm.Severity = ErrorCategory.Error;
             }

             int connectionTimeOut;
             if (!int.TryParse(resourceData.GetPropertyValue("MQTT Connection Timeout"), out connectionTimeOut))
             {
                 CheckerMessage cm = new CheckerMessage();
                 cm.ErrorMessage = "MQTT Connection Timeout property is invalid.";
                 cm.PropertyDisplayName = "MQTT Connection Timeout";
                 cm.PropertyID = 21;
                 cm.Severity = ErrorCategory.Error;
             }

             if (String.IsNullOrEmpty(resourceData.GetPropertyValue("Clean Session")))
             {
                 CheckerMessage cm = new CheckerMessage();
                 cm.ErrorMessage = "Clean Session property can't be blank.";
                 cm.PropertyDisplayName = "Clean Session";
                 cm.PropertyID = 22;
                 cm.Severity = ErrorCategory.Error;
             }*/


            return null;
        }

        /// <summary>
        /// This method called when user enter the Validate button for MQTT resource.
        /// </summary>
        /// <param name="resourceData">Resource detailes</param>
        /// <param name="errorMsg">error if exists</param>
        /// <returns></returns>
        public bool ValidateResource(IReadOnlyResourceConfiguration resourceData, out string errorMsg)
        {
            var brokerURLs = resourceData.GetPropertyValue("Broker URLs");
            var userName = resourceData.GetPropertyValue("User Name");
            var password = resourceData.GetPropertyValue("Password");

            var caCertificateFilePath = resourceData.GetPropertyValue("Server Certificate File");
            var securityProtocol = resourceData.GetPropertyValue("Security Protocol");

            var clientPKCS12CertificateFilePath = resourceData.GetPropertyValue("Client Certificate File");
            var pkcs12Password = resourceData.GetPropertyValue("Client Certificate Password");

            errorMsg = null;

            // Check mandetory field
            if (string.IsNullOrEmpty(brokerURLs))
            {
                errorMsg = MQTTResourceManager.MTTTBrokerURLsCantBeEmptyMsg;
                return false;
            }

            try
            {
                Utils.validateResource(brokerURLs, userName, password, true,
                    30, Utils.GetTemporaryDirectory(), 30,
                    caCertificateFilePath, clientPKCS12CertificateFilePath, pkcs12Password, securityProtocol,
                    false, string.Empty, string.Empty, 0, false); // LWT properties don't needed for validate action
            }
            catch (Exception e)
            {
                errorMsg = e.GetBaseException().Message;
                return false;
            }

           //MessageBox.Show(MQTTResourceManager.MQTTConnectionSuccessfullMsg, MQTTResourceManager.Information, MessageBoxButton.OK, MessageBoxImage.Information);
           iBolt.Common.Utils.ShowSuccessOrErrorMessage(MQTTResourceManager.MQTTConnectionSuccessfullMsg, MessageBoxImages.Information);
            return true;
        }

        /// <summary>
        /// This method receives an updatable Resource object and the name of the helper button that was pressed. 
        /// </summary>
        /// <param name="helperID"></param>
        /// <param name="resouceData"></param>
        public void InvokeResourceHelper(string helperID, IResourceConfiguration resouceData)
        {
            //In order to get a property from the Resource:
            var myProperty = resouceData.GetPropertyValue("ResourcePropertyName");

            //You should open your own configuration dialogs

            // MessageBox.Show(MQTTResourceManager.MQTTShouldDefineConfigurationDialogsMsg, MQTTResourceManager.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            iBolt.Common.Utils.ShowSuccessOrErrorMessage(MQTTResourceManager.MQTTShouldDefineConfigurationDialogsMsg, MessageBoxImages.Error);

            //In order to set a properties of the Resource:
            /*
            IResourceConfigurationWriter writer =  resouceData.BeginEditing();
            writer.SetPropertyValue("property1","Val1");
            writer.SetPropertyValue("property2","Val2");
            writer.AccectChanges();
            */
        }

        #endregion

        /// <summary>
        /// This method opens the DataMapper with xml schema in destinition side.
        /// Called after user entered the MQTT step.
        /// </summary>
        /// <returns></returns>
        public SchemaInfo GetXMLSchemaConfiguration()
        {
            // Create the schema info based on the user entered data...
            var xmlSchemaInfoLocal = new XMLSchemaInfo
            {
                SchemaName = "MQTT_Publish",
                AlwayCreateNodes = true,
                AppendData = false,
                DataDestinationType = 0,
                Description = "MQTT Publish",
                RecursionDepth = 1,
                XMLEncoding = XMLSchemaInfo.UTF_8,
                XMLValidation = false,
                XSDSchemaFilePath = "%AddOnConnector%MQTT\\XSD\\MQTT_Publish.xsd"
            };
            return xmlSchemaInfoLocal;
        }
    }
}
