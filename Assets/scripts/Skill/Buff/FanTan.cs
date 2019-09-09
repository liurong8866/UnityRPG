using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class FanTan : IEffect
    {
        int localId;
        AudioSource audio;
        public override void Init(Affix af, GameObject o)
        {
            base.Init(af, o);
            type = Affix.EffectType.FanTan;
            audio = BackgroundSound.Instance.PlayEffectLoop("skill/loopfirelarge1");
            audio.Play();
            localId = obj.GetComponent<NpcAttribute>().GetLocalId();
            MyEventSystem.myEventSystem.RegisterLocalEvent(localId, MyEvent.EventType.OnHit, OnEvent);
        }

        void OnEvent(MyEvent evt) {
            Log.AI("FanTan OnHit "+evt.type);
            var target = evt.attacker;
            var rd = Random.Range(0, 100);
            var critical = 1;
            if(rd < 25) {
                critical = 2;
            }
            SkillDamageCaculate.DoDamage(obj, 100*critical, evt.attacker);
        }

        public override void OnDie()
        {
            MyEventSystem.myEventSystem.DropLocalListener(localId, MyEvent.EventType.OnHit, OnEvent);
            audio.enabled = false;
            GameObject.Destroy(audio);
            base.OnDie();
        }
    }

}