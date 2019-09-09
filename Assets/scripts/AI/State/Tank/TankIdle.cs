using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class TankIdle : IdleState
    {
        public override void EnterState()
        {
            base.EnterState();
            aiCharacter.SetIdle();
        }

        //先进入MOVE_SHOOT 层状态机，再将状态注入
        public override bool CheckNextState(AIStateEnum next)
        {
            if (next == AIStateEnum.COMBAT || next == AIStateEnum.CAST_SKILL)
            {
                //进入MoveShoot状态机 再压入Combat命令
                GetAttr().StartCoroutine(MoveShoot());
                return false;
            }
            return base.CheckNextState(next);
        }
        public override bool CanChangeState(AIStateEnum next)
        {
            if (next == AIStateEnum.COMBAT || next == AIStateEnum.CAST_SKILL)
            {
                return true;
            }
            return base.CanChangeState(next);
        }

        IEnumerator MoveShoot()
        {
            Log.AI("MoveShoot");
            yield return null;
            aiCharacter.ChangeState(AIStateEnum.MOVE_SHOOT);
            aiCharacter.ChangeState(AIStateEnum.COMBAT);
        }

        public override IEnumerator RunLogic()
        {
            var playerMove = GetAttr().GetComponent<MoveController>();
            var vcontroller = playerMove.vcontroller;

            while (!quit)
            {
                if (CheckEvent())
                {
                    yield break;
                }

                float v = 0;
                float h = 0;
                if (vcontroller != null)
                {
                    h = vcontroller.inputVector.x;//CameraRight 
                    v = vcontroller.inputVector.y;//CameraForward
                }

                bool isMoving = Mathf.Abs(h) > 0.1 || Mathf.Abs(v) > 0.1;
                if (isMoving)
                {
                    aiCharacter.ChangeState(AIStateEnum.MOVE_SHOOT);
                }
                yield return null;
            }
        }
    }
}