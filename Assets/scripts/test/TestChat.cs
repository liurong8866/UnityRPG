using UnityEngine;
using System.Collections;

namespace MyLib
{
	public class TestChat : MonoBehaviour
	{

		// Use this for initialization
		IEnumerator Start ()
		{
			if (SaveGame.saveGame == null) {
				var g = new GameObject();
				var s = g.AddComponent<SaveGame>();
				s.InitData();
			}

			yield return new WaitForSeconds (1);
			Util.CreateUI ();

			WindowMng.windowMng.PushView ("UI/Chat");
		}


	
		// Update is called once per frame
		void Update ()
		{
	
		}
	}

}