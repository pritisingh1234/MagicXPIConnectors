using System.ComponentModel;
using MagicSoftware.Integration.UserComponents;
using MagicSoftware.Integration.UserComponents.Interfaces;

namespace MagicSoftware.MQTT.Step
{
    abstract class ZoomableStepFieldViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected MQTTStepModel WorkData { get; private set; }

        protected ZoomableStepFieldViewModel(MQTTStepModel workData)
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

    /// <summary>
    /// This class represents the 'Store Result In' field in the MQTT step's Configuration window.
    /// This class is used when the Store Result is a File.
    /// </summary>
    class ResultFileFieldViewModel : ZoomableStepFieldViewModel
    {
        public ResultFileFieldViewModel(MQTTStepModel workData):
            base(workData)
        {
            
        }

        public string FilePath
        {
            get { return WorkData.storeResultFile.GetValue(); }
            set
            {
                WorkData.storeResultFile.SetValue(value);
                OnPropertyChanged("FilePath");
            }
        }

        public override void OpenFieldEditor(ISDKStudioUtils utils, DataType datatype)
        {
            string newFilePath;
            if (utils.OpenFileSelectionDailog(MQTTResourceManager.SelectAFile, "(*.*)|*.*", FilePath, out newFilePath))
                FilePath = newFilePath;
        }

        public override void OpenExpressionEditor(ISDKStudioUtils utils, DataType datatype)
        {
            var expression = new Expression();
            expression.SetValue(FilePath);
            var newExpression = utils.OpenExpressionEditor(expression, datatype, MQTTResourceManager.FilePath);
            FilePath = newExpression.GetValue();
        }

        public override void ClearValue()
        {
            FilePath = null;
        }
    }

    abstract class VariableFieldViewModel : ZoomableStepFieldViewModel
    {
        public VariableFieldViewModel(MQTTStepModel workData) : base(workData) 
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
    /// This class represents the 'Store Result In' field in the MQTT step's Configuration window.
    /// This class is used when the Store Result is a Variable.
    /// </summary>
    class ResultVariableFieldViewModel : VariableFieldViewModel
    {
        public ResultVariableFieldViewModel(MQTTStepModel workData) : base(workData)
        {
            if (workData.storeResultVar.GetValue() == string.Empty)
                workData.storeResultVar.SetValue(Utils.DEFAULT_BLOB_VARIABLE);
        }

        public override Variable FieldVariable
        {
            get { return WorkData.storeResultVar; }
            set
            {
                WorkData.storeResultVar = value;
                OnPropertyChanged("FieldVariable");
            }
        }
    }

    /// <summary>
    /// This class represents the 'Operation Success' field in the MQTT step's Configuration window.
    /// </summary>
    class OperationSuccessVariableFieldViewModel : VariableFieldViewModel
    {
        public OperationSuccessVariableFieldViewModel(MQTTStepModel workData) : base(workData)
        {
        }

        public override Variable FieldVariable
        {
            get { return WorkData.operationSuccess; }
            set
            {
                WorkData.operationSuccess = value;
                OnPropertyChanged("FieldVariable");
            }
        }
    }
}
