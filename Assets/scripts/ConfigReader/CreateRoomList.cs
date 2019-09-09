using UnityEngine;
using System.Collections;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CreateRoomList : MonoBehaviour {
    [ButtonCallFunc()]
    public bool Create;
    public void CreateMethod(){
#if UNITY_EDITOR
        /*
        var roomPath = Path.Combine(Application.dataPath, "Resources/room");
        var resDir = new DirectoryInfo(roomPath);
        FileInfo[] fileInfo = resDir.GetFiles("*.prefab", SearchOption.AllDirectories); 
        GameObject g = new GameObject("RoomList");
        var list = g.AddComponent<RoomList>();

        foreach (FileInfo f in fileInfo)
        {
            Debug.Log("fileName " + f.FullName);
            Debug.Log("DataPath " + Application.dataPath);
            var pa = f.FullName.Replace(Application.dataPath, "Assets");
            
            var pre = Resources.LoadAssetAtPath<GameObject>(pa);
            list.roomPieces.Add(pre);
        }
        var tar = Path.Combine("Assets", "Resources/RoomList.prefab");
        Log.Sys("tar path is "+tar);
        PrefabUtility.CreatePrefab(tar, g);
        */
#endif
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
