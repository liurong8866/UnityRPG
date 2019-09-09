using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using SimpleJSON;
using System.Collections.Generic;
using System.Linq;
using MyLib;

public class SimpleMakeScene : MonoBehaviour
{
    public string path;
    [ButtonCallFunc()]public bool AdjustNoAni;

    /// <summary>
    /// 导入静态场景模型
    ///调整无动画模型 拼接 Mesh 和 collision 
    /// </summary>
    public void AdjustNoAniMethod()
    {
        AdjustNoAniModel(path);
    }

    public string lightPath;
    [ButtonCallFunc()]
    public bool LightMapEnv;

    public void LightMapEnvMethod()
    {
        var allModel = Path.Combine(Application.dataPath, lightPath);
        var resDir = new DirectoryInfo(allModel);
        FileInfo[] fileInfo = resDir.GetFiles("*.fbx", SearchOption.AllDirectories);
        AssetDatabase.StartAssetEditing();
        foreach (FileInfo file in fileInfo)
        {
            Debug.Log("file is " + file.Name + " " + file.Name);

            var ass = file.FullName.Replace(Application.dataPath, "Assets");
            var res = AssetDatabase.LoadAssetAtPath<GameObject>(ass);
            var shm = res.GetComponent<Renderer>().sharedMaterials;
            foreach (var s in shm)
            {
                s.shader = Shader.Find("Custom/lightMapEnv");
                EditorUtility.SetDirty(s);
            }
            Debug.Log("import change state ");
            AssetDatabase.WriteImportSettingsIfDirty(ass);
        }
        AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh();
    }

    public string lightModelPath = "lights";
    [ButtonCallFunc()]
    public bool CustomLight;

    public void CustomLightMethod()
    {
        var allModel = Path.Combine(Application.dataPath, lightModelPath);
        var resDir = new DirectoryInfo(allModel);
        FileInfo[] fileInfo = resDir.GetFiles("*.fbx", SearchOption.AllDirectories);
        AssetDatabase.StartAssetEditing();
        foreach (FileInfo file in fileInfo)
        {
            Debug.Log("file is " + file.Name + " " + file.Name);

            var ass = file.FullName.Replace(Application.dataPath, "Assets");
            var res = AssetDatabase.LoadAssetAtPath<GameObject>(ass);
            res.GetComponent<Renderer>().sharedMaterial.shader = Shader.Find("Custom/light");
            EditorUtility.SetDirty(res.GetComponent<Renderer>().sharedMaterial);

            Debug.Log("import change state ");
            AssetDatabase.WriteImportSettingsIfDirty(ass);
        }
        AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh();
    }

    public string lightLayerPath = "lightPrefab";
    [ButtonCallFunc()]
    public bool LightLayer;

    public void LightLayerMethod()
    {
        var allModel = Path.Combine(Application.dataPath, lightLayerPath);
        var resDir = new DirectoryInfo(allModel);
        FileInfo[] fileInfo = resDir.GetFiles("*.prefab", SearchOption.AllDirectories);
        AssetDatabase.StartAssetEditing();
        foreach (FileInfo file in fileInfo)
        {
            Debug.Log("file is " + file.Name + " " + file.Name);

            var ass = file.FullName.Replace(Application.dataPath, "Assets");
            var res = AssetDatabase.LoadAssetAtPath<GameObject>(ass);
            //res.renderer.sharedMaterial.shader = Shader.Find("Custom/light");
            Util.SetLayer(res, GameLayer.Light);
            EditorUtility.SetDirty(res);

            Debug.Log("import change state ");
            AssetDatabase.WriteImportSettingsIfDirty(ass);
        }
        AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh();
    }

    public string lightTargetPos = "OUTDOOR";
    public string lightModelPath2 = "lights";
    [ButtonCallFunc()]public bool MoveToPrefab;

    public void MoveToPrefabMethod()
    {
        var allModel = Path.Combine(Application.dataPath, lightModelPath2);
        var resDir = new DirectoryInfo(allModel);
        FileInfo[] fileInfo = resDir.GetFiles("*.fbx", SearchOption.AllDirectories);

        AssetDatabase.StartAssetEditing();
        foreach (FileInfo file in fileInfo)
        {
            Debug.Log("file is " + file.Name + " " + file.FullName);
            var dirName = file.Name.Replace(".fbx", "");
            var ass = file.FullName.Replace(Application.dataPath, "Assets");
            var tar = Path.Combine("Assets/lightPrefab",lightTargetPos);
            if (!File.Exists(tar))
            {
                Directory.CreateDirectory(tar);
            }

            tar = Path.Combine(tar,  dirName + ".prefab");

            var oldPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(tar);
            if (oldPrefab == null)
            {
                var prefab = PrefabUtility.CreatePrefab(tar, AssetDatabase.LoadAssetAtPath<GameObject>(ass));
            }
        }

        AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh();
    }

    public string path1;
    public string combinePath = "snow";
    [ButtonCallFunc()]public bool CombineToPrefab;

    /// <summary>
    /// 特定目录下到model 和 collsion合并 
    /// </summary>
    public void CombineToPrefabMethod()
    {
        CombineFileAndCollisionToPrefab(path1);
    }

    public string path3;
    [ButtonCallFunc()]public bool ImportAniModel;

    /// <summary>
    /// 导入NPC 或者 动画场景模型 
    /// </summary>
    public void ImportAniModelMethod()
    {
        var allModel = Path.Combine(Application.dataPath, path3);
        Debug.Log("Import Animation Model " + allModel);
        var resDir = new DirectoryInfo(allModel);

        var allFiles = resDir.GetFiles("*.fbx", SearchOption.TopDirectoryOnly);
        CreateAniModelPrefab(allFiles, resDir.Name);
    }

    GameObject CreateAniModelPrefab(FileInfo[] allFiles, string dirName)
    {
        var tar = Path.Combine("Assets/ModelPrefab", dirName + ".prefab");
        Debug.Log("CreateAniModelPrefab " + tar);
        //var tg = PrefabUtility.CreatePrefab(tar, g);
        Dictionary<string, string> aniFbx = new Dictionary<string, string>();
        AssetDatabase.StartAssetEditing();
        bool npc = false;
        foreach (var f in allFiles)
        {
            Debug.Log("fbx file is " + f.FullName);
            if (f.FullName.Contains("npc"))
            {
                npc = true;
            }
            var path = f.FullName.Replace(Application.dataPath, "Assets");
            var import = ModelImporter.GetAtPath(path) as ModelImporter;
            if (path.Contains("@"))
            {
                //AnimationFIle
                import.globalScale = 1;
                import.importAnimation = true;
                import.animationType = ModelImporterAnimationType.Legacy;
                var namePart = path.Split('@');
                var aniName = namePart [1].Replace(".fbx", "");
                aniFbx.Add(aniName, path);


            } else
            {
                //COllision File
                import.globalScale = 1;
                import.importAnimation = false;
                import.animationType = ModelImporterAnimationType.None;
                aniFbx.Add("collision", path);
            }
            AssetDatabase.WriteImportSettingsIfDirty(path);
        }
        AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh();


        //Use First Animation FBX idle as base
        var first = aniFbx.First();
        foreach (var a in aniFbx)
        {
            if (a.Key != "collision")
            {
                first = a;
                break;
            }
        }

        //aniFbx ["idle"]
        var prefab = PrefabUtility.CreatePrefab(tar, AssetDatabase.LoadAssetAtPath<GameObject>(first.Value));

        if (!npc)
        {
            prefab.transform.Find("Armature").localRotation = Quaternion.identity;
            prefab.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, 0));
        }
        if (aniFbx.ContainsKey("collision"))
        {
            var meshCollider = prefab.AddComponent<MeshCollider>();
            var colObj = AssetDatabase.LoadAssetAtPath<GameObject>(aniFbx ["collision"]);
            meshCollider.sharedMesh = colObj.GetComponent<MeshFilter>().sharedMesh;
        }

        var aniPart = prefab.GetComponent<Animation>();
        foreach (var ani in aniFbx)
        {
            if (ani.Key != first.Key && ani.Key != "collision")
            {
                var aniObj = AssetDatabase.LoadAssetAtPath<GameObject>(ani.Value);
                var clip = aniObj.GetComponent<Animation>().clip;
                aniPart.AddClip(clip, clip.name);
            }
        }

        //AssetDatabase.StartAssetEditing();
        foreach (Transform t in prefab.transform)
        {
            if (t.GetComponent<Renderer>() != null)
            {
                Debug.Log("render is " + t.name);
                if (npc)
                {
                    t.GetComponent<Renderer>().sharedMaterial.shader = Shader.Find("Custom/npcShader");
                    t.GetComponent<Renderer>().sharedMaterial.color = Color.white;
                } else
                {
                    t.GetComponent<Renderer>().sharedMaterial.shader = Shader.Find("Custom/lightMapEnv");
                }
                EditorUtility.SetDirty(t.GetComponent<Renderer>().sharedMaterial);
            }
        }

        return prefab;
    }

    void AdjustNoAniModel(string rootPath)
    {
        Debug.Log("AdjustNoAniModel " + rootPath);

        var allModel = Path.Combine(Application.dataPath, rootPath);
        var resDir = new DirectoryInfo(allModel);
        FileInfo[] fileInfo = resDir.GetFiles("*.fbx", SearchOption.TopDirectoryOnly);
        AssetDatabase.StartAssetEditing();
        foreach (FileInfo file in fileInfo)
        {
            Debug.Log("file is " + file.Name + " " + file.Name);
            
            //var ass = Path.Combine("Assets/" + modelStr.stringValue, file.Name);
            var ass = file.FullName.Replace(Application.dataPath, "Assets");
            var import = ModelImporter.GetAtPath(ass) as ModelImporter;
            Debug.Log("import is " + import);
            import.globalScale = 1;
            import.importAnimation = false;
            import.animationType = ModelImporterAnimationType.None;
            
            Debug.Log("import change state " + import);
            AssetDatabase.WriteImportSettingsIfDirty(ass);

        }
        AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh();
    }




    /// <summary>
    /// 融合了MineProp.dat Prop.dat Mine.dat 三个文件的map.json 组合所有的RoomPieces 模型
    /// </summary>
    void CombineFileAndCollisionToPrefab(string p)
    {
        var mapJson = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Config/map.json");
        var mapObj = JSON.Parse(mapJson.text).AsObject;
        AssetDatabase.StartAssetEditing();
        Debug.Log("path is " + p);
        var plow = p.ToLower();
        foreach (KeyValuePair<string, JSONNode> n in mapObj)
        {
            var f = n.Value ["FILE"].Value;
            var col = n.Value ["COLLISIONFILE"].Value;

            var fpath = ConvertPath(f);
            fpath = fpath.Replace("levelsets", "levelSets");
            var flow = fpath.ToLower();

            Debug.Log("filePath is " + flow);
            if (flow.Contains(plow))
            {
                CombineTwo(f, col);
                //break;
            }

        }
        AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh();
    }

    //With Animation
    GameObject CombineTwo(string f, string col)
    {
        Debug.Log("CombineTwo " + f + " col " + col);

        var fpath = ConvertPath(f);
        var g = AssetDatabase.LoadAssetAtPath<GameObject>(fpath);
        if (g != null)
        {
            if (!g.name.Contains("@"))
            {
                //AdjustModelImport(fpath);
                Debug.Log("Combine " + f);
                Debug.Log("ColFile " + col);
                GameObject cg = null;
                if (col != "")
                {
                    var cp = ConvertPath(col);
                    cg = AssetDatabase.LoadAssetAtPath<GameObject>(cp);
                    if (cg != null)
                    {
                        //AdjustModelImport(cp);
                    }
                }

                var fn = Path.GetFileName(fpath);
                var prefab = fn.Replace(".fbx", ".prefab");
                var oldPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefab);
                if (oldPrefab == null)
                {
                    var tar = Path.Combine("Assets/prefabs", combinePath);

                    var tarDir = Path.Combine(Application.dataPath, "prefabs/" + combinePath);
                    if (!File.Exists(tarDir))
                    {
                        Directory.CreateDirectory(tarDir);
                    }


                    tar = Path.Combine(tar, prefab);
                    var tg = PrefabUtility.CreatePrefab(tar, g);
                    if (cg != null)
                    {
                        var meshCollider = tg.AddComponent<MeshCollider>();
                        meshCollider.sharedMesh = cg.GetComponent<MeshFilter>().sharedMesh;
                    }

                    return tg;
                } else
                {
                    Debug.Log("old prefab exists " + prefab);
                }
            }
        } else
        {
            Debug.Log("Not Find: " + f + " col " + col);
        }
        return null;
    }

    /// <summary>
    /// 将media/xxx.mesh 转化成Assets/xxx.fbx
    /// </summary>
    /// <returns>The path.</returns>
    /// <param name="f">F.</param>
    string ConvertPath(string f)
    {
        var fpath = Path.Combine("Assets", f.Replace("media/", ""));
        fpath = fpath.Replace(".mesh", ".fbx");
        return fpath;
    }



    [ButtonCallFunc()]public bool RemoveMapJson;

    public void RemoveMapJsonMethod()
    {
        var mapJson = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Config/map.json");
        var mapObj = JSON.Parse(mapJson.text).AsObject;
        Debug.Log("path is ");
        foreach (KeyValuePair<string, JSONNode> n in mapObj)
        {
            var f = n.Value ["FILE"].Value;
            var col = n.Value ["COLLISIONFILE"].Value;

            var fpath = ConvertPath(f);
            fpath = fpath.Replace("levelsets", "levelSets");

            Debug.Log("filePath is " + fpath);

            var tarDir = Path.Combine(Application.dataPath, "prefabs/");
            var fn = Path.GetFileName(fpath);
            var prefab = fn.Replace(".fbx", ".prefab");
            var tar = Path.Combine(tarDir, prefab);
            Debug.Log("Delete: " + tar);
            File.Delete(tar);
        }
    }
}

#endif