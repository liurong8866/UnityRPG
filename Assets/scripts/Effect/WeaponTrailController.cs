using UnityEngine;
using System.Collections;

namespace MyLib
{
	public class WeaponTrailController : MonoBehaviour
	{
		float t = 0.033f;
		//float animationIncrement = 0.003f;

		protected virtual void LateUpdate() {
			RunAnimations ();
		}
		void RunAnimations() {
			if(t > 0) {
			}
		}
	}
}
