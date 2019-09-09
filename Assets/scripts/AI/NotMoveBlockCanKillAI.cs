using UnityEngine;
using System.Collections;

namespace MyLib
{

    public class BlockDead : DeadState
    {
        public System.Action deadCallback;

        public override void EnterState()
        {
            base.EnterState();
            if (deadCallback != null)
            {
                deadCallback();
            }
            Util.SetLayer(GetAttr().gameObject, GameLayer.IgnoreCollision);

            //GetAttr().animation.CrossFade("opening");
            GetAttr().IsDead = true;
            //GetAttr().OnlyShowDeadEffect();

            if (NetworkUtil.IsNetMaster())
            {
                DropGoods.DropStaticGoods(GetAttr());
            }
            //CreateParticle();
        }

        void CreateParticle()
        {
            var deathBlood = GameObject.Instantiate(Resources.Load<GameObject>("particles/deathblood")) as GameObject;
            deathBlood.transform.parent = ObjectManager.objectManager.transform;
            deathBlood.transform.localPosition = GetAttr().transform.localPosition + Vector3.up * 0.1f;
            deathBlood.transform.localRotation = Quaternion.identity;
            deathBlood.transform.localScale = Vector3.one;
            NGUITools.AddMissingComponent<RemoveSelf>(deathBlood);
        }

        public override IEnumerator RunLogic()
        {
            yield return new WaitForSeconds(2);
            yield return GetAttr().StartCoroutine(Util.SetBurn(GetAttr().gameObject));
            yield return null;

            //等网络属性同步上去了再删除对象 Hp = 0 接着等一会删除对象
            if (NetworkUtil.IsNetMaster())
            {
                yield return new WaitForSeconds(5);
                var cg = CGPlayerCmd.CreateBuilder();
                cg.Cmd = "RemoveEntity";
                var ety = EntityInfo.CreateBuilder();
                ety.Id = GetAttr().GetNetView().GetServerID();
                cg.EntityInfo = ety.Build();
                var world = WorldManager.worldManager.GetActive();
                world.BroadcastMsg(cg);
                ObjectManager.objectManager.DestroyByLocalId(GetAttr().GetComponent<KBEngine.KBNetworkView>().GetLocalId());
            } else
            {
                ObjectManager.objectManager.DestroyByLocalId(GetAttr().GetComponent<KBEngine.KBNetworkView>().GetLocalId());
            }
        }
    }

    public class NotMoveDead : DeadState
    {
        public System.Action deadCallback;
        public override void EnterState()
        {
            base.EnterState();
            if(deadCallback != null) {
                deadCallback();
            }
            Util.SetLayer(GetAttr().gameObject, GameLayer.IgnoreCollision);
            GetAttr().IsDead = true;

            if (NetworkUtil.IsNetMaster())
            {
                DropGoods.DropStaticGoods(GetAttr());
            }
        }

        public override IEnumerator RunLogic()
        {

            //等网络属性同步上去了再删除对象 Hp = 0 接着等一会删除对象
            if (NetworkUtil.IsNetMaster())
            {
                //yield return new WaitForSeconds(5);
                var cg = CGPlayerCmd.CreateBuilder();
                cg.Cmd = "RemoveEntity";
                var ety = EntityInfo.CreateBuilder();
                ety.Id = GetAttr().GetNetView().GetServerID();
                cg.EntityInfo = ety.Build();
                var world = WorldManager.worldManager.GetActive();
                world.BroadcastMsg(cg);
                ObjectManager.objectManager.DestroyByLocalId(GetAttr().GetComponent<KBEngine.KBNetworkView>().GetLocalId());
            } else
            {
                ObjectManager.objectManager.DestroyByLocalId(GetAttr().GetComponent<KBEngine.KBNetworkView>().GetLocalId());
            }
            yield return null;
        }
    }

    [RequireComponent(typeof(MonsterSync))]
    [RequireComponent(typeof(MonsterSyncToServer))]
    [RequireComponent(typeof(BlockPhysic))]
    public class NotMoveBlockCanKillAI : AIBase
    {
        void Awake()
        {
            attribute = GetComponent<NpcAttribute>();
            attribute.ShowBloodBar = false;
            ai = new ChestCharacter();
            ai.attribute = attribute;
            ai.AddState(new ChestIdle());
            var bd = new NotMoveDead();
            bd.deadCallback = () =>
            {
                Util.SpawnParticle("barrelbreak", transform.position+new Vector3(0, 0.3f, 0),false );
            };
            ai.AddState(bd);
            ai.AddState(new MonsterKnockBack());
        }


        // Use this for initialization
        void Start()
        {
            ai.ChangeState(AIStateEnum.IDLE);
	
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (attribute.IsDead)
            {
                Util.ClearMaterial(gameObject);
            }
        }
    }
}