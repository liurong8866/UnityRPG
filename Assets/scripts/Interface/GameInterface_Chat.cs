using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
	public class GameInterface_Chat
	{
		public static GameInterface_Chat chatInterface = new GameInterface_Chat();

		/*发送聊天信息*/
		public void SendChatMsg(string word, int channelId) {
			Talk.talk.SendChatMessage (word, channelId);
		}

		/*频道切换提取消息*/
		public Talk.Channel GetChannelMsg(Talk.ChannelType channelId){
			return Talk.talk.channelList[channelId];
		}

	}
}
