using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class LeftTimeUI : IUserInterface
    {
        UILabel notifyLabel;
        void Awake()
        {
            notifyLabel = GetLabel("notifyLabel");
        }
        public void SetLabel(string txt) {
            notifyLabel.text = txt;
        }
    }

}