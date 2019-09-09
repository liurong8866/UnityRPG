using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    public class HideBloodBar : MonoBehaviour
    {
        List<GameObject> hideObject = new List<GameObject>();
        void OnDestroy() {
            RestoreAll();
        }
        public void RestoreAll() {
            var arr = hideObject.ToArray();
            foreach(var o in arr) 
            {
                if(o != null) {
                    Show(o);
                }
            }
            hideObject.Clear();
        }

        void Awake()
        {
            var sp = gameObject.AddComponent<SphereCollider>();
            sp.isTrigger = true;
            sp.center = new Vector3(0, 2, 0);
            sp.radius = 2.5f;
        }

        void Hide(GameObject go) {
            var attr= go.GetComponent<NpcAttribute>();
            var tc = attr.TeamColor;
            var myTc = ObjectManager.objectManager.GetMyAttr().TeamColor;
            if(myTc != tc) {
                attr.ShowBloodBar = false;
            }
            attr.SetTeamHideShader();
            hideObject.Add(go);
        }

        void Show(GameObject go) {
            //Log.Sys("ShowBloodbar: "+go);
            var attr= go.GetComponent<NpcAttribute>();
            var tc = attr.TeamColor;
            var myTc = ObjectManager.objectManager.GetMyAttr().TeamColor;
            attr.ShowBloodBar = true;
            attr.SetTeamNormalShader();
            hideObject.Remove(go);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == GameTag.Player)
            {
                var attr = other.gameObject.GetComponent<NpcAttribute>();
                if(attr != null) {
                    Hide(attr.gameObject);
                }
            }
        }
        void OnTriggerExit(Collider other) {
            if(other.tag == GameTag.Player) {
                var attr = other.gameObject.GetComponent<NpcAttribute>();
                if(attr != null) {
                    
                    Show(attr.gameObject);
                }
            }
        }
    }
    public class  HideDead : DeadState
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
    public class HideBlockAI : AIBase
    {
        void Awake()
        {
            attribute = GetComponent<NpcAttribute>();
            attribute.ShowBloodBar = false;
            ai = new ChestCharacter();
            ai.attribute = attribute;
            var bd = new HideDead();
            ai.AddState(new ChestIdle());
            ai.AddState(bd);
            ai.AddState(new MonsterKnockBack());

            Util.SetLayer(gameObject, GameLayer.IgnoreCollision2);

            var g = new GameObject("HideBloodBar");
            g.transform.parent = transform;
            Util.InitGameObject(g);
            var hb = g.AddComponent<HideBloodBar>();
            bd.deadCallback = hb.RestoreAll;
        }



        void CreateParticle() {
            var p = Util.SpawnParticle("crystalGlow", transform.position, false);
            p.transform.parent = transform;
        }

        // Use this for initialization
        void Start()
        {
            ai.ChangeState(AIStateEnum.IDLE);
            CreateParticle();
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