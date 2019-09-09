using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class DropItemStatic : MonoBehaviour
    {
        private ItemData itemData;
        private GameObject Particle;
        private int num;
        private bool pickYet = false;
        KBEngine.KBNetworkView netView;

        void Awake()
        {
            netView = gameObject.AddMissingComponent<KBEngine.KBNetworkView>();
        }

        void Start()
        {
            var player = ObjectManager.objectManager.GetMyPlayer();
            var c = gameObject.AddComponent<SphereCollider>();
            c.center = new Vector3(0, 1, 0);
            c.radius = 2;
            c.isTrigger = true;
        }

        void OnTriggerEnter(Collider other)
        {
            //只有Master才可以控制谁拾取到了物品
            if (NetworkUtil.IsNetMaster())
            {
                if (!pickYet && other.tag == GameTag.Player)
                {
                    StartCoroutine(PickItem(other.gameObject));
                }
            }
        }

        IEnumerator PickItem(GameObject who)
        {
            PickItemFromNetwork(who);

            var cg = CGPlayerCmd.CreateBuilder();
            cg.Cmd = "Pick";
            var pickAction = PickItemAction.CreateBuilder();
            pickAction.Id = netView.GetServerID();
            pickAction.ItemId = itemData.ObjectId;
            pickAction.ItemNum = num;
            pickAction.Who = who.GetComponent<KBEngine.KBNetworkView>().GetServerID();
            cg.PickAction = pickAction.Build();
            NetworkUtil.Broadcast(cg);
            NetworkUtil.RemoveEntityToNetwork(netView);

            yield break;
        }


        public void PickItemFromNetwork(GameObject who) {
            pickYet = true;
            BackgroundSound.Instance.PlayEffect("pickup");
            GameObject.Destroy(gameObject);
            var me = ObjectManager.objectManager.GetMyPlayer();
            if(who == me) {
                if(itemData.UnitType == ItemData.UnitTypeEnum.POWER_DRUG) {
                    WindowMng.windowMng.ShowNotifyLog("炸弹威力上升");
                    GameInterface_Backpack.LearnSkill((int)SkillData.SkillConstId.Bomb);
                }else if(itemData.UnitType == ItemData.UnitTypeEnum.QIPAO_DRUG) {
                    var attr = ObjectManager.objectManager.GetMyAttr();
                    attr.AddMpMax(20);
                    attr.AddThrowSpeed(0.1f);
                    WindowMng.windowMng.ShowNotifyLog("MP上限增加");

                }else if(itemData.UnitType == ItemData.UnitTypeEnum.XieZi_DRUG) {
                    ObjectManager.objectManager.GetMyAttr().AddNetSpeed(0.1f); 
                    WindowMng.windowMng.ShowNotifyLog("速度提升");
                }
                else {
                    GameInterface_Backpack.PickItem(itemData, num);
                }
            }
        }


        public static GameObject MakeDropItemFromNet(ItemData itemData, Vector3 pos, int num, EntityInfo info)
        {
            var g = Instantiate(Resources.Load<GameObject>(itemData.DropMesh)) as GameObject;
            var com = g.AddComponent<DropItemStatic>();
            com.itemData = itemData;
            g.transform.position = pos;

            var netView = g.GetComponent<KBEngine.KBNetworkView>();
            netView.SetID(new KBEngine.KBViewID(info.Id, ObjectManager.objectManager.myPlayer));
            netView.IsPlayer = false;
            ObjectManager.objectManager.AddObject(info.Id, netView);

            var par = Instantiate(Resources.Load<GameObject>("particles/drops/generic_item")) as GameObject;
            par.transform.parent = g.transform;
            par.transform.localPosition = Vector3.zero;
            com.Particle = par;
            com.num = num;

            com.StartCoroutine(WaitSound("dropgem"));
            return g;
        }

        public static void  MakeDropItem(ItemData itemData, Vector3 pos, int num)
        {
            if (NetworkUtil.IsNetMaster())
            {
                var cg = CGPlayerCmd.CreateBuilder();
                cg.Cmd = "AddEntity";
                var etyInfo = EntityInfo.CreateBuilder();
                etyInfo.ItemId = itemData.ObjectId;
                etyInfo.ItemNum = num;
                var po = NetworkUtil.ConvertPos(pos);
                etyInfo.X = po [0];
                etyInfo.Y = po [1];
                etyInfo.Z = po [2];
                etyInfo.EType = EntityType.DROP;
                cg.EntityInfo = etyInfo.Build();
                NetworkUtil.Broadcast(cg);
            }
        }

        static IEnumerator WaitSound(string s)
        {
            yield return new WaitForSeconds(0.2f);
            BackgroundSound.Instance.PlayEffect(s);
        }

        void OnDestroy()
        {
            var view = GetComponent<KBEngine.KBNetworkView>();

            ObjectManager.objectManager.DestroyByLocalId(view.GetLocalId());
        }
    }

}