using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class MonsterSyncToServer : MonoBehaviour
    {
        EntityInfo lastInfo;
        //EntityInfo.Builder info;
        private void Awake()
        {
            lastInfo = EntityInfo.CreateBuilder().Build();
            //info = EntityInfo.CreateBuilder().Build();
        }

        public void SyncToServer()
        {
            var me = gameObject;
            var pos = me.transform.position;
            var dir = (int)me.transform.localRotation.eulerAngles.y;
            var meAttr = me.GetComponent<NpcAttribute>();
            var intPos = NetworkUtil.ConvertPos(pos);
            var ainfo = EntityInfo.CreateBuilder();
            ainfo.Id = meAttr.GetNetView().GetServerID();
            var change = false;
            if (intPos [0] != lastInfo.X || intPos [1] != lastInfo.Y || intPos [2] != lastInfo.Z)
            {
                change = true;
                ainfo.X = intPos [0];
                ainfo.Y = intPos [1];
                ainfo.Z = intPos [2];

                lastInfo.X = intPos [0];
                lastInfo.Y = intPos [1];
                lastInfo.Z = intPos [2];
            }
            if (meAttr.HP != lastInfo.HP)
            {
                change = true;
                ainfo.HP = meAttr.HP;
                lastInfo.HP = meAttr.HP;
            }
            if(change) {
                var etyInfo = ainfo.Build();
                var cg = CGPlayerCmd.CreateBuilder();
                cg.Cmd = "UpdateEntityData";
                cg.EntityInfo = etyInfo;
                NetworkUtil.Broadcast(cg);
            }

        }
    }
}