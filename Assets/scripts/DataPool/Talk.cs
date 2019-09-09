
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Chat System  
 * Chat Event Dispatcher
 */ 
namespace MyLib
{
	public class ChatMsg
	{
		public string Name {
			get {
				return"";
			}
		}

		public string Msg {
			get {
				return "";			
			}
		}
	}

	public class Talk
	{
		//public List<HistoryMsg> sendHisQue = new List<HistoryMsg>(); // History SendOut Message

		//UI Get History Message From Here

		//聊天频道
		public Dictionary<ChannelType, Channel> channelList = new Dictionary<ChannelType, Channel> ();


		public delegate void ChatDelegate (HistoryMsg msg);

		public ChatDelegate sendMsgDelegate;
		public ChatDelegate recvMsgDelegate;
		int maxSaveNum = 50;                    //最大保存消息数量

		//聊天频道分类
		public enum ChannelType
		{
			all = 999,
			World = 0,
			Team = 2,
			Private = 1,
			Guild = 3,
			System = 6,
			Laba = 5,
			Nearby = 4,
		}


		/*
		 * Hold Client To Server Or Server To Client HistoryMessage
		 */ 
		public class HistoryMsg
		{
			/*
			public long playerId;
			public string playerName;
			public int playerLevel;
			public int playerJob;
			public int playerVip;
			public long targetId;
			public ChannelType channel;
			public string chatContent;
			*/

			public GCPushChat2Client recvMsg = null;         //服务器到客户端      接受的消息
			public CGSendChat sendMsg = null;                   //客户端到服务器      发送的消息

			public HistoryMsg (CGSendChat sc)
			{
				sendMsg = sc;
			}

			public HistoryMsg (GCPushChat2Client rm)
			{
				recvMsg = rm;
			}
		}

		public class Channel
		{
			public List<HistoryMsg> recvHisQue = new List<HistoryMsg> (); //include Message Create By Self

			public ChannelType channelType;
			public bool isClose = false;

			public Channel (ChannelType ct)
			{
				channelType = ct;
			}

			public void setSendPacket (CGSendChat.Builder data)
			{
			
			}

			public bool TalkNeedCheck ()
			{
				return true;
			}

			public bool TalkTimeCheck ()
			{
				return true;
			}
		}


		static Talk _talk;

		public static Talk talk {
			get {
				if (_talk == null) {
					_talk = new Talk ();
				}
				return _talk;
			}
		}
		void AddChannel(Channel c) {
			channelList.Add (c.channelType, c);
		}
		public Talk ()
		{
			Channel newChannel = new Channel (ChannelType.all);
			AddChannel( newChannel);

			newChannel = new Channel (ChannelType.World);
			AddChannel( newChannel);

			newChannel = new Channel (ChannelType.Team);
			AddChannel (newChannel);

			newChannel = new Channel (ChannelType.Private);
			AddChannel (newChannel);

			newChannel = new Channel (ChannelType.Guild);
			AddChannel (newChannel);

			newChannel = new Channel (ChannelType.System);
			AddChannel(newChannel);

			newChannel = new Channel (ChannelType.Laba);
			AddChannel (newChannel);

			newChannel = new Channel (ChannelType.Nearby);
			AddChannel (newChannel);

		}
		 

		/*
		 * Client Send Message Out  
		 */ 
		public static void addTalkMsg (string msg, ChannelType channel)
		{
			if (msg.Length == 0) {
				return;
			}
				
			var myData = ObjectManager.objectManager.GetMyData ();

			var build = GCPushChat2Client.CreateBuilder ();
			build.PlayerId = ObjectManager.objectManager.GetMyServerID ();
			build.PlayerName = ObjectManager.objectManager.GetMyName ();
			build.PlayerLevel = myData.GetProp(CharAttribute.CharAttributeEnum.LEVEL);
			build.PlayerJob = ObjectManager.objectManager.GetMyJob ();
			build.PlayerVip = 0;
			build.TargetId = -1;
			build.Channel = (int)channel;
			build.ChatContent = msg;
			//msg.chatMsg = build.BuildPartial();
			HistoryMsg hisMsg = new HistoryMsg (build.BuildPartial ());
			talk.AddToRecvHistoryQue (hisMsg);

		}

		//根据频道查看不同的消息
		public void AddToRecvHistoryQue (HistoryMsg msg)
		{

			//Debug.Log (msg.recvMsg.Channel + ":这是错误的");
			Log.Sys ("Receive Message is "+msg.recvMsg);

			var cha = channelList [ChannelType.all];
			cha.recvHisQue.Add (msg);
			if (cha.recvHisQue.Count > maxSaveNum) {
				cha.recvHisQue.RemoveAt (0);
			}
			var ct = (ChannelType)msg.recvMsg.Channel;
			cha = channelList[ct];
			cha.recvHisQue.Add (msg);
			if (cha.recvHisQue.Count > maxSaveNum) {
				cha.recvHisQue.RemoveAt (0);
			}

			MyEventSystem.myEventSystem.PushEvent (MyEvent.EventType.NewChatMsg);

		}

		public int SendChatMessage (string chatMsg, ChannelType channel)
		{
			return 0;
		}
		bool HandleCommand(string cmd) {
			var cmds = cmd.Split(char.Parse("#"));
			if (cmds.Length > 0) {
				if(cmds[0] == "UpdateBackPack") {
					BackPack.backpack.StartCoroutine(BackPack.backpack.InitFromNetwork());

					return true;
				}
			}
			return false;
		}
		/*
		 * 客户端发送聊天消息
		 */ 
		public int SendChatMessage (string chatMsg, int channel)
		{
			//Limit Talk Speed

			//is GMCommand  Special Format For chatMsg 
			//:command par1 par2


			//Find Channel To Send Message
			Channel cha = channelList[(ChannelType)channel];

			if (HandleCommand (chatMsg)) {
				return 1;
			}

			if (cha == null) {
				Debug.LogError ("Talk::SendChatMessage Channel Not Exist ");
				return 1;
			}

			CGSendChat.Builder sendChat = CGSendChat.CreateBuilder ();
			sendChat.ChannelId = channel;
			sendChat.TargetName = "";
			sendChat.Content = chatMsg;
			cha.setSendPacket (sendChat);

			if (cha.isClose) {
				return 1;
			} else if (!cha.TalkNeedCheck ()) {
				return 1;
			} else if (!cha.TalkTimeCheck ()) {
				return 1;
			}

			//CGSendChat sc = 
            KBEngine.Bundle.sendImmediate (sendChat);
			return 0;
		}

		/*
		 * 客户端接受聊天消息
		 */ 
		public void HandleRecvTalkPacket (GCPushChat2Client chatMsg)
		{
			HistoryMsg msg = new HistoryMsg (chatMsg);

			//Handle Special Command Send From Server
			AddToRecvHistoryQue (msg);

			//UI Update 
			if (recvMsgDelegate != null) {
				recvMsgDelegate (msg);
			}
		}


	}
}
