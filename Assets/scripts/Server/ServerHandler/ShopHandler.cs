using UnityEngine;
using System.Collections;
using playerData = MyLib.PlayerData;

namespace ServerPacketHandler
{
    public class CGBuyShopProps : IPacketHandler
    {
        public static void HandlePacket(MyLib.CGBuyShopProps buy)
        {
            Log.Net("ShopHandler");
            //var buy = packet.protoBody as ChuMeng.CGBuyShopProps;
            var itemId = buy.ShopId;
            var data = MyLib.GMDataBaseSystem.SearchIdStatic<MyLib.PropsConfigData>(MyLib.GameData.PropsConfig, itemId);
            var player = MyLib.ServerData.Instance.playerInfo;
            var has = player.Gold;
            var hasJingShi = player.JingShi;
            //var playerData = ChuMeng.PlayerData;
            if (has < data.goldCoin)
            {
                var notify = MyLib.GCPushNotify.CreateBuilder();
                notify.SetNotify("金币不足");
                MyLib.ServerBundle.SendImmediatePush(notify);
            } else if (hasJingShi < data.JingShi)
            {
                var notify = MyLib.GCPushNotify.CreateBuilder();
                notify.SetNotify("[ff0a0a]晶石不足，请去晶石商店购买[-]");
                MyLib.ServerBundle.SendImmediatePush(notify);
            } else
            {
                if (playerData.IsPackageFull(itemId, 1))
                {
                    playerData.SendNotify("背包已满");
                    return;
                }
                playerData.SetGold(has - data.goldCoin);
                playerData.SetJingShi(hasJingShi - data.JingShi);
                playerData.AddItemInPackage(itemId, 1);
            }
        }

        public override void HandlePacket(KBEngine.Packet packet)
        {
            Log.Net("ShopHandler");
            var buy = packet.protoBody as MyLib.CGBuyShopProps;
            var itemId = buy.ShopId;
            var data = MyLib.GMDataBaseSystem.SearchIdStatic<MyLib.PropsConfigData>(MyLib.GameData.PropsConfig, itemId);
            var player = MyLib.ServerData.Instance.playerInfo;
            var has = player.Gold;
            var hasJingShi = player.JingShi;
            //var playerData = ChuMeng.PlayerData;
            if (has < data.goldCoin)
            {
                var notify = MyLib.GCPushNotify.CreateBuilder();
                notify.SetNotify("金币不足");
                MyLib.ServerBundle.SendImmediatePush(notify);
            } else if (hasJingShi < data.JingShi)
            {
                var notify = MyLib.GCPushNotify.CreateBuilder();
                notify.SetNotify("[ff0a0a]晶石不足，请去晶石商店购买[-]");
                MyLib.ServerBundle.SendImmediatePush(notify);
            } else
            {
                if (playerData.IsPackageFull(itemId, 1))
                {
                    playerData.SendNotify("背包已满");
                    return;
                }
                playerData.SetGold(has - data.goldCoin);
                playerData.SetJingShi(hasJingShi - data.JingShi);
                playerData.AddItemInPackage(itemId, 1);
            }

        }
    }

    public class CGUseUserProps : IPacketHandler
    {
        public override void HandlePacket(KBEngine.Packet packet)
        {
            var inpb = packet.protoBody as MyLib.CGUseUserProps;
            var ret = playerData.ReduceItem(inpb.UserPropsId, 1);
            var pb = MyLib.GCUseUserProps.CreateBuilder();
            if (ret)
            {
                MyLib.ServerBundle.SendImmediate(pb, packet.flowId);
            } else
            {
                MyLib.ServerBundle.SendImmediateError(pb, packet.flowId, 1);
            }
        }
    }

    public class CGPickItem : IPacketHandler
    {
        public override void HandlePacket(KBEngine.Packet packet)
        {
            var inpb = packet.protoBody as MyLib.CGPickItem;
            playerData.AddItemInPackage(inpb.ItemId, inpb.Num);
        }
    }
}
