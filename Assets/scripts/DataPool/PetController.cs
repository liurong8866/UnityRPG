
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;

namespace MyLib
{
	public class PetController : MonoBehaviour
	{
		public static PetController petController;
		void Awake() {
			petController = this;
			DontDestroyOnLoad (gameObject);
		}
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
