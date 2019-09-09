using UnityEngine;
using System.Collections;

namespace MyLib
{
	public class PlayerWindow : MonoBehaviour
	{
		public static PlayerWindow playerWindow;
		void Awake() {
			playerWindow = this;
			DontDestroyOnLoad (gameObject);
		}
        void OnDisable() {
            Log.Sys("WhoClosePlayerWindow");
        }
        void OnEnable() {
            Log.Sys("WhoOpenPlayerWindow");
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