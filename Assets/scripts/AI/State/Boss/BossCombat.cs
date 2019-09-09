using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class BossCombat : CombatState
    {
        float FastRotateSpeed = 10;
        float WalkSpeed = 3;
        float RunSpeed = 5;
        public override void EnterState() {
            base.EnterState();
            SetAttrState (CharacterState.Attacking);    
            WalkSpeed = GetAttr ().WalkSpeed;
            RunSpeed = WalkSpeed*2;
        }

        IEnumerator CheckFall(){
            while(!quit){
                var ori = GetAttr().OriginPos;
                if(GetAttr().transform.position.y < (ori.y-3)){
                    GetAttr().transform.position = GetAttr().OriginPos;    
                }
                yield return new WaitForSeconds(1);
            }
        }

        IEnumerator CastSkill(GameObject targetPlayer) {
            GetAttr ().GetComponent<SkillInfoComponent> ().SetRandomActive ();
            var activeSkill = GetAttr().GetComponent<SkillInfoComponent>().GetActiveSkill();
            var skillStateMachine = SkillLogic.CreateSkillStateMachine (GetAttr().gameObject, activeSkill.skillData, GetAttr().transform.position, targetPlayer);
            Log.AI ("Skill SetAni "+activeSkill.skillData.AnimationName);
            
            var realAttackTime = activeSkill.skillData.AttackAniTime/GetAttr().GetSpeedCoff();
            var rate = GetAttr().GetComponent<Animation>()[activeSkill.skillData.AnimationName].length/realAttackTime;
            SetAni (activeSkill.skillData.AnimationName, rate, WrapMode.Once);
            var physic = GetAttr().GetComponent<PhysicComponent>();
            while(GetAttr().GetComponent<Animation>().isPlaying && !quit) {
                if(CheckEvent()) {
                    break;
                }
                
                //自动向目标旋转
                Vector3 dir = targetPlayer.transform.position - GetAttr ().transform.position;
                dir.y = 0;
                var newDir = Vector3.Slerp(GetAttr().transform.forward, dir, Time.deltaTime * FastRotateSpeed );
                physic.TurnTo(newDir);
                yield return null;
            }
            skillStateMachine.Stop();
        }

        public override IEnumerator RunLogic ()
        {
            var physic = GetAttr ().GetComponent<PhysicComponent> ();
            GetAttr().StartCoroutine(CheckFall());
            
            var targetPlayer = ObjectManager.objectManager.GetMyPlayer();
            GetAttr().GetComponent<CommonAI>().SetTargetPlayer(targetPlayer);
            var tarNpc = targetPlayer.GetComponent<NpcAttribute>();
            
            while(!quit) {
                if(tarNpc.IsDead) {
                    aiCharacter.ChangeState(AIStateEnum.IDLE);
                    break;
                }
                //Rush to Player Follow
                aiCharacter.SetRun();
                while(!quit && !tarNpc.IsDead) {
                    if(CheckEvent()){
                        break;
                    }
                    
                    Vector3 dir = targetPlayer.transform.position-GetAttr().transform.position;
                    dir.y = 0;
                    if(dir.magnitude < GetAttr().AttackRange*0.7f) {
                        break;
                    }
                    var newDir = Vector3.Slerp(GetAttr().transform.forward, dir, Time.deltaTime*FastRotateSpeed);
                    physic.TurnTo(newDir);
                    var forward = GetAttr().transform.TransformDirection(Vector3.forward);
                    
                    physic.MoveSpeed(forward*RunSpeed);
                    yield return null;
                }
                
                yield return GetAttr().StartCoroutine(CastSkill(targetPlayer));
                
            }
        }

    }

}