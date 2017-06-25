using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using Entitas;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ClientControl : MonoBehaviour {
	Systems systems;

	void Start () {
		NetworkConfig.port = 0; // Auto-assign

		var ctxs = Contexts.sharedInstance;
		systems = new Systems()
			.Add(new PingTestSystem(ctxs))
			.Add(new NetworkingFeature(ctxs))
			.Add(new ClientConnectionSystem(ctxs));

		systems.Initialize();

		/*
		ctxs.network.CreateEntity()
			.AddPingTest(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345), 1000, 1); */

		ctxs.network.CreateEntity()
			.AddClientConnection(0, 0, null, 0, 0, ClientConnectionComponent.State.Disconnected);
	}
	
	void FixedUpdate() {
		systems.Execute();
		systems.Cleanup();
	}

	void OnApplicationQuit() {
		systems.TearDown();
	}
}

#if UNITY_EDITOR


[CustomEditor(typeof(ClientControl))]
[CanEditMultipleObjects]
public class ClientControlEditor : Editor {
	string startConnectonHost = "";
	int startConnectionPort = 12345;
	
	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		
		if (Application.isPlaying) {
			startConnectonHost = EditorGUILayout.TextField("Host", startConnectonHost);
			startConnectionPort = EditorGUILayout.IntField("Port", startConnectionPort);

			if (GUILayout.Button("Start Connection")) {
				var ctx = Contexts.sharedInstance.network;
				ctx.CreateEntity()
					.AddClientStartConnection(new IPEndPoint(IPAddress.Parse(startConnectonHost), startConnectionPort));
			}
		}
	}

}

#endif // UNITY_EDITOR