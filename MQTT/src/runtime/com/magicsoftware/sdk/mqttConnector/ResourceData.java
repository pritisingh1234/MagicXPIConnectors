package com.magicsoftware.sdk.mqttConnector;

import java.io.File;
import java.io.PrintStream;
import java.io.PrintWriter;
import java.io.StringWriter;
import java.net.URI;
import java.net.URISyntaxException;
import java.util.HashMap;

import org.eclipse.paho.client.mqttv3.MqttClient;
import org.eclipse.paho.client.mqttv3.MqttConnectOptions;
import org.eclipse.paho.client.mqttv3.MqttException;
import org.eclipse.paho.client.mqttv3.MqttSecurityException;
import org.eclipse.paho.client.mqttv3.persist.MqttDefaultFilePersistence;

import com.magicsoftware.ibolt.commons.logging.LogLevel;
import com.magicsoftware.ibolt.commons.logging.Logger;
import com.magicsoftware.xpi.sdk.SDKException;

public class ResourceData {

	private String[] MQTTurl;
	private MqttClient client;
	private boolean cleanSession;
	private String delimitedMQTTurl;
	private String clientID;
	private String PersistDir;
	private boolean sucess = true;
	private int intQoS = 0;
	private boolean retainFlag = false;
	private String userName;
	private String password;
	private boolean useLWT;
	private String topicLWT;
	private String messageLWT;
	private Integer QosLWT;
	private boolean retainLWT;
	private Integer keepAliveInterval;
	private Integer publishtimeOut;
	private Integer connectionTimeOut;
	private String caCertificateFilePath = null;
	private String clientPKCS12CertificateFilePath = null;
	private String pkcs12Password = null;
	private String securityProtocol = null;
	private String[] serverURIs = null;
	private ResourceData resource = null;
	PrintStream p = null;
	
	protected static LogModules module = LogModules.STEP;

	/**
	 * Create for JNI
	 */
	public ResourceData() {
	}
	public ResourceData(HashMap<String, String> resource) throws SDKException {
		
		if (resource != null) {

			try {
				this.userName=resource.get("User Name");
			}
			catch (Exception e) {
				Logger.logMessage(LogLevel.LEVEL_ERROR, module,e.getMessage());
			}
			try {
				this.cleanSession=Boolean.parseBoolean(resource.get("Clean Session").substring(0, (resource.get("Clean Session").length() - 3)).replaceAll("'", ""));
				Logger.logMessage(LogLevel.LEVEL_DEBUG, module,"Clean Session: " + cleanSession);
			} catch (Exception e) {
				Logger.logMessage(LogLevel.LEVEL_ERROR, module,e.getMessage());
			}

			try {
					this.keepAliveInterval=Integer.valueOf(resource.get("MQTT Keep Alive Interval"));
					Logger.logMessage(LogLevel.LEVEL_DEBUG, module,"MQTT Keep Alive Interval: " + keepAliveInterval);	
					
			} catch (NumberFormatException e) {
				this.keepAliveInterval=Constant.KEEP_ALIVE_INTERVAL_DEFAULT;
				Logger.logMessage(LogLevel.LEVEL_DEBUG, module,"Setting Default Keep Alive Interval: " + keepAliveInterval);

			} catch (Exception e) {
				Logger.logMessage(LogLevel.LEVEL_ERROR, module,e.getMessage());
			}
			try {
					this.publishtimeOut=Integer.valueOf(resource.get("MQTT Publish Timeout"));
					Logger.logMessage(LogLevel.LEVEL_DEBUG, module,"MQTT Publish Timeout: " + publishtimeOut);	
			} catch (NumberFormatException e) {
				this.publishtimeOut=2;
				Logger.logMessage(LogLevel.LEVEL_DEBUG, module,"Setting Default MQTT Publish Timeout: " + publishtimeOut);
			} catch (Exception e) {
				Logger.logMessage(LogLevel.LEVEL_ERROR, module,e.getMessage());
			}
			try {
					this.connectionTimeOut=Integer.valueOf(resource.get("MQTT Connection Timeout"));
					Logger.logMessage(LogLevel.LEVEL_DEBUG, module,"MQTT Connection Timeout: " + connectionTimeOut);	

			} catch (NumberFormatException e) {
				this.connectionTimeOut=Constant.CONNECTION_TIMEOUT_DEFAULT;
				Logger.logMessage(LogLevel.LEVEL_DEBUG, module,"Setting Default connectionTimeOut: " + connectionTimeOut);
			} catch (Exception e) {
				Logger.logMessage(LogLevel.LEVEL_ERROR, module,e.getMessage());
			}
			try {
					this.QosLWT=Integer.valueOf(resource.get("LWT QoS"));
					Logger.logMessage(LogLevel.LEVEL_DEBUG, module,"LWT QoS: " + QosLWT);	
				
			} catch (NumberFormatException e) {
				this.QosLWT=0;
				Logger.logMessage(LogLevel.LEVEL_DEBUG, module,"Setting Default LWT QoS: " + QosLWT);
			}
			try {
				this.password=resource.get("Password");
			}

			catch (Exception e) {
				Logger.logMessage(LogLevel.LEVEL_ERROR, module,e.getMessage());
			}
			try {
				this.useLWT=Boolean.parseBoolean(resource.get("Use LWT").substring(0, (resource.get("Use LWT").length() - 3)).replaceAll("'", ""));
				Logger.logMessage(LogLevel.LEVEL_DEBUG, module,"Use LWT: " + useLWT);
				this.topicLWT=resource.get("LWT Topic");
				Logger.logMessage(LogLevel.LEVEL_DEBUG, module,"LWT Topic: " + topicLWT);
				this.messageLWT=resource.get("LWT Message");
				Logger.logMessage(LogLevel.LEVEL_DEBUG, module,"LWT Message: " + messageLWT);
				this.retainLWT=Boolean.parseBoolean(resource.get("LWS Retained").substring(0, (resource.get("LWS Retained").length() - 3)).replaceAll("'", ""));
				Logger.logMessage(LogLevel.LEVEL_DEBUG, module,"LWS Retained: " + retainLWT);
			} catch (Exception e) {
				Logger.logMessage(LogLevel.LEVEL_ERROR, module,e.getMessage());
			}
			this.delimitedMQTTurl = resource.get("Broker URLs");
			this.MQTTurl = delimitedMQTTurl.split(",");
			this.caCertificateFilePath = resource.get("Server Certificate File");
			if (caCertificateFilePath != null) {
				this.clientPKCS12CertificateFilePath = resource
						.get("Client Certificate File");
				Logger.logMessage(LogLevel.LEVEL_DEBUG, module,"Server Certificate File: " + caCertificateFilePath);
				this.securityProtocol = resource.get("Security Protocol");
				Logger.logMessage(LogLevel.LEVEL_DEBUG, module,"Security Protocol: " + securityProtocol);
				this.pkcs12Password = resource.get("Client Certificate Password");
			}
		}

		else {
			throw new SDKException(Constant.Error_MQTT_Resource_Number,
					Constant.Error_MQTT_Resource_Details);
		}
	}

	public void setResource(ResourceData resource) {
		this.resource = resource;
	}

	public MqttConnectOptions getConnecitonOptionsData() throws SDKException{
		return ResourceData.getConnectionOptionsData(
				this.cleanSession,this.connectionTimeOut,this.keepAliveInterval,
				this.userName,this.password,this.delimitedMQTTurl , this.MQTTurl, 
				this.caCertificateFilePath, this.clientPKCS12CertificateFilePath, this.pkcs12Password,
				this.securityProtocol, this.useLWT, this.topicLWT, this.messageLWT, this.QosLWT , this.retainLWT
		);
	}
	/**
	 * *
	 * 
	 * @return
	 * @throws SDKException
	 */
	private static MqttConnectOptions getConnectionOptionsData(boolean cleanSession, 
			Integer connectionTimeOut, Integer keepAliveInterval, String userName, 
			String password, String delimitedMQTTurl, String[] MQTTurl, 
			String caCertificateFilePath, String clientPKCS12CertificateFilePath, 
			String pkcs12Password, String securityProtocol, Boolean useLWT, String topicLWT, 
			String messageLWT, Integer QosLWT, boolean retainLWT) throws SDKException {
		
		MqttConnectOptions connectOptions = new MqttConnectOptions();
		connectOptions.setCleanSession(cleanSession);
		
		/**
		 * Sets the connection timeout value. This value, measured in seconds,
		 * defines the maximum time interval that the client will wait for the
		 * network connection to the MQTT server to be established. The default
		 * timeout is 30 seconds. A value of 0 disables timeout processing
		 * meaning that the client will wait until the network connection  
		 * succeeds or fails.
		 * 
		 * @param publishtimeOut - the timeout value, measured in seconds. It must be > 0;
		 */

		if (connectionTimeOut > 0)
			connectOptions.setConnectionTimeout(connectionTimeOut);
		else
			connectOptions.setConnectionTimeout(Constant.CONNECTION_TIMEOUT_DEFAULT);

		/**
		 * Sets the "keep alive" interval. This value, measured in seconds,
		 * defines the maximum time interval between messages sent or received.
		 * It enables the client to detect if the server is no longer available,
		 * without having to wait for the TCP/IP timeout. The client will ensure
		 * that at least one message travels across the network within each keep
		 * alive period. In the absence of a data-related message during the
		 * time period, the client sends a very small "ping" message, which the
		 * server will acknowledge. A value of 0 disables keep alive processing
		 * in the client.
		 * <p>
		 * The default value is 60 seconds.
		 * </p>
		 *
		 * @param keepAliveInterval - the interval, measured in seconds, must be >= 0.
		 */
		if (keepAliveInterval >= 0)
			connectOptions.setKeepAliveInterval(keepAliveInterval);
		else
			connectOptions.setKeepAliveInterval(Constant.KEEP_ALIVE_INTERVAL_DEFAULT);

		if (userName != null && !userName.equals("")) {

			connectOptions.setUserName(userName);
			if (password != null) {
				connectOptions.setPassword(password.toCharArray());
			}
		}
		Logger.logMessage(LogLevel.LEVEL_DEBUG, module,"MQTTurl" + delimitedMQTTurl);
		

		/**
		 * Sets a list of one or more serverURIs the client may connect to.
		 * <p>
		 * Each <code>serverURI</code> specifies the address of a server that
		 * the client may connect to. Two types of connection are supported
		 * <code>tcp://</code> for a TCP connection and <code>ssl://</code> for
		 * a TCP connection secured by SSL/TLS. For example:
		 * <ul>
		 * <li><code>tcp://localhost:1883</code></li>
		 * <li><code>ssl://localhost:8883</code></li>
		 * </ul>
		 * If the port is not specified, it will default to 1883 for
		 * <code>tcp://</code>" URIs, and 8883 for <code>ssl://</code> URIs.
		 * <p>
		 * If serverURIs is set then it overrides the serverURI parameter passed
		 * in on the constructor of the MQTT client.
		 * <p>
		 * When an attempt to connect is initiated the client will start with
		 * the first serverURI in the list and work through the list until a
		 * connection is established with a server. If a connection cannot be
		 * made to any of the servers then the connect attempt fails.
		 * <p>
		 * Specifying a list of servers that a client may connect to has several
		 * uses:
		 */
		connectOptions.setServerURIs(MQTTurl);

		/**
		 * Sets the <code>SocketFactory</code> to use. This allows an
		 * application to apply its own policies around the creation of network
		 * sockets. If using an SSL connection, an <code>SSLSocketFactory</code>
		 * can be used to supply application-specific security settings.
		 * 
		 * @param socketFactory
		 *            the factory to use.
		 */

		if (delimitedMQTTurl.toLowerCase().trim().startsWith("ssl")
				&& caCertificateFilePath != null) {
			try {
				File f = new File(caCertificateFilePath);
				if (f.exists())
					connectOptions.setSocketFactory(SslUtil.getSocketFactory(
							caCertificateFilePath,
							clientPKCS12CertificateFilePath, pkcs12Password,
							securityProtocol));

			}

			catch (Exception e) {
				System.err.println("MqttException:");
				Logger.logMessage(LogLevel.LEVEL_DEBUG, module,"Unable to set caCertificateFilePath: " + e.toString());
				
				throw new SDKException(
						Constant.Error_MQTT_Connection_Exception_Number,
						Constant.Error_MQTT_Connection_Exception_details
								+ e.getMessage());
			}
		}

		/**
		 * Sets the "Last Will and Testament" (LWT) for the connection. In the
		 * event that this client unexpectedly loses its connection to the
		 * server, the server will publish a message to itself using the
		 * supplied details.
		 *
		 * @param topicLWT
		 *            the topic to publish to.
		 * @param messageLWT
		 *            the byte payload for the message.
		 * @param QosLWT
		 *            the quality of service to publish the message at (0, 1 or
		 *            2).
		 * @param retainLWT
		 *            whether or not the message should be retained.
		 */

		if (useLWT) {
			if ((topicLWT == null) || (messageLWT == null)) {
				throw new IllegalArgumentException();
			}
			connectOptions.setWill(topicLWT, messageLWT.getBytes(), QosLWT, retainLWT);
		}
		return connectOptions;
	}

	/**
	 * Validate a URI
	 * 
	 * @param srvURI
	 * @return the URI type
	 */

	protected static int validateURI(String srvURI) {
		try {
			URI vURI = new URI(srvURI);
			if (!vURI.getPath().equals("")) {
				throw new IllegalArgumentException(srvURI);
			}
			if (vURI.getScheme().equals("tcp")) {
				return Constant.URI_TYPE_TCP;
			} else if (vURI.getScheme().equals("ssl")) {
				return Constant.URI_TYPE_SSL;
			} else if (vURI.getScheme().equals("local")) {
				return Constant.URI_TYPE_LOCAL;
			} else {
				throw new IllegalArgumentException(srvURI);
			}
		} catch (URISyntaxException ex) {
			throw new IllegalArgumentException(srvURI);
		}
	}
	
	public static String validate(String delimitedMQTTurl, String userName, String password, 
			boolean cleanSession, Integer keepAliveInterval, String persistDir, 
			Integer connectionTimeOut, String caCertificateFilePath, 
			String clientPKCS12CertificateFilePath, String pkcs12Password, String securityProtocol,
			boolean useLWT, String topicLWT, String  messageLWT, Integer QosLWT, 
			boolean retainLWT) throws SDKException {

		String result = "Y"; 
		
		try {

			String [] MQTTurl = delimitedMQTTurl.split(",");
			MqttDefaultFilePersistence dataStore = new MqttDefaultFilePersistence(persistDir);
			MqttConnectOptions connectOpt = 
				ResourceData.getConnectionOptionsData(
						cleanSession,connectionTimeOut,keepAliveInterval,
						userName,password,delimitedMQTTurl , MQTTurl, 
						caCertificateFilePath, clientPKCS12CertificateFilePath, pkcs12Password,
						securityProtocol, useLWT, topicLWT, messageLWT, QosLWT , retainLWT);
			
			Logger.logMessage(LogLevel.LEVEL_DEBUG, module, "MQTTurl" + delimitedMQTTurl);
			String clientID = java.util.UUID.randomUUID().toString();
			MqttClient client = new MqttClient(MQTTurl[0], clientID, dataStore);
			client.connect(connectOpt);
			Logger.logMessage(LogLevel.LEVEL_DEBUG, module, "MQTT client created");
		
		} catch (Exception e) {
			
			String delimiter = ";@!#";
			result = "N" + delimiter + "Failed to connect to MQTT Resource.\n";
			StringWriter sw = new StringWriter();
			PrintWriter pw = new PrintWriter(sw);
			e.printStackTrace(pw);
			
			if (e instanceof MqttSecurityException) {
				result += delimiter + Constant.Error_MQTT_Connection_security_Exception_details;
			} else {
				result += delimiter + Constant.Error_MQTT_Connection_Exception_details;
			} 
			
			result += "\n" + sw.toString();
		}	
		
		return result;
	}

	public String[] getMQTTurl() {
		return MQTTurl;
	}

	public boolean isCleanSession() {
		return cleanSession;
	}

	public String getClientID() {
		return clientID;
	}

	public String getPersistDir() {
		return PersistDir;
	}

	public int getIntQoS() {
		return intQoS;
	}

	public boolean isRetainFlag() {
		return retainFlag;
	}

	public String getUserName() {
		return userName;
	}

	public String getPassword() {
		return password;
	}

	public boolean isUseLWT() {
		return useLWT;
	}

	public String getTopicLWT() {
		return topicLWT;
	}

	public String getMessageLWT() {
		return messageLWT;
	}

	public Integer getQosLWT() {
		return QosLWT;
	}
    public boolean isRetainLWT() {
		return retainLWT;
	}

	public Integer getKeepAliveInterval() {
		return keepAliveInterval;
	}

	public Integer getPublishtimeOut() {
		return publishtimeOut;
	}

	public String getCaCertificateFilePath() {
		return caCertificateFilePath;
	}

	public String getClientPKCS12CertificateFilePath() {
		return clientPKCS12CertificateFilePath;
	}

	public String getPkcs12Password() {
		return pkcs12Password;
	}

	public String getSecurityProtocol() {
		return securityProtocol;
	}
	
	public String[] getServerURIs() {
		return serverURIs;
	}

	public ResourceData getResource() {
		return resource;
	}

	public boolean isSucess() {
		return sucess;
	}

	public MqttClient getClient() {
		return client;
	}
}
