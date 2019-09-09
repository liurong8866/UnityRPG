using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class FollowNpcCharacter : AICharacter
    {
        public override void SetIdle() {
            SetAni ("idle", 1, WrapMode.Loop);
        }
        public override void SetRun() {
            SetAni ("run", 2, WrapMode.Loop);
        }
    }

}