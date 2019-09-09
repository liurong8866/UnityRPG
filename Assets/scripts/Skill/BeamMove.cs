using UnityEngine;
using System.Collections;

namespace MyLib
{
	public class BeamMove : MonoBehaviour
	{
		//世界坐标
		public float targetPos;
		public float speed = 1;
		Vector3 vel;
		float time = 0;
		// Use this for initialization
		void Start ()
		{
			Vector3 realTarget = transform.position + targetPos * (transform.rotation*Vector3.forward);
			var diff = realTarget - transform.position;
			vel = diff.normalized*speed;
			time = diff.magnitude / vel.magnitude;
			StartCoroutine (MoveBeam());
		}
		IEnumerator MoveBeam() {
			float passTime = 0;
			while (passTime < time) {
				transform.position += vel * Time.deltaTime;
				passTime += Time.deltaTime;
				yield return null;
			}
		}
		// Update is called once per frame
		void Update ()
		{
		}
	}

}
