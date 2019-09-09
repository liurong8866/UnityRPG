using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class NetworkLoadZone : MonoBehaviour
    {
        IEnumerator  Start()
        {
            var player = ObjectManager.objectManager.GetMyPlayer();
            while (player == null)
            {
                player = ObjectManager.objectManager.GetMyPlayer();
                yield return null;
            }
            Log.Sys("NetworkLoadZone: isMaster");
            MyEventSystem.myEventSystem.RegisterLocalEvent(player.GetComponent<NpcAttribute>().GetLocalId(), MyEvent.EventType.IsMaster, OnLocalEvent);
        }

        void OnDestory()
        {
            var localId = ObjectManager.objectManager.GetMyLocalId();
            MyEventSystem.myEventSystem.DropLocalListener(localId, MyEvent.EventType.IsMaster, OnLocalEvent);
        }

        void OnLocalEvent(MyEvent evt)
        {
            Log.Sys("NetworkLoadZone: " + evt.type);
            if (evt.type == MyEvent.EventType.IsMaster)
            {
                var world = WorldManager.worldManager.GetActive();
                var player = ObjectManager.objectManager.GetMyAttr();
                Log.Sys("WorldPlayer: " + world.IsNet + " master " + player.IsMaster);
                if (world.IsNet && player.IsMaster)
                {
                    StartCoroutine(InitEntities());
                }
            }
        }

        //确认我是主机则开始生成 怪物实例等同时同步给其它玩家
        IEnumerator InitEntities()
        {
            Log.Sys("InitEntities For Network");
            var streamLoader = gameObject.GetComponent<StreamLoadLevel>();
            while (!streamLoader.InitYet)
            {
                yield return null;
            }
            streamLoader.LoadZoneNetwork();
        }
    }

}
