using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class Stunned : IEffect
    {
        public override void Init (Affix af, GameObject o)
        {
            base.Init (af, o);
            type = Affix.EffectType.Stunned;
        }


        /// <summary>
        /// 进入STUNNED状态不出来了 如果死亡也会出来 
        /// </summary>
        public override void OnActive ()
        {
            Log.AI ("Stunned Buff Active");
            var ani = obj.GetComponent<MyAnimationEvent> ();
            var msg = new MyAnimationEvent.Message(MyAnimationEvent.MsgType.STUNNED);
            ani.InsertMsg(msg);
        }

        /// <summary>
        /// 恢复IDLE状态 
        /// </summary>
        public override void OnDie()
        {
            base.OnDie();
            var ani = obj.GetComponent<MyAnimationEvent> ();
            var msg = new MyAnimationEvent.Message(MyAnimationEvent.MsgType.EXIT_STUNNED);
            ani.InsertMsg(msg);
        }
    }
}
