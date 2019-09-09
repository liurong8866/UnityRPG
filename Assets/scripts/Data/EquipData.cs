
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/


namespace MyLib
{
	public class EquipData : ActionItem
	{
		//背包物品ID 或者商城物品ID UniqueItemId
		public long id {
			get {
				return entry.Id;
			}
		}

		public PackEntry entry;
		PackInfo info;
		public ItemData.EquipPosition Slot{
			get {
				return itemData.equipPosition;
			}
		} //装备所在的槽, PackEntry的Index 描述这个信息， 后续可能不需要这个数据，因为装备的槽唯一


		public EquipData (PackInfo pkinfo)
		{
			info = pkinfo;
			entry = info.PackEntry;
			itemData = Util.GetItemData (1, pkinfo.PackEntry.BaseId);
			KBEngine.Dbg.Assert (itemData == null, "EquipData::initError " + pkinfo.PackEntry.BaseId);
		}

		public EquipData (PackEntry e) {
			entry = e;
			itemData = Util.GetItemData (1, entry.BaseId);
		}

		             
	}

}