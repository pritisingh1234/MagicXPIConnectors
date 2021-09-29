package com.magicsoftware.sdk.mqttConnector;

import java.security.SecureRandom;
import java.util.HashMap;

import com.magicsoftware.ibolt.commons.logging.LogLevel;
import com.magicsoftware.ibolt.commons.logging.Logger;
import com.magicsoftware.xpi.sdk.UserProperty;

public class ServiceData {

	private String clientID ;
	protected static LogModules module = LogModules.STEP;
	static final String AB = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
	  static SecureRandom rnd = new SecureRandom();
	
	  public ServiceData(HashMap<String, String> serviceData,int bpId,int flowId){
		  //get client ID
		  clientID=serviceData.get("Client ID");

		  //Prefix it with BP ID and Flow ID for uniqueness #135660 
		  clientID=bpId+"_"+flowId+"_"+clientID;
		  
		  Logger.logMessage(LogLevel.LEVEL_DEBUG, module,"Client ID: " + clientID);
		}
	public static String generateClintId(){
	     StringBuilder sb = new StringBuilder( 23 );
	     for( int i = 0; i < 23; i++ ) 
	        sb.append( AB.charAt( rnd.nextInt(AB.length()) ) );
	     return sb.toString();
	  }

	public String getClientID() {
		return clientID;
	}
}
