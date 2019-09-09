using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    public partial class Map2 : CScene 
    {
        void TalkToZhiRuo() {
            var step = GameInterface_Player.GetIntState(GameBool.cunZhangState);
            if(step == 0) {
                if(!GameInterface_Player.GetGameState(GameBool.zhiruo1)) {
                    StartCoroutine(ZhiRuo1());
                }else {
                    ZhiRuo2();
                }
            }else if(step == 1){
                var c = new List<string>() {
                    GameBool.zhiruo3,
                };
                if(CheckCondition(c)) {
                    StartCoroutine(ZhiRuo4());
                }else {
                    StartCoroutine(ZhiRuo3());    
                }
            }else if(step == 3) {
                ZhiRuo5();
            }else if(step == 9) {
                ZhiRuo9();
            }
        }

        void ZhiRuo5() {
            string[] text = new string[]{
                "至若快和我一起去寻苍冥水，救东湖", 
                "好的哥哥。",
            };

            NpcDialogInterface.ShowTextList(new System.Collections.Generic.List<string>(text), delegate() {
                GameInterface_Player.SetIntState(GameBool.cunZhangState, 4);
            });
        }

        IEnumerator  ZhiRuo3() {
            string[] text = new string[]{
                "至若要和我一起去，找宝贝么？", 
                "好的哥哥。",
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
            
            GameInterface_Player.SetGameState(GameBool.zhiruo3, true);
            WindowMng.windowMng.PopView();
        }
        IEnumerator ZhiRuo4() {
            string[] text = new string[]{
                "哥哥我准备好，咱们快出发吧，不要被我阿婆发现了。", 
            };
            var npcDialog = WindowMng.windowMng.PushView("UI/NpcDialog", false);
            var dia = npcDialog.GetComponent<NpcDialog>();
            dia.ShowText(string.Format(text [0], ObjectManager.objectManager.GetMyName()));
            yield return null;
        }

        IEnumerator ZhiRuo1() {
            string[] text = new string[]{
                @"{0}哥哥,叔叔和婶婶应该不久就会回来，你别难过哦。", 
                @"至若，等我变厉害了，我就出村去找他们。",
                @"{0}哥哥，我也想出村去看看，听村长爷爷说，村外有很多神奇的东西，还有好多好多人呢。",
                @"恩没问题,到时候我一定带你一起出去",
                @"哥哥，你真好。",
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
            
            GameInterface_Player.SetGameState(GameBool.zhiruo1, true);
            WindowMng.windowMng.PopView();
        }
        void ZhiRuo2() {
            string[] text = new string[]{
                @"{0}哥哥,你去看看东湖哥哥吧，他好像在等你。", 
            };
            var npcDialog = WindowMng.windowMng.PushView("UI/NpcDialog", false);
            var dia = npcDialog.GetComponent<NpcDialog>();
            dia.ShowText(string.Format(text[0], ObjectManager.objectManager.GetMyName())); 

        }

        void ZhiRuo9() {
            var text = new string[]{
                "至若，巨牙子爷爷吩咐今后不能靠近试炼之地，那里现在危机重重",
                "恩，哥哥我知道你出村心切，但是你也万万不可再进入呀",
                "那日你我和魔神交手，它也不过尔尔，若不能尽早找到父母留给我的东西，又怎能尽早出村呢，你不必劝我了，今夜我自己去一趟，寻得宝物",
                "哥哥，我知道劝你也劝不住，不过你要万分小心呀，这是我们传家之宝--九曜石，也许在你危险的时候可以帮你。",
                "多谢至若，东湖就拜托你多多照顾一下了",
                "恩哥哥，万事要小心呀",
            };
            NpcDialogInterface.ShowTextList(text, null);
            GameInterface_Player.SetIntState(GameBool.cunZhangState, 10);
        }
    
    }

}