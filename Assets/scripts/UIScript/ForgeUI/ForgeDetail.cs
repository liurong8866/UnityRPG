using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System;

namespace MyLib
{
    public class ForgeDetail : IUserInterface
    {
        ForgeConfigData forgeData;

        public void SetForgeData(ForgeConfigData fd)
        {
            forgeData = fd;
        }

        void Awake()
        {
            SetCallback("closeButton", Hide);
            SetCallback("Info", OnForge);
        }

        void OnForge()
        {
            WindowMng.windowMng.PopView();
            GameInterface_Forge.Forge(forgeData.id);
        }

        // Use this for initialization
        void Start()
        {
            var item = Util.GetItemData((int)GoodsTypeEnum.Equip, forgeData.output);
            var needMat = JSON.Parse(forgeData.materials).AsObject;
            var matStr = "";
            foreach(KeyValuePair<string, JSONNode> m in needMat) {
                var hasCount = BackPack.backpack.GetItemCount((int)GoodsTypeEnum.Props, System.Convert.ToInt32(m.Key));
                var needCount = m.Value.AsInt;
                var matData = Util.GetItemData((int)GoodsTypeEnum.Props, System.Convert.ToInt32(m.Key));
                Log.GUI("NeedMaterial "+matData.ItemName+" num "+needCount);
                if(hasCount < needCount) {
                    matStr += string.Format("[0aa0ff]{0} {1}/[f00a0a]{2}[-]\n", matData.ItemName, needCount, hasCount);
                }else {
                    matStr += string.Format("[0aa0ff]{0} {1}/{2}\n", matData.ItemName, needCount, hasCount);
                }
            }

            var s = string.Format("[ff9500]{0}[-]\n{1}", item.ItemName, matStr);
            GetLabel("Name").text = s;    
        }
	
    }
}
