
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System;
/*
 * 
	背包物品结构：
		BackPackData ----> ItemData + EquipExtraInfo
 */ 
namespace MyLib
{

	public class BackpackData : ActionItem
	{
        Guid gid = Guid.NewGuid();
        public Guid InstanceID {
            get{
                return gid;
            }
        }

		public long cdTime = 0;
		public long id {
			get {
				return entry.Id;
			}
		}

		public int baseId {
			get {
				return entry.BaseId;
			}
		}

		public int goodsType {
			get {
				return entry.GoodsType;
			}
		}

		public int num {
			get {
				return entry.Count;
			}
		}

		public int index {
			get {
				return entry.Index;
			}
		}


		public int GetNeedLevel ()
		{
			return -1;
		}


	
		public PackInfo packInfo = null;
		public PackEntry entry = null;

		/*
		 * 从服务器初始化背包数据
		 */ 
		public BackpackData (PackInfo pinfo)
		{
			if (pinfo != null) {
				packInfo = pinfo;
				entry = packInfo.PackEntry;
                Log.Important ("Init ItemData is " + pinfo.PackEntry.GoodsType+" num "+entry.Count);
				itemData = Util.GetItemData (pinfo.PackEntry.GoodsType, baseId);
				if (itemData == null) {
					Debug.LogError ("BackpackData:: Init Error " + baseId);
				}
			}
		}
	
		public BackpackData(PackEntry e) {
            var pinfo = PackInfo.CreateBuilder();
            pinfo.PackEntry = e;
            packInfo = pinfo.Build();
			entry = e;
			//num = entry.Count;
			itemData = Util.GetItemData (e.GoodsType, baseId);
		}
	
	}

}
