﻿using UnityEngine;
using System.Collections;

namespace MyLib
{
    /// <summary>
    /// 宠物的AI状态机构建
    /// </summary>

    public class PetAI : AIBase
    {
		
        void Awake()
        {
            attribute = GetComponent<NpcAttribute>();
            ai = new MonsterCharacter();
            ai.attribute = attribute;
            ai.AddState(new PetIdle());
            ai.AddState(new PetDead());
        }

        void Start()
        {
            ai.ChangeState(AIStateEnum.IDLE);
            CheckNearByBomb();
        }

        /// <summary>
        ///检测附近如果有相关的机关则引爆
        /// 当落到地面的时候 
        /// </summary>
        void CheckNearByBomb()
        {
            var sp = gameObject.AddComponent<SphereCollider>();
            sp.isTrigger = true;
            sp.radius = 4;

            var col = Physics.OverlapSphere(transform.position, 4, 1 << (int)GameLayer.Npc);
            var goDie = false;
            var meType = attribute.ObjUnitData.ID;
            foreach (var c in col)
            {
                var npc = c.GetComponent<NpcAttribute>();
                if(npc.ObjUnitData.ID == meType && npc.GetLocalId() != attribute.GetLocalId() && !npc.IsDead){
                    var evt = npc.GetComponent<MyAnimationEvent>();
                    var et = new MyAnimationEvent.Message();
                    et.type = MyAnimationEvent.MsgType.BOMB;
                    evt.InsertMsg(et);
                    goDie = true;
                }
            }
            Log.AI("CheckNearBy ToBomb "+goDie+" col "+col.Length+" name "+gameObject.name);
            if(goDie) {
                //GetComponent<MyAnimationEvent>().timeToDead = true;
                var evt = new MyAnimationEvent.Message();
                evt.type = MyAnimationEvent.MsgType.BOMB;
                GetComponent<MyAnimationEvent>().InsertMsg(evt);
            }
        }
    }
}
