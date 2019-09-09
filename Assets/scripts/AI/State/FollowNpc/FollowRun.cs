using UnityEngine;
using System.Collections;

namespace MyLib
{
    //Move To Target Point
    public class FollowRun : MoveState
    {
        float speed = 5;
        //一次性初始化代码
        public override void EnterState()
        {
            base.EnterState();
            aiCharacter.SetRun();
        } 
        public override IEnumerator RunLogic()
        {
            var myTransform = GetAttr().transform;
            var targetPlayer = ObjectManager.objectManager.GetMyPlayer().transform;
            var physic = myTransform.GetComponent<PhysicComponent>();
            var rd = new Vector3(Random.Range(-3.0f, 3.0f), 0, Random.Range(-3.0f, 3.0f)); 
            while(!quit) {
                if(CheckEvent()) {
                    break;
                }
                var tarPos = targetPlayer.position + rd;
                Vector3 dir = tarPos - myTransform.position;
                dir.y = 0;
                if(dir.sqrMagnitude < 25) {
                    aiCharacter.ChangeState(AIStateEnum.IDLE);
                    yield break;
                }

                if(dir.sqrMagnitude > 200) {
                    aiCharacter.ChangeState(AIStateEnum.IDLE);
                    physic.transform.position = tarPos+ new Vector3(Random.Range(-3.0f, 3.0f), 0.2f, Random.Range(-3.0f, 3.0f));
                    yield break;
                }

                dir.Normalize();
                //var rotation = Quaternion.LookRotation(dir);
                //myTransform.rotation = Quaternion.Slerp(myTransform.rotation, rotation, Mathf.Min(1, Time.deltaTime*10));
                physic.TurnTo(dir);
                physic.MoveSpeed(dir * speed);
                yield return null ;
            }
        }
    }
}
