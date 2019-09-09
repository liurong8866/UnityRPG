using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class SummonIdle : IdleState
    {
        public override void EnterState()
        {
            base.EnterState();
            aiCharacter.SetIdle();
        }

        public override IEnumerator RunLogic()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.5f);
                GameObject player = ObjectManager.objectManager.GetMyPlayer();
                if (player && !player.GetComponent<NpcAttribute>().IsDead)
                {
                    aiCharacter.ChangeState(AIStateEnum.COMBAT);
                }
            }
        }

    }
}
