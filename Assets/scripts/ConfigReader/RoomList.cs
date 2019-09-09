using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif
[System.Serializable]
public class TypeParts
{
    public string type;
    public List<GameObject> parts = new List<GameObject>();
}

public class RoomList : MonoBehaviour
{
	public List <TypeParts> typeParts = new List<TypeParts>();

    static Dictionary<string, Dictionary<string, GameObject>> nameToObj;
    static RoomList Instance;

    /// <summary>
    /// 初始化根据名字 类型 映射到GameObject 
    /// </summary>
    void InitNameToObj()
    {
        var elements = new HashSet<string>()
        {
            "ENTRANCE",
            "EXIT",
            "EW",
            "S",
            "NE",
            "NS",
            "NW",
            "PB",
            "LM",
            "W",
            "E",
            "KG",
            "SW",
            "SE",
            "N",
            "TOWN",
            "JD",
        };

        nameToObj = new Dictionary<string, Dictionary<string, GameObject>>();
        foreach (var t in typeParts)
        {
            var dic = nameToObj [t.type] = new Dictionary<string, GameObject>();
            foreach (var part in t.parts)
            {
                var ele = part.name.ToUpper().Split(char.Parse("_"));
                var eleStr = "";
                bool isFirst = true;
                foreach (var e in ele)
                {
                    if (elements.Contains(e))
                    {
                        if (isFirst)
                        {
                            isFirst = false;
                            eleStr += e;
                        } else
                        {
                            eleStr += "_" + e;
                        }
                    }
                }
                dic [eleStr] = part;
            }
        }
    }
    public static GameObject GetStaticObj(string type, string name) {
        if(Instance == null) {
            var g= Resources.Load<GameObject>("RoomList");
            Instance = g.GetComponent<RoomList>();
        }    
        return Instance.GetObj(type, name);
    }

    GameObject GetObj(string type, string name)
    {
        if(nameToObj == null) {
            InitNameToObj();
        }
        Log.Sys("GetPiece "+type+" name "+name);
        return nameToObj[type][name];
    }

    //public List<GameObject> roomPieces = new List<GameObject>();
    [ButtonCallFunc()] public bool LoadAllRooms;

    public void LoadAllRoomsMethod()
    {
        //roomPieces.Clear();
        typeParts.Clear();

        var prefabList = new DirectoryInfo(Path.Combine(Application.dataPath, "Resources/room"));
        var dirInfo = prefabList.GetDirectories();

        FileInfo[] fileInfo = prefabList.GetFiles("*.prefab", SearchOption.TopDirectoryOnly);
        var tp1 = Contains("mine");
        foreach (var p in fileInfo)
        {
            var n = p.FullName.Replace(Application.dataPath, "Assets");
			Log.Sys("AddRoom "+n);
            var go = AssetDatabase.LoadAssetAtPath<GameObject>(n);
            tp1.parts.Add(go);
        }

        foreach (var dir in dirInfo)
        {
            var finfo = dir.GetFiles("*.prefab", SearchOption.TopDirectoryOnly);
            var tp = Contains(dir.Name);
			Log.Sys("AddType "+tp.type);
            foreach (var p in finfo)
            {
                var n = p.FullName.Replace(Application.dataPath, "Assets");
				Log.Sys("AddRoom "+n);
                var go = AssetDatabase.LoadAssetAtPath<GameObject>(n);
                tp.parts.Add(go);
                
            }
        }
		Debug.Log("RoomListLength "+typeParts.Count);
#if UNITY_EDITOR
        //EditorUtility.SetDirty(gameObject);
        EditorUtility.SetDirty(this);
        AssetDatabase.Refresh();

#endif
    }

    TypeParts  Contains(string type)
    {
        foreach (var t in typeParts)
        {
            if (t.type == type)
            {
                return t;
            }
        }

        var tp = new TypeParts();
        tp.type = type;
        typeParts.Add(tp);

        return tp;
    }

    public void AddRoom(string type, GameObject g)
    {
        //roomPieces.Add(g);
        var roomPieces = Contains(type);
        roomPieces.parts.Add(g);
    }
	
}
