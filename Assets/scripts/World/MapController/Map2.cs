using UnityEngine;
using System.Collections;

namespace MyLib
{
    /// <summary>
    ///主城场景控制类配置 
    /// 配置所有Npc的Interactive事件的处理机制
    /// </summary>
    public partial class Map2 : CScene 
    {
        public override void Init()
        {
        }

        public override void EnterScene()
        {

        }

        public override void LeaveScene()
        {
            base.LeaveScene();
        }

        public override void ManagerInitOver()
        {
        }

        //Wait For All Npc Init Over
        //Then Set Npc TalkHandler
        IEnumerator Start(){
            yield break;
            yield return new WaitForSeconds(1f);
            var cunZhang = NpcManager.Instance.GetNpc("巨牙子");
            cunZhang.TalkToMe = TalkToCunZhang;

            var miePo = NpcManager.Instance.GetNpc("灭婆");
            miePo.TalkToMe = TalkToMiePo;

            var zhiRuo = NpcManager.Instance.GetNpc("至若");
            zhiRuo.TalkToMe = TalkToZhiRuo;

            var donghu  = NpcManager.Instance.GetNpc("东湖");
            donghu.TalkToMe = TalkToDongHu;

            var qinqing = NpcManager.Instance.GetNpc("秦情");
            qinqing.TalkToMe = TalkToQinQing;

            var wanshan = NpcManager.Instance.GetNpc("万山");
            wanshan.TalkToMe = TalkToWanShan;

            var aniu  = NpcManager.Instance.GetNpc("阿牛");
            aniu.TalkToMe = TalkToANiu;
        }

        void TalkToMiePo() {
            string[] text = new string[]{
                @"离我们家至若远点,你这不祥之子", 
            };
            var npcDialog = WindowMng.windowMng.PushView("UI/NpcDialog", false);
            var dia = npcDialog.GetComponent<NpcDialog>();
            dia.ShowText(text[0]); 
        }

       



       

        IEnumerator  QinQing1() {
            string[] text = new string[]{
                @"秦情姐姐，你好美呀", 
                @"臭小孩一边去，又想从我这里偸药么？听说你想去外面闯荡。",
                @"恩，我想去找我父母。",
                @"外面世界很凶险，我送你几瓶药吧。",
                @"谢谢姐姐。",
            };
            var npcDialog = WindowMng.windowMng.PushView("UI/NpcDialog", false);
            var dia = npcDialog.GetComponent<NpcDialog>();
            bool next = false;
            dia.ShowNext = delegate() {
                next = true;
            };
            
            foreach(var t in text) {
                dia.ShowText(string.Format(t, ObjectManager.objectManager.GetMyName())); 
                while(!next) {
                    yield return new WaitForSeconds(0.1f);
                }
                next = false;
            }
            //Send Drug To User
            GameInterface_Chat.chatInterface.SendChatMsg("add_item 101 10", 0);

            GameInterface_Player.SetGameState(GameBool.qinqing1, true);
            WindowMng.windowMng.PopView();
        }

        void QinQing2() {
            string[] text = new string[]{
                @"{0}，外面很危险，你一定要多加小心。", 
            };
            var npcDialog = WindowMng.windowMng.PushView("UI/NpcDialog", false);
            var dia = npcDialog.GetComponent<NpcDialog>();
            dia.ShowText(string.Format(text[0], ObjectManager.objectManager.GetMyName())); 
        }

        void TalkToQinQing() {
            if(!GameInterface_Player.GetGameState(GameBool.qinqing1)) {
                StartCoroutine(QinQing1());
            }else {
                QinQing2();
            }
        }

        IEnumerator WanShan1() {
            string[] text = new string[]{
                @"{0}，我在村外找到的好玩的玩意，只要十个金币，你有钱了就可以到我这里耍耍。",
                @"万山叔，等我有钱了，再来找你。",
                @"等等，你能帮我带件礼物给秦情么，但别说是我送的，如果你答应的话，我可以送你件礼物。",
                @"恩，好的。",
                @"......,嘿嘿。",
            };
            var npcDialog = WindowMng.windowMng.PushView("UI/NpcDialog", false);
            var dia = npcDialog.GetComponent<NpcDialog>();
            bool next = false;
            dia.ShowNext = delegate() {
                next = true;
            };
            
            foreach(var t in text) {
                dia.ShowText(string.Format(t, ObjectManager.objectManager.GetMyName())); 
                while(!next) {
                    yield return new WaitForSeconds(0.1f);
                }
                next = false;
            }
            
            GameInterface_Player.SetGameState(GameBool.wanshan1, true);
            WindowMng.windowMng.PopView(); 
        }

        void WanShan2() {
            string[] text = new string[]{
                @"{0},托你给秦情的礼物送到了么?", 
            };
            var npcDialog = WindowMng.windowMng.PushView("UI/NpcDialog", false);
            var dia = npcDialog.GetComponent<NpcDialog>();
            dia.ShowText(string.Format(text[0], ObjectManager.objectManager.GetMyName())); 
        }

        void TalkToWanShan() {
            if(!GameInterface_Player.GetGameState(GameBool.wanshan1)) {
                StartCoroutine(WanShan1());
            }else {
                WanShan2();
            }
        }

        void TalkToANiu() {
            string[] text = new string[]{
                @"我叫阿牛，我爸爸离开村子了，好久没回来了。", 
            };
            var npcDialog = WindowMng.windowMng.PushView("UI/NpcDialog", false);
            var dia = npcDialog.GetComponent<NpcDialog>();
            dia.ShowText(string.Format(text[0], ObjectManager.objectManager.GetMyName())); 
        }
    }

}