using UnityEngine;
using System.Collections;

namespace MyLib
{
    public static class PlayerDataInterface
    {
        public static void DressEquip(AvatarInfo info)
        {
            var dressInfo = info.DressInfoList;
            var player = ObjectManager.objectManager.GetPlayer(info.Id);
            if (player != null)
            {
                var localId = player.GetComponent<KBEngine.KBNetworkView>().GetLocalId();
                foreach (PackInfo pkinfo in dressInfo)
                {
                    var ed = new EquipData(pkinfo);
                    var evt = new MyEvent(MyEvent.EventType.CharEquipChanged);
                    evt.localID = localId;
                    evt.equipData = ed;
                    evt.boolArg = true;
                    MyEventSystem.myEventSystem.PushEvent(evt);
                }
            }
        }
    }

}