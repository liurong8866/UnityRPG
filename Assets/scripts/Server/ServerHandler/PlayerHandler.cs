using UnityEngine;
using System.Collections;
using playerData = MyLib.PlayerData;
using System;
using System.Collections.Generic;

namespace ServerPacketHandler
{
	public static class  HoldCode
	{
		public static Dictionary<string, System.Type> staticTypeMap;

		public static void Init ()
		{
			staticTypeMap = new Dictionary<string, Type> () {
				{ "ServerPacketHandler.CGGetCharacterInfo", typeof(CGGetCharacterInfo) },
				{ "ServerPacketHandler.CGAddProp", typeof(CGAddProp) },
				{ "ServerPacketHandler.CGSetProp", typeof(CGSetProp) },
				{ "ServerPacketHandler.CGCreateCharacter", typeof(CGCreateCharacter) },
				{ "ServerPacketHandler.CGLoadPackInfo", typeof(CGLoadPackInfo) },
				{ "ServerPacketHandler.CGGetKeyValue", typeof(CGGetKeyValue) },
				{ "ServerPacketHandler.CGSetKeyValue", typeof(CGSetKeyValue) },
                
				{ "ServerPacketHandler.CGLevelUpEquip", typeof(CGLevelUpEquip) },
				{ "ServerPacketHandler.CGLevelUpGem", typeof(CGLevelUpGem) },
				{ "ServerPacketHandler.CGSellUserProps", typeof(CGSellUserProps) },
                
				{ "ServerPacketHandler.CGBuyShopProps", typeof(CGBuyShopProps) },
				{ "ServerPacketHandler.CGUseUserProps", typeof(CGUseUserProps) },
				{ "ServerPacketHandler.CGPickItem", typeof(CGPickItem) },
                
				{ "ServerPacketHandler.CGSendChat", typeof(CGSendChat) },
                
				{ "ServerPacketHandler.CGLoadSkillPanel", typeof(CGLoadSkillPanel) },
				{ "ServerPacketHandler.CGSkillLevelUp", typeof(CGSkillLevelUp) },
				{ "ServerPacketHandler.CGLoadShortcutsInfo", typeof(CGLoadShortcutsInfo) },
                
			};
            
		}
	}

	public class CGGetCharacterInfo : IPacketHandler
	{
		public override void HandlePacket (KBEngine.Packet packet)
		{
			playerData.GetProp (packet);
		}
	}

	public class CGAddProp : IPacketHandler
	{
		public override void HandlePacket (KBEngine.Packet packet)
		{
			playerData.AddProp (packet);
		}
	}

	public class CGSetProp : IPacketHandler
	{
		public override void HandlePacket (KBEngine.Packet packet)
		{
			playerData.SetProp (packet);
		}
	}

	public class CGCreateCharacter : IPacketHandler
	{
		public override void HandlePacket (KBEngine.Packet packet)
		{
			playerData.CreateCharacter (packet);
		}
	}

	public class CGLoadPackInfo : IPacketHandler
	{
		public override void HandlePacket (KBEngine.Packet packet)
		{
			playerData.LoadPackInfo (packet);
		}
	}

	public class CGGetKeyValue : IPacketHandler
	{
		public override void HandlePacket (KBEngine.Packet packet)
		{
			playerData.GetKeyValue (packet);
		}
	}

	/// <summary>
	/// new  NewUserStory 
	/// </summary>
	public class CGSetKeyValue : IPacketHandler
	{
		public override void HandlePacket (KBEngine.Packet packet)
		{
			playerData.SetKeyValue (packet);
		}
	}

	public class CGLevelUpEquip : IPacketHandler
	{
		public override void HandlePacket (KBEngine.Packet packet)
		{
			playerData.LevelUpEquip (packet);
		}
	}

	public class CGLevelUpGem : IPacketHandler
	{
		public override void HandlePacket (KBEngine.Packet packet)
		{
			playerData.LevelUpGem (packet);
		}
	}

	public class CGSellUserProps : IPacketHandler
	{
		public override void HandlePacket (KBEngine.Packet packet)
		{
			playerData.SellUserProps (packet);
		}
	}
}
