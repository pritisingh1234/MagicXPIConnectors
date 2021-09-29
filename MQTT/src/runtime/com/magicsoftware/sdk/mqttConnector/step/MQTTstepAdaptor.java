package com.magicsoftware.sdk.mqttConnector.step;

import java.io.ByteArrayInputStream;
import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.sql.Timestamp;
import java.util.Random;

import javax.xml.bind.DatatypeConverter;
import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;

import org.eclipse.paho.client.mqttv3.MqttClient;
import org.eclipse.paho.client.mqttv3.MqttConnectOptions;
import org.eclipse.paho.client.mqttv3.MqttException;
import org.eclipse.paho.client.mqttv3.MqttMessage;
import org.eclipse.paho.client.mqttv3.MqttPersistenceException;
import org.eclipse.paho.client.mqttv3.MqttSecurityException;
import org.eclipse.paho.client.mqttv3.MqttTopic;
import org.eclipse.paho.client.mqttv3.persist.MqttDefaultFilePersistence;
import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;
import org.xml.sax.SAXException;

import com.magicsoftware.ibolt.commons.logging.LogLevel;
import com.magicsoftware.ibolt.commons.logging.Logger;
import com.magicsoftware.sdk.mqttConnector.Constant;
import com.magicsoftware.sdk.mqttConnector.Environment;
import com.magicsoftware.sdk.mqttConnector.LogModules;
import com.magicsoftware.sdk.mqttConnector.ResourceData;
import com.magicsoftware.sdk.mqttConnector.ServiceData;
import com.magicsoftware.xpi.sdk.SDKException;
import com.magicsoftware.xpi.sdk.step.IStep;
import com.magicsoftware.xpi.sdk.step.StepGeneralParams;

public class MQTTstepAdaptor implements IStep {

	private MqttClient client;
	private String clientID;
	private boolean success = true;
	private boolean opSuccessFlag = true;
	private ResourceData resourceData = null;
	private BusinessData businessData = null;
	private Environment environment;
	protected LogModules module = LogModules.STEP;

	@Override
	public void invoke(StepGeneralParams args) throws SDKException {
		try {
			initData(args);
            MqttDefaultFilePersistence dataStore = new MqttDefaultFilePersistence(environment.getPersistDir());
			MqttConnectOptions connectOptions = resourceData.getConnecitonOptionsData();
			createClient(dataStore, connectOptions);
			String publishResult = publishMessages(args.getPayloadOBject());
			Logger.logMessage(LogLevel.LEVEL_ERROR, module,
					"Opration success:", ""+opSuccessFlag);
			updateSuccess(publishResult,opSuccessFlag);
			if (publishResult != null)
				updateResultData(publishResult);

		} catch (MqttSecurityException e) {
			Logger.logMessage(LogLevel.LEVEL_ERROR, module,
					"MqttSecurityException:", e.getMessage());
			opSuccessFlag=false;
			try {
				businessData.operationSuccess.setLogical(opSuccessFlag);
			} catch (Exception e1) {
				Logger.logMessage(LogLevel.LEVEL_ERROR, module,
						"Opration success:", ""+opSuccessFlag);
			}
	
			throw new SDKException(
					Constant.Error_MQTT_Connection_security_Exception_Number,
					Constant.Error_MQTT_Connection_security_Exception_details
							+ e.getMessage());
			
		} catch (MqttException e) {
			Logger.logMessage(LogLevel.LEVEL_ERROR, module, "MqttException:",
					e.getMessage());
			opSuccessFlag=false;
			try {
				businessData.operationSuccess.setLogical(opSuccessFlag);
			} catch (Exception e1) {
				Logger.logMessage(LogLevel.LEVEL_ERROR, module,
						"Opration success:", ""+opSuccessFlag);
			}
			throw new SDKException(
					Constant.Error_MQTT_Connection_Exception_Number,
					Constant.Error_MQTT_Connection_Exception_details
							+ e.getMessage());
		} catch (Exception e) {
			Logger.logMessage(LogLevel.LEVEL_ERROR, module, "Exception:",
					e.getMessage());
			opSuccessFlag=false;
			try {
				businessData.operationSuccess.setLogical(opSuccessFlag);
			} catch (Exception e1) {
				Logger.logMessage(LogLevel.LEVEL_ERROR, module,
						"Opration success:", ""+opSuccessFlag);
			}
			throw new SDKException(Constant.Error_MQTT_Exception_Number,
					Constant.Error_MQTT_Exception_details + e.getMessage());
		}finally{
			try {
				if(client.isConnected())
				{
					client.disconnect();
				}
			} catch (MqttException e) {
				e.printStackTrace();
			}catch (Exception e){
				e.printStackTrace();
			}
		}

	}
	private void createClient(MqttDefaultFilePersistence dataStore,
			MqttConnectOptions connectOptions) throws MqttException,
			MqttSecurityException {
		client = new MqttClient(resourceData.getMQTTurl()[0], clientID,dataStore);
		client.connect(connectOptions);
		Logger.logMessage(LogLevel.LEVEL_DEBUG, module,"MQTT client created");
	}

	private void updateResultData(String publishResult) throws Exception,
			UnsupportedEncodingException {
          /* Code changed for defect 135671
           * - Store result in file.
            */
		 String filePath = businessData.storeResultFile.getValue().toString();
		
			if (businessData.storeResultVar.getValue() == null && filePath.equals("")){
				businessData.storeResultVar.setBlob(publishResult
						.getBytes("UTF-16LE"));
			}
			else if (filePath != null && !filePath.equals("")){
				File file=new File(businessData.storeResultFile.getValue().toString());
				FileOutputStream fos = new FileOutputStream(file);
				fos.write(publishResult.getBytes("UTF-16LE"));
				fos.flush();
				fos.close();
			}
		
	}

	private void updateSuccess(String publishResult,boolean opSuccessFlag) throws Exception {
		if (businessData.operationSuccess != null) {
			if (publishResult != null && opSuccessFlag)
				businessData.operationSuccess.setLogical(success);
			else
				businessData.operationSuccess.setLogical(false);
		}
	}
	private void initData(StepGeneralParams args) throws SDKException {
		resourceData = new ResourceData(args.getResourceObject());
		businessData = new BusinessData(args.getUserProperties());
		environment = new Environment(args.getEnviromentSettings());
		Random r = new Random( System.currentTimeMillis() );
		clientID = args.getFSID()+""+((1 + r.nextInt(2)) * 10000 + r.nextInt(10000));
		Logger.logMessage(LogLevel.LEVEL_DEBUG, module, "Client ID : "
				+ clientID);
	}
	public String publishMessages(byte[] DMpayload) throws SDKException {
		StringBuffer sb = new StringBuffer();
		sb.append("<?xml version=\"1.0\" encoding=\"UTF-16\"?>");
		sb.append("<PublishResult>");
		DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
		DocumentBuilder db = null;

		try {
			db = dbf.newDocumentBuilder();
		} catch (ParserConfigurationException e1) {
			// TODO Auto-generated catch block
			Logger.logMessage(LogLevel.LEVEL_ERROR, module,"publishMessages > ParserConfigurationException", e1.getMessage());
		}
		Document dom = null;
		try {
			dom = db.parse(new ByteArrayInputStream(DMpayload));
		} catch (SAXException e1) {
			Logger.logMessage(LogLevel.LEVEL_ERROR, module,"publishMessages > SAXException", e1.getMessage());
		} catch (IOException e1) {
			Logger.logMessage(LogLevel.LEVEL_ERROR, module,"publishMessages > IOException", e1.getMessage());
		}
		Element docEle = dom.getDocumentElement();
		NodeList nl = docEle.getChildNodes();
		if (nl != null && nl.getLength() > 0) {
			for (int i = 0; i < nl.getLength(); i++) {
				if (nl.item(i).getNodeType() == Node.ELEMENT_NODE) {
					Element el = (Element) nl.item(i);
					if (el.getNodeName().contains("message")) {
						sb.append("<message>");
						String mi="";
						if(el.getElementsByTagName("message_identifier").item(0) != null){
							mi = el.getElementsByTagName("message_identifier").item(0).getTextContent();
						}

						if (mi != null && !mi.equals("")) {
							sb.append("<message_identifier>");
							sb.append(mi);
							sb.append("</message_identifier>");
						}
						
						sb.append("<topic>");
						String topic = el.getElementsByTagName("topic").item(0).getTextContent();
						sb.append(topic);
						sb.append("</topic>");

						String payload = el.getElementsByTagName("payload").item(0).getTextContent();
						
						
						sb.append("<success>");

						try {
							 publish(topic.trim(), businessData.getIntQoS(),DatatypeConverter.parseBase64Binary(payload));
							 sb.append("true");
							 sb.append("</success>");

						} catch (MqttSecurityException e) {
							sb.append("false");
							sb.append("</success>");
							sb.append("<error>");
							sb.append(e.getMessage());
							sb.append("</error>");
							opSuccessFlag=false;
						} catch (MqttException e) {
							sb.append("false");
							sb.append("</success>");
							sb.append("<error>");
							sb.append(e.getMessage());
							sb.append("</error>");
							opSuccessFlag=false;
						}
					}
					sb.append("</message>");
				}
			}
			sb.append("</PublishResult>");
		}
		return sb.toString();
	}
	
	/**
	 * Publishes a message to a topic on the server.
 	 * <p>A convenience method, which will
	 * create a new {@link MqttMessage} object with a byte array payload and the
	 * specified QoS, and then publish it.
	 * </p>
	 *
	 * @param topicName  to deliver the message to, for example "finance/stock/ibm".
	 * @param payload the byte array to use as the payload
	 * @param qos the Quality of Service to deliver the message at.  Valid values are 0, 1 or 2.
	 * @param retainFlag whether or not this message should be retained by the server.
	  */ 

	private void publish(String topicName, double qos, byte[] payload)
			throws MqttSecurityException, MqttException, SDKException {

		// Get an instance of the topic
		MqttTopic topic = client.getTopic(topicName);
		MqttMessage message = new MqttMessage(payload);
		Double dqos = new Double(qos);
		int intQos = dqos.intValue();
		message.setQos(intQos);
		message.setRetained(businessData.isRetainFlag());
		
		// Publish the message
		String time = new Timestamp(System.currentTimeMillis()).toString();
		try {
			topic.publish(message).waitForCompletion(
					resourceData.getPublishtimeOut());
			Logger.logMessage(LogLevel.LEVEL_DEBUG, module,"The following message was published for topic: "
					+ topicName + " Message: " + new String(payload));
			
		} catch (MqttPersistenceException e) {

			throw new SDKException(Constant.Error_MQTT_Persistance_Number,
					Constant.Error_MQTT_Persistance_details);
		} catch (MqttException e) {
			Logger.logMessage(LogLevel.LEVEL_ERROR, module,"Unable to Publish message at: " + time + " to topic \""
					+ topicName + "\" qos " + qos, e.getMessage());
			throw new SDKException(Constant.Error_MQTT_Exception_Number,
					Constant.Error_MQTT_Exception_details);
		} catch (Exception e) {

			throw new SDKException(Constant.Error_MQTT_Exception_Number,
					Constant.Error_MQTT_Exception_details + e.getMessage());
		}
	}
}