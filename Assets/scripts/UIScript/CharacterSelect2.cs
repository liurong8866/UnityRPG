using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    public class CharacterSelect2 : IUserInterface
    {
        GameObject enterBut;
        GameObject createBut;
        UILabel namelevel;
        GameObject SelChar;

        void Awake(){
            enterBut = GetName("enterGame");
            createBut = GetName("createChar");
            namelevel = GetLabel("Name");
            SetCallback("enterGame", OnEnter);
            SetCallback("createChar", OnCreate);
            SelChar = GetName("SelChar");
            SelChar.SetActive(false);

            regEvt = new List<MyEvent.EventType> (){
                MyEvent.EventType.UpdateSelectChar,
            };
            RegEvent ();
        }
        void OnEnter(GameObject g){
            MyEventSystem.myEventSystem.PushEvent(MyEvent.EventType.MeshHide);
            GameInterface_Login.loginInterface.StartGame (selectRoleInfo);
        }
        void ShowCreate(){
            Log.GUI ("Push View Create");
            WindowMng.windowMng.ReplaceView ("UI/CharCreate", false, false);
            MyEventSystem.myEventSystem.PushEvent(MyEvent.EventType.UpdateCharacterCreateUI);
        }

        void OnCreate(GameObject g){
            ShowCreate();
        }

        protected override void OnEvent (MyEvent evt)
        {
            UpdateFrame ();
        }
        protected override void OnDestroy(){
            base.OnDestroy();
            //MyEventSystem.myEventSystem.PushEvent(MyEvent.EventType.MeshHide);
        }
        RolesInfo selectRoleInfo;
        void UpdateFrame() {
            var charInfo = GameInterface_Login.loginInterface.GetCharInfo ();
            if (charInfo != null) {
                if(charInfo.RolesInfosList.Count >0) {
                    enterBut.SetActive(true);
                    createBut.SetActive(false);
                    var charI =  charInfo.RolesInfosList[0];
                    selectRoleInfo = charI;
                    namelevel.text = string.Format("[d2691e]姓名:{0}[-]\n[a0522d]等级:{1}[-]", charI.Name, charI.Level);
                    namelevel.gameObject.SetActive(true);

                    SelChar.SetActive(true);
                    var evt = new MyEvent(MyEvent.EventType.MeshShown);
                    evt.intArg = -1;
                    evt.rolesInfo = charI;

                    MyEventSystem.myEventSystem.PushEvent(evt);
                }else {
                    SelChar.SetActive(false);
                    enterBut.SetActive(false);
                    createBut.SetActive(true);
                    namelevel.gameObject.SetActive(false);
                }

                

            }
        }
    }
}
