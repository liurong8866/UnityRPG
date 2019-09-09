using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class HumanStunned : StunnedState 
    {
        public override void EnterState ()
        {
            Log.AI ("Enter Stunned State");
            base.EnterState ();
            aiCharacter.SetIdle ();
        }

    }
}
