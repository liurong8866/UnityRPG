using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class DialogUI : IUserInterface
    {
        void Awake()
        {
            SetCallback("Ok", delegate(GameObject g){
                Hide(null);
                act(true);
           });
            SetCallback("Cancel", delegate (GameObject g){
                Hide(null);
                act(false);
            });
        }

        BoolDelegate act;

        public void SetCb(BoolDelegate action)
        {
            act = action;
        }

    }

}