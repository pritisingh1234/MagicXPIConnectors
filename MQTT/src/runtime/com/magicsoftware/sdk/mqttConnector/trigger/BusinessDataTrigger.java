package com.magicsoftware.sdk.mqttConnector.trigger;

import java.io.StringReader;
import java.security.SecureRandom;
import java.util.HashMap;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;

import org.eclipse.paho.client.mqttv3.MqttClient;
import org.eclipse.paho.client.mqttv3.MqttConnectOptions;
import org.eclipse.paho.client.mqttv3.MqttException;
import org.eclipse.paho.client.mqttv3.MqttSecurityException;
import org.eclipse.paho.client.mqttv3.persist.MqttDefaultFilePersistence;
import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;
import org.xml.sax.InputSource;

import com.magicsoftware.sdk.mqttConnector.Constant;
import com.magicsoftware.xpi.sdk.SDKException;
import com.magicsoftware.xpi.sdk.UserProperty;
import com.magicsoftware.xpi.sdk.trigger.external.FlowLauncher;
import com.magicsoftware.sdk.mqttConnector.LogModules;
import com.magicsoftware.ibolt.commons.logging.LogLevel;
import com.magicsoftware.ibolt.commons.logging.Logger;

public class BusinessDataTrigger{ 

	private MqttClient client;
	private String [] topics;
	private  int [] qosArray;
	boolean retrying = false;
	private FlowLauncher f1;
	private int subscribeatmpt=1;
	
	protected LogModules module = LogModules.TRIGGER;
	static final String AB = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
	 static SecureRandom rnd = new SecureRandom();
	
	BusinessDataTrigger(HashMap<String, UserProperty> args,FlowLauncher flowlauncher)
	{
		DocumentBuilderFactory dbFactory = DocumentBuilderFactory.newInstance();
		
		try{
			this.f1=flowlauncher;
		String delimitedlistOfTopicsAndQoS = args.get("ListOfTopicsAndQoS").getValue().toString();
       
		DocumentBuilder dBuilder = dbFactory.newDocumentBuilder();
		InputSource is = new InputSource();
	    is.setCharacterStream(new StringReader(delimitedlistOfTopicsAndQoS));
		Document doc = dBuilder.parse(is);
		doc.getDocumentElement().normalize();			
		NodeList nList = doc.getElementsByTagName("TopicAndQoS");

		topics=new String[nList.getLength()]; 
		qosArray=new int[nList.getLength()];
		
		for (int temp = 0; temp < nList.getLength(); temp++) {

			Node nNode = nList.item(temp);
					
			if (nNode.getNodeType() == Node.ELEMENT_NODE) {
				
				Element eElement = (Element) nNode;
				topics[temp]=eElement.getElementsByTagName("Topic").item(0).getTextContent();
				qosArray[temp]=Integer.parseInt(eElement.getElementsByTagName("QoS").item(0).getTextContent());
				Logger.logMessage(LogLevel.LEVEL_DEBUG, module,"QOS :" + qosArray[temp]);
			}   
		}
		}
		catch(Exception e)
		{
			Logger.logMessage(LogLevel.LEVEL_ERROR, module,e.getMessage());
		}
	}
	
	public FlowLauncher getF1() {
		return f1;
	}

	public String[] getTopics() {
		return topics;
	}

	public int[] getQosArray() {
		return qosArray;
	}
	
	public MqttClient getClient() {
		return client;
	}

	public void setClient(MqttClient client) {
		this.client = client;
	}
	
	public void subscribe(String [] topicsNames, int [] qos) throws SDKException  {
    	
		// Subscribe to the topic
    	Logger.logMessage(LogLevel.LEVEL_DEBUG, module,"Subscribing to topics \""+topicsNames[0]+"\" qos :" +qos[0]);
    	Logger.logMessage(LogLevel.LEVEL_DEBUG, module," Subscription attempts: "+subscribeatmpt++);
    	try {
			client.subscribe(topicsNames, qos);
		} catch (MqttSecurityException e) {
			// TODO Auto-generated catch block
			Logger.logMessage(LogLevel.LEVEL_ERROR, module,"Unable to subscribe security exception: " +e.getMessage());
			throw new SDKException(Constant.Error_MQTT_Subscribe_Connection_security_Exception_Number,Constant.Error_MQTT_Subscribe_Connection_security_Exception_details+e.getMessage());
		} catch (MqttException e) {
			// TODO Auto-generated catch block
			Logger.logMessage(LogLevel.LEVEL_ERROR, module,"Unable to subscribe: " +e.getMessage());
			throw new SDKException(Constant.Error_MQTT_Connection_Exception_Number,Constant.Error_MQTT_Connection_Exception_details+e.getMessage());
		}
    }
	
	public void DisconnectClient() throws SDKException {
    	if(client!=null && client.isConnected())
    	{
    		Logger.logMessage(LogLevel.LEVEL_DEBUG, module,"Disconnecting Client...");
    		try {
				client.disconnect();
			} catch (MqttException e) {
				
				Logger.logMessage(LogLevel.LEVEL_ERROR, module,"Unable to disconnect client: " +e.getMessage());
				throw new SDKException(Constant.Error_MQTT__Connection_Disconnect_Number,
						Constant.Error_MQTT__Connection_Disconnect_details +e.getMessage());
				}
    	}
    }

	public boolean reConnect(MqttDefaultFilePersistence dataStore,MqttConnectOptions connectOptions) throws SDKException{
        boolean connectSuccess = false;
        while (!connectSuccess) {
           
                          try {
                            if ( !client.isConnected()) {	
                            	client.connect(connectOptions);
                                connectSuccess = true;
                            	} 
                            if (connectSuccess) {
                    			try {
                    				
                    				subscribe(topics, qosArray);
                    				
                    				break;
                    			} catch (Exception e ) {
                    				DisconnectClient();
                    				Logger.logMessage(LogLevel.LEVEL_ERROR, module, "MqttException:",
                        					e.getMessage());
                        		throw new SDKException(
                        					Constant.Error_MQTT_Connection_Exception_Number,
                        					Constant.Error_MQTT_Connection_Exception_details
                        							+ e.getMessage());
                    			}			
                    		}	
                        } catch (MqttException e) {
                			Logger.logMessage(LogLevel.LEVEL_ERROR, module, "MqttException:",
                					e.getMessage());
                		throw new SDKException(
                					Constant.Error_MQTT_Connection_Exception_Number,
                					Constant.Error_MQTT_Connection_Exception_details
                							+ e.getMessage());
                				} catch (Exception e) {
                			Logger.logMessage(LogLevel.LEVEL_ERROR, module, "Exception:",
                					e.getMessage());
                			throw new SDKException(Constant.Error_MQTT_Exception_Number,
                					Constant.Error_MQTT_Exception_details + e.getMessage());
                		}
                    }
        return connectSuccess;
         }
}