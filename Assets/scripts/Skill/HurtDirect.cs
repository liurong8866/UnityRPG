using UnityEngine;
using System.Collections;

namespace MyLib
{
	/// <summary>
	/// 直接确定攻击目标为伤害对象  怪物的近战攻击
	/// </summary>
	public class HurtDirect : MonoBehaviour
	{
		// Use this for initialization
		void Start ()
		{
			var runner = transform.parent.GetComponent<SkillLayoutRunner> ();
			runner.DoDamage (runner.stateMachine.target);
			
			Log.AI ("HurtDirect "+runner.stateMachine.target.name);
		}
	
	}
}
