using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MyLib
{
    /// <summary>
    ///配置在zone中的NPc生成器 参考SpawnTrigger 
    /// </summary>
    public class SpawnNpc : MonoBehaviour
    {
        GameObject Resource; //NPC 
        GameObject oldResource = null;
        public int NpcId = -1; //
        void Awake() {
            ClearChildren();
        }

        public void UpdateEditor() {
#if UNITY_EDITOR
            if(!EditorApplication.isPlaying) {
                if(NpcId != -1){
                    var mData = GMDataBaseSystem.database.SearchId<NpcConfigData>(GameData.NpcConfig, NpcId );
                    if(mData != null) {
                        Resource = Resources.Load<GameObject>(mData.model);
                    }

                    if(oldResource != Resource) {
                        ClearChildren();

                        if(Resource != null) {
                            var showRes = GameObject.Instantiate(Resource) as GameObject;
                            showRes.transform.parent = transform;
                            Util.InitGameObject(showRes);
                        }
                        oldResource = Resource;
                    }
                    
                }
            }
#endif
        }
        //清理Spawner的孩子
        void ClearChildren()
        {
            List<GameObject> g = new List<GameObject>();
            foreach (Transform t in transform)
            {
                g.Add(t.gameObject);
            }
            foreach (GameObject g1 in g)
            {
                GameObject.DestroyImmediate(g1);
            }
        }

        void SpawnNpcNow(){
            ObjectManager.objectManager.CreateNpc(Util.GetNpcData(NpcId), gameObject);
        }

        void Start()
        {
            SpawnNpcNow();
        }
    
    }
}
