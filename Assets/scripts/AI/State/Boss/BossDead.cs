using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class BossDead : MonsterDead 
    {
        public override void EnterState()
        {
            base.EnterState();
            MyEventSystem.myEventSystem.PushEvent(MyEvent.EventType.BossDead);
        }
    }

}