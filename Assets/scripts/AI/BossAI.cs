
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/

/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;

namespace MyLib
{
	public class BossAI : AIBase 
	{
        void Awake() {
            attribute = GetComponent<NpcAttribute> ();
            ai = new BossCharacter ();
            ai.attribute = attribute;
            ai.AddState (new BossIdle ());
            ai.AddState (new MonsterCombat ());
            ai.AddState (new BossDead ());
            ai.AddState (new MonsterFlee ());
            ai.AddState (new MonsterKnockBack ());
        }
        void Start() {
            ai.ChangeState (AIStateEnum.IDLE);
        }
	}

}