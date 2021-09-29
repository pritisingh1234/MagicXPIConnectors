package com.magicsoftware.sdk.mqttConnector.step;

import java.util.HashMap;

import com.magicsoftware.ibolt.commons.logging.LogLevel;
import com.magicsoftware.ibolt.commons.logging.Logger;
import com.magicsoftware.sdk.mqttConnector.LogModules;
import com.magicsoftware.xpi.sdk.UserProperty;

public class BusinessData {

	private double intQoS = 0;
	private boolean retainFlag = false;
	
	UserProperty operationSuccess = null;
	UserProperty storeResultVar = null;
	UserProperty storeResultFile = null;
	UserProperty QoS = null;
	protected LogModules module = LogModules.STEP;

	public BusinessData(HashMap<String, UserProperty> args) {
		operationSuccess = args.get("operationSuccess");
		storeResultVar = args.get("storeResultVar");
		storeResultFile = args.get("storeResultFile");
		QoS = args.get("QoS");
		try {
			intQoS = Double.valueOf(QoS.getValue().toString());
		} catch (NumberFormatException e) {
			Logger.logMessage(LogLevel.LEVEL_ERROR, module,"Unable to convert QoS to a number. Setting QoS to 0. " + e.getMessage());
		}

		UserProperty retain = args.get("Retain");
		retainFlag = (Boolean) retain.getValue();
		Logger.logMessage(LogLevel.LEVEL_DEBUG, module,"QOS :" + intQoS + " " + "Retain Flag" + retainFlag);
	}
	public double getIntQoS() {
		return intQoS;
	}

	public boolean isRetainFlag() {
		return retainFlag;
	}
}
