using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class TankShoot : AIState
    {
        private SkillFullInfo activeSkill;

        public override IEnumerator RunLogic()
        {
            activeSkill = GetAttr().GetComponent<SkillInfoComponent>().GetActiveSkill();
            tower = Util.FindChildRecursive(GetAttr().transform, "tower").transform;
            yield return GetAttr().StartCoroutine(Shoot());
        }

        private string GetAttackAniName()
        {
            var name = string.Format("rslash_{0}", 1);
            return name;
        }

        private string attackAniName;
        private Vector3 forward;

        IEnumerator Shoot()
        {
            var trans = GetAttr().transform;
            var enemy = SkillLogic.FindNearestEnemy(trans.gameObject);
            var physic = GetAttr().GetComponent<GeneraPhysic>();
            Log.Sys("FindEnemyIs: " + enemy);
            if (enemy != null)
            {
                var dir = enemy.transform.position - trans.position;
                dir.y = 0;
                Log.Sys("EnemyIs: " + dir);
                //forward = dir;
                forward = tower.forward;

                //physic.TurnToImmediately(dir);
                physic.TurnTower(dir);
            } else
            {
                forward = trans.forward;
                physic.TurnTower(forward);
            }

            attackAniName = GetAttackAniName(); 
            /*
            var realAttackTime = activeSkill.skillData.AttackAniTime / GetAttr().GetSpeedCoff();
            var rate = GetAttr().animation [attackAniName].length / realAttackTime;
            PlayAni(attackAniName, rate, WrapMode.Once);
            */
            yield return GetAttr().StartCoroutine(WaitForAttackAnimation(GetAttr().GetComponent<Animation>()));
            yield return new WaitForSeconds(0.1f);
        }
        private Transform tower;
        private IEnumerator WaitForAttackAnimation(Animation animation)
        {
            var skillStateMachine = SkillLogic.CreateSkillStateMachine(GetAttr().gameObject, activeSkill.skillData, tower.transform.position);
            skillStateMachine.SetForwardDirection(forward);
            //skillStateMachine.SetRelativePos(tower.localPosition);

            Log.AI("Wait For Combat Animation");
            float passTime = 0;
            //yield return new WaitForSeconds(0.1f);
            yield return null;
            MyEventSystem.PushLocalEventStatic(GetAttr().GetLocalId(), MyEvent.EventType.EventTrigger);
            var realAttackTime = activeSkill.skillData.AttackAniTime / GetAttr().GetSpeedCoff();
            //realAttackTime -= 0.3f;
            do
            {
                if (passTime >= realAttackTime * 0.8f)
                {
                    break;
                }
                passTime += Time.deltaTime;

                yield return null;
            } while(!quit);
            
            Log.Ani("Animation is Playing stop ");
            skillStateMachine.Stop();
        }
    }
}