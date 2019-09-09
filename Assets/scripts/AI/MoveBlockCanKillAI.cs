using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class MoveBlock : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            Log.Sys("MoveBlock Enter : " + other.gameObject);
            if (NetworkUtil.IsNetMaster())
            {
                if (other.tag == GameTag.Player)
                {
                    //击退技能
                    var pos = other.transform.position;
                    var otherGo = other.gameObject;
                    //dy dx 比较 那个大 保留那个 同时另外一个修正为 自己的pos
                    var par = transform.parent.gameObject;
                    var myPos = par.transform.position;

                    //假设箱子都是 正方体
                    var dx = myPos.x - pos.x;
                    var dz = myPos.z - pos.z;
                    if (Mathf.Abs(dx) < Mathf.Abs(dz))
                    {
                        pos.x = myPos.x;
                    } else
                    {
                        pos.z = myPos.z;
                    }

                    var skill = Util.GetSkillData((int)SkillData.SkillConstId.KnockBack, 1);
                    var skillInfo = SkillLogic.GetSkillInfo(skill);
                    var evt = skillInfo.eventList [0];
                    var ret = par.GetComponent<BuffComponent>().AddBuff(evt.affix, pos);
                    if (ret)
                    {
                        NetDateInterface.FastAddBuff(evt.affix, otherGo, par, skill.Id, evt.EvtId, pos);
                    }
                }
            }
        }
    }

    public class MoveBlockDead : DeadState  {
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
            /*
            yield return new WaitForSeconds(2);
            yield return GetAttr().StartCoroutine(Util.SetBurn(GetAttr().gameObject));
            yield return null;
            */

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
    public class MoveBlockCanKillAI : AIBase
    {
        void Awake()
        {
            attribute = GetComponent<NpcAttribute>();
            attribute.ShowBloodBar = false;

            ai = new ChestCharacter();
            ai.attribute = attribute;
            ai.AddState(new ChestIdle());
            var bd = new MoveBlockDead ();
            bd.deadCallback = () =>
            {
                Util.SpawnParticle("barrelbreak", transform.position+new Vector3(0, 0.3f, 0),false );
            };

            ai.AddState(bd);
            ai.AddState(new MonsterKnockBack());

            var cb = transform.Find("Cube");
            cb.gameObject.AddComponent<MoveBlock>();
        }

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