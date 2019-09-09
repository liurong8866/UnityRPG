using UnityEngine;
using System.Collections;

/*
 * UIScript Access Data By Script Global Variable
 * 
 * My Player Or Other Player 
 * 
 */ 
namespace MyLib
{
	public class DebugPanel : MonoBehaviour
	{
		//public UILabel label;
		public GameObject close;
		public UITextList textList;
		/*
		 * Data Source Pool
		 */ 
		public GameObject Player = null;
		/*
		 * Register Some Event
		 */ 
		void Awake ()
		{
			UIEventListener.Get (close).onClick = OnClose;
		}

		void OnClose (GameObject g)
		{
			gameObject.SetActive (false);
		}

		void OnEnable() {
			
		}

		void OnDisable() {
		
		}

		// Use this for initialization
		void Start ()
		{
			
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}

		public void SetText(string st) {
			//label.text = st;
			textList.Add (st);
		}
	}

}