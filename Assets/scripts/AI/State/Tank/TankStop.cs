using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class TankStop : AIState 
    {
        public TankStop() {
            this.type = AIStateEnum.STOP;
        }
        public override void EnterState()
        {
            base.EnterState();

        }
        public override IEnumerator RunLogic()
        {
            while(!quit) {
                ClearEvent ();
                yield return null;
            }
        }
    }
}