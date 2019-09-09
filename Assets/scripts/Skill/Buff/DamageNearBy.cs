using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class DamageNearBy : IEffect
    {
        int localId;
        AudioSource audio;
        public override void Init(Affix af, GameObject o)
        {
            base.Init(af, o);
            type = Affix.EffectType.DamageNearBy;
            audio = BackgroundSound.Instance.PlayEffectLoop("skill/loopfirelarge1");
            audio.Play();
            localId = obj.GetComponent<NpcAttribute>().GetLocalId();
            obj.GetComponent<NpcAttribute>().StartCoroutine(DamageNear());
        }

        IEnumerator DamageNear() {

            while(!IsDie) {
                yield return new WaitForSeconds(3);
                BackgroundSound.Instance.PlayEffect("skill/fireimp1");
                var stateMachine = SkillLogic.CreateSkillStateMachine(obj, Util.GetSkillData(129, 1), obj.transform.position, null);
                if(affix.fireParticle != null) {
                    var evt = new MyEvent(MyEvent.EventType.SpawnParticle);
                    evt.particleOffset = new Vector3(0, 1, 0);
                    evt.particle2 = affix.fireParticle;
                    evt.boneName = null;
                    evt.player = obj;
                    MyEventSystem.myEventSystem.PushEvent(evt);
                }
            }
        }


        public override void OnDie()
        {
            audio.enabled = false;
            GameObject.Destroy(audio);
            base.OnDie();
        }

    }

}