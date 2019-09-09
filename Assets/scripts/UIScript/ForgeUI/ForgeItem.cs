using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class ForgeItem : IUserInterface
    {
        ForgeConfigData fd;

        void Awake()
        {
            SetCallback("Info", OnForge);
        }

        void OnForge()
        {
            var forgeDetail = WindowMng.windowMng.PushView("UI/ForgeDetail");
            forgeDetail.GetComponent<ForgeDetail>().SetForgeData(fd);

        }

        public void SetForgeItem(ForgeConfigData data)
        {
            fd = data;
            var canForge = GameInterface_Forge.CanForge(data.id);
            var s = "";
            if (canForge)
            {
                s = string.Format("[ff9500]{0}.{1}[-]\n[0affa0]可以打造[-]", data.id, Util.GetItemData((int)GoodsTypeEnum.Equip, data.output).ItemName);
            } else
            {
                s = string.Format("[ff9500]{0}.{1}[-]\n[ff0a0a]材料不足[-]", data.id, Util.GetItemData((int)GoodsTypeEnum.Equip, data.output).ItemName);
            }
            GetLabel("Name").text = s;
        }
    }

}