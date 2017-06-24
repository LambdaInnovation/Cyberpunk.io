using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using Entitas;

public class ClientControl : MonoBehaviour {
	Systems systems;

	void Start () {
		NetworkConfig.port = 0; // Auto-assign

		var ctxs = Contexts.sharedInstance;
		systems = new Systems()
			.Add(new NetworkingFeature(ctxs))
			.Add(new PingTestSystem(ctxs));
		
		#if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
		var observer = new Entitas.VisualDebugging.Unity.ContextObserver(ctxs.game);
		GameObject.DontDestroyOnLoad(observer.gameObject);
		#endif

		systems.Initialize();

		ctxs.game.CreateEntity()
			.AddPingTest(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345), 1000, 1);
	}
	
	void FixedUpdate() {
		systems.Execute();
		systems.Cleanup();
	}

	void OnApplicationQuit() {
		systems.TearDown();
	}
}
