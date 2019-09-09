using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public static class AutoBuilder {
	static string APPNAME = "test";
	static string TARGET = "target";
	static string [] CollectScenePaths() {
		string[] scenes = new string[EditorBuildSettings.scenes.Length];
		for (int i = 0; i < scenes.Length; i++) {
			scenes[i] = EditorBuildSettings.scenes[i].path;
		}
		return scenes;
	}
	
	[MenuItem("File/AutoBuilder/iOS")]
	static void PerformiOSBuild() {
		PlayerSettings.productName = "test";
		PlayerSettings.bundleIdentifier = "com.chumeng.test";
		PlayerSettings.bundleVersion = "1.0";
		BuildOptions opt = BuildOptions.Development | BuildOptions.SymlinkLibraries ;
		string targetDir = TARGET+"/ios";
		string error = BuildPipeline.BuildPlayer (CollectScenePaths(), targetDir, BuildTarget.iOS, opt );
		if (error != null && error.Length > 0) {
			throw new IOException("Build Failed: "+error);
		}

	}
	[MenuItem("File/AutoBuilder/Windows")]
	static void PerformWindowsBuild() {
		PlayerSettings.productName = "test";
		PlayerSettings.bundleIdentifier = "com.chumeng.test";
		PlayerSettings.bundleVersion = "1.0";
		BuildOptions opt = BuildOptions.Development | BuildOptions.SymlinkLibraries ;
		string targetDir = TARGET+"/windows";
		string error = BuildPipeline.BuildPlayer (CollectScenePaths(), targetDir, BuildTarget.StandaloneWindows64, opt );
		if (error != null && error.Length > 0) {
			throw new IOException("Build Failed: "+error);
		}

	}
}
