using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    public class LevelConfig
    {
        public string type = "mine";
        public string room;
        public int x;
        public int y;
        public bool useOtherZone = false;
        public int zoneId;
        public bool flip = false;

        public LevelConfig(string r, int x1, int y1)
        {
            room = r;
            x = x1;
            y = y1;
        }
    }

    /// <summary>
    /// 流式关卡加载器，分Room加载
    /// </summary>
    public class StreamLoadLevel : MonoBehaviour
    {
        int currentRoomIndex = -1;
        //static Dictionary<string, string> namePieces = null;
        List<int> loadedRoom = new List<int>();
        Dictionary<int, GameObject> loadedZone = new Dictionary<int, GameObject>();
        public static StreamLoadLevel Instance = null;

        public  bool InitYet
        {
            private set;
            get;
        }

        /// <summary>
        /// 加载第一个房间
        /// </summary>
        /// <returns>The first room.</returns>
        public IEnumerator LoadFirstRoom()
        {
            Log.Sys("LoadFirstRoom");
            currentRoomIndex = 0;
            loadedRoom.Add(currentRoomIndex);

            var root = new GameObject("Root_0"); //FirstRoom
            Util.InitGameObject(root);

            var first = configLists [0];
            var firstOffset = new Vector3(first.x * 96, 9, first.y * 96 + 48);
            root.transform.localPosition = firstOffset;

            Log.Sys("First Room NamePices " + first.room);
            var roomConfig = RoomList.GetStaticObj(first.type, first.room);
            yield return StartCoroutine(LoadRoom(roomConfig));
            yield return null;
            yield return StartCoroutine(LoadLight(roomConfig));
            yield return StartCoroutine(LoadProps(roomConfig));
            yield return StartCoroutine(LoadZone());

            var zone = loadedZone [currentRoomIndex];

            //单一出生点位置
            var zoneConfigStart = zone.transform.FindChild("PlayerStart");
            if (zoneConfigStart != null)
            {
                var g = new GameObject("PlayerStart");
                g.transform.position = zoneConfigStart.transform.position; 
            }
            InitYet = true;
        }

        /// <summary>
        /// 关卡房间相关怪物配置
        /// level_1_4_0
        /// 网络游戏 怪物实例是由Master 负责生成和管理的
        /// 不加载properties下面的对象
        /// 动态对象
        /// 其它的是静态对象
        /// </summary>
        /// <returns>The zones.</returns>
        IEnumerator LoadZone()
        {
            var roomConfig = configLists [currentRoomIndex];
            int index = currentRoomIndex;
            if (roomConfig.useOtherZone)
            {
                index = roomConfig.zoneId;
            }

            var name = "zones/level_1_4_" + index;
            var root = GameObject.Find("Root_" + currentRoomIndex);
            var zoneConfig = Resources.Load<GameObject>(name);
            if (zoneConfig == null)
            {
                Log.Sys("LoadZoneConfig Error " + name);
                yield break;
            }
            var zone = GameObject.Instantiate(zoneConfig) as GameObject;
            var ze = zone.AddComponent<ZoneEntityManager>();

            var world = WorldManager.worldManager.GetActive();
            Log.Sys("WorldIs Net: " + world.IsNet + " act " + world + " pro ");
            if (world.IsNet)
            {
                //pro.gameObject.AddComponent<TestDisable>();
                //pro.gameObject.SetActive(false);
                ze.DisableProperties();
            }

            if (zone == null)
            {
                Debug.LogError("LoadZone Null " + zone + " config " + zoneConfig);
            }

            zone.transform.parent = root.transform;
            Util.InitGameObject(zone);
            loadedZone [currentRoomIndex] = zone;
            BattleManager.battleManager.AddZone(zone);

            yield return null;
        }

        /// <summary>
        /// 网络关卡的话加载关卡的动态Entity资源  
        /// </summary>
        /// <returns>The zone network.</returns>
        public void  LoadZoneNetwork()
        {
            Log.Sys("LoadZoneNetwork");
            var zone = loadedZone [0];
            //var pro = Util.FindChildRecursive(zone.transform, "properties");
            var ze = zone.GetComponent<ZoneEntityManager>();
            ze.EnableProperties();
            //pro.gameObject.SetActive(true);
        }


        IEnumerator LoadRoom(GameObject roomConfig, bool slowly = false)
        {

            var pieces = roomConfig.transform.Find("RoomPieces_data");
            var rd = pieces.GetComponent<RoomData>();
            var root = GameObject.Find("Root_" + currentRoomIndex);

            var rootOfPieces = new GameObject("RoomPieces");
            rootOfPieces.transform.parent = root.transform;
            Util.InitGameObject(rootOfPieces);

            int c = 0;
            //Batch Rooom
            foreach (var p in  rd.Prefabs)
            {
                if (p != null && p.prefab != null)
                {
                    var r = GameObject.Instantiate(p.prefab) as GameObject;
                    r.isStatic = true;
                    r.transform.parent = rootOfPieces.transform;
                    r.transform.localPosition = p.pos;
                    r.transform.localRotation = p.rot;
                    r.transform.localScale = p.scale;
                }
                c++;
                if (slowly && c >= batchNum)
                {
                    c = 0;
                    yield return null;
                }
            }
            Log.Sys("LoadRoomFinish " + roomConfig.name + " num " + rd.Prefabs.Count);
            yield return null;


        }

        const int batchNum = 10;

        IEnumerator LoadLight(GameObject roomConfig, bool slowly = false)
        {
            var light = roomConfig.transform.Find("light_data");
            var rd = light.GetComponent<RoomData>();
            var root = GameObject.Find("Root_" + currentRoomIndex);
            var rootOfLight = new GameObject("Light");
            rootOfLight.transform.parent = root.transform;
            Util.InitGameObject(rootOfLight);
            
            //Batch Rooom
            int c = 0;
            foreach (var p in  rd.Prefabs)
            {
                var r = GameObject.Instantiate(p.prefab) as GameObject;
                r.isStatic = true;
                r.transform.parent = rootOfLight.transform;
                r.transform.localPosition = p.pos;
                r.transform.localRotation = p.rot;
                r.transform.localScale = p.scale;
                c++;
                if (slowly && c > batchNum)
                {
                    c = 0;
                    yield return null;
                }
            }
            Log.Sys("LoadLightFinish " + roomConfig.name + " num " + rd.Prefabs.Count);
            yield return null;
        }

        IEnumerator LoadProps(GameObject roomConfig, bool slowly = false)
        {
            var light = roomConfig.transform.Find("Props_data");
            var rd = light.GetComponent<RoomData>();
            var root = GameObject.Find("Root_" + currentRoomIndex);
            var rootOfLight = new GameObject("Props");
            rootOfLight.transform.parent = root.transform;
            Util.InitGameObject(rootOfLight);
             
            //Batch Rooom
            int c = 0;
            foreach (var p in  rd.Prefabs)
            {
                var r = GameObject.Instantiate(p.prefab) as GameObject;
                r.transform.parent = rootOfLight.transform;
                r.transform.localPosition = p.pos;
                r.transform.localRotation = p.rot;
                r.transform.localScale = p.scale;

                r.AddComponent<CheckNearMainCamera>();

                c++;
                if (slowly && c > batchNum)
                {
                    c = 0;
                    yield return null;
                }
            }
            Log.Sys("LoadPropsFinish " + roomConfig.name + " num " + rd.Prefabs.Count);
            yield return null;
        }

        List<int> loadRequest = new List<int>();
        bool inLoad = false;

        /// <summary>
        /// 加载某个房间的邻居 
        /// 进入某个房间正在加载，而又有了加载Neibor的请求
        /// </summary>
        /// <returns>The current room neibor.</returns>
        public IEnumerator LoadRoomNeibor()
        {
            Log.Sys("LoadRoomNeibor");
            var nextRoom = currentRoomIndex + 1;
            if (loadedRoom.Contains(nextRoom) || configLists.Count <= nextRoom)
            {
                yield break;
            }
            if (inLoad)
            {
                loadRequest.Add(currentRoomIndex + 1);
                yield break;
            }

            inLoad = true;
            //InitNamePieces();

            currentRoomIndex++;
            loadedRoom.Add(currentRoomIndex);

            var root = new GameObject("Root_" + currentRoomIndex); //FirstRoom
            Util.InitGameObject(root);

            var first = configLists [currentRoomIndex];
            var firstOffset = new Vector3(first.x * 96, 9, first.y * 96 + 48);
            Log.Sys("FirstRoom " + first.room);
            root.transform.localPosition = firstOffset;
            if (first.flip)
            {
                root.transform.localScale = new Vector3(-1, 1, 1);
            }
            
            //var piece = namePieces [first.room];
            //var roomConfig = Resources.Load<GameObject>("room/" + piece);
            var roomConfig = RoomList.GetStaticObj(first.type, first.room);
            yield return StartCoroutine(LoadRoom(roomConfig, true));
            yield return null;
            yield return StartCoroutine(LoadLight(roomConfig, true));
            yield return StartCoroutine(LoadProps(roomConfig, true));
            yield return StartCoroutine(LoadZone());
            yield return null;

            inLoad = false;

            if (loadRequest.Count > 0)
            {
                Log.Sys("Cache Load Request");
                loadRequest.RemoveAt(0);
                StartCoroutine(LoadRoomNeibor());
            }
        }

        public IEnumerator MoveInNewRoom()
        {
            StartCoroutine(ReleaseOldRoom());
            yield return StartCoroutine(LoadRoomNeibor());
        }

        /// <summary>
        /// 切换房间之后，进入新的房间并且激活了这个房间的怪物则释放旧的房间，同时预先加载下一个房间
        /// </summary>
        /// <returns>The old room.</returns>
        IEnumerator ReleaseOldRoom()
        {
            var toRemove = loadedRoom [0];
            loadedRoom.RemoveAt(0);
            Log.Sys("toRemove Room " + toRemove);
            if (toRemove >= 0 && toRemove < configLists.Count)
            {
                var g = GameObject.Find("Root_" + toRemove);
                //GameObject.Destroy(g);
                List<GameObject> childs = new List<GameObject>();
                foreach (Transform t in g.transform)
                {
                    childs.Add(t.gameObject);
                    //yield return null;
                }
                int c = 0;
                for (int i = 0; i < childs.Count; i++)
                {
                    List<GameObject> subChild = new List<GameObject>();
                    var cc = childs [i].transform.childCount;
                    var ci = childs [i].transform;
                    for (int j = 0; j < cc; j++)
                    {
                        subChild.Add(ci.GetChild(j).gameObject);
                        //yield return null;
                        c++;
                        if (c > batchNum)
                        {
                            c = 0;
                            yield return null;
                        }
                    }
                    for (int j = 0; j < subChild.Count; j++)
                    {
                        GameObject.Destroy(subChild [j]);
                        if (c > batchNum)
                        {
                            c = 0;
                            yield return null;
                        }
                    }
                    yield return null;
                }

                GameObject.Destroy(g);
            }
            yield return null;
        }



        /// <summary>
        /// 切换进入某个新的房间
        /// </summary>
        /// <returns>The room.</returns>
        /// <param name="index">Index.</param>
        public IEnumerator ChangeRoom(int index)
        {
            yield return null;
       
        }

        public List<LevelConfig> configLists
        {
            private set;
            get;
        }


        void Awake()
        {
            Instance = this;
            var sceneId = WorldManager.worldManager.GetActive().def.id;
            Log.Sys("Inital SceneId Layout " + sceneId);
            LevelConfigData.Init();
            configLists = LevelConfigData.LevelLayout [sceneId];
            Log.Sys("ConfigList " + configLists.Count);

        }
    
    }
}
