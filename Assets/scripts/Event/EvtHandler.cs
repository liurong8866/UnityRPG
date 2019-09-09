using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    public class EvtHandler : MonoBehaviour
    {
        private Dictionary<MyEvent.EventType, EventDel> regEvts = new Dictionary<MyEvent.EventType, EventDel>();

        Dictionary<MyEvent.EventType, MyEvent> happenedEvt = new Dictionary<MyEvent.EventType, MyEvent>();

        public void AddEvent(MyEvent.EventType type, EventDel cb)
        {
            if(cb != null) {
                regEvts.Add(type, cb);
                MyEventSystem.myEventSystem.RegisterEvent(type, cb);
            }else {
                var del = new EventDel(delegate(MyEvent evt){
                    if(!happenedEvt.ContainsKey(type)) {
                        happenedEvt.Add(type, evt);
                    }
                });
                MyEventSystem.myEventSystem.RegisterEvent(type, del);
            }
        }

        public IEnumerator WaitEvt(MyEvent.EventType type)
        {
            while(!happenedEvt.ContainsKey(type)){
                yield return null;
            }
            happenedEvt.Remove(type);
        }

        void OnDestory() {
            foreach(var e in regEvts) {
                MyEventSystem.myEventSystem.dropListener(e.Key, e.Value);
            }
            regEvts.Clear();
        }
    }
}
