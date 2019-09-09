using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class MonsterIdle : IdleState
    {
        bool birthYet = false;
        float directionChangeInterval = 3;
        float RunSpeed = 5;
        
        //一次性初始化代码
        public override void EnterState()
        {
            base.EnterState();
            SetAttrState(CharacterState.Idle);
            aiCharacter.SetIdle();
        }
        
        IEnumerator Birth()
        {
            if (CheckAni("spawn"))
            {
                SetAttrState(CharacterState.Birth);
                
                PlayAni("spawn", 2, WrapMode.Once);
                
                if (GetAttr().ObjUnitData.SpawnEffect != "")
                {
                    GameObject g = GameObject.Instantiate(Resources.Load<GameObject>(GetAttr().ObjUnitData.SpawnEffect)) as GameObject;
                    g.transform.parent = SaveGame.saveGame.EffectMainNode.transform;
                    g.transform.position = GetAttr().transform.position;
                    NGUITools.AddMissingComponent<RemoveSelf>(g);
                }
                //yield return GetAttr ().StartCoroutine (Util.WaitForAnimation (GetAttr ().animation));
                yield return new WaitForSeconds(1f);
                
                SetAttrState(CharacterState.Idle);
                SetAni("idle", 1, WrapMode.Loop);
            } else
            {
                SetAni("idle", 1, WrapMode.Once);
                GameObject g = null;
                if (GetAttr().ObjUnitData.SpawnEffect != "")
                {
                    g = GameObject.Instantiate(Resources.Load<GameObject>(GetAttr().ObjUnitData.SpawnEffect)) as GameObject;
                } else
                {
                    g = GameObject.Instantiate(Resources.Load<GameObject>("particles/playerskills/impsummon")) as GameObject;
                }
                //g.transform.parent = SaveGame.saveGame.EffectMainNode.transform;
                g.transform.position = GetAttr().transform.position;
                NGUITools.AddMissingComponent<RemoveSelf>(g);
                //yield return GetAttr ().StartCoroutine (Util.WaitForAnimation (GetAttr ().animation));
                yield return new WaitForSeconds(1f);

                SetAttrState(CharacterState.Idle);
                SetAni("idle", 1, WrapMode.Loop);
            }
            birthYet = true;
            var rd = Random.Range(1, 3);
            BackgroundSound.Instance.PlayEffectPos("batmanspawn" + rd, GetAttr().transform.position);

        }

        IEnumerator IdleSound()
        {
            while (!quit)
            {
                var rd = Random.Range(2, 4);
                yield return new WaitForSeconds(rd);
                var rd1 = Random.Range(1, 3);
                BackgroundSound.Instance.PlayEffectPos("batmanidle" + rd1, GetAttr().transform.position);
            }
        }



        public override IEnumerator RunLogic()
        {
            if (!birthYet)
            {
                yield return GetAttr().StartCoroutine(Birth());
            }
            GetAttr().StartCoroutine(IdleSound());
            yield return GetAttr().StartCoroutine(NewHeading());
            
            Log.AI("State Logic Over " + type);
        }
        
        bool CheckTarget()
        {
            if (CheckEvent())
            {
                return true;
            }

            if(BattleManager.battleManager.StopAttack) {
                return false;
            }

            GameObject player = ObjectManager.objectManager.GetMyPlayer();
            if (player && !player.GetComponent<NpcAttribute>().IsDead)
            {
                float distance = (player.transform.position - GetAttr().transform.position).magnitude;
                if (distance < GetAttr().ApproachDistance)
                {
                    aiCharacter.ChangeState(AIStateEnum.COMBAT);
                    return true;
                }
            }
            return false;
        }
        
        IEnumerator NewHeadingRoutine()
        {
            while (!quit)
            {
                var heading = Random.Range(0, 360);
                var targetRotation = new Vector3(0, heading, 0);
                Quaternion qua = Quaternion.Euler(targetRotation);
                Vector3 dir = (qua * Vector3.forward);
                
                RaycastHit hitInfo;
                if (!Physics.Raycast(GetAttr().transform.position, dir, out hitInfo, 3))
                {
                    break;
                }
                yield return null;
            }
        }
        
        IEnumerator NewHeading()
        {
            while (!quit)
            {
                aiCharacter.SetIdle();
                float passTime = Random.Range(1, 3);
                while (passTime > 0)
                {
                    if (CheckTarget())
                    {
                        yield break;
                    }
                    passTime -= Time.deltaTime;
                    yield return null;
                }
                yield return GetAttr().StartCoroutine(NewHeadingRoutine());

                aiCharacter.SetRun();
                passTime = directionChangeInterval;
                var attr = GetAttr();
                while (passTime > 0)
                {
                    if (CheckTarget())
                    {
                        yield break;
                    }
                    passTime -= Time.deltaTime;
                    
                    var forward = GetAttr().transform.TransformDirection(Vector3.forward);
                    GetAttr().GetComponent<PhysicComponent>().MoveSpeed(forward * RunSpeed);
                    yield return null;
                }
                
                yield return null;
            }
        }
        
    }

}