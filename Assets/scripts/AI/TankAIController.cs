using UnityEngine;
using System.Collections;

namespace MyLib
{
    [RequireComponent(typeof(AnimationController))]
    [RequireComponent(typeof(SkillCombineBuff))]
    [RequireComponent(typeof(MySelfAttributeSync))]
    [RequireComponent(typeof(PlayerSyncToServer))]
    [RequireComponent(typeof(TankPhysicComponent))]
    public class TankAIController : AIBase
    {
        void Awake()
        {
            //var bx = Util.FindChildRecursive(transform, "boxColldier");
            //var bx1 = Util.FindChildRecursive(transform, "boxColldier2");
            //Physics.IgnoreCollision(bx.collider, bx1.collider);

            var tower = Util.FindChildRecursive(transform, "tower");
            tower.gameObject.AddComponent<TowerAutoCheck>();

            attribute = GetComponent<NpcAttribute>();

            ai = new TankCharacter();
            ai.attribute = attribute;
            ai.AddState(new TankIdle());
            ai.AddState(new TankMoveAndShoot());
            ai.AddState(new TankDead());
            ai.AddState(new TankKnockBack());
            ai.AddState(new HumanStunned());
            ai.AddState(new TankStop());
        }

        void Start()
        {
            ai.ChangeState(AIStateEnum.IDLE);
        }
    }
}