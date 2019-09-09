using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class ChestDead : DeadState 
    {
        public override void EnterState ()
        {
            base.EnterState ();
            GetAttr().GetComponent<Animation>().CrossFade ("opening");
            GetAttr().IsDead = true;
            GetAttr().OnlyShowDeadEffect();
        }
    }

}