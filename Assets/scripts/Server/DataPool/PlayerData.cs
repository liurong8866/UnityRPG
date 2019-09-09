using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    /// <summary>
    /// Player data InServer
    /// 对服务器数据的修改都要通过PlayerData进行客户端不能直接修改
    /// 对服务器对象的RPC 简化通过直接函数调用的方式命令队列话
    /// </summary>
    public static class PlayerData
    {
        public static void AddExp(int add)
        {
            var pinfo = ServerData.Instance.playerInfo;
            pinfo.Exp += add;
            var push = GCPushExpChange.CreateBuilder();
            push.Exp = pinfo.Exp;
            ServerBundle.SendImmediatePush(push);
        }

        public static void SetLevel(int v)
        {
            var pinfo = ServerData.Instance.playerInfo;
            pinfo.Roles.Level = v;
        
            var push = GCPushLevel.CreateBuilder();
            push.Level = pinfo.Roles.Level;
            ServerBundle.SendImmediatePush(push);
        }

        public static void AddLevel(int add)
        {
            Log.Net("AddLevel " + add);
            var pinfo = ServerData.Instance.playerInfo;
            pinfo.Roles.Level += add;
            AddSkillPoint(1);

            var push = GCPushLevel.CreateBuilder();
            push.Level = pinfo.Roles.Level;
            ServerBundle.SendImmediatePush(push);
        }

        public static void AddSkillPoint(int sp)
        {
            var pinfo = ServerData.Instance.playerInfo;
            if (!pinfo.HasSkill)
            {
                pinfo.Skill = GCLoadSkillPanel.CreateBuilder().Build();
            }
            pinfo.Skill.TotalPoint += sp;

            var psp = GCPushSkillPoint.CreateBuilder();
            psp.SkillPoint = pinfo.Skill.TotalPoint;
            ServerBundle.SendImmediatePush(psp);
            if (sp > 0)
            {
                //SendNotify("升级技能点+" + sp);
            } else
            {
                //SendNotify("技能点" + sp);
            }
        }

        public static void AddJingShi(int num)
        {
            var itemData = Util.GetItemData(0, (int)ItemData.ItemID.JING_SHI);
            var has = ServerData.Instance.playerInfo.JingShi;
            SetJingShi(has + num);
            if (num > 0)
            {
                SendNotify(string.Format("[ff9500]{0}+{1}[-]", itemData.ItemName, num));
            } else if (num < 0)
            {
                SendNotify(string.Format("[ff1010]{0}{1}[-]", itemData.ItemName, num));
            }
            ServerData.Instance.SaveUserData();
        }

        public static void AddGold(int num)
        {
            var itemData = Util.GetItemData(0, (int)ItemData.ItemID.GOLD);
            var has = ServerData.Instance.playerInfo.Gold;
            SetGold(has + num);
            if (num > 0)
            {
                SendNotify(string.Format("[ff9500]{0}+{1}[-]", itemData.ItemName, num));
            } else if (num < 0)
            {
                SendNotify(string.Format("[ff1010]{0}{1}[-]", itemData.ItemName, num));
            }
        }

        public static void SetJingShi(int num)
        {
            ServerData.Instance.playerInfo.JingShi = num;
            //Notify
            var gc = GoodsCountChange.CreateBuilder();
            gc.Type = 0;
            gc.BaseId = (int)ItemData.ItemID.JING_SHI;
            gc.Num = Mathf.Max(0, num);
            var n = GCPushGoodsCountChange.CreateBuilder();
            n.AddGoodsCountChange(gc);
            ServerBundle.SendImmediatePush(n);
        }

        public static void SetGold(int num)
        {
            ServerData.Instance.playerInfo.Gold = num;
            //Notify
            var gc = GoodsCountChange.CreateBuilder();
            gc.Type = 0;
            gc.BaseId = (int)ItemData.ItemID.GOLD;
            gc.Num = Mathf.Max(0, num);
            var n = GCPushGoodsCountChange.CreateBuilder();
            n.AddGoodsCountChange(gc);
            ServerBundle.SendImmediatePush(n);
        }

        public static bool IsPackageFull(int itemId, int num)
        {
            var pinfo = ServerData.Instance.playerInfo;
            var itemData = Util.GetItemData(0, itemId);
            if (itemData.UnitType == ItemData.UnitTypeEnum.GOLD)
            {
                return false;
            }

            if (itemData.MaxStackSize > 1)
            {
                Log.Sys("AddItemInStack");
                foreach (var p in pinfo.PackInfoList)
                {
                    if (p.PackEntry.BaseId == itemId)
                    {
                        return false;
                    }
                }
            }

            PackInfo[] packList = new PackInfo[BackPack.MaxBackPackNumber];
            foreach (var p in pinfo.PackInfoList)
            {
                packList [p.PackEntry.Index] = p;
            }

            for (int i = 0; i < BackPack.MaxBackPackNumber; i++)
            {
                if (packList [i] == null)
                {
                    return false;
                }
            }
            return true;
        }

        private static bool RemoveEquipFromPackage(long id)
        {
            var player = MyLib.ServerData.Instance.playerInfo;
            foreach (var pinfo in player.PackInfoList)
            {
                if (pinfo.PackEntry.Id == id)
                {
                    pinfo.PackEntry.Count = 0;
                    player.PackInfoList.Remove(pinfo);

                    var push = GCPushPackInfo.CreateBuilder();
                    push.BackpackAdjust = false;
                    push.PackType = PackType.DEFAULT_PACK;
                    push.PackInfoList.Add(pinfo);
                    ServerBundle.SendImmediatePush(push);
                    return true;
                }
            }
            SendNotify("未找到该道具");
            return false;
        }

        private static void PutEquipInPackage(PackInfo pi)
        {
            var pinfo = ServerData.Instance.playerInfo;
            PackInfo[] packList = new PackInfo[BackPack.MaxBackPackNumber];

            foreach (var p in pinfo.PackInfoList)
            {
                packList [p.PackEntry.Index] = p;
            }

            for (int i = 0; i < BackPack.MaxBackPackNumber; i++)
            {
                if (packList [i] == null)
                {
                    pi.PackEntry.Index = i;
                    pi.PackEntry.Count = 1;
                    pinfo.PackInfoList.Add(pi);

                    var push = GCPushPackInfo.CreateBuilder();
                    push.BackpackAdjust = false;
                    push.PackType = PackType.DEFAULT_PACK;
                    push.PackInfoList.Add(pi);
                    ServerBundle.SendImmediatePush(push);
                    return;
                }
            }


            SendNotify("背包空间不足");


        }

        /// <summary>
        /// 背包中增加新获得的武器
        /// 背包改造为无限长度 并且没有限制 
        /// </summary>
        public static void AddEquipInPackage(int equipId, int initAttack, int initDefense)
        {
            Log.Sys("AddEquipInpackage " + equipId + " initAttack " + initAttack + " initDef " + initDefense);
            var pinfo = ServerData.Instance.playerInfo;
            var itemData = Util.GetItemData((int)GoodsTypeEnum.Equip, equipId);

            PackInfo[] packList = new PackInfo[BackPack.MaxBackPackNumber];
            long maxId = 0;
            foreach (var p in pinfo.PackInfoList)
            {
                packList [p.PackEntry.Index] = p;
                maxId = (long)Mathf.Max(p.PackEntry.Id + 1, maxId);
            }

            foreach (var p in pinfo.DressInfoList)
            {
                maxId = (long)Mathf.Max(p.PackEntry.Id + 1, maxId);
            }

            if (maxId < 0)
            {
                maxId++;
            }
            for (int i = 0; i < BackPack.MaxBackPackNumber; i++)
            {
                if (packList [i] == null)
                {
                    var pkInfo = PackInfo.CreateBuilder();
                    var pkentry = PackEntry.CreateBuilder();
                    pkInfo.CdTime = 0;

                    pkentry.Id = maxId;
                    pkentry.BaseId = equipId;
                    pkentry.GoodsType = 1;
                    pkentry.Count = 1;
                    pkentry.Index = i;
                    pkentry.Level = 1;
                    pkentry.RndAttack = initAttack;
                    pkentry.RndDefense = initDefense;

                    pkInfo.PackEntry = pkentry.Build();
                    var msg = pkInfo.Build();
                    pinfo.PackInfoList.Add(msg);

                    var push = GCPushPackInfo.CreateBuilder();
                    push.BackpackAdjust = false;
                    push.PackType = PackType.DEFAULT_PACK;
                    push.PackInfoList.Add(msg);
                    ServerBundle.SendImmediatePush(push);

                    SendNotify(string.Format("[ff0a0a]{0}+{1}[-]", itemData.ItemName, 1));
                    return;
                }
            }

            //PackFull
            var notify = GCPushNotify.CreateBuilder();
            notify.Notify = "[f00b00]背包已满[-]";
            ServerBundle.SendImmediatePush(notify);
        }


        //Notify
        //Props Stack
        /// <summary>
        /// 得到的道具装备都是白装，自己不断升级，类似怪物猎人 
        /// </summary>
        /// <param name="itemId">Item identifier.</param>
        /// <param name="num">Number.</param>
        public static void AddItemInPackage(int itemId, int num)
        {
            var pinfo = ServerData.Instance.playerInfo;
            var itemData = Util.GetItemData(0, itemId);
            if (itemData.UnitType == ItemData.UnitTypeEnum.GOLD)
            {
                PlayerData.AddGold(num);
                return;
            }

            //Has Such Objects 
            if (itemData.MaxStackSize > 1)
            {
                Log.Sys("AddItemInStack");
                foreach (var p in pinfo.PackInfoList)
                {
                    if (p.PackEntry.BaseId == itemId && p.PackEntry.GoodsType == (int)ItemData.GoodsType.Props)
                    {
                        pinfo.PackInfoList.Remove(p);
                        var newPkinfo = PackInfo.CreateBuilder(p);
                        var newPkEntry = PackEntry.CreateBuilder(p.PackEntry);
                        newPkEntry.Count += num;
                        newPkinfo.PackEntry = newPkEntry.Build();
                        var msg = newPkinfo.Build();
                        pinfo.PackInfoList.Add(msg);


                        var push = GCPushPackInfo.CreateBuilder();
                        push.BackpackAdjust = false;
                        push.PackType = PackType.DEFAULT_PACK;
                        push.PackInfoList.Add(msg);

                        ServerBundle.SendImmediatePush(push);

                        SendNotify(string.Format("[ff0a0a]{0}+{1}[-]", itemData.ItemName, num));
                        return;
                    }
                }
            }
            //new Item
            //all Slot
            PackInfo[] packList = new PackInfo[BackPack.MaxBackPackNumber];
            long maxId = 0;
            foreach (var p in pinfo.PackInfoList)
            {
                packList [p.PackEntry.Index] = p;
                maxId = (long)Mathf.Max(p.PackEntry.Id + 1, maxId);
            }

            foreach (var p in pinfo.DressInfoList)
            {
                maxId = (long)Mathf.Max(p.PackEntry.Id + 1, maxId);
            }

            if (maxId < 0)
            {
                maxId++;
            }

            for (int i = 0; i < BackPack.MaxBackPackNumber; i++)
            {
                if (packList [i] == null)
                {
                    var pkInfo = PackInfo.CreateBuilder();
                    var pkentry = PackEntry.CreateBuilder();
                    pkInfo.CdTime = 0;

                    pkentry.Id = maxId;
                    pkentry.BaseId = itemId;
                    pkentry.GoodsType = 0;
                    pkentry.Count = num;
                    pkentry.Index = i;

                    pkInfo.PackEntry = pkentry.Build();
                    var msg = pkInfo.Build();
                    pinfo.PackInfoList.Add(msg);

                    var push = GCPushPackInfo.CreateBuilder();
                    push.BackpackAdjust = false;
                    push.PackType = PackType.DEFAULT_PACK;
                    push.PackInfoList.Add(msg);
                    ServerBundle.SendImmediatePush(push);

                    SendNotify(string.Format("[ff0a0a]{0}+{1}[-]", itemData.ItemName, num));
                    return;
                }
            }

            //PackFull
            var notify = GCPushNotify.CreateBuilder();
            notify.Notify = "背包已满";
            ServerBundle.SendImmediatePush(notify);

        }

        public static bool ReduceItem(long userPropsId, int num)
        {
            var player = MyLib.ServerData.Instance.playerInfo;
            foreach (var pinfo in player.PackInfoList)
            {
                if (pinfo.PackEntry.Id == userPropsId)
                {
                    if (pinfo.PackEntry.Count < num)
                    {
                        SendNotify("道具数量不足");
                        return false;
                    }
                    pinfo.PackEntry.Count -= num;
                    if (pinfo.PackEntry.Count <= 0)
                    {
                        player.PackInfoList.Remove(pinfo);
                    }


                    var push = GCPushPackInfo.CreateBuilder();
                    push.BackpackAdjust = false;
                    push.PackType = PackType.DEFAULT_PACK;
                    push.PackInfoList.Add(pinfo);
                    ServerBundle.SendImmediatePush(push);

                    return true;
                }
            }
            SendNotify("未找到该道具");
            return false;
        }

        public static void  SendNotify(string str)
        {
            Log.Sys("SendNotify: "+str);
            var no = GCPushNotify.CreateBuilder();
            no.Notify = str;
            ServerBundle.SendImmediatePush(no);
        }

        public static void ResetSkillLevel()
        {
            var pinfo = ServerData.Instance.playerInfo;
            var skPanel = GCLoadSkillPanel.CreateBuilder();
            pinfo.Skill = skPanel.Build(); 
            pinfo.Roles.Level = 10;
            AddSkillPoint(10);
            var skData = GameData.SkillConfig;
            var count = 0;
            foreach (var s in skData)
            {
                if (s.job == (int)pinfo.Roles.Job)
                {
                    //LevelUpSkill(s.id);
                    LearnSkill(s.id);
                    count++;
                    SetSkillPos(s.id, count);
                    if (count >= 2)
                    {
                        break;
                    }
                }
            }
            //LevelUpSkill(3);
            //LevelUpSkill(4);

            /*
            if (pinfo.HasSkill)
            {
                foreach (var s in pinfo.Skill.SkillInfosList)
                {
                    s.Level = 1;
                    var activeSkill = GCPushActivateSkill.CreateBuilder();
                    activeSkill.SkillId = s.SkillInfoId;
                    activeSkill.Level = 1;
                    ServerBundle.SendImmediatePush(activeSkill);
                }
            }
            */
        }

        public static void LevelUpSkill(int skId)
        {
            Debug.Log("LevelUpSkill: "+skId);
            var pinfo = ServerData.Instance.playerInfo;
            int totalSP = 0;
            int skillLevel = 0;
            if (pinfo.HasSkill)
            {
                totalSP = pinfo.Skill.TotalPoint;
                foreach (var s in pinfo.Skill.SkillInfosList)
                {
                    if (s.SkillInfoId == skId)
                    {
                        skillLevel = s.Level;
                        break;
                    }
                }
            }
            var skData = Util.GetSkillData(skId, skillLevel);
            var maxLev = skData.MaxLevel;
            var playerLev = pinfo.Roles.Level;
            var next = Util.GetSkillData(skId, skillLevel + 1);
            var needLev = next.LevelRequired;

            if (totalSP > 0 && skillLevel < maxLev && playerLev >= needLev)
            {
                Log.Sys("LearnSkill");
                AddSkillPoint(-1);

                bool find = false;
                foreach (var sk in pinfo.Skill.SkillInfosList)
                {
                    if (sk.SkillInfoId == skId)
                    {
                        sk.Level++;
                        find = true;
                        break;
                    }
                }
                if (!find)
                {
                    var newinfo = SkillInfo.CreateBuilder();
                    newinfo.SkillInfoId = skId;
                    newinfo.Level = 1;
                    pinfo.Skill.SkillInfosList.Add(newinfo.Build());
                }

                //pinfo.Skill = newSk.Build();

                var activeSkill = GCPushActivateSkill.CreateBuilder();
                activeSkill.SkillId = skId;
                activeSkill.Level = skillLevel + 1;
                ServerBundle.SendImmediatePush(activeSkill);

            } else if (totalSP <= 0)
            {
                SendNotify("剩余技能点不足");
            } else if (playerLev < needLev)
            {
                SendNotify("等级不足");
            } else
            {
                SendNotify("技能已经升到满级");
            }
            PushSkillShortcutsInfo();

        }

        static SkillInfo GetSkillInfo(int skillId)
        {
            var pinfo = ServerData.Instance.playerInfo;
            foreach (var sk in pinfo.Skill.SkillInfosList)
            {
                if (sk.SkillInfoId == skillId)
                {
                    return sk;
                }
            }
            return null;
        }

        /// <summary>
        /// 没有初始化的时候 按照技能的ID顺序显示技能的快捷键
        /// 技能的Pos是 1 2 3 4
        /// 存储的Short信息是 0 1 2 3  
        /// </summary>
        /// <returns>The short cuts info.</returns>
        static int[] GetShortCutsInfo()
        {
            var pinfo = ServerData.Instance.playerInfo;
            var defaultSlot = new int[4]{ 0, 0, 0, 0 };
            if (pinfo.HasSkill)
            {
                int ind = 0;
                foreach (var sk in pinfo.Skill.SkillInfosList)
                {
                    defaultSlot [ind] = sk.SkillInfoId;
                    ind++;
                    if (ind >= defaultSlot.Length)
                    {
                        break;
                    }
                }

                foreach (var sk in pinfo.Skill.SkillInfosList)
                {
                    if (sk.Pos != 0)
                    {
                        defaultSlot [sk.Pos - 1] = sk.SkillInfoId;
                    }
                }


                ind = 0;
                //防止同一个技能出现两次, 同一个技能出现在default和设置的Pos里面
                foreach (var sk in defaultSlot)
                {
                    var skill = GetSkillInfo(sk);
                    if (skill != null && skill.Pos != 0 && (skill.Pos - 1) != ind)
                    {
                        defaultSlot [ind] = 0;
                    }
                    ind++;
                }
            }
            return defaultSlot;
        }

        /// <summary>
        /// 默认前4种技能占用 1 2 3 4 的槽
        /// 当有技能设置了槽 则使用该技能占用 特定槽 
        /// </summary>
        static void PushSkillShortcutsInfo()
        {
            var au = GCPushShortcutsInfo.CreateBuilder();
            var pinfo = ServerData.Instance.playerInfo;
            if (pinfo.HasSkill)
            {
                var defaultSlot = GetShortCutsInfo(); 
                var ind = 0;
                foreach (var sk in defaultSlot)
                {
                    if (sk != 0)
                    {
                        var sh = ShortCutInfo.CreateBuilder();
                        sh.Index = ind;
                        sh.BaseId = sk;
                        au.AddShortCutInfo(sh);
                    }
                    ind++;
                }
            }
            ServerBundle.SendImmediatePush(au);
        }

        public static void GetShortCuts(KBEngine.Packet packet)
        {
            var au = GCLoadShortcutsInfo.CreateBuilder();
            var pinfo = ServerData.Instance.playerInfo;
            if (pinfo.HasSkill)
            {
                var defaultSlot = GetShortCutsInfo();
                int ind = 0;
                foreach (var sk in defaultSlot)
                {
                    if (sk != 0)
                    {
                        var sh = ShortCutInfo.CreateBuilder();
                        sh.Index = ind;
                        sh.BaseId = sk;
                        au.AddShortCutInfo(sh);
                    }
                    ind++;
                }
            }
            ServerBundle.SendImmediate(au, packet.flowId);
        }

        public static void GetProp(KBEngine.Packet g)
        {
            var pinfo = ServerData.Instance.playerInfo;
            var getChar = g.protoBody as CGGetCharacterInfo;
            var au = GCGetCharacterInfo.CreateBuilder();
            foreach (var k in getChar.ParamKeyList)
            {
                var att = RolesAttributes.CreateBuilder();
                var bd = BasicData.CreateBuilder();
                att.AttrKey = k;
                bd.Indicate = 1;
                if (k == (int)CharAttribute.CharAttributeEnum.LEVEL)
                {
                    bd.TheInt32 = pinfo.Roles.Level; 
                } else if (k == (int)CharAttribute.CharAttributeEnum.EXP)
                {
                    bd.TheInt32 = pinfo.Exp;
                } else if (k == (int)CharAttribute.CharAttributeEnum.GOLD_COIN)
                {
                    bd.TheInt32 = pinfo.Gold;
                } else if (k == (int)CharAttribute.CharAttributeEnum.JING_SHI)
                {
                    bd.TheInt32 = pinfo.JingShi;
                }
                att.BasicData = bd.Build();
                au.AddAttributes(att);
            }

            ServerBundle.SendImmediate(au, g.flowId);
        }

        public static void AddProp(KBEngine.Packet p)
        {
            Log.Net("ServerAdddProp");
            var pinfo = ServerData.Instance.playerInfo;
            var inpb = p.protoBody as CGAddProp;
            if (inpb.Key == (int)CharAttribute.CharAttributeEnum.EXP)
            {
                pinfo.Exp += inpb.Value; 
            } else if (inpb.Key == (int)CharAttribute.CharAttributeEnum.LEVEL)
            {
                AddLevel(inpb.Value);
            }
        }

        public static void SetProp(KBEngine.Packet p)
        {
            var pinfo = ServerData.Instance.playerInfo;
            var inpb = p.protoBody as CGSetProp;
            if (inpb.Key == (int)CharAttribute.CharAttributeEnum.EXP)
            {
                pinfo.Exp = inpb.Value;
            } else if (inpb.Key == (int)CharAttribute.CharAttributeEnum.LEVEL)
            {
                SetLevel(inpb.Value);
            }
        }

        public static void CreateCharacter(KBEngine.Packet packet)
        {
            var inpb = packet.protoBody as CGCreateCharacter;
            var au = GCCreateCharacter.CreateBuilder();
            var role = RolesInfo.CreateBuilder();
            role.Name = inpb.PlayerName;
            role.PlayerId = 100;
            //playerId++;
            role.Level = 1;
            role.Job = inpb.Job;

            var msg = role.Build();
            ServerData.Instance.playerInfo.Roles = RolesInfo.CreateBuilder(msg).Build();

            var dress = new int[]
            {
                97, 68, 69, 70, 71, 72,
            };
            int id = 1;
            var pinfo = ServerData.Instance.playerInfo;
            foreach (var d in dress)
            {
                var pkinfo = PackInfo.CreateBuilder();
                var pkEntry = PackEntry.CreateBuilder();
                pkEntry.Id = id++;
                pkEntry.BaseId = d;
                pkEntry.GoodsType = 1;
                pkEntry.Level = 1;
                pkinfo.PackEntry = pkEntry.Build();
                pinfo.AddDressInfo(pkinfo);
            }

            var cinfo = GCCopyInfo.CreateBuilder();
            var cin = CopyInfo.CreateBuilder();
            cin.Id = 209;
            cin.IsPass = true;
            cinfo.AddCopyInfo(cin);
            var msg2 = cinfo.Build();
            pinfo.CopyInfos = msg2;

            au.AddRolesInfos(msg);
            ServerBundle.SendImmediate(au, packet.flowId);    

            //投掷 跳跃
            pinfo.Roles.Level = 10;
            AddSkillPoint(10);
            LevelUpSkill(3);
            LevelUpSkill(4);
        }

        /// <summary>
        /// First Chapter Lev 
        /// </summary>
        /// <param name="openLev">Open lev.</param>
        public static void PassLev(int chapter, int openLev)
        {
            var pinfo = ServerData.Instance.playerInfo;
            if (!pinfo.HasCopyInfos)
            {
                var au = GCCopyInfo.CreateBuilder();
                var cin = CopyInfo.CreateBuilder();
                cin.Id = 101;
                cin.IsPass = false;
                au.AddCopyInfo(cin);
                var msg = au.Build();
                pinfo.CopyInfos = msg;
            }
            int levId = chapter * 100 + openLev;
            //int levId = -1;
            //int count = -1;
            bool find = false;
            foreach (var c in pinfo.CopyInfos.CopyInfoList)
            {
                if (c.Id == levId)
                {
                    c.IsPass = true;
                    find = true;
                    break;
                }
            }
            if (!find)
            {
                var cinfo = CopyInfo.CreateBuilder();
                cinfo.Id = levId;
                cinfo.IsPass = true;
                pinfo.CopyInfos.CopyInfoList.Add(cinfo.Build());
            }
            foreach (var c in pinfo.CopyInfos.CopyInfoList)
            {
                if (c.Id < levId)
                {
                    c.IsPass = true;
                }
            }

            var open = GCPushLevelOpen.CreateBuilder();
            open.Chapter = chapter;
            open.Level = openLev;
            ServerBundle.SendImmediatePush(open);

            Log.Net("PassLevelData " + pinfo.Build().ToString());
        }

        public static void LoadPackInfo(KBEngine.Packet packet)
        {
            var inpb = packet.protoBody as CGLoadPackInfo;
            var au = GCLoadPackInfo.CreateBuilder();
            if (inpb.PackType == PackType.DEFAULT_PACK)
            {
                au.PackType = PackType.DEFAULT_PACK;
                var pkInfo = ServerData.Instance.playerInfo.PackInfoList;
                au.AddRangePackInfo(pkInfo);
            } else if (inpb.PackType == PackType.DRESSED_PACK)
            {
                au.PackType = PackType.DRESSED_PACK;
                var pkInfo = ServerData.Instance.playerInfo.DressInfoList;
                au.AddRangePackInfo(pkInfo);
            }
            ServerBundle.SendImmediate(au, packet.flowId);
        }

        public static void GetKeyValue(KBEngine.Packet packet)
        {
            var playerInfo = ServerData.Instance.playerInfo;
            var inpb = packet.protoBody as CGGetKeyValue;
            var key = inpb.Key;
            var get = GCGetKeyValue.CreateBuilder();
            bool find = false;
            foreach (var kv1 in playerInfo.GameStateList)
            {
                if (kv1.Key == key)
                {
                    get.Value = kv1.Value;
                    find = true;
                    break;
                }
            }
            if (!find)
            {
                get.Value = "";
            }
            ServerBundle.SendImmediate(get, packet.flowId);
        }

        public static void SetKeyValue(KBEngine.Packet packet)
        {
            var playerInfo = ServerData.Instance.playerInfo;
            var inpb = packet.protoBody as CGSetKeyValue;
            bool find = false;
            foreach (var kv in playerInfo.GameStateList)
            {
                if (kv.Key == inpb.Kv.Key)
                {
                    kv.Value = inpb.Kv.Value;
                    find = true;
                    break;
                }
            }
            if (!find)
            {
                playerInfo.GameStateList.Add(inpb.Kv);
            }
        }

        static bool CheckGold(int cost)
        {
            return true;
        }

        /// <summary>
        /// 根据ID获得背包中的道具 
        /// </summary>
        /// <returns>The item in pack.</returns>
        /// <param name="id">Identifier.</param>
        public static PackInfo GetItemInPack(long id)
        {
            var playerInfo = ServerData.Instance.playerInfo;
            foreach (var p in playerInfo.PackInfoList)
            {
                if (p.PackEntry.Id == id)
                {
                    return p;
                }
            }
            return null;
        }

        public static void LevelUpEquip(KBEngine.Packet packet)
        {
            var playerInfo = ServerData.Instance.playerInfo;
            var inpb = packet.protoBody as CGLevelUpEquip;
            var eid = inpb.EquipId;
            var curLev = 0;
            PackInfo packInfo = null; 
            foreach (var e in playerInfo.DressInfoList)
            {
                if (e.PackEntry.Id == eid)
                {
                    packInfo = e;
                    break;
                }
            }
            if (packInfo != null)
            {
                curLev = packInfo.PackEntry.Level + 1;
                var levCost = GMDataBaseSystem.SearchIdStatic<EquipLevelData>(GameData.EquipLevel, curLev);
                if (levCost == null)
                {
                    SendNotify("装备已经升到顶级了");
                    return;
                }

                var myGold = playerInfo.Gold;
                if (levCost.gold > myGold)
                {
                    SendNotify("金币不足");
                    return;
                }
                AddGold(-levCost.gold);

                var extraAtt = 0;
                var extraDef = 0;
                foreach (var g in inpb.GemIdList)
                {
                    var pinfo = GetItemInPack(g);
                    var itemData = Util.GetItemData(0, pinfo.PackEntry.BaseId);
                    extraAtt += itemData.GetRndAtk();
                    extraDef += itemData.GetRndDef();
                    ReduceItem(g, 1);
                }
                var rate = Random.Range(0, 100);
                if (rate < levCost.rate)
                {
                    packInfo.PackEntry.Level++;
                    packInfo.PackEntry.ExtraAttack += extraAtt;
                    packInfo.PackEntry.ExtraDefense += extraDef;
                    SendNotify("装备升级成功, 本次概率" + rate);

                    var update = GCPushEquipDataUpdate.CreateBuilder();
                    update.PackInfo = packInfo;
                    ServerBundle.SendImmediatePush(update);
                } else
                {
                    SendNotify("装备升级失败, 本次概率" + rate + " 需要概率 " + levCost.rate);
                }
            }
        }

        public static void LevelUpGem(KBEngine.Packet packet)
        {
            var inpb = packet.protoBody as CGLevelUpGem;
            if (inpb.GemIdCount == 2)
            {
                var gemId = inpb.GemIdList [0];
                var gem = GetItemInPack(gemId);
                var itemData = Util.GetItemData(0, gem.PackEntry.BaseId);

                var targetGem = GameInterface_Package.GetRndGem(itemData.Level + 1);
                if (targetGem != null)
                {
                    AddItemInPackage(targetGem.id, 1);
                    SendNotify("合成成功，本次概率" + GameInterface_Package.lastPossibility);
                } else
                {
                    SendNotify("合成失败, 本次概率" + GameInterface_Package.lastPossibility);
                }
                foreach (var g in inpb.GemIdList)
                {
                    ReduceItem(g, 1);
                }
            } else if (inpb.GemIdCount == 1)
            {//一键快速合成
                var gemId = inpb.GemIdList [0];
                var gem = GetItemInPack(gemId);
                var itemData = Util.GetItemData(0, gem.PackEntry.BaseId);
                int count = gem.PackEntry.Count / 2;
                int useNum = count;
                int allRate = GameInterface_Package.GetAllGemRate(itemData.Level + 1);
                PropsConfigData targetGem = null;
                int getNum = 0;
                while (count > 0)
                {
                    if (targetGem == null)
                    {
                        targetGem = GameInterface_Package.GetRndGem(itemData.Level + 1);
                        if (targetGem != null)
                        {
                            getNum++;
                        }
                    } else
                    {
                        var ret = GameInterface_Package.GetRndGemForId(itemData.Level + 1, targetGem.id, allRate);
                        if (ret > 0)
                        {
                            getNum++;
                        }
                    }
                    count--;
                }
                if (getNum > 0)
                {
                    AddItemInPackage(targetGem.id, getNum);
                }

                ReduceItem(gemId, useNum * 2);
            }
        }

        public static void SellUserProps(KBEngine.Packet packet)
        {
            var inpb = packet.protoBody as CGSellUserProps;
            var item = GetItemInPack(inpb.UserPropsId);
            var num = item.PackEntry.Count;
            var itemData = Util.GetItemData(inpb.GoodsType, item.PackEntry.BaseId);
            ReduceItem(inpb.UserPropsId, num);
            if (itemData.GoldCost > 0)
            {
                AddGold(itemData.GoldCost * num);
            } else
            {
                AddGold(itemData.propsConfig.JingShi * 100 * num);
            }
        }

        public static void UserDressEquip(KBEngine.Packet packet)
        {
            var inpb = packet.protoBody as CGUserDressEquip;
            var pinfo = ServerData.Instance.playerInfo;

            PackInfo oldEquip = null;
            if (inpb.DestEquipId != 0)
            {
                foreach (var p in pinfo.DressInfoList)
                {
                    if (p.PackEntry.Id == inpb.DestEquipId)
                    {
                        oldEquip = p;
                        pinfo.DressInfoList.Remove(oldEquip);
                        break;
                    }
                }
            }
            PackInfo newEquip = null;
            if (inpb.SrcEquipId != 0)
            {
                foreach (var p in pinfo.PackInfoList)
                {
                    if (p.PackEntry.Id == inpb.SrcEquipId)
                    {
                        newEquip = p;
                        RemoveEquipFromPackage(inpb.SrcEquipId);
                        break;
                    }
                }
            }

            if (oldEquip != null)
            {
                PutEquipInPackage(oldEquip);
            }
            Log.Net("PutEquipInPackage " + oldEquip.PackEntry.Id);

            var newEquip2 = PackInfo.CreateBuilder(newEquip);
            newEquip2.PackEntry.Count = 1;

            pinfo.DressInfoList.Add(newEquip2.Build());

            var au = GCUserDressEquip.CreateBuilder();
            au.DressEquip = newEquip.PackEntry;
            if (oldEquip != null)
            {
                au.PackEquip = oldEquip.PackEntry;
            }
            ServerBundle.SendImmediate(au, packet.flowId);
        }

        public static void LearnSkill(int skillId)
        {
            Log.Net("LearnSkill: "+skillId);
            var pinfo = ServerData.Instance.playerInfo;
            var allSkill = pinfo.Skill;
            var skInfo = SkillInfo.CreateBuilder();
            skInfo.SkillInfoId = skillId;
            skInfo.Level = 1;
            skInfo.Pos = 0;
            allSkill.SkillInfosList.Add(skInfo.Build());
            var skData = Util.GetSkillData(skillId, 1);
            SendNotify("恭喜学会技能 [ff1010]" + skData.SkillName + "[-]");
        }


        public static void SetSkillPos(int skillId, int pos)
        {
            var pinfo = ServerData.Instance.playerInfo;
            var allSkill = pinfo.Skill;

            foreach (var sk in allSkill.SkillInfosList)
            {
                if (sk.Pos == pos)
                {
                    sk.Pos = 0;
                }
                if (sk.SkillInfoId == skillId)
                {
                    sk.Pos = pos;
                }
            }
            //PushSkillInfo
            PushSkillShortcutsInfo();
        }

        public static GCLoadSkillPanel GetLearnedSkill()
        {
            var pinfo = ServerData.Instance.playerInfo;
            return pinfo.Skill;
        }

        public static void HelpMe()
        {
            var pinfo = ServerData.Instance.playerInfo;
            var key = "helpme";
            var find = false;
            foreach (var kv1 in pinfo.GameStateList)
            {
                if (kv1.Key == key)
                {
                    find = true;
                    break;
                }
            }
            if (!find)
            {
                var kv = KeyValue.CreateBuilder();
                kv.Key = key;
                kv.Value = "1";
                pinfo.GameStateList.Add(kv.Build());
                AddJingShi(350);
            }

        }
    }



}