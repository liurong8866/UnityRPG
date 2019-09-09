
using UnityEngine;
using System.Collections.Generic;
using Google.ProtocolBuffers;


namespace MyLib {
public partial class Util {
	public delegate IMessageLite MsgDelegate(ByteString buf); 
	

	static IMessageLite GetGCGetKeyValue(ByteString buf) {
		var retMsg = MyLib.GCGetKeyValue.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCListBranchinges(ByteString buf) {
		var retMsg = MyLib.GCListBranchinges.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGLoginAccount(ByteString buf) {
		var retMsg = MyLib.CGLoginAccount.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGGetKeyValue(ByteString buf) {
		var retMsg = MyLib.CGGetKeyValue.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCPushFightModeChangeWithMap(ByteString buf) {
		var retMsg = MyLib.GCPushFightModeChangeWithMap.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCPickUpLootReward(ByteString buf) {
		var retMsg = MyLib.GCPickUpLootReward.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCSaveGuideStep(ByteString buf) {
		var retMsg = MyLib.GCSaveGuideStep.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGForgotPassword(ByteString buf) {
		var retMsg = MyLib.CGForgotPassword.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGPropsRevive(ByteString buf) {
		var retMsg = MyLib.CGPropsRevive.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGChangeScreen4Point(ByteString buf) {
		var retMsg = MyLib.CGChangeScreen4Point.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGChangeFightMode(ByteString buf) {
		var retMsg = MyLib.CGChangeFightMode.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCBindingSession(ByteString buf) {
		var retMsg = MyLib.GCBindingSession.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGGetCharacterInfo(ByteString buf) {
		var retMsg = MyLib.CGGetCharacterInfo.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCPushAttribute2Members(ByteString buf) {
		var retMsg = MyLib.GCPushAttribute2Members.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGRegisterAccount(ByteString buf) {
		var retMsg = MyLib.CGRegisterAccount.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGSetKeyValue(ByteString buf) {
		var retMsg = MyLib.CGSetKeyValue.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCPushPlayerModifyName(ByteString buf) {
		var retMsg = MyLib.GCPushPlayerModifyName.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCGetCharacterInfo(ByteString buf) {
		var retMsg = MyLib.GCGetCharacterInfo.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCPushAttrChange(ByteString buf) {
		var retMsg = MyLib.GCPushAttrChange.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCSelectCharacter(ByteString buf) {
		var retMsg = MyLib.GCSelectCharacter.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGListBranchinges(ByteString buf) {
		var retMsg = MyLib.CGListBranchinges.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGAutoRegisterAccount(ByteString buf) {
		var retMsg = MyLib.CGAutoRegisterAccount.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGModifyPassword(ByteString buf) {
		var retMsg = MyLib.CGModifyPassword.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCPushNotice2Kick(ByteString buf) {
		var retMsg = MyLib.GCPushNotice2Kick.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCPushLevelUpgrade(ByteString buf) {
		var retMsg = MyLib.GCPushLevelUpgrade.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGHeartBeat(ByteString buf) {
		var retMsg = MyLib.CGHeartBeat.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGPlayerCmd(ByteString buf) {
		var retMsg = MyLib.CGPlayerCmd.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCPushLootReward(ByteString buf) {
		var retMsg = MyLib.GCPushLootReward.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGBindingSession(ByteString buf) {
		var retMsg = MyLib.CGBindingSession.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCPropsRevive(ByteString buf) {
		var retMsg = MyLib.GCPropsRevive.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCChangeFightMode(ByteString buf) {
		var retMsg = MyLib.GCChangeFightMode.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGModifyPlayerName(ByteString buf) {
		var retMsg = MyLib.CGModifyPlayerName.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGCreateCharacter(ByteString buf) {
		var retMsg = MyLib.CGCreateCharacter.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCModifyPassword(ByteString buf) {
		var retMsg = MyLib.GCModifyPassword.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCPushLevel(ByteString buf) {
		var retMsg = MyLib.GCPushLevel.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCEnterScene(ByteString buf) {
		var retMsg = MyLib.GCEnterScene.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCPushNotify(ByteString buf) {
		var retMsg = MyLib.GCPushNotify.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCPushLootRewardRemove(ByteString buf) {
		var retMsg = MyLib.GCPushLootRewardRemove.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCForgotPassword(ByteString buf) {
		var retMsg = MyLib.GCForgotPassword.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGPickItem(ByteString buf) {
		var retMsg = MyLib.CGPickItem.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCPushExpChange(ByteString buf) {
		var retMsg = MyLib.GCPushExpChange.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGPlayerMove(ByteString buf) {
		var retMsg = MyLib.CGPlayerMove.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCPushPlayerPower(ByteString buf) {
		var retMsg = MyLib.GCPushPlayerPower.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGDelCharacter(ByteString buf) {
		var retMsg = MyLib.CGDelCharacter.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGSaveGuideStep(ByteString buf) {
		var retMsg = MyLib.CGSaveGuideStep.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCSettingClothShow(ByteString buf) {
		var retMsg = MyLib.GCSettingClothShow.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGAddProp(ByteString buf) {
		var retMsg = MyLib.CGAddProp.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCRegisterAccount(ByteString buf) {
		var retMsg = MyLib.GCRegisterAccount.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCChangeScreen4Point(ByteString buf) {
		var retMsg = MyLib.GCChangeScreen4Point.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGEnterScene(ByteString buf) {
		var retMsg = MyLib.CGEnterScene.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGSettingClothShow(ByteString buf) {
		var retMsg = MyLib.CGSettingClothShow.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCDelCharacter(ByteString buf) {
		var retMsg = MyLib.GCDelCharacter.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCAutoRegisterAccount(ByteString buf) {
		var retMsg = MyLib.GCAutoRegisterAccount.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCPushPlayerToilState(ByteString buf) {
		var retMsg = MyLib.GCPushPlayerToilState.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCCreateCharacter(ByteString buf) {
		var retMsg = MyLib.GCCreateCharacter.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCPushPlayerResurrect(ByteString buf) {
		var retMsg = MyLib.GCPushPlayerResurrect.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCPlayerMove(ByteString buf) {
		var retMsg = MyLib.GCPlayerMove.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCPlayerCmd(ByteString buf) {
		var retMsg = MyLib.GCPlayerCmd.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCModifyPlayerName(ByteString buf) {
		var retMsg = MyLib.GCModifyPlayerName.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGSelectCharacter(ByteString buf) {
		var retMsg = MyLib.CGSelectCharacter.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGPickUpLootReward(ByteString buf) {
		var retMsg = MyLib.CGPickUpLootReward.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGSetProp(ByteString buf) {
		var retMsg = MyLib.CGSetProp.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCPushPlayerDressAttributeChanges(ByteString buf) {
		var retMsg = MyLib.GCPushPlayerDressAttributeChanges.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCLoginAccount(ByteString buf) {
		var retMsg = MyLib.GCLoginAccount.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCCopyInfo(ByteString buf) {
		var retMsg = MyLib.GCCopyInfo.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGCopyInfo(ByteString buf) {
		var retMsg = MyLib.CGCopyInfo.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCCopyReward(ByteString buf) {
		var retMsg = MyLib.GCCopyReward.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCPushLevelOpen(ByteString buf) {
		var retMsg = MyLib.GCPushLevelOpen.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGResetSkillPoint(ByteString buf) {
		var retMsg = MyLib.CGResetSkillPoint.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGSkillLevelUp(ByteString buf) {
		var retMsg = MyLib.CGSkillLevelUp.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGInjectPropsLevelUp(ByteString buf) {
		var retMsg = MyLib.CGInjectPropsLevelUp.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGLoadInjectPropsLevelUpInfo(ByteString buf) {
		var retMsg = MyLib.CGLoadInjectPropsLevelUpInfo.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCPushMemberSkillCD(ByteString buf) {
		var retMsg = MyLib.GCPushMemberSkillCD.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGLoadSkillPanel(ByteString buf) {
		var retMsg = MyLib.CGLoadSkillPanel.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGSkillLevelDown(ByteString buf) {
		var retMsg = MyLib.CGSkillLevelDown.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCInjectPropsLevelUp(ByteString buf) {
		var retMsg = MyLib.GCInjectPropsLevelUp.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCLoadSkillPanel(ByteString buf) {
		var retMsg = MyLib.GCLoadSkillPanel.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCPushActivateSkill(ByteString buf) {
		var retMsg = MyLib.GCPushActivateSkill.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCPushUnitAddBuffer(ByteString buf) {
		var retMsg = MyLib.GCPushUnitAddBuffer.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCLoadInjectPropsLevelUpInfo(ByteString buf) {
		var retMsg = MyLib.GCLoadInjectPropsLevelUpInfo.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCResetSkillPoint(ByteString buf) {
		var retMsg = MyLib.GCResetSkillPoint.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCPushSkillPoint(ByteString buf) {
		var retMsg = MyLib.GCPushSkillPoint.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCSkillLevelUp(ByteString buf) {
		var retMsg = MyLib.GCSkillLevelUp.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCSkillLevelDown(ByteString buf) {
		var retMsg = MyLib.GCSkillLevelDown.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGSendChat(ByteString buf) {
		var retMsg = MyLib.CGSendChat.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGLoadMChatShowInfo(ByteString buf) {
		var retMsg = MyLib.CGLoadMChatShowInfo.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCViewChatGoods(ByteString buf) {
		var retMsg = MyLib.GCViewChatGoods.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCLoadMChatShowInfo(ByteString buf) {
		var retMsg = MyLib.GCLoadMChatShowInfo.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCPushChat2Client(ByteString buf) {
		var retMsg = MyLib.GCPushChat2Client.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCSendChat(ByteString buf) {
		var retMsg = MyLib.GCSendChat.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCPushSendNotice(ByteString buf) {
		var retMsg = MyLib.GCPushSendNotice.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCPushNotice(ByteString buf) {
		var retMsg = MyLib.GCPushNotice.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGViewChatGoods(ByteString buf) {
		var retMsg = MyLib.CGViewChatGoods.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGBuyShopProps(ByteString buf) {
		var retMsg = MyLib.CGBuyShopProps.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCBuyShopProps(ByteString buf) {
		var retMsg = MyLib.GCBuyShopProps.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCListUserEquip(ByteString buf) {
		var retMsg = MyLib.GCListUserEquip.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCCheckout4Storage(ByteString buf) {
		var retMsg = MyLib.GCCheckout4Storage.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCSellUserProps(ByteString buf) {
		var retMsg = MyLib.GCSellUserProps.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCPushGoodsCountChange(ByteString buf) {
		var retMsg = MyLib.GCPushGoodsCountChange.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGSellUserProps(ByteString buf) {
		var retMsg = MyLib.CGSellUserProps.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCPushPackInfo(ByteString buf) {
		var retMsg = MyLib.GCPushPackInfo.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGAutoAdjustPack(ByteString buf) {
		var retMsg = MyLib.CGAutoAdjustPack.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGSplitUserProps(ByteString buf) {
		var retMsg = MyLib.CGSplitUserProps.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCLoadShortcutsInfo(ByteString buf) {
		var retMsg = MyLib.GCLoadShortcutsInfo.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCUserDressEquip(ByteString buf) {
		var retMsg = MyLib.GCUserDressEquip.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGSwapShortcuts(ByteString buf) {
		var retMsg = MyLib.CGSwapShortcuts.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCUnbindingGoods(ByteString buf) {
		var retMsg = MyLib.GCUnbindingGoods.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGUseUserProps(ByteString buf) {
		var retMsg = MyLib.CGUseUserProps.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGUnbindingGoods(ByteString buf) {
		var retMsg = MyLib.CGUnbindingGoods.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCPushEquipDataUpdate(ByteString buf) {
		var retMsg = MyLib.GCPushEquipDataUpdate.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCPushPlayerDressInfo(ByteString buf) {
		var retMsg = MyLib.GCPushPlayerDressInfo.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGLoadShortcutsInfo(ByteString buf) {
		var retMsg = MyLib.CGLoadShortcutsInfo.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCModifyShortcutsInfo(ByteString buf) {
		var retMsg = MyLib.GCModifyShortcutsInfo.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCDressCloth(ByteString buf) {
		var retMsg = MyLib.GCDressCloth.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGPut2Storage(ByteString buf) {
		var retMsg = MyLib.CGPut2Storage.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGCheckout4Storage(ByteString buf) {
		var retMsg = MyLib.CGCheckout4Storage.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCTakeOffCloth(ByteString buf) {
		var retMsg = MyLib.GCTakeOffCloth.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGLevelUpEquip(ByteString buf) {
		var retMsg = MyLib.CGLevelUpEquip.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCMergeUserProps(ByteString buf) {
		var retMsg = MyLib.GCMergeUserProps.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCUseUserProps(ByteString buf) {
		var retMsg = MyLib.GCUseUserProps.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGModifyShortcutsInfo(ByteString buf) {
		var retMsg = MyLib.CGModifyShortcutsInfo.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGListUserEquip(ByteString buf) {
		var retMsg = MyLib.CGListUserEquip.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGLoadPackInfo(ByteString buf) {
		var retMsg = MyLib.CGLoadPackInfo.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGDressCloth(ByteString buf) {
		var retMsg = MyLib.CGDressCloth.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCPut2Storage(ByteString buf) {
		var retMsg = MyLib.GCPut2Storage.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCSplitUserProps(ByteString buf) {
		var retMsg = MyLib.GCSplitUserProps.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGQueryUserEquipInfo(ByteString buf) {
		var retMsg = MyLib.CGQueryUserEquipInfo.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCSwapShortcuts(ByteString buf) {
		var retMsg = MyLib.GCSwapShortcuts.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCLoadPackInfo(ByteString buf) {
		var retMsg = MyLib.GCLoadPackInfo.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGUserDressEquip(ByteString buf) {
		var retMsg = MyLib.CGUserDressEquip.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCAutoAdjustPack(ByteString buf) {
		var retMsg = MyLib.GCAutoAdjustPack.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCPushShortcutsInfo(ByteString buf) {
		var retMsg = MyLib.GCPushShortcutsInfo.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCPushPlayerDressedEquipChange(ByteString buf) {
		var retMsg = MyLib.GCPushPlayerDressedEquipChange.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGTakeOffCloth(ByteString buf) {
		var retMsg = MyLib.CGTakeOffCloth.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetGCQueryUserEquipInfo(ByteString buf) {
		var retMsg = MyLib.GCQueryUserEquipInfo.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGLevelUpGem(ByteString buf) {
		var retMsg = MyLib.CGLevelUpGem.ParseFrom(buf);
		return retMsg;
	}	

	static IMessageLite GetCGMergeUserProps(ByteString buf) {
		var retMsg = MyLib.CGMergeUserProps.ParseFrom(buf);
		return retMsg;
	}	


	static Dictionary<string, MsgDelegate> msgMap = new Dictionary<string, MsgDelegate>(){

	{"GCGetKeyValue", GetGCGetKeyValue},

	{"GCListBranchinges", GetGCListBranchinges},

	{"CGLoginAccount", GetCGLoginAccount},

	{"CGGetKeyValue", GetCGGetKeyValue},

	{"GCPushFightModeChangeWithMap", GetGCPushFightModeChangeWithMap},

	{"GCPickUpLootReward", GetGCPickUpLootReward},

	{"GCSaveGuideStep", GetGCSaveGuideStep},

	{"CGForgotPassword", GetCGForgotPassword},

	{"CGPropsRevive", GetCGPropsRevive},

	{"CGChangeScreen4Point", GetCGChangeScreen4Point},

	{"CGChangeFightMode", GetCGChangeFightMode},

	{"GCBindingSession", GetGCBindingSession},

	{"CGGetCharacterInfo", GetCGGetCharacterInfo},

	{"GCPushAttribute2Members", GetGCPushAttribute2Members},

	{"CGRegisterAccount", GetCGRegisterAccount},

	{"CGSetKeyValue", GetCGSetKeyValue},

	{"GCPushPlayerModifyName", GetGCPushPlayerModifyName},

	{"GCGetCharacterInfo", GetGCGetCharacterInfo},

	{"GCPushAttrChange", GetGCPushAttrChange},

	{"GCSelectCharacter", GetGCSelectCharacter},

	{"CGListBranchinges", GetCGListBranchinges},

	{"CGAutoRegisterAccount", GetCGAutoRegisterAccount},

	{"CGModifyPassword", GetCGModifyPassword},

	{"GCPushNotice2Kick", GetGCPushNotice2Kick},

	{"GCPushLevelUpgrade", GetGCPushLevelUpgrade},

	{"CGHeartBeat", GetCGHeartBeat},

	{"CGPlayerCmd", GetCGPlayerCmd},

	{"GCPushLootReward", GetGCPushLootReward},

	{"CGBindingSession", GetCGBindingSession},

	{"GCPropsRevive", GetGCPropsRevive},

	{"GCChangeFightMode", GetGCChangeFightMode},

	{"CGModifyPlayerName", GetCGModifyPlayerName},

	{"CGCreateCharacter", GetCGCreateCharacter},

	{"GCModifyPassword", GetGCModifyPassword},

	{"GCPushLevel", GetGCPushLevel},

	{"GCEnterScene", GetGCEnterScene},

	{"GCPushNotify", GetGCPushNotify},

	{"GCPushLootRewardRemove", GetGCPushLootRewardRemove},

	{"GCForgotPassword", GetGCForgotPassword},

	{"CGPickItem", GetCGPickItem},

	{"GCPushExpChange", GetGCPushExpChange},

	{"CGPlayerMove", GetCGPlayerMove},

	{"GCPushPlayerPower", GetGCPushPlayerPower},

	{"CGDelCharacter", GetCGDelCharacter},

	{"CGSaveGuideStep", GetCGSaveGuideStep},

	{"GCSettingClothShow", GetGCSettingClothShow},

	{"CGAddProp", GetCGAddProp},

	{"GCRegisterAccount", GetGCRegisterAccount},

	{"GCChangeScreen4Point", GetGCChangeScreen4Point},

	{"CGEnterScene", GetCGEnterScene},

	{"CGSettingClothShow", GetCGSettingClothShow},

	{"GCDelCharacter", GetGCDelCharacter},

	{"GCAutoRegisterAccount", GetGCAutoRegisterAccount},

	{"GCPushPlayerToilState", GetGCPushPlayerToilState},

	{"GCCreateCharacter", GetGCCreateCharacter},

	{"GCPushPlayerResurrect", GetGCPushPlayerResurrect},

	{"GCPlayerMove", GetGCPlayerMove},

	{"GCPlayerCmd", GetGCPlayerCmd},

	{"GCModifyPlayerName", GetGCModifyPlayerName},

	{"CGSelectCharacter", GetCGSelectCharacter},

	{"CGPickUpLootReward", GetCGPickUpLootReward},

	{"CGSetProp", GetCGSetProp},

	{"GCPushPlayerDressAttributeChanges", GetGCPushPlayerDressAttributeChanges},

	{"GCLoginAccount", GetGCLoginAccount},

	{"GCCopyInfo", GetGCCopyInfo},

	{"CGCopyInfo", GetCGCopyInfo},

	{"GCCopyReward", GetGCCopyReward},

	{"GCPushLevelOpen", GetGCPushLevelOpen},

	{"CGResetSkillPoint", GetCGResetSkillPoint},

	{"CGSkillLevelUp", GetCGSkillLevelUp},

	{"CGInjectPropsLevelUp", GetCGInjectPropsLevelUp},

	{"CGLoadInjectPropsLevelUpInfo", GetCGLoadInjectPropsLevelUpInfo},

	{"GCPushMemberSkillCD", GetGCPushMemberSkillCD},

	{"CGLoadSkillPanel", GetCGLoadSkillPanel},

	{"CGSkillLevelDown", GetCGSkillLevelDown},

	{"GCInjectPropsLevelUp", GetGCInjectPropsLevelUp},

	{"GCLoadSkillPanel", GetGCLoadSkillPanel},

	{"GCPushActivateSkill", GetGCPushActivateSkill},

	{"GCPushUnitAddBuffer", GetGCPushUnitAddBuffer},

	{"GCLoadInjectPropsLevelUpInfo", GetGCLoadInjectPropsLevelUpInfo},

	{"GCResetSkillPoint", GetGCResetSkillPoint},

	{"GCPushSkillPoint", GetGCPushSkillPoint},

	{"GCSkillLevelUp", GetGCSkillLevelUp},

	{"GCSkillLevelDown", GetGCSkillLevelDown},

	{"CGSendChat", GetCGSendChat},

	{"CGLoadMChatShowInfo", GetCGLoadMChatShowInfo},

	{"GCViewChatGoods", GetGCViewChatGoods},

	{"GCLoadMChatShowInfo", GetGCLoadMChatShowInfo},

	{"GCPushChat2Client", GetGCPushChat2Client},

	{"GCSendChat", GetGCSendChat},

	{"GCPushSendNotice", GetGCPushSendNotice},

	{"GCPushNotice", GetGCPushNotice},

	{"CGViewChatGoods", GetCGViewChatGoods},

	{"CGBuyShopProps", GetCGBuyShopProps},

	{"GCBuyShopProps", GetGCBuyShopProps},

	{"GCListUserEquip", GetGCListUserEquip},

	{"GCCheckout4Storage", GetGCCheckout4Storage},

	{"GCSellUserProps", GetGCSellUserProps},

	{"GCPushGoodsCountChange", GetGCPushGoodsCountChange},

	{"CGSellUserProps", GetCGSellUserProps},

	{"GCPushPackInfo", GetGCPushPackInfo},

	{"CGAutoAdjustPack", GetCGAutoAdjustPack},

	{"CGSplitUserProps", GetCGSplitUserProps},

	{"GCLoadShortcutsInfo", GetGCLoadShortcutsInfo},

	{"GCUserDressEquip", GetGCUserDressEquip},

	{"CGSwapShortcuts", GetCGSwapShortcuts},

	{"GCUnbindingGoods", GetGCUnbindingGoods},

	{"CGUseUserProps", GetCGUseUserProps},

	{"CGUnbindingGoods", GetCGUnbindingGoods},

	{"GCPushEquipDataUpdate", GetGCPushEquipDataUpdate},

	{"GCPushPlayerDressInfo", GetGCPushPlayerDressInfo},

	{"CGLoadShortcutsInfo", GetCGLoadShortcutsInfo},

	{"GCModifyShortcutsInfo", GetGCModifyShortcutsInfo},

	{"GCDressCloth", GetGCDressCloth},

	{"CGPut2Storage", GetCGPut2Storage},

	{"CGCheckout4Storage", GetCGCheckout4Storage},

	{"GCTakeOffCloth", GetGCTakeOffCloth},

	{"CGLevelUpEquip", GetCGLevelUpEquip},

	{"GCMergeUserProps", GetGCMergeUserProps},

	{"GCUseUserProps", GetGCUseUserProps},

	{"CGModifyShortcutsInfo", GetCGModifyShortcutsInfo},

	{"CGListUserEquip", GetCGListUserEquip},

	{"CGLoadPackInfo", GetCGLoadPackInfo},

	{"CGDressCloth", GetCGDressCloth},

	{"GCPut2Storage", GetGCPut2Storage},

	{"GCSplitUserProps", GetGCSplitUserProps},

	{"CGQueryUserEquipInfo", GetCGQueryUserEquipInfo},

	{"GCSwapShortcuts", GetGCSwapShortcuts},

	{"GCLoadPackInfo", GetGCLoadPackInfo},

	{"CGUserDressEquip", GetCGUserDressEquip},

	{"GCAutoAdjustPack", GetGCAutoAdjustPack},

	{"GCPushShortcutsInfo", GetGCPushShortcutsInfo},

	{"GCPushPlayerDressedEquipChange", GetGCPushPlayerDressedEquipChange},

	{"CGTakeOffCloth", GetCGTakeOffCloth},

	{"GCQueryUserEquipInfo", GetGCQueryUserEquipInfo},

	{"CGLevelUpGem", GetCGLevelUpGem},

	{"CGMergeUserProps", GetCGMergeUserProps},

	};

	public static IMessageLite GetMsg(int moduleId, int messageId, ByteString buf) {
		var module = SaveGame.saveGame.getModuleName(moduleId);
		var msg = SaveGame.saveGame.getMethodName(module, messageId);
		Debug.LogWarning ("modulename "+module+" "+msg);

		return msgMap[msg](buf);
	}
}

}
