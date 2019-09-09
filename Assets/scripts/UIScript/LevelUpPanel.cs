
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/

/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;
using StringInject;

namespace MyLib
{
	public class LevelUpPanel : IUserInterface
	{
		GameObject levelUp;
		public UILabel whatLevel;
		void Awake ()
		{
			Init ();
		}

		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void Update ()
		{
		}

		IEnumerator WaitRemove() {
			yield return new WaitForSeconds (0.1f);
			levelUp.SetActive (true);
			yield return new WaitForSeconds (3);
			levelUp.SetActive (false);

            OnlyHide();
		}

		public void Init() {
			levelUp = GetName ("levelUp");
			levelUp.SetActive (false);
			whatLevel = GetLabel("whatLevel");
		}

		public void ShowLevelUp(int level) {
			var ht = new Hashtable ();
			ht.Add ("Level", level);
			whatLevel.text = Util.GetString ("levelUp").Inject (ht);
			StartCoroutine (WaitRemove());
		}



	}

}