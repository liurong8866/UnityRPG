using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class FollowIdle : IdleState
    {
        public override void EnterState()
        {
            base.EnterState();
            aiCharacter.SetIdle();
        }

        IEnumerator CheckMoveToPlayer ()
        {
            var player = ObjectManager.objectManager.GetMyPlayer();
            while (!quit)
            {
                yield return new WaitForSeconds(1);
                var pos = GetAttr().transform.localPosition;
                var tarPos = player.transform.localPosition;
                var dis = Util.XZSqrMagnitude(pos, tarPos);
                if(dis > 36) {
                    aiCharacter.ChangeState(AIStateEnum.MOVE);
                    yield break;
                }
                yield return null;
            }
        }

        public override IEnumerator RunLogic()
        {
            GetAttr().StartCoroutine(CheckMoveToPlayer());
            while(!quit) {
                if (CheckEvent())
                {
                    break;
                }
                yield return null;
            }
        }
    }

}