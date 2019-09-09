/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    public enum BattleState {
        Prepare,
        Battling,
        Finish,
    }
    /// <summary>
    /// 关卡相关配置信息
    /// </summary>
    public class BattleManager : KBEngine.MonoBehaviour
    {
        public BattleState state = BattleState.Battling;
        public int waveNum = 0;
        public static BattleManager battleManager;
        //[HideInInspector]
        public List<GameObject> enemyList;
        //public AstarPath PathInfo;
        public bool StartSpawn = false;
        public int MaxWave = 5;
        public List<GameObject> Zones;
        public int currentZone = -1;
        // Use this for initialization
        public GameObject exitZone;
        public List<SpawnTrigger> allWaves;
        public GameObject enterZone;
        public bool levelOver = false;
        [ButtonCallFunc()]
        public bool
            killAll = false;

        public static bool allElite = false;
        public bool StopAttack = false;


        public void killAllMethod()
        {
            foreach (var e in enemyList)
            {
                var npc = e.GetComponent<NpcAttribute>();
                npc.ChangeHP(-npc.HP_Max);

            }
        }

        public void AddEnemy(GameObject g) {
            enemyList.Add(g);
            if(AlwaysKill) {
                var npc = g.GetComponent<NpcAttribute>();
                npc.ChangeHP(-npc.HP_Max);
            }
        }
        public bool AlwaysKill = false;

        [ButtonCallFunc()]
        public bool killAndMove;

        public void killAndMoveMethod()
        {
            killAllMethod();
            if (exitZone != null)
            {
                ObjectManager.objectManager.GetMyPlayer().transform.position = exitZone.GetComponent<ExitWall>().colliderObj.transform.position + new Vector3(0, 0.5f, 0);
            }
        }

        void Awake()
        {
            gameObject.AddComponent<ReviveBattleManager>();

            Zones = new List<GameObject>();

            levelOver = false;
            battleManager = this;
            allWaves = new List<SpawnTrigger>();

            Debug.Log("BattleManager:: init UI ");
            enemyList = new List<GameObject>();

            currentZone = -1;

            for (int i = 1; i < Zones.Count; i++)
            {
                var zone = Zones [i];
                var ze = zone.GetComponent<ZoneEntityManager>();
                ze.DisableProperties();
            }
            if (Zones.Count > 0)
            {
                InitZone();
            }

        }

        /// <summary>
        /// 初始化Room怪物信息
        /// </summary>
        /// <param name="z">The z coordinate.</param>
        public void AddZone(GameObject z)
        {

            Zones.Add(z);
            InitZoneState(z);
            if (currentZone == -1)
            {
                currentZone = 0;
                InitZone();
            }
        }

        void InitZoneState(GameObject z)
        {
            //z.transform.Find("properties").gameObject.SetActive(false);
            z.GetComponent<ZoneEntityManager>().DisableProperties();
        }

        IEnumerator Start()
        {
            while (ObjectManager.objectManager == null)
            {
                yield return null;
            }
            regEvt = new List<MyEvent.EventType>()
            {
                MyEvent.EventType.PlayerDead,
            };
            RegEvent();
        }

        protected override void OnEvent(MyEvent evt)
        {
            if (evt.type == MyEvent.EventType.PlayerDead)
            {
                var localId = evt.localID;
                var p = ObjectManager.objectManager.GetLocalPlayer(localId);
                if(p != null && p.GetComponent<NpcAttribute>().IsMine()) {
                    OnPlayerDead(null);
                }
            }
        }

        IEnumerator ShowFail()
        {
            var failUI = WindowMng.windowMng.PushView("UI/FailPanel").GetComponent<FailUI>();
            while (!failUI.quit)
            {
                yield return null;
            }
            WindowMng.windowMng.PopView();

            //从副本回到主城
            yield return WorldManager.worldManager.StartCoroutine(WorldManager.worldManager.ChangeScene(2, false));
        }

        void OnPlayerDead(GameObject g)
        {
            levelOver = true;
            var scene = WorldManager.worldManager.GetActive();
            if(scene.IsRevive) {
                var rb = GetComponent<ReviveBattleManager>();
                rb.ReviveMe();
            }else {
                StartCoroutine(ShowFail());
            }
        }

        public GameObject GetZone() {
            return Zones[currentZone];
        }

        void InitZone()
        {
            Log.Sys("InitZone Properties ");
            allWaves.Clear();

            var ze = Zones [currentZone].GetComponent<ZoneEntityManager>();
            var prop = ze.properties.transform;

            var ex = Util.FindChildRecursive(Zones [currentZone].transform, "exitZone");
            if (ex != null)
            {
                exitZone = ex.gameObject;
            }

            waveNum = 0;
            MaxWave = 0;
            foreach (Transform t in prop)
            {
                var spawn = t.gameObject.GetComponent<SpawnTrigger>();
                if (spawn != null)
                {
                    allWaves.Add(spawn);    
                    if (spawn.waveNum >= MaxWave)
                    {
                        MaxWave = spawn.waveNum + 1;
                    }
                }
            }

            //prop.gameObject.SetActive(true);
            var world = WorldManager.worldManager.GetActive();
            if(world.IsNet) {
                //StartCoroutine(CheckNetSpawn());
            }else {
                ze.EnableProperties();
            }
        }

        /*
        IEnumerator CheckNetSpawn() {
            var ze = Zones[currentZone].GetComponent<ZoneEntityManager>();
            var player = ObjectManager.objectManager.GetMyPlayer();
            while(player == null) {
                player = ObjectManager.objectManager.GetMyPlayer();
                yield return null;
            }

            var world = WorldManager.worldManager.GetActive();
            if(world.IsNet && ObjectManager.objectManager.GetMyAttr().IsMaster) {
                ze.EnableProperties();
            }
        }
        */

        /*
         * Disappear Wall Between
         */
        IEnumerator GotoNextZone()
        {
            Log.Sys("GotoNextZone");
            if (exitZone != null)
            {
                //exitZone.SetActive (false);
                exitZone.GetComponent<ExitWall>().ZoneClear();
            }

            var ez = Zones [currentZone].transform.Find("enterZone");

            if (ez == null)
            {
                Debug.LogError("BattleManager::Next Zone has No EnterZone");
            }
            enterZone = ez.gameObject;
            var ezone = enterZone.GetComponent<EnterZone>();
            var protectWall = Zones [currentZone].transform.Find("protectWall");
            protectWall.gameObject.SetActive(false);

            //Wait For Player pass Zone Enter New Zone
            while (!ezone.Enter)
            {
                yield return null;
            }
            //Can't GoBack
            if (exitZone != null)
            {
                //exitZone.SetActive (true);
                exitZone.GetComponent<ExitWall>().CloseDoor();
            }


            if (protectWall != null)
            {
                protectWall.GetComponent<ProtectWall>().ShowWall();
            }
            MyEventSystem.myEventSystem.PushEvent(MyEvent.EventType.EnterNextZone);


            InitZone();
            StartCoroutine(StreamLoadLevel.Instance.MoveInNewRoom());

        }

        //wait for a while to rest goto next wave
        IEnumerator NextWave()
        {
            Log.Sys("NextWave Start");
            yield return new WaitForSeconds(3);
            if (levelOver)
            {
                yield break;
            }

            waveNum++;
            Log.Sys("NewWaveNum " + waveNum + " MaxWave " + MaxWave);
            if (waveNum >= MaxWave)
            {
                currentZone++;
                Log.Sys("currentZone zoneCount " + currentZone + " count " + Zones.Count);
                if (currentZone < Zones.Count)
                {
                    yield return StartCoroutine(GotoNextZone());
                } else
                {
                    var n = MyEventSystem.myEventSystem.GetRegEventHandler(MyEvent.EventType.LevelFinish);
                    Log.Sys("BattleManager::NextWave No Wave Battle Finish " + MaxWave + " evtNum " + n);
                    if (n > 0)
                    {
                        MyEventSystem.myEventSystem.PushEvent(MyEvent.EventType.LevelFinish);
                    } else
                    {
                        yield return StartCoroutine(LevelFinish());
                    }
                }
            }
        }

        public void GameOver()
        {
            StartCoroutine(LevelFinish());
        }

        /// <summary>
        /// Open NextLevel 
        /// </summary>
        /// <returns>The finish.</returns>
        IEnumerator LevelFinish()
        {
            Log.Sys("LevelFinish ");
            CopyController.copyController.PassLevel();
            float leftTime = 5f;
            //var notify = 
            GameObject not = null;
            WindowMng.windowMng.ShowNotifyLog("", 5.2f, delegate(GameObject n)
            {
                not = n;
            });
            while (not == null)
            {
                yield return new WaitForSeconds(1);
            }
            var notify = not.GetComponent<NotifyUI>();
            if (notify != null)
            {
                while (leftTime > 0)
                {
                    Log.GUI("CountLeftTime " + leftTime);
                    //notify.SetTime (leftTime);
                    notify.SetText(string.Format("退出副本倒计时{0}s", (int)leftTime));
                    leftTime -= Time.deltaTime;
                    yield return null;
                }
                notify.SetText(string.Format("退出副本倒计时{0}s", (int)0));
            }
            Log.GUI("Finish ThisLevel " + leftTime);

            var victoryUI = WindowMng.windowMng.PushView("UI/victory").GetComponent<VictoryUI>();
            while (!victoryUI.con)
            {
                yield return null;
            }
            WindowMng.windowMng.PopView();

            yield return WorldManager.worldManager.StartCoroutine(WorldManager.worldManager.ChangeScene(2, false));
        }

        IEnumerator WaitToShowNextWave()
        {
            yield return new WaitForSeconds(3);

            if (enemyList.Count > 0)
            {
                Log.Sys("DeadSkill CallNewEnemy");
                yield break;
            }
            enemyList.Clear();
            StartCoroutine(NextWave());
        }

        /// <summary>
        /// 怪物死亡事件 触发下一波怪
        /// </summary>
        /// <param name="go">Go.</param>
        public void EnemyDead(GameObject go)
        {
            Log.Sys("MonsterDead " + go.name + " list " + enemyList.Count);
            if (!enemyList.Contains(go))
            {
                return;
            }
            enemyList.Remove(go);
            //有可能死亡之后又召唤了一个怪物 因此需要等一会进入下一波
            if (enemyList.Count > 0)
            {
                return;
            }
            StartCoroutine(WaitToShowNextWave());
        }



    }
}