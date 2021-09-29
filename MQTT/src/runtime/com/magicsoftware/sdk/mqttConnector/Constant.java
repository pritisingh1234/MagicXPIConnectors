package com.magicsoftware.sdk.mqttConnector;

public class Constant {
	
	public static final boolean	CLEAN_SESSION_DEFAULT =	true;
	public static final int	CONNECTION_TIMEOUT_DEFAULT =30;
	public static final int	KEEP_ALIVE_INTERVAL_DEFAULT	=60;
	public static final int	MQTT_RETRY_INTERVAL	=60;
	
	protected static final int URI_TYPE_TCP = 0;
	protected static final int URI_TYPE_SSL = 1;
	protected static final int URI_TYPE_LOCAL = 2; 
	
	public static final String GOOGLE="www.google.com";
	
	
	public static final int	Error_MQTT_Connection_security_Exception_Number	= 100;
	public static final int	Error_MQTT_Connection_Exception_Number	=101;
	public static final int	Error_MQTT_Publish_Connection_security_Exception_Number	=102;
	public static final int	Error_MQTT_Subscribe_Connection_security_Exception_Number	=103;
	public static final int	Error_Invalid_Resource_Parameter_Number	=104;
	public static final int	Error_MQTT_Resource_Number	=105;
	public static final int	Error_MQTT_Persistance_Number=106;
	public static final int	Error_MQTT_Exception_Number	=107;
	public static final int	Error_MQTT__Connection_Disconnect_Number=108;
	
	
	public static final String	Error_MQTT_Resource_Details	="An MQTT Resource is not available. The Resource object is null: ";
	public static final String	Error_MQTT_Exception_details="MQTT Exception: ";
	public static final String	Error_MQTT_Connection_Exception_details="MQTT Connection Exception:";
	public static final String	Error_MQTT_Connection_security_Exception_details="MQTT Connection Security Exception: ";
	public static final String	Error_MQTT_Persistance_details="MQTT Persistence Exception: ";
	public static final String	Error_Invalid_Resource_Parameter_details="Invalid resource parameter";
	public static final String  Error_MQTT_Publish_Connection_security_Exception_details	="MQTT Publish Connection Security Exception:";
	public static final String  Error_MQTT_Subscribe_Connection_security_Exception_details="MQTT Subscribe Connection Security Exception:";
	public static final String 	Error_MQTT__Connection_Disconnect_details="MQTT Unable to Disconnect Client Exception:";
	
	
	

}
