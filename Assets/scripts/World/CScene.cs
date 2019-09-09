using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

//一个场景中的数据，包括LevelInit AStar 以及 BattleManager 以及Forever怪物出生点等信息
namespace MyLib
{
    public enum SceneState
    {
        InGame,
        GameOver,
    }

    public class CScene : KBEngine.MonoBehaviour
    {
        private static Dictionary<string, Type> staticTypeMap;
        public SceneState state = SceneState.InGame;

        public static void InitStatic()
        {
            staticTypeMap = new Dictionary<string, Type>()
            {
                { "MyLib.Map2", typeof(Map2) }, 
                { "MyLib.Map3", typeof(Map3) }, 
                { "MyLib.Map101", typeof(Map101) }, 
                { "MyLib.Map102", typeof(Map102) }, 
                { "MyLib.Map103", typeof(Map103) }, 
                { "MyLib.Map105", typeof(Map105) }, 
            };
        }

        public virtual bool IsEnemy(GameObject a, GameObject b)
        {
            return a != b;
        }

        public bool IsCity
        {
            get
            {
                return def.isCity;
            }
        }

        public virtual bool IsNet
        {
            get
            {
                return false;
            }
        }

        public virtual bool ShowTeamColor
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsRevive
        {
            get
            {
                return false;
            }
        }

        public DungeonConfigData def;

        protected virtual void Awake()
        {
            GameObject.DontDestroyOnLoad(gameObject);
        }

        public static CScene CreateScene(DungeonConfigData sceneDef)
        {
            var tpName = "MyLib.Map" + sceneDef.id;
            var tp = typeof(CScene).Assembly.GetType(tpName);
            //var tp = Type.GetType ();
            var g = new GameObject("CScene");
            if (tp == null)
            {
                if (staticTypeMap.ContainsKey(tpName))
                {
                    tp = staticTypeMap [tpName];
                }
            }
            if (tp == null)
            {
                g.AddComponent<CScene>();
            } else
            {
                var t = typeof(NGUITools);
                var m = t.GetMethod("AddMissingComponent");
                var geMethod = m.MakeGenericMethod(tp);
                geMethod.Invoke(null, new object[]{ g });// as AIBase;
            }

            var sc = g.GetComponent<CScene>();
            sc.def = sceneDef;
            return sc;
        }

        //初始化静态资源
        public virtual void Init()
        {
            //创建网格Astar和LevelInit 等对象

            //加载环境音效
        }

        //
        public virtual  void EnterScene()
        {
            //预先加载Npc和怪物资源

            //播放背景音乐
        }

        /// <summary>
        /// 战斗管理器等初始化结束 
        /// </summary>
        public virtual void ManagerInitOver()
        {
        }

        /// <summary>
        /// 销毁场景元素
        /// </summary>
        public virtual  void LeaveScene()
        {
            //销毁环境音效和背景音乐

            //销毁场景网络对象 其它玩家 服务器Npc
            var keys = ObjectManager.objectManager.Actors.ToArray();

            foreach (var k in keys)
            {
                ObjectManager.objectManager.DestroyPlayer(k.Key);
            }

            //销毁本地怪物 本地玩家对象
            ObjectManager.objectManager.DestroyMySelf();

        }

        /// <summary>
        /// 客户端向服务器广播消息 Map3重写 
        /// </summary>
        /// <param name="cmd">Cmd.</param>
        public virtual void BroadcastMsg(CGPlayerCmd.Builder cmd)
        {
            
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Debug.LogError("QuitScene: " + this);
        }

    }
}
