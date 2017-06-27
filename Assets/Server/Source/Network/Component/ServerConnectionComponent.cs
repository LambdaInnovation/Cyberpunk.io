using System.Collections;
using System.Collections.Generic;
using Entitas;
using System.Net;

[Network]
public class ServerConnectionComponent : IComponent {

	public enum State {
		Initialize,
		
		// Destroy the connection when passively disconnect.
		// Goto TearDown when actively disconnect.
		Connected,

		// Semi-stable state waiting for another FinAck packet to fully close.
		// or if the state time outs, destroy the connection.
		TearDown
	}

	State state_ = State.Connected;
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
	public float timeoutCounter; // Generic counter for state timeout operation.
	public int retryTime = 0; // Generic retry time counter for state operation.
	public float keepAliveCounter;

	public PlayerMetadata playerMetadata; // Player metadata.
	public IPEndPoint clientEP; // Doesn't change after initialized.

	public GameEntity playerView;

}
