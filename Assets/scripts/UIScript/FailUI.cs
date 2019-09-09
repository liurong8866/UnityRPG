
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;

namespace MyLib
{
	public class FailUI : IUserInterface
	{
		public GameObject failBut;
		public bool quit = false;

		void Awake ()
		{
			quit = false;
			failBut = GetName("failBut");
			SetCallback ("failBut", OnQuit);
		}

		void OnQuit (GameObject g)
		{
			quit = true;
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
