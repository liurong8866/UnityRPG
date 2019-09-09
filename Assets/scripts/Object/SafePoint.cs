﻿using UnityEngine;
using System.Collections;

namespace MyLib
{
    /// <summary>
    /// 标记地图某些点为安全点，以便在快速下坡的时候调整位置
    /// 如何防止对象掉落
    /// </summary>
    [RequireComponent(typeof(SphereCollider))]
    public class SafePoint : MonoBehaviour
    {
        void Start() {
            StartCoroutine(CheckCollision());
        }
        IEnumerator CheckCollision() {
            Vector3 pos = transform.position;
            var radius = GetComponent<SphereCollider>().radius;
            bool inSafe = false;
            while(true) {
                var col = Physics.OverlapSphere(pos, radius, 1 << (int)GameLayer.Npc);
                bool find = false;
                foreach(var c in col) {
                    if(c.tag == GameTag.Player) {
                        find = true;
                        if(!inSafe) {
                            inSafe = true;
                            MyEventSystem.myEventSystem.PushEvent(MyEvent.EventType.EnterSafePoint);
                        }
                        break;
                    }
                }
                if(!find) {
                    if(inSafe) {
                        inSafe = false;
                        MyEventSystem.myEventSystem.PushEvent(MyEvent.EventType.ExitSafePoint);
                    }
                }
                yield return new WaitForSeconds(0.5f);
            }
        }
        /*
        void OnTriggerEnter(Collider other)
        {
            Log.Sys("TriggerEnter "+collider);
            if (collider.tag == GameTag.Player)
            {
                MyEventSystem.myEventSystem.PushEvent(MyEvent.EventType.EnterSafePoint);
            }
        }
        void OnTriggerExit(Collider other){
            Log.Sys("TriggerExit "+collider);
            if(collider.tag == GameTag.Player) {
                MyEventSystem.myEventSystem.PushEvent(MyEvent.EventType.ExitSafePoint);
            }
        }
        */
    }


}