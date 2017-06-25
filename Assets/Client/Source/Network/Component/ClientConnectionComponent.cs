using System.Collections;
using System.Collections.Generic;
using Entitas;
using System.Net;

[Network]
public class ClientConnectionComponent : IComponent {

	public enum State { 
		// Default state when start. Goto initialize when player chooses to connect.
		Disconnected,
		// Handshaking semi-stable state. Waits for server SyncAck packet and if
		//  times out, resending Sync packet multiple times, eventually going back to Disconnected state.
		//  If SyncAck is received, goto Connected.
		Initalize,
		// Connected state where data packet exchange is allowed to happen.
		// Goto Disconnected when passively disconnected, i.e. 
		// 1) Time out, 2) Server kick player or close message
		Connected,
		// Teardown semi-stable state waiting for FinAck packet to fully close.
		//  or if the state time outs, goto Disconnected state.
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
	public float timeoutCounter; // Generic counter for state timeout operation.
	public int retryTime = 0; // Generic retry time counter for state operation.

}


