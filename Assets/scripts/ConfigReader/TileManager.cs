
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/

/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System;
using System.IO;
using SimpleJSON;
using System.Collections.Generic;
using UnityEditor;

/*
 * Configure Level Map Block  
 * Floor and Wall Position
 */
namespace MyLib 
{
    public class TileManager : MonoBehaviour
    {

        [ButtonCallFunc()]public bool Sort;

        public void SortMethod()
        {
            var g = transform.Find("RoomPieces_data");
            var count = 0;
            foreach (Transform t in g)
            {
                count++;
                t.localPosition = Vector3.zero;
                foreach (Transform child in t)
                {
                    float x = child.localPosition.x;
                    int xc = Mathf.RoundToInt(x / 4.0f) * 4;
                    float z = child.localPosition.z;
                    int zc = Mathf.RoundToInt(z / 4.0f) * 4;

                    float y = child.localPosition.y;
                    child.localPosition = new Vector3(xc, y, zc);
                }
            }
            Debug.Log(count);
        }

        void ExportRoomPieces(GameObject g)
        {
            var saveData = new GameObject("RoomPieces_data");
            saveData.AddComponent<RoomData>();
            saveData.transform.parent = g.transform;
            Util.InitGameObject(saveData);

            var resPath = Path.Combine(Application.dataPath, "prefabs");
            var dir = new DirectoryInfo(resPath);
            var prefabs = dir.GetFiles("*.prefab", SearchOption.TopDirectoryOnly);

            resPath = Path.Combine(Application.dataPath, "prefabs/props");
            dir = new DirectoryInfo(resPath);
            var propsPrefab = dir.GetFiles("*.prefab", SearchOption.TopDirectoryOnly);


            GameObjectDelegate gg = delegate (string name)
            {
                return GetPrefab(name, new List<FileInfo[]>()
                {
                    prefabs,
                    propsPrefab
                });
            };

            TranverseTree(transform.Find("RoomPieces_data").gameObject, saveData, gg);
        }

        void ExportLight(GameObject g)
        {
            var saveData = new GameObject("light_data");
            saveData.AddComponent<RoomData>();
            saveData.transform.parent = g.transform;
            Util.InitGameObject(saveData);

            var resPath = Path.Combine(Application.dataPath, "lightPrefab");
            var dir = new DirectoryInfo(resPath);
            var prefabs = dir.GetFiles("*.prefab", SearchOption.TopDirectoryOnly);

            GameObjectDelegate gg = delegate (string name)
            {
                return GetPrefab(name, new List<FileInfo[]>()
                {
                    prefabs,
                });
            };

            TranverseSingleTree(transform.Find("light_data").gameObject, saveData, gg);
        }

        void ExportProps(GameObject g)
        {
            var saveData = new GameObject("Props_data");
            saveData.AddComponent<RoomData>();
            saveData.transform.parent = g.transform;
            Util.InitGameObject(saveData);

            var resPath = Path.Combine(Application.dataPath, "LevelPrefab");
            var dir = new DirectoryInfo(resPath);
            var prefabs = dir.GetFiles("*.prefab", SearchOption.TopDirectoryOnly);

            GameObjectDelegate gg = delegate (string name)
            {
                return GetPrefab(name, new List<FileInfo[]>()
                {
                    prefabs,
                });
            };

            TranverseSingleTree(transform.Find("Props_data").gameObject, saveData, gg);
        }

        public delegate GameObject GameObjectDelegate(string name);

        public string type = "suntemple";
        public string exportName = "ENTRANCE_N_PB";
        [ButtonCallFunc()]public bool ExportRoomList;

        public void ExportRoomListMethod()
        {
            var g = new GameObject(exportName);
            ExportRoomPieces(g);
            ExportLight(g);
            ExportProps(g);

            string resName = "";
            if (type != "")
            {
                Directory.CreateDirectory("Assets/Resources/room/" + type); 
                resName = "Assets/Resources/room/" + type + "/" + g.name + ".prefab";
            } else
            {
                resName = "Assets/Resources/room/" + g.name + ".prefab";
            }

            PrefabUtility.CreatePrefab(resName, g);
            GameObject.DestroyImmediate(g);

            Debug.LogError("请刷新RoomList 将该房间加入到RoomList中： " + resName);
        }

        void TranverseSingleTree(GameObject root, GameObject saveData, GameObjectDelegate del)
        {
            var sd = saveData.GetComponent<RoomData>();
            foreach (Transform child in root.transform)
            {
                var pb = new RoomData.RoomPosRot();
                pb.prefab = del(child.name);
                pb.pos = child.localPosition;
                pb.rot = child.localRotation;
                pb.scale = child.localScale;

                sd.Prefabs.Add(pb);
            }
        }


        void TranverseTree(GameObject root, GameObject saveData, GameObjectDelegate del)
        {
            var sd = saveData.GetComponent<RoomData>();

            foreach (Transform t in root.transform)
            {
                foreach (Transform child in t)
                {
                    var pb = new RoomData.RoomPosRot();
                    pb.prefab = del(child.name);
                    pb.pos = child.localPosition;
                    pb.rot = child.localRotation;
                    pb.scale = child.localScale;

                    sd.Prefabs.Add(pb);
                }
            }
        }

        Dictionary<string, GameObject> nameCache = new Dictionary<string, GameObject>();

        GameObject GetPrefab(string name, List<FileInfo[]> dirs)
        {
            var nameLower = name.ToLower();
            if (nameCache.ContainsKey(nameLower))
            {
                return nameCache [nameLower];
            }

            int lastNameLen = 0;
            FileInfo bestMatch = null;

            foreach (var fi in dirs)
            {
                foreach (var f in fi)
                {
                    var pn = f.Name.Replace(".prefab", "").ToLower();
                    if (pn.Length > lastNameLen && nameLower.Contains(pn))
                    {
                        lastNameLen = pn.Length;
                        bestMatch = f;
                    }
                }
            }
            if (bestMatch != null)
            {
                var assPath = bestMatch.FullName.Replace(Application.dataPath, "Assets");
                var g = AssetDatabase.LoadAssetAtPath<GameObject>(assPath) as GameObject;
                //var g = PrefabUtility.InstantiatePrefab() as GameObject;
                return g;
            }
            return null;
        }

    }
}
#endif