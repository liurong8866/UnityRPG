using UnityEngine;
using System.Collections;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MakeAudio : MonoBehaviour {

    [ButtonCallFunc()]
    public bool Audio;
    public void AudioMethod() {
#if UNITY_EDITOR
        var allModel = Path.Combine(Application.dataPath, "Resources/sound");
        //var import = ModelImporter.GetAtPath("Assets/Resources/sound") as ModelImporter;
        Debug.Log("allModel "+allModel);
        var resDir = new DirectoryInfo(allModel);
        var fileInfo = resDir.GetFiles("*.wav", SearchOption.AllDirectories);
        AssetDatabase.StartAssetEditing();
        foreach (FileInfo file in fileInfo)
        {
            Debug.Log("Directory name " + file.FullName);
            var ass = file.FullName.Replace(Application.dataPath, "Assets");
            var import = AudioImporter.GetAtPath(ass) as AudioImporter ;
            //Debug.Log("import "+import);

            //import.format = AudioImporterFormat.Compressed;
            var defSamp = new AudioImporterSampleSettings(){
                compressionFormat = AudioCompressionFormat.Vorbis,
                loadType = AudioClipLoadType.CompressedInMemory,
                quality = 1,
            };
            import.defaultSampleSettings = defSamp;
            //import.defaultSampleSettings.compressionFormat = AudioCompressionFormat.Vorbis;
            //import.defaultSampleSettings.loadType = AudioClipLoadType.CompressedInMemory;
            //import.compressionBitrate = 32000;

            //Debug.Log("import "+import.compressionBitrate);
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
