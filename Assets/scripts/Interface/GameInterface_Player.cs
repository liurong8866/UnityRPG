using UnityEngine;
using System.Collections;
using System;

namespace MyLib
{
    public static class GameInterface_Player
    {
        public static int GetLevel()
        {
            return ObjectManager.objectManager.GetMyData().GetProp(CharAttribute.CharAttributeEnum.LEVEL);
        }

        public static void UpdateExp(GCPushExpChange p)
        {
            ObjectManager.objectManager.GetMyAttr().SetExp(p.Exp);
        }

        public static void TalkToNpc()
        {
            Log.GUI("TalkToNpc");
            var myp = ObjectManager.objectManager.GetMyPlayer();
            var hitColliders = Physics.OverlapSphere(myp.transform.position, 2, 1 << (int)GameLayer.IgnoreCollision);
            var cosAngle = Mathf.Cos(Mathf.Deg2Rad * 90);
            var myfor = myp.transform.forward;
            myfor.y = 0;
            Log.GUI("TalkTONum "+hitColliders.Length);
            for (int i = 0; i < hitColliders.Length; i++)
            {
                var obj = hitColliders [i].gameObject;
                if (obj != myp)
                {
                    var dir = hitColliders [i].gameObject.transform.position - myp.transform.position;
                    dir.y = 0;
                    var cos = Vector3.Dot(dir.normalized, myfor);
                    Log.GUI("Cos and dir "+cos+" angle "+cosAngle);
                    if (cos > cosAngle)
                    {
                        NpcManager.Instance.TalkTo(obj);
                        break;
                    }
                }
            }
        }

        public static int GetIntState(string key) {
            var list = ServerData.Instance.playerInfo.GameStateList;
            foreach(var kv in list) {
                if(kv.Key == key) {
                    return Convert.ToInt32(kv.Value);
                }
            }
            return 0;
        }

        public static void SetIntState(string key, int v) {
            var list = ServerData.Instance.playerInfo.GameStateList;
            foreach(var kv in list) {
                if(kv.Key == key) {
                    kv.Value = v.ToString();
                    return;
                }
            }
            
            var kv2 = KeyValue.CreateBuilder();
            kv2.Key = key;
            kv2.Value = v.ToString();
            list.Add(kv2.Build());
        }


        public static bool GetGameState(string key) {
            var list = ServerData.Instance.playerInfo.GameStateList;
            foreach(var kv in list) {
                if(kv.Key == key) {
                    return Convert.ToBoolean(kv.Value);
                }
            }
            return false;
        }

        public static void SetGameState(string key, bool v) {
            var list = ServerData.Instance.playerInfo.GameStateList;
            foreach(var kv in list) {
                if(kv.Key == key) {
                    kv.Value = v.ToString();
                    return;
                }
            }

            var kv2 = KeyValue.CreateBuilder();
            kv2.Key = key;
            kv2.Value = v.ToString();
            list.Add(kv2.Build());
        }
    }

}