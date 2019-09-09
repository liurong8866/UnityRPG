using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    public static class GameInterface_Backpack
    {
        /// <summary>
        /// 拾取某个物品
        /// 本地拾取物品
        /// </summary>
        public static void PickItem(ItemData itemData, int num)
        {
            var pick = CGPickItem.CreateBuilder();
            pick.ItemId = itemData.ObjectId;
            pick.Num = num;
            KBEngine.Bundle.sendImmediate(pick);
        }

        public static bool BuyItem(int itemId)
        {
            var buyItem = CGBuyShopProps.CreateBuilder();
            buyItem.ShopId = itemId;
            buyItem.Count = 1;
            //KBEngine.Bundle.sendImmediate(buyItem);
            var data = buyItem.Build();
            ServerPacketHandler.CGBuyShopProps.HandlePacket(data);
            return true;
        }

        public static int GetHpNum()
        {
            var hp = BackPack.backpack.GetHpPotion();
            if (hp == null)
            {
                return 0;
            }
            return hp.num;
        }

        public static void UseItem(int itemId)
        {
            var me = ObjectManager.objectManager.GetMyPlayer().GetComponent<AIBase>();
            if (me.GetAI().state.type != AIStateEnum.IDLE)
            {
                me.GetComponent<MyAnimationEvent>().InsertMsg(new MyAnimationEvent.Message(MyAnimationEvent.MsgType.IDLE));
            }
            me.GetComponent<NpcAttribute>().StartCoroutine(UseItemCor(itemId));
        }

        public static void ClearDrug()
        {
            var itemId = (int)ItemData.ItemID.DRUG;
            var backPackId = BackPack.backpack.GetItemId(itemId);
            var count = BackPack.backpack.GetItemCount((int)ItemData.GoodsType.Props, itemId);
            PlayerData.ReduceItem(backPackId, count);
        }

        static System.Collections.IEnumerator UseItemCor(int itemId)
        {
            yield return new WaitForSeconds(0.1f);
            var id = BackPack.backpack.GetItemId(itemId);
            var itemData = Util.GetItemData(0, itemId);

            var use = CGUseUserProps.CreateBuilder();
            use.UserPropsId = id;
            use.Count = 1;
            var packet = new KBEngine.PacketHolder();
            Log.Net("Send Use Item");
            yield return ClientApp.Instance.StartCoroutine(KBEngine.Bundle.sendSimple(ClientApp.Instance, use, packet));
            Log.Sys("UseResult " + packet.packet.flag);
            if (packet.packet.responseFlag == 0)
            {
                //GameInterface_Skill.MeUseSkill(itemData.triggerBuffId);
                GameInterface_Skill.AddSkillBuff(ObjectManager.objectManager.GetMyPlayer(), itemData.triggerBuffId, Vector3.zero);
            }
        }

        public static List<NumMoney> GetChargetList()
        {
            return null;
        }


        public static bool inTransaction = false;
        public static NumMoney lastCharge;

        public static void Charge(NumMoney nm)
        {
            if (inTransaction)
            {
                Debug.LogError("InCharing");
                return;
            }

            inTransaction = true;
            lastCharge = nm;
            SimpleIAP.GetInstance().ChargeItem(nm.itemId);
        }

        public static void SetSkillPos(int skillId, int pos)
        {
            PlayerData.SetSkillPos(skillId, pos);
        }

        /// <summary>
        /// 学习技能书 学习书籍的新技能
        /// 或者增加一个技能点 
        /// </summary>
        /// <param name="propsId">Properties identifier.</param>
        public static void LearnSkillBook(long propsId)
        {
            var backpackData = BackPack.backpack.GetItemInBackPack(propsId);
            if (backpackData != null)
            {
                Log.Sys("LearnForgeSkill " + propsId + " userData " + backpackData.itemData.propsConfig.UserData);
                //backpackData.itemData.propsConfig.
                var skillId = (int)System.Convert.ToSingle(backpackData.itemData.propsConfig.UserData);

                var pinfo = ServerData.Instance.playerInfo;
                var allSkill = pinfo.Skill;
                var find = false;
                foreach (var s in allSkill.SkillInfosList)
                {
                    if (s.SkillInfoId == skillId)
                    {
                        find = true;
                        break;
                    }
                }

                var ret = PlayerData.ReduceItem(propsId, 1);
                if (ret)
                {
                    //AddSkillPoint
                    if (find)
                    {
                        PlayerData.AddSkillPoint(1);
                    } else
                    {
                        PlayerData.LearnSkill(skillId);
                    }
                }

            }
        }

        public static void LearnSkill(int skillId)
        {
            var pinfo = ServerData.Instance.playerInfo;
            var allSkill = pinfo.Skill;
            var find = false;

            Log.Sys("LearnSkill: "+skillId+" info "+ServerData.Instance.p2);
            foreach (var s in allSkill.SkillInfosList)
            {
                if (s.SkillInfoId == skillId)
                {
                    find = true;
                    break;
                }
            }

            if (find)
            {
                PlayerData.AddSkillPoint(1);
                PlayerData.LevelUpSkill(skillId);
            } else
            {
                PlayerData.LearnSkill(skillId);
            }
        }

    }

}