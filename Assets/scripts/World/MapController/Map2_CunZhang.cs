using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    public partial class Map2 : CScene
    {
        IEnumerator ShowChapter1StartDialog()
        {
            var npcDialog = WindowMng.windowMng.PushView("UI/NpcDialog", false);
            var dia = npcDialog.GetComponent<NpcDialog>();
            dia.ShowNext = delegate()
            {
                GameInterface_Player.SetGameState(GameBool.chapter1Start, true);
                WindowMng.windowMng.PopView();
            };
            
            string[] text = new string[]{
                @"孩子你父母有重要的事情要做，他们嘱托我照顾你，等你有了力量，就可以去找他们了。你父母走之前留给你一些东西，现在你可以先去村子里转转，一会再过来找我。",
            };
            dia.ShowText(text [0]);
            yield return null;
        }
        
        void ShowCunZhangNormalWord()
        {
            string[] text = new string[]{
                @"{0}，你再耐心等一会，我正在施法。", 
            };
            var npcDialog = WindowMng.windowMng.PushView("UI/NpcDialog", false);
            var dia = npcDialog.GetComponent<NpcDialog>();
            dia.ShowText(string.Format(text [0], ObjectManager.objectManager.GetMyName()));
        }

        bool CheckCondition(List<string> con)
        {
            foreach (var c in con)
            {
                if (!GameInterface_Player.GetGameState(GameBool.chapter1Start))
                {
                    return false;
                }
            }
            return true;
        }

        IEnumerator  CunZhang3()
        {
            string[] text = new string[]{
                "{0},你回来了\n我刚才施法打开了通往试炼之境的密道，你可以去那里，找到你父母留给你的东西。", 
                "巨牙子爷爷，试炼之境是什么地方？",
                "那里是村子的地下的一个秘境，据说是上古通天神魔用无边法力所构建的一个空间，里面居住着一些魔神的子子孙孙，到那后你要小心。",
                "父母留给我什么宝物呢？",
                "这个我也不清楚，应该是拥有通灵之力的神物，只有这种神物，才不会被魔气所侵袭。\n你可以叫上至若和东湖帮你，试炼之境虽不是万分险恶，但那些魔物也不是好惹之徒，你们还是要万分小心的。",
                "恩，多谢巨牙子爷爷。",
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
            
            GameInterface_Player.SetIntState(GameBool.cunZhangState, 1);
            WindowMng.windowMng.PopView();
        }

        void CunZhang4()
        {
            string[] text = new string[]{
                @"{0},早去早回呀。", 
            };
            var npcDialog = WindowMng.windowMng.PushView("UI/NpcDialog", false);
            var dia = npcDialog.GetComponent<NpcDialog>();
            dia.ShowText(string.Format(text [0], ObjectManager.objectManager.GetMyName()));
        }

       void CunZhang5(){
            string[] text = new string[]{
                "东湖受了魔气侵袭，需要苍冥水方能恢复，此物在试炼之境可以寻到，你快快和至若一起去寻回来，晚了，怕东湖会有危险。",
                "巨牙子爷爷放心，我这就去找至若，一起去苍冥水。",
            };

            NpcDialogInterface.ShowTextList(new System.Collections.Generic.List<string>(text), delegate() {
                GameInterface_Player.SetIntState(GameBool.cunZhangState, 3);
            });

        }

        void CunZhang6() {
            string[] text = new string[]{
                @"早去早回呀。", 
            };
            NpcDialogInterface.ShowTextList(new System.Collections.Generic.List<string>(text), null);
        }
        void CunZhang7() {
            var text = new string[]{
                @"你们终于回来了，找到苍冥水了么？",
                @"至若：不知道这个是不是，我们遇到了一只苍狼，从它身上找到的",
                @"正是此物，苍冥水本是苍狼性命本源，拥有无限生机,正好可以用来治好东湖身上伤",
                @"不过这苍冥水有些浑浊，怕是那狼恐是暮年",
                @"至若：怪不得这么好对付呢",
                @"巨牙子：东湖那边现在危在旦夕，此物需速速给他服下，你们同我一起去吧。",
                @"是的爷爷",
            };
            NpcDialogInterface.ShowTextList(text, null);
            GameInterface_Package.SellQuestItem((int)ItemData.ItemID.CANG_MING_SHUI);
            GameInterface_Player.SetIntState(GameBool.cunZhangState, 6);
        }

        void CunZhang8() {
            var text = new string[] {
                @"巨牙子：东湖体内魔气现在已经被清除干净，还是静养数日，你和至若，最近不要再靠近试炼之地了，那里危机重重。",
                @"是的巨牙子爷爷",
                @"你去把我的吩咐告诉至若吧。",
            };
            NpcDialogInterface.ShowTextList(text, null);
            GameInterface_Player.SetIntState(GameBool.cunZhangState, 9);
        }



        void TalkToCunZhang()
        {
            Log.GUI("TalkTOCunZhange");
            //未曾开始对话
            if (!GameInterface_Player.GetGameState(GameBool.chapter1Start))
            {
                StartCoroutine(ShowChapter1StartDialog());
            } else
            {
                var step = GameInterface_Player.GetIntState(GameBool.cunZhangState);
                if (step == 0)
                {
                    var c = new List<string>() {
                        GameBool.chapter1Start,
                        GameBool.zhiruo1,
                        GameBool.donghu1,
                        GameBool.qinqing1,
                        GameBool.wanshan1,
                    };
                    if (CheckCondition(c))
                    {
                        StartCoroutine(CunZhang3());
                    } else
                    {
                        ShowCunZhangNormalWord();
                    }
                }else if(step == 1){
                    CunZhang4();
                }else if(step == 2){
                    CunZhang5();
                }else if(step == 3){
                    CunZhang6();
                }else if(step == 5) {
                    CunZhang7();
                }else if(step == 8) {
                    CunZhang8();
                }
            }
        }
    }

}