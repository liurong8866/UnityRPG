using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class StoreItem : IUserInterface
    {
        UILabel Name;
        StoreUI storeUI;

        void Awake()
        {
            SetCallback("Buy", OnBuy);
            Name = GetLabel("Name");
        }

        void OnBuy()
        {
            storeUI.OnBuy(itemId);
        }

        int itemId;

        public void SetId(StoreUI s, int id)
        {
            storeUI = s;
            itemId = id;
            var item = Util.GetItemData(0, id);
            var count = BackPack.backpack.GetItemCount(0, id); 
            if (item.GoldCost > 0)
            {
                Name.text = string.Format("{0} [ffaa0a]{1}金币[-] 拥有:{2}", item.ItemName, item.GoldCost, count);
            }else {
                Name.text = string.Format("{0} [f0f00a]{1}晶石[-] 拥有:{2}", item.ItemName, item.propsConfig.JingShi, count);
            }
        }

    }
}
