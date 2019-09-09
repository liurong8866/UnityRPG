using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class QuestNpcCharacter : AICharacter
    {
        public override void SetIdle() {
            SetAni ("idle", 1, WrapMode.Loop);
        }
    }

}