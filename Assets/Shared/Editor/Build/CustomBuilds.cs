using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class CustomBuilds {
	static string[] clientScenes = new string[] {
		// Assets/Test.unity",
		"Assets/Client/Scene/ClientInit.unity"
	};

	static string[] serverScenes = new string[] {
		"Assets/Server/Scene/ServerInit.unity"
	};

	[MenuItem("Build/Build Client (WebGL)")]
	static void BuildClientWebGL() {
		ExecuteBuild(clientScenes, "/", BuildTarget.WebGL, BuildOptions.None);
	}

	[MenuItem("Build/Build Client (Win)")]
	static void BuildClientWin() {
		ExecuteBuild(clientScenes, "/cybio.exe", BuildTarget.StandaloneWindows, BuildOptions.None);
	}

	[MenuItem("Build/Build Game Server (Windows)")]
	static void BuildServerWin() {
		ExecuteBuild(serverScenes, "/cybio-server.exe", BuildTarget.StandaloneWindows, BuildOptions.None);
	}

	[MenuItem("Build/Build Game Server (Linux)")]
	static void BuildServerLinux() {
		ExecuteBuild(serverScenes, "/cybio-server", BuildTarget.StandaloneLinux, BuildOptions.None);
	}

	static void ExecuteBuild(string[] scenes, string pathPostfix, BuildTarget buildTarget, BuildOptions buildOptions) {
		var path = GetSaveFolder();

		/*
		foreach (var path2 in EditorBuildSettings.scenes) {
			Debug.Log(path2.path);
		} */

		var options = new BuildPlayerOptions();
		options.scenes = scenes;
		options.locationPathName = path + pathPostfix;
		options.target = buildTarget;
		options.options = buildOptions;

		BuildPipeline.BuildPlayer(options);
	}

	static string GetSaveFolder() {
		return EditorUtility.SaveFolderPanel("Choose Location", "Build", "");
	}
	
}
