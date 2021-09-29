using System;
using Jni.Net.Utils;
using Jni.Net.Bridge;

namespace MagicSoftware.MQTT
{
    /// <summary>
    /// This class represents the ResourceData Java class.
    /// This class implements JNI Functionality. 
    /// </summary>
    public class MQTTResourceJni
    {
        #region fields

        const string JAVA_CLASS_NAME = "com.magicsoftware.sdk.mqttConnector.ResourceData";
        JObject javaObject;
        readonly JClass _clazz;
        readonly JMethod _validate;
        readonly ClassLoader    _classLoader;

        #endregion

        #region Constructors

        /// <summary>
        /// This creates an instance of the JAVA_CLASS_NAME. 
        /// </summary>
        /// <param name="classLoader"></param>
        public MQTTResourceJni(ClassLoader classLoader)
        {
            _classLoader = classLoader;
            _clazz = classLoader.GetClass(JAVA_CLASS_NAME);

            _validate = _clazz.GetStaticMethod("validate",
                JTypeSpecifier.JavaString,
                JTypeSpecifier.JavaString,
                JTypeSpecifier.JavaString,
                JTypeSpecifier.JavaString,
                JTypeSpecifier.JavaBoolean,
                JTypeSpecifier.JavaInteger,
                JTypeSpecifier.JavaString,
                JTypeSpecifier.JavaInteger,
                JTypeSpecifier.JavaString,
                JTypeSpecifier.JavaString,
                JTypeSpecifier.JavaString,
                JTypeSpecifier.JavaString,
                JTypeSpecifier.JavaBoolean,
                JTypeSpecifier.JavaString,
                JTypeSpecifier.JavaString,
                JTypeSpecifier.JavaInteger,
                JTypeSpecifier.JavaBoolean);

            javaObject = classLoader.GetObject(JAVA_CLASS_NAME);
        }

        #endregion

        #region Methods

        /// <summary>
        /// This method validates the MQTT resource.
        /// </summary>
        /// <param name="delimitedMQTTurl"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="cleanSession"></param>
        /// <param name="keepAliveInterval"></param>
        /// <param name="persistDir"></param>
        /// <param name="connectionTimeOut"></param>
        /// <param name="caCertificateFilePath"></param>
        /// <param name="clientPKCS12CertificateFilePath"></param>
        /// <param name="pkcs12Password"></param>
        /// <param name="securityProtocol"></param>
        /// <param name="useLWT"></param>
        /// <param name="topicLWT"></param>
        /// <param name="messageLWT"></param>
        /// <param name="QosLWT"></param>
        /// <param name="retainLWT"></param>
        public void Validate(string delimitedMQTTurl, string userName, string password,
            bool cleanSession, int keepAliveInterval, string persistDir, int connectionTimeOut, 
            string caCertificateFilePath, string clientPKCS12CertificateFilePath, string pkcs12Password, 
            string securityProtocol, bool useLWT, string topicLWT, string messageLWT, int QosLWT,
            bool retainLWT)
        {
           var signatures = _classLoader.createJObjectArray(JClass.ForName("java/lang/String"),
               new JString("java.lang.String"),
               new JString("java.lang.String"),
               new JString("java.lang.String"),
               new JString("boolean"),
               new JString("java.lang.Integer"),
               new JString("java.lang.String"),
               new JString("java.lang.Integer"),
               new JString("java.lang.String"),
               new JString("java.lang.String"),
               new JString("java.lang.String"),
               new JString("java.lang.String"),
               new JString("boolean"),
               new JString("java.lang.String"),
               new JString("java.lang.String"),
               new JString("java.lang.Integer"),
               new JString("boolean"));

            var arguments = _classLoader.createJObjectArray(JClass.ForName("java/lang/Object"),
               new JString(delimitedMQTTurl),
               new JString(userName),
               new JString(password),
               ConvertToJavaBool(cleanSession),
               ConvertToJavaInteger(keepAliveInterval),
               new JString(persistDir),
               ConvertToJavaInteger(connectionTimeOut),
               new JString(caCertificateFilePath),
               new JString(clientPKCS12CertificateFilePath),
               new JString(pkcs12Password),
               new JString(securityProtocol),
               ConvertToJavaBool(useLWT),
               new JString(topicLWT),
               new JString(messageLWT),
               ConvertToJavaInteger(QosLWT),
               ConvertToJavaBool(retainLWT));

            var delimiter = ";@!#".ToCharArray();

            var resultJNI = _classLoader.CallStaticFunction(JAVA_CLASS_NAME, _validate.Name, signatures, arguments);
            var resultStr = JString.GetString(resultJNI);
            var results = resultStr.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            
            // If the first cell is 'Y', this means that the validation succeeded.
            // [0] - 'Y'(yes) or 'N'(no)
            // [1] - Exception message
            // [2] - stack trace
            if (results[0] == "N")
            {
                Magicsoftware.iBolt.Common.Logger.iBLog.AddToLogger(results[1] + results[2]);
                throw new Exception(results[1]);
            }
        }

        /// <summary>
        /// This converts a Java Boolean to a .NET Boolean using JNI.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool ConvertJavaBooleanToNETBool(JObject result)
        {
           // This converts a Java Boolean to a string.
            var signatures = _classLoader.createJObjectArray(JClass.ForName("java/lang/String"),
               new JString("java.lang.Object"));

            var arguments = _classLoader.createJObjectArray(JClass.ForName("java/lang/Boolean"),
               result);

            // This gets a sessionID, such as a String.
            var sessionIDJString = _classLoader.CallStaticFunction("java.lang.String",
                "valueOf", signatures, arguments);

            return bool.Parse(JString.GetString(sessionIDJString));
        }

        /// <summary>
        /// This converts a .NET int to a Java int using JNI.
        /// </summary>
        /// <param name="netInt"></param>
        /// <returns></returns>
        private JObject ConvertToJavaInteger(int netInt)
        {
           // Create new Integer
            var signatures = _classLoader.createJObjectArray(JClass.ForName("java/lang/String"),
               new JString("java.lang.String"));


            var arguments = _classLoader.createJObjectArray(JClass.ForName("java/lang/String"),
               new JString(netInt.ToString()));

            return _classLoader.CallStaticFunction("java.lang.Integer",
                "parseInt", signatures, arguments);
        }

        /// <summary>
        /// This converts a .NET Boolean to a Java Boolean using JNI.
        /// </summary>
        /// <param name="netBool"></param>
        /// <returns></returns>
        private JObject ConvertToJavaBool(bool netBool)
        {
           // This creates a new integer.
            var signatures = _classLoader.createJObjectArray(JClass.ForName("java/lang/String"),
               new JString("java.lang.String"));


            var arguments = _classLoader.createJObjectArray(JClass.ForName("java/lang/String"),
               new JString(netBool.ToString()));


            return _classLoader.CallStaticFunction("java.lang.Boolean",
                "parseBoolean", signatures, arguments);
        }

        #endregion
    }
}
