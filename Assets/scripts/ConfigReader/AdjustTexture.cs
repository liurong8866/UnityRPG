using UnityEngine;
using System.Collections;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AdjustTexture : MonoBehaviour {
    public string directoryName;
    [ButtonCallFunc()]
    public bool Adjust;

    public void AdjustMethod() {
#if UNITY_EDITOR
        var texDir = Path.Combine(Application.dataPath,  directoryName);
        var resDir = new DirectoryInfo(texDir);
        FileInfo[] fileInfo = resDir.GetFiles("*.png", SearchOption.AllDirectories);
        AssetDatabase.StartAssetEditing();

        foreach(FileInfo file in fileInfo) {
            var realPath = file.FullName.Replace(Application.dataPath, "");
            var ass = "Assets"+realPath;
            var import = TextureImporter.GetAtPath(ass) as TextureImporter;
            import.mipmapEnabled = false;
            //import.globalScale = 1;
            //import.importAnimation = true;
            //import.animationType = ModelImporterAnimationType.Legacy;
            //var ani = import.clipAnimations[0];
            
            AssetDatabase.WriteImportSettingsIfDirty(ass);
        }
        AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh();
#endif
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
