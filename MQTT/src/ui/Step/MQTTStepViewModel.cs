using System.Collections.Generic;
using System.Collections.ObjectModel;
using MagicSoftware.Integration.UserComponents.Interfaces;
using MagicSoftware.Integration.UserComponents;
using System.Windows;
using Magic = MagicSoftware.Integration.UserComponents;

namespace MagicSoftware.MQTT.Step
{

    #region Store Result Enumeration

    public enum StoreResultInEnum
    {
        Variable,
        File
    }

    #endregion

    #region Operation Success

    public enum OperationSuccessEnum
    {
        None,
        Variable
    }

    #endregion

    #region Retained

    public enum RetainedEnum
    {
        No,
        Yes
    }

    #endregion

    /// <summary>
    /// This is the MQTT step's ViewModel class.
    /// </summary>
    class MQTTStepViewModel : MQTTViewModel
    {
        MQTTStepModel StepWorkData { get { return (MQTTStepModel)WorkData; } }

        public ICollection<KeyValuePair<string, ZoomableStepFieldViewModel>> StoreResultFieldTypes { get; private set; }

        public QosEnum QoS
        {
            get { return (QosEnum)(int)StepWorkData.QoS.GetNumeric(); }
            set
            {
                StepWorkData.QoS.SetNumeric((double)value);
                OnPropertyChanged("QoS");
            }
        }

        private ZoomableStepFieldViewModel resultStorage;
        public ZoomableStepFieldViewModel ResultStorage
        {
            get
            {
                return resultStorage; 
            }
            set
            {
                resultStorage = value;
                OnPropertyChanged("ResultStorage");
            }
        }

        public ICollection<KeyValuePair<string, VariableFieldViewModel>> OperationSuccessFieldTypes { get; private set; }

        private VariableFieldViewModel operationSuccess;
        public VariableFieldViewModel OperationSuccess
        {
            get
            {
                return operationSuccess;
            }
            set
            {
                operationSuccess = value;
                OnPropertyChanged("OperationSuccess");
            }
        }

        public RetainedEnum Retained
        {
            get { return StepWorkData.Retain.GetLogical() ? RetainedEnum.Yes : RetainedEnum.No; }
            set
            {
                StepWorkData.Retain.SetLogical(value == RetainedEnum.Yes);
                OnPropertyChanged("Retained");
            }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="model"></param>
        public MQTTStepViewModel(MQTTStepModel model, ISDKStudioUtils utils, IReadOnlyResourceConfiguration resourceData)
            : base(model, utils, resourceData)
        {
           var resultStorageTypes = new List<KeyValuePair<string, ZoomableStepFieldViewModel>>
           {
              new KeyValuePair<string, ZoomableStepFieldViewModel>("Variable",
                 new ResultVariableFieldViewModel(StepWorkData)),
              new KeyValuePair<string, ZoomableStepFieldViewModel>("File", new ResultFileFieldViewModel(StepWorkData))
           };

           StoreResultFieldTypes = new ReadOnlyCollection<KeyValuePair<string, ZoomableStepFieldViewModel>>(resultStorageTypes);

            // This initializes the Store Result field with a value according 
            // to the value in the data. This implementation is derived from the fact
            // that the data (model) does not have a value to determine the storage type.
            // This needs to be reflected later, when committing the data.
            ResultStorage = !string.IsNullOrEmpty(StepWorkData.storeResultFile.GetValue()) ? resultStorageTypes[1].Value : resultStorageTypes[0].Value;

           var operationSuccessTypes = new List<KeyValuePair<string, VariableFieldViewModel>>
           {
              new KeyValuePair<string, VariableFieldViewModel>("Variable",
                 new OperationSuccessVariableFieldViewModel(StepWorkData)),
              new KeyValuePair<string, VariableFieldViewModel>("None", null)
           };

           OperationSuccessFieldTypes = new ReadOnlyCollection<KeyValuePair<string, VariableFieldViewModel>>(operationSuccessTypes);

            // This initializes the Operation Success field with a value according 
            // to the value in the data. This implementation is derived from the fact
            // that the data (model) does not have a value to determine the storage type.
            // This needs to be reflected later, when committing the data.
            OperationSuccess = !string.IsNullOrEmpty(StepWorkData.operationSuccess.GetValue()) ? operationSuccessTypes[0].Value : operationSuccessTypes[1].Value;

        }

        internal void OpenExpressionEditor(object parameters)
        {
            var editorParameters = ((string)parameters).Split(';');
            if (editorParameters.Length == 0 || (editorParameters[0] == "result"))
            {
                ResultStorage.OpenExpressionEditor(Utils, DataType.Alpha);
            }
        }

        internal void Prompt(object p)
        {
            var promptParameters = (string)p;
            if (!string.IsNullOrEmpty(promptParameters))
            {
                var parameters = promptParameters.Split(';');
                if (parameters[0] == "result")
                {
                    ResultStorage.OpenFieldEditor(Utils, DataType.Blob);
                }
                else if (parameters[0] == "operationSuccess")
                {
                    OperationSuccess.OpenFieldEditor(Utils, DataType.Logical);
                }
            }
        }

        /// <summary>
        /// This commits the user's changes.
        /// </summary>
        public override void CommitChanges()
        {
            // This updates the storage data for the Store Result field, before calling the base commit:
            foreach (var storageTypeEntry in StoreResultFieldTypes)
            {
                if (!ReferenceEquals(storageTypeEntry.Value, ResultStorage))
                {
                    storageTypeEntry.Value.ClearValue();
                }
            }

            // this updates the storage data for the Operation Success field, before calling the base commit:
            foreach (var operationSuccessEntry in OperationSuccessFieldTypes)
            {
                if (!ReferenceEquals(operationSuccessEntry.Value, OperationSuccess))
                {
                    if (operationSuccessEntry.Value != null)
                        operationSuccessEntry.Value.ClearValue(); 
              
                }
            }

            var originalData = (MQTTStepModel)OriginalData;
            var newData = (MQTTStepModel)WorkData;

            originalData.operationSuccess = newData.operationSuccess;
            originalData.QoS = newData.QoS;
            originalData.Retain = newData.Retain;
            originalData.storeResultFile = newData.storeResultFile;
            originalData.storeResultVar = newData.storeResultVar;
        }

        /// <summary>
        /// This validates the current values in MQTT step fields when the user clicks the OK button.
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        internal bool CheckDetails(Window window)
        {
            var workdata = (MQTTStepModel)WorkData;
            
            // This checks if the Store Result is a File.
            if (resultStorage.GetType() == typeof(ResultFileFieldViewModel))
            {
                if (string.IsNullOrEmpty(workdata.storeResultFile.GetValue()))
                {
                    MessageBox.Show(window, MQTTResourceManager.MQTTStoreResultValueInFileNameMsg, MQTTResourceManager.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            // This checks if the Store Result is a Variable.
            else
            {
                if (string.IsNullOrEmpty(workdata.storeResultVar.GetValue()))
                {
                    MessageBox.Show(window, MQTTResourceManager.MQTTStoreResultValueInVariableNameMsg, MQTTResourceManager.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            if (operationSuccess != null)
            {
                if (string.IsNullOrEmpty(workdata.operationSuccess.GetValue()))
                {
                    MessageBox.Show(window, MQTTResourceManager.MQTTSelectVariableToStoreOperationStatusMsg, MQTTResourceManager.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            return true;
        }
    }
}

