using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class OtherPlayerJump : JumpState
    {
        const float RushSpeed = 5;
        const float AddSpeed = 20;
        const float MaxSpeed = 8;


        const float UpSpeed = 10;
        const float gravity = 10;
        const float dropGravity = 20;
        const float friction = 20;

        public override void EnterState()
        {
            base.EnterState();
            aiCharacter.SetRun();
            var ab = GetAttr().GetComponent<AIBase>();
            ab.ignoreFallCheck = true;
            var ret = GetAttr().GetComponent<PhysicComponent>().EnterSkillMoveState();
            GetAttr().GetComponent<ShadowComponent>().LockShadowPlane();
            BackgroundSound.Instance.PlayEffect("fall4");
            GetAttr().JumpForwardSpeed = RushSpeed;
        }

        public override void ExitState()
        {
            var ab = GetAttr().GetComponent<AIBase>();
            ab.ignoreFallCheck = false;
            GetAttr().GetComponent<PhysicComponent>().ExitSkillMove();
            GetAttr().GetComponent<ShadowComponent>().UnLockShadowPlane();
            base.ExitState();
        }


        public override bool CheckNextState(AIStateEnum next)
        {
            if(next == AIStateEnum.IDLE) {
                return true;
            }
            if(next == AIStateEnum.COMBAT) {
                GetAttr().StartCoroutine(SkillState());
                return false;
            }
            return false;
        }

        private bool inSkill = false;
        IEnumerator SkillState() {
            if(inSkill) {
                yield break;
            }

            inSkill = true;
            var js = new JumpSkillState();
            aiCharacter.AddTempState(js);
            yield return GetAttr().StartCoroutine(js.RunLogic());
            inSkill = false;
        }
      


        public override IEnumerator RunLogic()
        {
            var attr = GetAttr();
            var playerForward = GetAttr().transform.forward;
            var physics = GetAttr().GetComponent<PhysicComponent>();
            var upSpeed = UpSpeed;

            var controller = GetAttr().GetComponent<CharacterController>();
            var passTime = 0.0f;
            var soundYet = false;

            while (!quit)
            {
                if (CheckEvent())
                {
                    break;
                }

                var forwardSpeed = attr.JumpForwardSpeed;
                var movement = playerForward * forwardSpeed + Vector3.up * upSpeed;
                physics.JumpMove(movement);

                if (upSpeed <= 0)
                {
                    upSpeed -= dropGravity * Time.deltaTime;
                } else
                {
                    upSpeed -= gravity * Time.deltaTime;
                }
                passTime += Time.deltaTime;
                yield return null;
                if (passTime >= 0.2f)
                {
                    if ((controller.collisionFlags & CollisionFlags.Below) != 0)
                    {
                        if (!soundYet)
                        {
                            soundYet = true;
                            BackgroundSound.Instance.PlayEffect("fall1");
                        }
                        forwardSpeed -= friction * Time.deltaTime;
                        forwardSpeed = Mathf.Max(0, forwardSpeed);
                        attr.JumpForwardSpeed = forwardSpeed;
                        if (forwardSpeed <= 0)
                        {
                            break;
                        }
                    }
                }
            }

            aiCharacter.ChangeState(AIStateEnum.IDLE);
        }
    }
}