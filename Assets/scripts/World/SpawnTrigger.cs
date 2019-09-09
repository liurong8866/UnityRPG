
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/

/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

/*
 * Configure Level Monster Spawn Position And Monster Type And Number 
 */
namespace MyLib
{
    /// <summary>
    ///怪物生成器 
    /// </summary>
    public class SpawnTrigger : MonoBehaviour
    {
        public const int EliteRate = 10;
        public int waveNum;
        public bool Forever = false;
        public List<MultipleSpawner> multiSpawner;



        public enum GroupEnum
        {
            Monster,
        }

        public GroupEnum Group;
        public GameObject Resource;
        public int MonsterID = -1;
        public bool reset = false;
        public bool isSpawnYet = false;
        public int Level = 1;
        public int GroupNum = 1;
        public float Radius = 0;
        public bool AddLevel = false;
        public int TotalWave = 5;
        public GameObject waveText;
        int oldWaveNum = -1;
        int curWaveNum = 0;
        public GameObject FirstMonster;

        void Awake()
        {
            if (gameObject.name.Contains("wave"))
            {
                var num = Convert.ToInt32(gameObject.name.Replace("wave", ""));
                waveNum = num;
            }
            /*
            float dir = UnityEngine.Random.Range(0, 360);
            transform.localRotation = Quaternion.Euler(new Vector3(0, dir, 0));
            */

            isSpawnYet = false;
            ClearChildren();
            HideChild();
            HideTracePoint();
        }

        List<GameObject> tracePoint = null;

        /// <summary>
        ///翠花蛇用于行走的寻路点 
        /// </summary>
        /// <returns>The trace point.</returns>
        public List<GameObject> GetTracePoint()
        {
            if (tracePoint == null)
            {
                tracePoint = new List<GameObject>();
                foreach (Transform c in transform)
                {
                    if (c.name.Contains("TracePoint"))
                    {
                        tracePoint.Add(c.gameObject);
                    }
                }
            }
            return tracePoint;
        }

        List<GameObject> childPoint = null;

        public List<GameObject> GetChildPoint()
        {
            if (childPoint == null)
            {
                childPoint = new List<GameObject>();
                foreach (Transform c in transform)
                {
                    if (c.name.Contains("Child"))
                    {
                        childPoint.Add(c.gameObject);
                    }
                }
            }
            return childPoint;
        }

        // Use this for initialization
        void Start()
        {
            
        }

        bool setResourceYet = false;
        GameObject oldResource;
        GameObject showRes;

        string GetWave()
        {
            if (gameObject != null && gameObject.name.Contains("wave"))
            {
                var num = Convert.ToInt32(gameObject.name.Replace("wave", ""));
                waveNum = num;
            }
            return "" + waveNum;
        }

        public void UpdateEditor()
        {

#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                string replaceTexture = "";
                if (MonsterID != -1)
                {
                    var mData = GMDataBaseSystem.database.SearchId<MonsterFightConfigData>(GameData.MonsterFightConfig, MonsterID);
                    if (mData != null)
                    {
                        Resource = Resources.Load<GameObject>(mData.model);
                        replaceTexture = mData.textureReplace;
                    }
                }

                if (oldResource != Resource)
                {
                    if (showRes != null)
                    {
                        GameObject.DestroyImmediate(showRes);
                        showRes = null;
                    }
                    ClearChildren();
                
                    if (Resource != null)
                    {
                        showRes = GameObject.Instantiate(Resource) as GameObject;
                        showRes.transform.parent = transform;
                        showRes.transform.localPosition = Vector3.zero;
                        if (replaceTexture.Length > 0)
                        {
                            var skins = showRes.GetComponentInChildren<SkinnedMeshRenderer>();
                            var tex = Resources.Load<Texture>(replaceTexture);
                            if (skins != null && tex != null)
                            {
                                Log.Sys("Set Texture " + tex);
                                var mat = new Material(skins.GetComponent<Renderer>().sharedMaterial);
                                mat.mainTexture = tex;
                                skins.GetComponent<Renderer>().sharedMaterial = mat;
                            }
                        }
                    }
                    oldResource = Resource;
                }
                if (showRes != null)
                {
                    showRes.transform.localPosition = Vector3.zero;
                }
                if (reset)
                {
                    ClearChildren();
                }
            } else
            {
                if (showRes != null)
                {
                    GameObject.Destroy(showRes);
                    showRes = null;
                    oldResource = null;
                }
            }
            GetWave();
            if (oldWaveNum != waveNum || waveText == null)
            {
                if (waveText != null)
                {
                    GameObject.DestroyImmediate(waveText);
                    waveText = null;
                }
                waveText = Instantiate(Resources.Load<GameObject>("TextFont")) as GameObject;
                waveText.transform.parent = transform;
                waveText.transform.localPosition = Vector3.zero;
                waveText.GetComponent<TextMesh>().text = GetWave();
                oldWaveNum = waveNum;
            }
#endif
        }

        void HideTracePoint()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).name.Contains("TracePoint"))
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                } 
            }
        }

        void HideChild()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).name.Contains("Child"))
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }

        public void ClearChildren()
        {

            for (int i = 0; i < transform.childCount;)
            {
                if (transform.GetChild(i).name.Contains("Child") || transform.GetChild(i).name.Contains("TracePoint"))
                {
                    i++;
                } else
                {
                    GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
                }
            }

            reset = false;
            showRes = null;
            oldResource = null;
        }

        /// <summary>
        /// 通过ObjectManager来生成新的怪物对象
        /// </summary>
        /// <returns>The monster.</returns>
        IEnumerator GenerateMonster()
        {
            Log.Sys("GenerateMonsters " + GroupNum);
            for (int i = 0; i < GroupNum; i++)
            {
                var rd = UnityEngine.Random.Range(0, 100);
                var unitData = Util.GetUnitData(false, MonsterID, 0);
                if (rd < EliteRate || BattleManager.allElite)
                {
                    var elites = unitData.EliteIds;
                    if (elites.Count > 0)
                    {
                        var rd2 = UnityEngine.Random.Range(0, elites.Count);
                        var id2 = elites [rd2];
                        unitData = Util.GetUnitData(false, id2, 0);
                        Log.Sys("CreateEliteMonster " + id2);
                    }
                }

                ObjectManager.objectManager.CreateMonster(unitData, this);
                yield return new WaitForSeconds(1);
            }

            if (multiSpawner.Count > 0)
            {
                multiSpawner [0].SpawnChild(this);
            }
            
            curWaveNum++;
            if (Forever && curWaveNum < TotalWave)
            {
                waveNum++;
                isSpawnYet = false;
                if (AddLevel)
                {
                    Level++;
                }
            }
            yield return null;
        }

        // Update is called once per frame
        void Update()
        {
            if (isSpawnYet)
            {
                return;
            }

            if (BattleManager.battleManager == null)
            {
                Debug.LogError("SpawnTrigger:: battleManager Not Init");
                return;
            }
            if (WorldManager.worldManager.station != WorldManager.WorldStation.Enter)
            {
                Log.Sys("Spawn Not Enter World");
                return;
            }

            if (!isSpawnYet && BattleManager.battleManager.waveNum == waveNum && (Resource != null || MonsterID != -1))
            {
                isSpawnYet = true;
                StartCoroutine(GenerateMonster());
            }

        }
        
    }
}
