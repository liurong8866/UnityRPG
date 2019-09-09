using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class ReduceHP : IEffect
    {
        public override void Init(Affix af, GameObject o)
        {
            base.Init(af, o);
            type = Affix.EffectType.ReduceHP;
            BackgroundSound.Instance.PlayEffect("skill/fireattack3");
        }
        IEnumerator Add(){
            var duration = affix.Duration;
            var abs  = System.Convert.ToInt32(affix.GetPara(PairEnum.Abs));
            var attr = obj.GetComponent<CharacterInfo>();
            var hpmax = attr.GetProp(CharAttribute.CharAttributeEnum.HP_MAX);
            var npcAttr = obj.GetComponent<NpcAttribute>();

            float goneTime = 0;
            int count = 0;
            var period = 1f;
            int tc = Mathf.RoundToInt (duration/period);

            var addNumHP = abs;
            Log.Sys("AddHpNum AddMpNum "+addNumHP+" mp "+" max "+hpmax+" mpmax ");
            //0.5s 造成一次伤害数值  -10
            Log.Sys("TotalAddCount "+count+" total "+tc);
            while(count < tc && !IsDie) {
                if(goneTime > period) {
                    npcAttr.ChangeHP(-addNumHP);
                    goneTime -= period;
                    count++;
                    BackgroundSound.Instance.PlayEffect("skill/fireattack3");
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
