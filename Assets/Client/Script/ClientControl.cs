﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using Entitas;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ClientControl : MonoBehaviour {

	public PlayerSettings playerSettings;

	Systems systems;

	void Start () {
		NetworkConfig.port = 0; // Auto-assign

		var ctxs = Contexts.sharedInstance;
		systems = new Systems()
			// Network
			.Add(new CleanupSystem(ctxs))
			.Add(new PingTestSystem(ctxs))
			.Add(new NetworkingFeature(ctxs))
			.Add(new ClientConnectionFeature(ctxs))
			// Game
			.Add(new ClientFrameSyncFeature(ctxs))
			.Add(new ClientPlayerFeature(ctxs, playerSettings))
			.Add(new ViewFeature(ctxs));

		systems.Initialize();

		
		// ctxs.network.CreateEntity().AddPingTest(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345), 1000, 1); 

		ctxs.network.CreateEntity()
			.AddComponent(NetworkComponentsLookup.ClientConnection, new ClientConnectionComponent());
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
	string startConnectonHost = "127.0.0.1";
	int startConnectionPort = 12345;
	string playerName = "Unnamed";
	
	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		
		if (Application.isPlaying) {
			startConnectonHost = EditorGUILayout.TextField("Host", startConnectonHost);
			startConnectionPort = EditorGUILayout.IntField("Port", startConnectionPort);
			playerName = EditorGUILayout.TextField("Player", playerName);

			var ctx = Contexts.sharedInstance.network;
			if (GUILayout.Button("Start Connection")) {
				ctx.CreateEntity()
					.AddComponent(NetworkComponentsLookup.ClientStartConnection, new ClientStartConnectionComponent {
						serverEP = new IPEndPoint(IPAddress.Parse(startConnectonHost), startConnectionPort),
						playerMetadata = new PlayerMetadata {
							playerName = playerName
						}
					});
			}

			if (GUILayout.Button("Disconnect")) {
				var ent = ctx.CreateEntity();
				ent.isClientDisconnect = true;
				ent.isCleanup = true;
			}
		}
	}

}

#endif // UNITY_EDITOR