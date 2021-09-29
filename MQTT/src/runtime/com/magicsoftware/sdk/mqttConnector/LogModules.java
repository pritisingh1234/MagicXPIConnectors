package com.magicsoftware.sdk.mqttConnector;

import com.magicsoftware.ibolt.commons.logging.ILogModules;

public enum LogModules implements ILogModules{

	STEP 	("magicxpi.component.mqtt.step"),
	TRIGGER	("magicxpi.component.mqtt.trigger");

	private final String stringValue;
	private LogModules(final String matchStringValue) {
		this.stringValue = matchStringValue;
	}
 
	@Override
	public String description() {
		return stringValue;
	}

}
