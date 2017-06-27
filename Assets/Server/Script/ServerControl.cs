using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

public class ServerControl : MonoBehaviour {

	Systems systems;

	void Start() {
		NetworkConfig.port = 12345;

		var ctxs = Contexts.sharedInstance;
		systems = new Systems()
			.Add(new NetworkingFeature(ctxs))
			.Add(new ServerConnectionFeature(ctxs))
			.Add(new ServerFrameSyncFeature(ctxs));

		systems.Initialize();
	}
	
	void FixedUpdate() {
		systems.Execute();
		systems.Cleanup();
	}

	void OnApplicationQuit() {
		systems.TearDown();
	}
}

