using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class ChestDead2 : DeadState
    {
        public override void EnterState()
        {
            base.EnterState();
            Util.SetLayer(GetAttr().gameObject, GameLayer.IgnoreCollision);

            GetAttr().GetComponent<Animation>().CrossFade("opening");
            GetAttr().IsDead = true;
            GetAttr().OnlyShowDeadEffect();

            if (NetworkUtil.IsNetMaster())
            {
                DropGoods.DropStaticGoods(GetAttr());
            }
            CreateParticle();
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

    [RequireComponent(typeof(MonsterSync))]
    [RequireComponent(typeof(MonsterSyncToServer))]
    public class ChestAI2 : AIBase
    {
        void Awake()
        {
            attribute = GetComponent<NpcAttribute>();
            attribute.ShowBloodBar = false;
            //测试可以被推动的怪物
            //attribute.Pushable = true;

            ai = new ChestCharacter();
            ai.attribute = attribute;
            ai.AddState(new ChestIdle());
            ai.AddState(new ChestDead2());
            ai.AddState(new MonsterKnockBack());

        }

        void Start()
        {
            ai.ChangeState(AIStateEnum.IDLE);
            var par = Instantiate(Resources.Load<GameObject>("particles/drops/generic_item")) as GameObject;
            par.transform.parent = transform;
            par.transform.localPosition = Vector3.zero;
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
