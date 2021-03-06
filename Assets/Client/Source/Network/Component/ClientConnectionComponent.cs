﻿using System.Collections;
using System.Collections.Generic;
using Entitas;
using System.Net;

[Network]
public class ClientConnectionComponent : IComponent {

	public enum State { 
		Disconnected,
		Initalize,
		Connected,
		TearDown
	}

	State state_ = State.Disconnected;
	public State state {
		get { return state_; }
		set {
			state_ = value;
			stateTime = 0.0f;
			packetUnreceivedTime = 0.0f;
			timeoutCounter = 0.0f;
			retryTime = 0;
		}
	}

	public float stateTime = 0.0f;
	public float packetUnreceivedTime = 0.0f;
	public IPEndPoint serverEP; // This only changes in Disconnected state

	public PlayerMetadata playerMetadata; // Player metadata.

	public float timeoutCounter; // Generic counter for state timeout operation.
	public float keepAliveCounter;
	public int retryTime = 0; // Generic retry time counter for state operation.

}


