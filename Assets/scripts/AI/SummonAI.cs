using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class SummonAI : AIBase
    {
        void Awake() {
            attribute = GetComponent<NpcAttribute> ();
            ai = new MonsterCharacter ();
            ai.attribute = attribute;
            ai.AddState (new SummonIdle ());
            ai.AddState (new SummonCombat ());
            ai.AddState (new MonsterDead ());
        }
        void Start() {
            ai.ChangeState (AIStateEnum.IDLE);
        }
    }
}
