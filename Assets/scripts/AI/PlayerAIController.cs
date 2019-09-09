
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

namespace MyLib
{
    //自己玩家不接受服务器控制
    //远端玩家接受服务器控制
    [RequireComponent(typeof(AnimationController))]
    [RequireComponent(typeof(SkillCombineBuff))]
    [RequireComponent(typeof(MySelfAttributeSync))]
    [RequireComponent(typeof(PlayerSyncToServer))]
	[RequireComponent(typeof(PhysicComponent))]
    public class PlayerAIController : AIBase
    {
        void Awake()
        {
            attribute = GetComponent<NpcAttribute>();

			ai = new HumanCharacter();
            ai.attribute = attribute;
            ai.AddState(new HumanIdle());
            //ai.AddState(new TankMoveAndShoot());
            ai.AddState(new HumanMove());
            ai.AddState(new HumanCombat());
            ai.AddState(new HumanSkill());

            ai.AddState(new HumanDead());
            ai.AddState(new MonsterKnockBack());
            ai.AddState(new HumanStunned());
            ai.AddState(new HumanJump());

            this.regEvt = new List<MyEvent.EventType>()
            {
                MyEvent.EventType.EnterSafePoint,
                MyEvent.EventType.ExitSafePoint,
            };
            RegEvent();

        }


        IEnumerator CheckFallDead()
        {
            while(true) {
                var pos = transform.position;
                if(pos.y <= -5) {
                    break;
                }
                yield return new WaitForSeconds(1);
            }

            attribute.ChangeHP(-attribute.HP_Max);
        }

        List<Vector3> samplePos = new List<Vector3>();

        IEnumerator CheckFall()
        {
            Vector3 originPos = attribute.OriginPos;
            samplePos = new List<Vector3>(){ originPos };
            while (true)
            {
                if (!ignoreFallCheck)
                {
                    var lastOne = transform.position;
                    if (samplePos.Count > 0)
                    {
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
            Log.Sys("InSafeNow " + inSafe + " evt " + evt);
        }

        void Start()
        {
            ai.ChangeState(AIStateEnum.IDLE);
            //StartCoroutine(CheckFall());
            StartCoroutine(CheckFallDead());
        }

    }



}
