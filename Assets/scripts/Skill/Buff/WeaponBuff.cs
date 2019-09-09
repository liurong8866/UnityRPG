using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class WeaponBuff : IEffect
    {
        GameObject particle;
        int localId;

        public override void Init(Affix af, GameObject o)
        {
            base.Init(af, o);
            type = Affix.EffectType.WeaponBuff;
            BackgroundSound.Instance.PlayEffect("emberlightningblast1");

            localId = obj.GetComponent<NpcAttribute>().GetLocalId();
            MyEventSystem.myEventSystem.RegisterLocalEvent(localId, MyEvent.EventType.HitTarget, OnEvent);
        }

        void OnEvent(MyEvent evt)
        {
            if(evt.type == MyEvent.EventType.HitTarget){
                var target = evt.target;
                //在目标身上立即添加这个武器Buff造成的伤害Buff
                //Hurt Direct 但是伤害数值0 WeaponDamagePCT= 0
                var skmachine = SkillLogic.CreateSkillStateMachine(obj, 
                    Util.GetSkillData(
                    System.Convert.ToInt32(affix.GetPara(PairEnum.Abs)), 1), Vector3.zero, target);
            }
        }

        public override void OnActive()
        {
            base.OnActive();
            particle = GameObject.Instantiate(affix.buffParticle) as GameObject;
            var sync = particle.AddComponent<SyncPosWithTarget>();
            var ne = obj.GetComponent<NpcEquipment>();
            sync.target = ne.GetRightHand().gameObject;
            sync.transform.localPosition = affix.ThemePos;
            var rot = SimpleJSON.JSON.Parse(affix.GetPara(PairEnum.Rot)).AsArray;
            sync.transform.localRotation = Quaternion.Euler(new Vector3(rot [0].AsInt, rot [1].AsInt, rot [2].AsInt));

        }

        public override void OnDie()
        {
            base.OnDie();
            GameObject.Destroy(particle);
            MyEventSystem.myEventSystem.DropLocalListener(localId, MyEvent.EventType.HitTarget, OnEvent);
        }

    }

}