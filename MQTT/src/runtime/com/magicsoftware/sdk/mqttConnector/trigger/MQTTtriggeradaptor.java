package com.magicsoftware.sdk.mqttConnector.trigger;

import java.io.PrintStream;
import java.util.HashMap;
import java.util.concurrent.TimeUnit;

import org.eclipse.paho.client.mqttv3.IMqttDeliveryToken;
import org.eclipse.paho.client.mqttv3.MqttCallback;
import org.eclipse.paho.client.mqttv3.MqttClient;
import org.eclipse.paho.client.mqttv3.MqttConnectOptions;
import org.eclipse.paho.client.mqttv3.MqttException;
import org.eclipse.paho.client.mqttv3.MqttMessage;
import org.eclipse.paho.client.mqttv3.MqttSecurityException;
import org.eclipse.paho.client.mqttv3.persist.MqttDefaultFilePersistence;

import com.magicsoftware.sdk.mqttConnector.Constant;
import com.magicsoftware.sdk.mqttConnector.Environment;
import com.magicsoftware.sdk.mqttConnector.LogModules;
import com.magicsoftware.sdk.mqttConnector.ResourceData;
import com.magicsoftware.sdk.mqttConnector.ServiceData;
import com.magicsoftware.xpi.sdk.SDKException;
import com.magicsoftware.xpi.sdk.trigger.TriggerGeneralParams;
import com.magicsoftware.xpi.sdk.trigger.external.FlowLauncher;
import com.magicsoftware.xpi.sdk.trigger.external.IExternalTrigger;
import com.magicsoftware.ibolt.commons.logging.LogLevel;
import com.magicsoftware.ibolt.commons.logging.Logger;


public class MQTTtriggeradaptor implements IExternalTrigger{
	
	
	private ResourceData resourceData = null;
	private BusinessDataTrigger businessData = null;
	private Environment environment;
	private  MqttDefaultFilePersistence dataStore=null;
	private  MqttConnectOptions connectOptions=null;
	private ServiceData serviceData=null;
	private MqttClient client;
	HashMap<String, Object> args=null;
	PrintStream p = null;
	
	protected LogModules module = LogModules.TRIGGER;

	@Override
	public void load(TriggerGeneralParams triggergeneralparams,FlowLauncher flowlauncher) throws SDKException {
		try{
		 initData(triggergeneralparams,flowlauncher);
         dataStore = new MqttDefaultFilePersistence(environment.getPersistDir());
		 connectOptions = resourceData.getConnecitonOptionsData();
		
		MQTTtriggeradaptor.createCallback in = this.new createCallback();
		  in.createclient(dataStore, connectOptions);
		}
		catch (MqttSecurityException e) {
			Logger.logMessage(LogLevel.LEVEL_ERROR, module,
					"MqttSecurityException:", e.getMessage());
			throw new SDKException(
					Constant.Error_MQTT_Connection_security_Exception_Number,
					Constant.Error_MQTT_Connection_security_Exception_details
							+ e.getMessage());
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
	
    private void initData(TriggerGeneralParams args,FlowLauncher flowlauncher) throws SDKException {
    
    	resourceData = new ResourceData(args.getResourceObject());
		businessData = new BusinessDataTrigger(args.getConfigurations(),flowlauncher);
		environment = new Environment(args.getEnviromentSettings());
		serviceData =  new ServiceData(args.getServiceObject(),flowlauncher.getBpId(),flowlauncher.getFlowId());
	}
    
    class createCallback implements MqttCallback
    	{
		@Override
		public void connectionLost(Throwable arg0) {
			Logger.logMessage(LogLevel.LEVEL_ERROR, module,"MQTT connection lost. Attempting to reconnect... " +  arg0.getMessage());
			Logger.logMessage(LogLevel.LEVEL_DEBUG, module,"Disconnecting client..." +arg0.getMessage());
			
				try {
					DisconnectClient();
				} catch (SDKException e1) {
					Logger.logMessage(LogLevel.LEVEL_ERROR, module,"** connectionLost - failed to disconnect client:" + e1.getMessage());			
				}
			client=null;
			Logger.logMessage(LogLevel.LEVEL_DEBUG, module,"Recreating client..." +arg0.getMessage());
			try {
				businessData.reConnect(dataStore,connectOptions);
				Logger.logMessage(LogLevel.LEVEL_DEBUG, module,"After client.connect(). client.isConnected()=" +client.isConnected());
			}
			catch (SDKException e) {
				long endTime = System.nanoTime() + TimeUnit.NANOSECONDS.convert(10L, TimeUnit.MINUTES);
            	
				int delay = 1000;
				int attempt=1;
            	while ( System.nanoTime() < endTime ){
            		delay = delay*2;
            		try {
						Thread.sleep(delay);
					} catch (InterruptedException e2) {
						e2.printStackTrace();
					};
            		Logger.logMessage(LogLevel.LEVEL_ERROR, module,"Trying to reconnect"+" "+"attempts "+attempt);
            		attempt++;
            		try {
            			if(businessData.reConnect(dataStore,connectOptions)) 
            			{
            				break;
            			}
					} catch (SDKException e1) {
						e1.printStackTrace();
					}
              	}
				Logger.logMessage(LogLevel.LEVEL_ERROR, module,e.getMessage());
			}
		}

		@Override
		public void messageArrived(String topic, MqttMessage msg)
				throws Exception {
			try{
				String message = new String(msg.getPayload());
				
				args = new HashMap<String, Object>();
				args.put("message", message.getBytes());
				args.put("topic", topic);
				Logger.logMessage(LogLevel.LEVEL_TRACE, module,"Payload :" +message.getBytes());
				Logger.logMessage(LogLevel.LEVEL_DEBUG, module,"Topic:" +topic);
				}
		        catch (Exception e) {
					
					Logger.logMessage(LogLevel.LEVEL_ERROR, module,e.getMessage());
				}
				
				try {
					
					Logger.logMessage(LogLevel.LEVEL_DEBUG, module,"** Triggering the flow topic:" +topic);
					businessData.getF1().invoke(args);
				} catch (Exception e) {
					Logger.logMessage(LogLevel.LEVEL_ERROR, module,e.getMessage());
				}
		}
		
		public void DisconnectClient() throws SDKException 
	    {
	    	if(client!=null && client.isConnected())
	    	{
	    		Logger.logMessage(LogLevel.LEVEL_DEBUG, module,"Disconnecting client...");
	    		try {
					client.disconnect();
				} catch (MqttException e) {
					Logger.logMessage(LogLevel.LEVEL_ERROR, module,"Unable to disconnect client: " +e.getMessage());
					throw new SDKException(Constant.Error_MQTT__Connection_Disconnect_Number,Constant.Error_MQTT__Connection_Disconnect_details +e.getMessage());
					}
	    	}
	    }

		@Override
		public void deliveryComplete(IMqttDeliveryToken arg0) {
			Logger.logMessage(LogLevel.LEVEL_DEBUG, module,"DeliveryComplete..." +arg0);
			
		}
		
		public void createclient(MqttDefaultFilePersistence dataStore,
				MqttConnectOptions connectOptions) throws MqttException,
				MqttSecurityException, SDKException {
				client = new MqttClient(resourceData.getMQTTurl()[0], serviceData.getClientID(),dataStore);
				client.setCallback(this);
				client.connect(connectOptions);
				businessData.setClient(client);
				
		       try {
					businessData.subscribe(businessData.getTopics(), businessData.getQosArray());
				} catch (Exception e) {
					Logger.logMessage(LogLevel.LEVEL_ERROR, module, "Exception:",
							e.getMessage());
					throw new SDKException(Constant.Error_MQTT_Exception_Number,
							Constant.Error_MQTT_Exception_details + e.getMessage());
					
				}
				Logger.logMessage(LogLevel.LEVEL_DEBUG, module,"MQTT client created");
			}
   }
	
	@Override
	public void unload() {
		System.out.println("^^^^^^^ Unload - Disconnecting MQTT client ^^^^^^^^^^");
		try {
			businessData.DisconnectClient();
		} catch (SDKException e) {
			Logger.logMessage(LogLevel.LEVEL_ERROR, module,e.getMessage());
		}
	}
	
	@Override
	public void disable() {
	}

	@Override
	public void enable() {
	}
	
	public MqttDefaultFilePersistence getDataStore() {
		return dataStore;
	}

	public MqttConnectOptions getConnectOptions() {
		return connectOptions;
	}
}
