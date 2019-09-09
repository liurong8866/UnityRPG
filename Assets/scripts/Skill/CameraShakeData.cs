using UnityEngine;
using System.Collections;

namespace MyLib
{
	public class CameraShakeData : MonoBehaviour
	{
		public AnimationCurve shakeCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}
	}
}
