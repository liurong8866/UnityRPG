using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class GoldSteal : IEffect
    {
        public override void Init(Affix af, GameObject o)
        {
            base.Init(af, o);
            type = Affix.EffectType.GoldSteal;
            BackgroundSound.Instance.PlayEffect("dropgold");
        }

        public override void OnActive()
        {
            base.OnActive();
            if (attri.IsMine())
            {
                var abs = System.Convert.ToInt32(affix.GetPara(PairEnum.Abs));
                PlayerData.AddGold(-abs);
            }
        }


    }

}