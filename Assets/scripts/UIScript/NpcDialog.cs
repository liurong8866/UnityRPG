using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    public class NpcDialog : IUserInterface 
    {
        UILabel label;
        GameObject button;
        public EmptyDelegate ShowNext;
        void Awake() {
            label = GetLabel("Label");
            button = GetName("Next");
            SetCallback("Next", OnNext);
            SetCallback("Fast", OnFast);
            button.SetActive(false);
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            MyEventSystem.myEventSystem.PushEvent(MyEvent.EventType.ShowStory);
        }
        protected override void OnDisable()
        {
            MyEventSystem.myEventSystem.PushEvent(MyEvent.EventType.EndStory);
            base.OnDisable();
        }

        bool onNext = false;
        void OnNext() {
            onNext = true;
        }

        bool fast = false;
        int count = 0;
        void OnFast() {
            Log.GUI("OnFast "+count);
            count++;
            if(count >= 2){
                fast = true;
            }
        }

        IEnumerator PrintText(string t) {
            button.SetActive(false);
            fast = false;
            onNext = false;
            for(int c = 0; c<= t.Length; c++){
                if(fast) {
                    break;
                }
                label.text = t.Substring(0, c);
                yield return new WaitForSeconds(0.05f);
                BackgroundSound.Instance.PlayEffect("pickup");
            }
            label.text = t;
            button.SetActive(true);

            while(!onNext) {
                yield return null;
            }

            if(ShowNext != null) {
                ShowNext();
            }else {
                WindowMng.windowMng.PopView();
            }
        }


        public void ShowText(string t) {
            StartCoroutine(PrintText(t));
        }

    }

}