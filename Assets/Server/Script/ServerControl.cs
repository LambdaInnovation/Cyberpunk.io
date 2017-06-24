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
			.Add(new NetworkingFeature(ctxs));
		
		#if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
		var observer = new Entitas.VisualDebugging.Unity.ContextObserver(ctxs.game);
		GameObject.DontDestroyOnLoad(observer.gameObject);
		#endif

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
