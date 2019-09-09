using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class GMCmd : IUserInterface
    {
        UIInput input;
        void Awake() {
            input = GetInput("Input");
            SetCallback("okBut", OnOk);
        }
        void OnOk(GameObject g){
            var text = input.value;
            GameInterface_Chat.chatInterface.SendChatMsg(text, 0);

            Hide(null);
        }
    }

}