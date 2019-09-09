using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    /// <summary>
    /// 增加网络命令缓冲池 
    /// </summary>
    [RequireComponent(typeof(AnimationController))]
    [RequireComponent(typeof(PlayerSync))]
	[RequireComponent(typeof(PhysicComponent))]
    public class OtherPlayerAI : AIBase
    {
        void Awake()
        {
            attribute = GetComponent<NpcAttribute>();

            ai = new HumanCharacter();
            ai.attribute = attribute;
            ai.AddState(new HumanIdle());
            ai.AddState(new OtherPlayerMove());
            ai.AddState(new HumanCombat());
            ai.AddState(new HumanSkill());
            ai.AddState(new HumanDead());
            ai.AddState(new MonsterKnockBack());
            ai.AddState(new HumanStunned());
            ai.AddState(new OtherPlayerJump());

            this.regEvt = new List<MyEvent.EventType>()
            {
                MyEvent.EventType.EnterSafePoint,
                MyEvent.EventType.ExitSafePoint,
            };
            RegEvent();

        }

        List<Vector3> samplePos;
        IEnumerator CheckFall()
        {
            Vector3 originPos = attribute.OriginPos;
            samplePos = new List<Vector3>(){ originPos };
            while (true)
            {
                var lastOne = transform.position;
                if(samplePos.Count > 0) {
                    lastOne = samplePos [0];
                }
                Log.Sys("lastPos nowPos " + lastOne + " now " + transform.position);
                if (transform.position.y < (lastOne.y - 3))
                {
                    if (!inSafe)
                    {
                        transform.position = lastOne;    
                    }
                } else
                {
                    if (inSafe)
                    {
                        samplePos.Clear();
                    } else
                    {
                        var pos = transform.position;
                        samplePos.Add(pos);
                        if (samplePos.Count > 4)
                        {
                            samplePos.RemoveAt(0);
                        }
                    }
                }
                yield return new WaitForSeconds(1);
            }
        }

        private bool inSafe = false;

        protected override void OnEvent(MyEvent evt)
        {
            if (evt.type == MyEvent.EventType.EnterSafePoint)
            {
                inSafe = true;
                samplePos.Clear();
            } else if (evt.type == MyEvent.EventType.ExitSafePoint)
            {
                inSafe = false;
                samplePos.Add(transform.position);
            }
            Log.Sys("InSafeNow "+inSafe+" evt "+evt);
        }

        void Start()
        {
            ai.ChangeState(AIStateEnum.IDLE);
            //StartCoroutine(CheckFall());
        } 

    }
}
