package com.magicsoftware.sdk.mqttConnector;

import java.util.HashMap;

public class Environment {
	
	private String persistDir;
	
	public Environment(){
		
	}
	
	public Environment(HashMap<String, String> enviroment){
		persistDir = enviroment.get("PROJECT_DIR") + "MQTT";
	}

	public String getPersistDir() {
		return persistDir;
	}
}
