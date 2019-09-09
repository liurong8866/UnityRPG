using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class QuestIdle : IdleState
    {
        public override void EnterState()
        {
            base.EnterState();
            aiCharacter.SetIdle();
        }
    }
}
