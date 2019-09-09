using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    public partial class Map2  : CScene
    {
        void DongHu2()
        {
            string[] text = new string[]{
                @"{0}，刚才村长那里发出一声巨响，似乎发生了什么事情，咱们快过去看看吧。", 
            };
            var npcDialog = WindowMng.windowMng.PushView("UI/NpcDialog", false);
            var dia = npcDialog.GetComponent<NpcDialog>();
            dia.ShowText(string.Format(text [0], ObjectManager.objectManager.GetMyName()));  
        }
        
        void TalkToDongHu()
        {
            var step = GameInterface_Player.GetIntState(GameBool.cunZhangState);
            if (step == 0)
            {
                if (!GameInterface_Player.GetGameState(GameBool.donghu1))
                {
                    StartCoroutine(DongHu1());
                } else
                {
                    DongHu2();
                }
            } else if(step == 1)
            {

                if (!GameInterface_Player.GetGameState(GameBool.donghu3))
                {
                    DongHu3();
                } else
                {
                    DongHu4();
                }
            }else if(step == 6) {
                DongHu6();
            }
        }

        void DongHu3() {
            string[] text = new string[]{
                "东湖一起找宝贝么,至若妹妹也一起去呀", 
                "有宝贝，那我也想去看看",
            }; 
            NpcDialogInterface.ShowTextList(new System.Collections.Generic.List<string>(text), delegate() {
                GameInterface_Player.SetGameState(GameBool.donghu3, true);
           });
        }
        void DongHu4() {
            string[] text = new string[]{
                "我们快点出发吧。", 
            }; 
            NpcDialogInterface.ShowTextList(new System.Collections.Generic.List<string>(text), null);
        }
        void DongHu6() {
            var text = new string[] {
                "巨牙子爷爷，快给东湖服下这药水吧！",
                "好的",
                "咕噜噜...",
                "突然一道血光从东湖嘴里喷薄而出，传来阴森森的声音，哈哈哈，终于等到这一天了",
                "巨牙子：不好，你们快快离开，我要施法封住魔神之灵。",
                "遵命",
            };
            NpcDialogInterface.ShowTextList(text, null);
            GameInterface_Player.SetIntState(GameBool.cunZhangState, 8);
        }

        IEnumerator  DongHu1()
        {
            string[] text = new string[]{
                @"{0}，叔叔婶婶都是极厉害的人，他们一定是去干惊天大事情去了，真想有一天像他们一样呀。", 
                @"东湖，你想和我一起出去闯荡么？",
                @"听说外面世界很大，可是我还是喜欢呆在村子里，每天看见至若妹妹，我就很开心了。",
                @"至若妹妹说她也想出去呢",
                @"是吗，如果你们都走了我也要一起",
                @"好的一言为定，到时候我们一起闯荡天下。"
            };
            var npcDialog = WindowMng.windowMng.PushView("UI/NpcDialog", false);
            var dia = npcDialog.GetComponent<NpcDialog>();
            bool next = false;
            dia.ShowNext = delegate()
            {
                next = true;
            };
            
            foreach (var t in text)
            {
                dia.ShowText(string.Format(t, ObjectManager.objectManager.GetMyName())); 
                while (!next)
                {
                    yield return new WaitForSeconds(0.1f);
                }
                next = false;
            }
            
            GameInterface_Player.SetGameState(GameBool.donghu1, true);
            WindowMng.windowMng.PopView(); 
        }
    }

}