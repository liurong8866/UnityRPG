
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/

/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MyLib
{
    /// <summary>
    /// Backpack Data Controller
    /// </summary>
    public class BackPack : MonoBehaviour
    {
        public const int MaxBackPackNumber = 100;

        public static BackPack backpack;
        GameObject uiRoot;
        /*
		 * BackPack Data Init From Network
		 */
        private List<BackpackData> SlotData;


        /*
		 * EquipmentData Init From Network
		 */
        private List<EquipData> EquipmentData;

        public List<EquipData> GetEquipmentData()
        {
            return EquipmentData;
        }

        //要卸下来的装备的槽
        ItemData.EquipPosition equipSlot;

        EquipData oldEquip;
        GameObject equipObj;

        //背包物品所在槽
        int slotId;

        public int GetItemId(int itemId)
        {
            foreach (BackpackData bd in SlotData)
            {
                if (bd != null && bd.itemData != null && bd.baseId == itemId && bd.goodsType == (int)GoodsTypeEnum.Props)
                {
                    return (int)bd.id;
                }
            }
            return -1;
        }

        public int GetItemCount(int goodsType, int objId)
        {
            var count = 0;
            foreach (BackpackData bd in SlotData)
            {
                if (bd != null && bd.itemData != null && bd.baseId == objId && bd.goodsType == goodsType)
                {
                    count += bd.num;
                }
            }
            return count;
        }


        /// <summary>
        ///服务器更新背包数据  Don't ClearBag 
        /// </summary>
        /// <param name="info">Info.</param>
        public void SetItemInfo(GCPushPackInfo info)
        {
            //整理背包首先清空
            if (info.BackpackAdjust)
            {
                UserBagClear();
            }

            Log.Net("SetItemInfo " + info);
            foreach (PackInfo pkinfo in info.PackInfoList)
            {
                PutItemInBackpackIndex(pkinfo.PackEntry.Index, new BackpackData(pkinfo));
            }
            MyEventSystem.myEventSystem.PushEvent(MyEvent.EventType.UpdateItemCoffer);
        }

		
        //枚举背包物品
        public BackpackData EnumItem(MyLib.GameInterface_Package.PackagePageEnum type, int index)
        {
            var allSlots = SlotData;
            if (index < allSlots.Count)
            {
                return allSlots [index];
            }
            return null;
        }


        public BackpackData GetItemByTypeId(int id)
        {
            foreach (var s in SlotData)
            {
                if (s != null && s.itemData != null && s.itemData.ObjectId == id)
                {
                    return s;
                }
            }
            return null;
        }

        public BackpackData GetItemInBackPack(long propsId)
        {
            foreach (var s in SlotData)
            {
                if (s != null && s.itemData != null && s.id == propsId)
                {
                    return s;
                }
            }
            return null;
        }

        public EquipData EnumAction(ItemData.EquipPosition type)
        {
            Log.Important("Enum Action " + type);
            Log.Important("count " + EquipmentData.Count);
            foreach (EquipData ed in EquipmentData)
            {	

                if (ed.itemData != null && ed.itemData.equipPosition == type)
                {
                    return ed;
                }
            }
            return null;
        }




        void Awake()
        {
            DontDestroyOnLoad(gameObject);

            SlotData = new List<BackpackData>();
            EquipmentData = new List<EquipData>();

            for (int i = 0; i < MaxBackPackNumber; i++)
            {
                SlotData.Add(new BackpackData((PackInfo)null));
            }
            backpack = this;
        }


        /// <summary>
        /// 穿上装备 
        /// </summary>
        /// <param name="result">Result.</param>
        void UseEquipNetworkResp(GCUserDressEquip result)
        {
            Log.Net("UseEquipNetworkResp " + result);
            if (oldEquip != null)
            {
                PopEquipData(equipSlot);
            }
            var ed = new EquipData(result.DressEquip);
            EquipmentData.Add(ed);

            if (result.HasPackEquip)
            {
                var newBagItem = new BackpackData(result.PackEquip);
                PutItemInBackpackIndex(newBagItem);
            }
            MyEventSystem.PushEventStatic(MyEvent.EventType.UpdateItemCoffer);

            //更新角色属性面板的装备信息
            //以及角色本身的装备信息
            var evt = new MyEvent(MyEvent.EventType.CharEquipChanged);
            evt.localID = ObjectManager.objectManager.GetMyLocalId();
            evt.equipData = ed;
            evt.boolArg = true;
            MyEventSystem.myEventSystem.PushEvent(evt);

            //通知角色属性面板 更新UI上面的icon
            evt = new MyEvent(MyEvent.EventType.RefreshEquip);
            MyEventSystem.myEventSystem.PushEvent(evt);
        }

        //通过网络使用装备
        public IEnumerator UseEquipForNetwork()
        {
            Log.Important("Use EquipData For Network");
            var newEquip = SlotData [slotId];

            CGUserDressEquip.Builder equip = CGUserDressEquip.CreateBuilder();
            equip.DressType = true;
            equip.SrcEquipId = newEquip.id;

            if (oldEquip != null)
            {
                equip.DestEquipId = oldEquip.id;
            } else
            {
                equip.DestEquipId = 0;
            }

            var packet = new KBEngine.PacketHolder();
            yield return StartCoroutine(KBEngine.Bundle.sendSimple(this, equip, packet));
            if (packet.packet.responseFlag == 0)
            {
                var useResult = packet.packet.protoBody as GCUserDressEquip;

                UseEquipNetworkResp(useResult);
            } else
            {
            }
        }


        //初始化角色装备
        //只有玩家初始化结束之后 采取 初始化玩家的装备
        void UseEquip(EquipData ed)
        {
            Log.Important("initial role equip");
            EquipmentData.Add(ed);

            //角色本身的装备信息
            var evt = new MyEvent(MyEvent.EventType.CharEquipChanged);
            evt.localID = ObjectManager.objectManager.GetMyLocalId();
            evt.equipData = ed;
            evt.boolArg = true;
            MyEventSystem.myEventSystem.PushEvent(evt);
			
            //通知角色属性面板 更新UI上面的icon
            evt = new MyEvent(MyEvent.EventType.RefreshEquip);
            MyEventSystem.myEventSystem.PushEvent(evt);
        }

        int GetIndex(int ind)
        {
            return ind;
        }
        //设定操作的物品对象
        public void SetSlotItem(BackpackData data)
        {
            //背包index 和 slot数组的需要不同 index 从1开始 slot从0开始
            slotId = GetIndex(data.index);
            equipSlot = data.itemData.equipPosition;
            oldEquip = GetEquipData(equipSlot);
        }


        public BackpackData GetHpPotion()
        {
            for (int i = 0; i < SlotData.Count; i++)
            {
                var item = SlotData[i];
                if (SlotData [i] != null && SlotData [i].itemData != null &&  item.goodsType == (int)ItemData.GoodsType.Props && SlotData [i].itemData.UnitType == ItemData.UnitTypeEnum.POTION)
                {
                    return SlotData [i];
                }
            }
            return null;
        }

        BackpackData GetMpPotion()
        {
            for (int i = 0; i < SlotData.Count; i++)
            {
                if (SlotData [i] != null && SlotData [i].itemData != null && SlotData [i].itemData.UnitType == ItemData.UnitTypeEnum.POTION && SlotData [i].itemData.UnitEffect == ItemData.UnitEffectEnum.AddMP)
                {
                    return SlotData [i];
                }
            }
            return null;
        }


        //获取某个装备槽上面的装备
        public EquipData GetEquipData(ItemData.EquipPosition slot)
        {
            foreach (EquipData e in EquipmentData)
            {
                if (e.itemData != null && e.Slot == slot)
                    return e;
            }
            return null;
        }

        EquipData PopEquipData(ItemData.EquipPosition slot)
        {
            foreach (EquipData e in EquipmentData)
            {
                if (e.Slot == slot)
                {
                    EquipmentData.Remove(e);

                    return e;
                }
            }
            return null;
        }

        //打怪捡到钱之后，通知单人服务器，通过服务器通知更新数据
        //单人模式下连接本地服务器即可
        public void PutGold(int gold)
        {
            throw new System.NotImplementedException();
        }

        /*
		 * Put Item In Default Packet
		 * 物品数量更新只从服务器接受报文更新 本地不更新
		 */


        private void PutItemInBackpackIndex(BackpackData bd)
        {
            PutItemInBackpackIndex(bd.index, bd);
        }
        /*
		 * Initial Backpack Data From Network
		 */
        void PutItemInBackpackIndex(int index, BackpackData bd)
        {
            Log.Sys("Backpack::PutItem From Network In backpack " + bd.packInfo);
            if (index >= SlotData.Count || index < 0)
            {
                //Debug.LogError ("BackPack:: index out of Range " + index);
                return;
            }
            int realIndex = GetIndex(index);
            if (SlotData [realIndex] != null && SlotData [realIndex].itemData != null)
            {
                //Debug.LogError ("BackPack:; has object in index " + realIndex);
                //return;
            }
            if (bd.num == 0)
            {
                SlotData [realIndex] = null;
            } else
            {
                SlotData [realIndex] = bd;
            }
        }


        /*
		 * Clear All Item From BackPack 
		 * Init From Network
		 */
        private void UserBagClear()
        {
            for (int i = 0; i < SlotData.Count; i++)
            {
                ClearSlot(i);
            }
        }

        /*
		 * Remove All Equip From Slot To Load From Network
		 */
        private void UserEquipClear()
        {
            var values = System.Enum.GetValues(typeof(ItemData.EquipPosition)).Cast<ItemData.EquipPosition>();
            foreach (ItemData.EquipPosition s in values)
            {
                PopEquipData(s);
            }
        }



        /// <summary>
        /// 检查背包是否有空槽可以使用 
        /// </summary>
        /// <returns><c>true</c>, if backpack empty was checked, <c>false</c> otherwise.</returns>
        public bool CheckBackpackEmpty()
        {
            for (int i = 0; i < SlotData.Count; i++)
            {
                if (SlotData [i] == null || SlotData [i].itemData == null)
                    return true;
            }
            return false;
        }
		

        /*
		 * Network Init EquipmentData
		 */
        IEnumerator InitEquipData()
        {
            Debug.Log("BackPack::InitEquipData  ");
            UserEquipClear();

            var packet = new KBEngine.PacketHolder();
            CGLoadPackInfo.Builder equip = CGLoadPackInfo.CreateBuilder();
            equip.PackType = PackType.DRESSED_PACK;
            var data = equip.BuildPartial();
            KBEngine.Bundle bundle = new KBEngine.Bundle();
            bundle.newMessage(data.GetType());
            uint fid = bundle.writePB(data);
            yield return StartCoroutine(bundle.sendCoroutine(KBEngine.KBEngineApp.app.networkInterface(), fid, packet));
            if (packet.packet.responseFlag == 0)
            {
                var ret = packet.packet.protoBody as GCLoadPackInfo;
                foreach (PackInfo pkinfo in ret.PackInfoList)
                {
                    var eqData = new EquipData(pkinfo);
                    UseEquip(eqData);
                }
            } else
            {
            }
        }




        /// <summary>
        /// 每次进入一个新场景都重新初始化背包和装备信息
        /// </summary>
        /// <returns>The from network.</returns>
        public IEnumerator InitFromNetwork()
        {
            Log.Sys("BackPack::InitFromNetwork ");
            if (KBEngine.KBEngineApp.app == null)
            {
                Log.Sys("BackPack:: no network connection");

            } else
            {
                UserBagClear();

                var packet = new KBEngine.PacketHolder();
                CGLoadPackInfo.Builder load = CGLoadPackInfo.CreateBuilder();
                load.PackType = PackType.DEFAULT_PACK;
                var data = load.BuildPartial();
                KBEngine.Bundle bundle = new KBEngine.Bundle();
                bundle.newMessage(data.GetType());
                uint fid = bundle.writePB(data);
                yield return StartCoroutine(bundle.sendCoroutine(KBEngine.KBEngineApp.app.networkInterface(), fid, packet));
                if (packet.packet.responseFlag == 0)
                {
                    var ret = packet.packet.protoBody as GCLoadPackInfo;
                    foreach (PackInfo pkinfo in ret.PackInfoList)
                    {
                        Log.Sys("read Pack info is " + pkinfo.PackEntry.BaseId);
                        PutItemInBackpackIndex(pkinfo.PackEntry.Index, new BackpackData(pkinfo));
                    }
                } else
                {
                }
                Log.Important("LoadPacketInfo is " + load);
                yield return StartCoroutine(InitEquipData());
                MyEventSystem.myEventSystem.PushEvent(MyEvent.EventType.UpdateItemCoffer);
            }
        }

        //清空特定槽
        void ClearSlot(int sid)
        {
            SlotData [sid] = null;

            MyEventSystem.myEventSystem.PushEvent(MyEvent.EventType.UpdateItemCoffer);
        }

        /// <summary>
        ///更新金币数量 
        /// </summary>
        /// <param name="gc">Gc.</param>
        public void UpdateGoodsCount(GoodsCountChange gc)
        {
            if (gc.BaseId == 4)
            {
                var me = ObjectManager.objectManager.GetMyData();
                me.SetProp(CharAttribute.CharAttributeEnum.GOLD_COIN, gc.Num);
            } else if (gc.BaseId == 5)
            {
                var me = ObjectManager.objectManager.GetMyData();
                me.SetProp(CharAttribute.CharAttributeEnum.JING_SHI, gc.Num);
            }
        }

        /// <summary>
        /// 装备宝石升级成功 
        /// </summary>
        /// <param name="update">Update.</param>
        public void EquipDataUpdate(GCPushEquipDataUpdate update)
        {
            int c = 0;
            foreach (var b in EquipmentData)
            {
                if (b.id == update.PackInfo.PackEntry.Id)
                {
                    EquipmentData.RemoveAt(c);
                    break;
                }
                c++;
            }
            EquipmentData.Add(new EquipData(update.PackInfo));
            MyEventSystem.PushEventStatic(MyEvent.EventType.UpdateItemCoffer);
        }
    }
}