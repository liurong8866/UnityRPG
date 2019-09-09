using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class AddHPMP :IEffect
    {
        public override void Init(Affix af, GameObject o)
        {
            base.Init(af, o);
            type = Affix.EffectType.AddHPMP;
            BackgroundSound.Instance.PlayEffect("healdrink");
        }
        IEnumerator Add(){
            var duration = affix.Duration;
            var percent = System.Convert.ToSingle(affix.GetPara(PairEnum.Percent))/100.0f;
            var attr = obj.GetComponent<CharacterInfo>();
            var hpmax = attr.GetProp(CharAttribute.CharAttributeEnum.HP_MAX);
            var mpmax = attr.GetProp(CharAttribute.CharAttributeEnum.MP_MAX);
            var npcAttr = obj.GetComponent<NpcAttribute>();

            float goneTime = 0;
            int count = 0;
            var period = 0.2f;
            int tc = Mathf.RoundToInt (duration/period);

            var addNumHP = Mathf.RoundToInt(hpmax*percent/duration*period);
            var addNumMp = Mathf.RoundToInt(mpmax*percent/duration*period);
            Log.Sys("AddHpNum AddMpNum "+addNumHP+" mp "+addNumMp+" max "+hpmax+" mpmax "+mpmax);

            Log.Sys("TotalAddCount "+count+" total "+tc);
            while(count < tc && !IsDie) {
                if(goneTime > period) {
                    npcAttr.ChangeHP(addNumHP);
                    npcAttr.ChangeMP(addNumMp);
                    goneTime -= period;
                    count++;
                }
                goneTime += Time.deltaTime;
                yield return null;
            }

            Log.Sys("AddBUff over");
        }
        public override void OnActive()
        {
            base.OnActive();
            var buff = obj.GetComponent<BuffComponent>();
            buff.StartCoroutine(Add());
        }
    }
}
