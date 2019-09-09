﻿using UnityEngine;
using System.Collections;

namespace MyLib
{
    /// <summary>
    /// 火焰陷阱
    /// </summary>
    public class PetIdle : IdleState
    {
        bool birthYet = false;
        public override void EnterState() {
            base.EnterState ();
            SetAttrState(CharacterState.Idle);
            aiCharacter.SetIdle ();
        }

        IEnumerator Birth() {
            if(CheckAni("spawn")) {
                SetAttrState(CharacterState.Birth);
                
                PlayAni("spawn", 2, WrapMode.Once);
                Log.AI("spawn particle is "+GetAttr().ObjUnitData.SpawnEffect);
                if(GetAttr().ObjUnitData.SpawnEffect != "")  {
                    GameObject g = GameObject.Instantiate(Resources.Load<GameObject>(GetAttr().ObjUnitData.SpawnEffect)) as GameObject;
                    //g.transform.position = GetAttr().transform.position;
                    g.transform.parent = GetAttr().transform;
                    g.transform.localPosition = Vector3.zero;
                    g.transform.localRotation = Quaternion.identity;
                    g.transform.localScale = Vector3.one;
                }
                yield return GetAttr().StartCoroutine(Util.WaitForAnimation(GetAttr().GetComponent<Animation>()));
                
                SetAttrState(CharacterState.Idle);
                aiCharacter.SetIdle ();
            }
            birthYet = true;
			Log.AI ("Birth finish "+GetAttr().gameObject);
        }

        //TODO:后续做成全局Static函数  CommonAI 静态一次性函数
		//查找附近敌人不考虑方向
        GameObject NearestEnemy ()
        {
			return SkillLogic.FindNearestEnemy (GetAttr().gameObject);
		}

		//使用默认技能 作为主动攻击技能
        IEnumerator FindTarget() {
			Log.AI ("Find Target For AI "+GetAttr().gameObject);
            while(!quit) {
                if(CheckEvent()) {
                    yield break;
                }
                yield return null;
            }
        }

        public override IEnumerator RunLogic ()
        {
            if (!birthYet) {
                yield return GetAttr ().StartCoroutine (Birth ());
            }
            yield return GetAttr().StartCoroutine(FindTarget());
            
            Log.AI ("State Logic Over "+type);
        }

        protected override bool CheckEventOverride(MyAnimationEvent.Message msg)
        {
            Log.AI("CheckEventOverride "+msg.type);
            if(msg.type == MyAnimationEvent.MsgType.BOMB) {
                Log.AI("CheckBombEvent "+msg.type);
                var sdata = Util.GetSkillData(137, 1);
                aiCharacter.GetAttr().StartCoroutine(SkillLogic.MakeSkill(aiCharacter.GetAttr().gameObject, sdata, GetAttr().transform.position));
                aiCharacter.ChangeState(AIStateEnum.DEAD);
                return true;
            }
            return base.CheckEventOverride(msg);
        }

    }

}