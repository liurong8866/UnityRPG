using UnityEngine;
using System.Collections;

namespace MyLib
{
	public class RemoveParticle : MonoBehaviour
	{
		//public float time = 5;
		// Use this for initialization
		void Start ()
		{
			//GameObject.Destroy (gameObject, time);
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}
		void OnDisable() {
			GameObject.Destroy (gameObject);
		}
	}

}