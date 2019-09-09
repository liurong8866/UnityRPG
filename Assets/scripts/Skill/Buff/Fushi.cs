using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class Fushi : IEffect
    {
        public override void Init(Affix af, GameObject o)
        {
            base.Init(af, o);
            type = Affix.EffectType.Fushi;
            BackgroundSound.Instance.PlayEffect("evilmagic");
        }
        public override void OnActive ()
        {
            //obj.GetComponent<CharacterInfo> ().SetDirty (CharAttribute.CharAttributeEnum.DEFENSE);
        }

        public override int GetArmor ()
        {
            return affix.addDefense;
        }

        public override void OnDie ()
        {
            base.OnDie ();
            //obj.GetComponent<CharacterInfo> ().SetDirty (CharAttribute.CharAttributeEnum.DEFENSE);
        }
    }
}
