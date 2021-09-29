using MagicSoftware.Integration.UserComponents;

namespace MagicSoftware.MQTT.Step
{
    /// <summary>
    /// This is the Data class of MQTT step
    /// </summary>
    class MQTTStepModel : MQTTModelBase
    {
        [Id(1)]
        public Numeric QoS { get; set; }

        [Id(2)]
        public Logical Retain { get; set; }

        [Id(3)]
        [PrimitiveDataTypes(DataType.Blob)]
        [MagicSoftware.Integration.UserComponents.Encoding(EncodingType.Unicode)]
        [Out]
        [DisplayPropertyName("Store Result")]
        [AllowEmptyExpression]
        public Variable storeResultVar { get; set; }

        [Id(4)]
        [Out]
        [PrimitiveDataTypes(DataType.Alpha)]
        [DisplayPropertyName("Store Result")]
        [AllowEmptyExpression]
        public Expression storeResultFile { get; set; }

        [Id(5)]
        [PrimitiveDataTypes(DataType.Logical)]
        [Out]
        [DisplayPropertyName("Operation Success")]
        [AllowEmptyExpression]
        public Variable operationSuccess { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        public MQTTStepModel()
        {
            InitDataMembers();
        }

        /// <summary>
        /// Initializing
        /// </summary>
        private void InitDataMembers()
        {
            storeResultVar = new Variable();
            storeResultVar.SetValue(string.Empty);
            operationSuccess = new Variable();
            operationSuccess.SetValue(string.Empty);
            QoS = new Numeric();
            QoS.SetNumeric(0);
            Retain = new Logical();
            Retain.SetLogical(true);
            storeResultFile = new Expression();
            storeResultFile.SetValue(string.Empty);
        }
    }
}
