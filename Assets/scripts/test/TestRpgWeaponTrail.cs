using UnityEngine;
using System.Collections;

public class TestRpgWeaponTrail : MonoBehaviour {
	AnimationController ani;
	GameObject trail;
	WeaponTrail wt;
	// Use this for initialization
	void Start () {
		ani = GetComponent<AnimationController> ();

		trail = Instantiate(Resources.Load<GameObject>("particles/newWeaponTrail")) as GameObject;
		var rightHand = MyLib.Util.FindChildRecursive(transform, "Point001");
		if(rightHand != null) {
			trail.transform.parent = rightHand;
			trail.transform.localPosition = Vector3.zero;
			trail.transform.localScale = Vector3.one;
			//X Rotate 90 For New Game Model
			//模型的挂点的z轴向上 因此需要调整 weaponTrail 的Y轴和模型z轴一致
			trail.transform.localRotation = Quaternion.Euler (90, 0, 0);
		}
		//GetComponent<AnimationController>().AddTrail(trail.GetComponent<WeaponTrail>());

		wt = trail.GetComponent<WeaponTrail>();
		wt.SetTime(0, 0, 1);
		trail.SetActive (true);
		ani.AddTrail (wt);

		StartCoroutine (PlayNow());
	}

	IEnumerator PlayNow() {
		yield return null;
		ani.PlayAnimation (GetComponent<Animation>()["idle"]);
		//.CrossfadeAnimation (animation["rslash_1"], 0.2f);
		yield return new WaitForSeconds(2);
		ani.CrossfadeAnimation (GetComponent<Animation>()["rslash_1"], 0.2f);
		wt.SetTime (1, 0, 1);

		yield return new WaitForSeconds (2);
		wt.SetTime (0.2f, 0, 1);
		//trail.SetActive (false);

		//yield return new WaitForSeconds (2);
		//wt.SetTime (1, 0, 1);

		//trail.SetActive (true);
		while (true) {
			wt.SetTime(1, 0, 2);
			yield return new WaitForSeconds(0.8f);
			wt.SetTime(0.2f, 0, 2);
			yield return new WaitForSeconds(0.2f);
		}
	}
	// Update is called once per frame
	void Update () {
	
	}
}
