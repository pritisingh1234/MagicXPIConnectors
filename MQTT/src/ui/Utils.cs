using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jni.Net.Utils;
using System.IO;
using System.Reflection;

namespace MagicSoftware.MQTT
{
    /// <summary>
    /// This class publishes general methods for MQTT.
    /// </summary>
    public class Utils
    {
        public static string DEFAULT_BLOB_VARIABLE = "C.UserBlob";
        public static string DEFAULT_ALPHA_VARIABLE = "C.UserString";
        public static string UNKNOWN_RESOURCE_NAME = "Unknown resource name";

        /// <summary>
        /// This method generates a random string according to parameters.
        /// </summary>
        /// <param name="length">The length of the random string.</param>
        /// <param name="chars">The chars of the random string.</param>
        /// <returns></returns>
        public static string RandomString(int length, string chars)
        {
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Validation of the MQTT resource.
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
        public static void validateResource(string delimitedMQTTurl, string userName, string password,
            bool cleanSession, int keepAliveInterval, string persistDir, int connectionTimeOut, 
            string caCertificateFilePath, string clientPKCS12CertificateFilePath, string pkcs12Password, 
            string securityProtocol, bool useLWT, string topicLWT, string messageLWT, int QosLWT,
            bool retainLWT)
        {
            string rootFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            DirectoryInfo UIFolder = Directory.GetParent(rootFolder);
            DirectoryInfo MQTTFolder = UIFolder.Parent;

            var classLoader = new ClassLoader("MQTT", MQTTFolder.FullName + @"\runtime\java\lib");
            MQTTResourceJni resource = new MQTTResourceJni(classLoader);

            resource.Validate(delimitedMQTTurl, userName, password, cleanSession, keepAliveInterval, 
                persistDir, connectionTimeOut, caCertificateFilePath, clientPKCS12CertificateFilePath, 
                pkcs12Password, securityProtocol, useLWT, topicLWT, messageLWT, QosLWT, retainLWT);
        }

        /// <summary>
        /// This method creates a temp folder and returns the folder's path.
        /// </summary>
        /// <returns></returns>
        public static string GetTemporaryDirectory()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }
    }
}
