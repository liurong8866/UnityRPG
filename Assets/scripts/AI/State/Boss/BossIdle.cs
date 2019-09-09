using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class BossIdle : IdleState
    {
        bool isFirst = true;

        public override void EnterState()
        {
            base.EnterState();
            aiCharacter.SetIdle();
            if (isFirst)
            {
                if(MyEventSystem.myEventSystem.GetRegEventHandler(MyEvent.EventType.BossSpawn) > 0){
                    MyEventSystem.myEventSystem.PushEvent(MyEvent.EventType.BossSpawn);
                }
                else {
                    isFirst = false;
                }
                BackgroundSound.Instance.PlayEffect("summon3");
                var g = GameObject.Instantiate(Resources.Load<GameObject>("particles/playerskills/impsummon")) as GameObject;
                var xft = g.GetComponent<XffectComponent>();
                xft.enabled = false;
                //xft.Scale = 2;
                //g.transform.parent = SaveGame.saveGame.EffectMainNode.transform;
                g.transform.position = GetAttr().transform.position;
                GetAttr().StartCoroutine(WaitEnable(xft));
                NGUITools.AddMissingComponent<RemoveSelf>(g);
            }
        }
        IEnumerator WaitEnable(XffectComponent xft) {
            yield return new WaitForSeconds(0.05f);
            xft.enabled = true;
            xft.Reset();
        }
        IEnumerator WaitForSpeakOver()
        {
            bool ret = false;
            EventDel del = delegate(MyEvent evt)
            {
                ret = true;
            }; 
            MyEventSystem.myEventSystem.RegisterEvent(MyEvent.EventType.SpeakOver, del);
            while (!ret)
            {
                yield return new WaitForSeconds(1);
            }
            MyEventSystem.myEventSystem.dropListener(MyEvent.EventType.SpeakOver, del);
            isFirst = false;
        }

        public override IEnumerator RunLogic()
        {
            if (isFirst)
            {
                yield return GetAttr().StartCoroutine(WaitForSpeakOver());
            }
            //aiCharacter.ChangeState(AIStateEnum.IDLE, 1);
            while (true)
            {
                yield return new WaitForSeconds(2);
                GameObject player = ObjectManager.objectManager.GetMyPlayer();
                if (player && !player.GetComponent<NpcAttribute>().IsDead)
                {
                    aiCharacter.ChangeState(AIStateEnum.COMBAT);
                }
            }
        }

    }
}