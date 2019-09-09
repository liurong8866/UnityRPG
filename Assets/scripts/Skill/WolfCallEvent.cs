using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class WolfCallEvent : MonoBehaviour
    {
        void Start() {
            var runner = transform.parent.GetComponent<SkillLayoutRunner>();
            var evt = new MyEvent(MyEvent.EventType.WolfCall);
            var npc =runner.stateMachine.attacker.GetComponent<NpcAttribute>(); 
            var comm = npc.GetComponent<CommonAI>();
            evt.localID = npc.GetLocalId();
            evt.target = comm.targetPlayer;
            MyEventSystem.myEventSystem.PushEvent(evt);
        }
    }

}