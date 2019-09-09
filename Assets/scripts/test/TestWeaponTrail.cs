using UnityEngine;
using System.Collections;

public class TestWeaponTrail : MonoBehaviour {
	public bool TestLocalEnableLater = false;

	// Use this for initialization
	void Start () {
		if (!TestLocalEnableLater) {
			//加载的RibbonTrail Xffect 的prefab 需要是NotActive这样 才能计算到正确的坐标
			var par = MyLib.Util.FindChildRecursive (transform, "Point001");
			var weaponTrail = GameObject.Instantiate (Resources.Load<GameObject> ("particles/weaponTrail")) as GameObject;
			weaponTrail.transform.parent = par;
			weaponTrail.transform.localPosition = Vector3.zero;
			weaponTrail.transform.localScale = Vector3.one;
			weaponTrail.transform.localRotation = Quaternion.Euler (90, 0, 0);

			weaponTrail.GetComponent<XffectComponent> ().enabled = true;
			//weaponTrail.GetComponent<XffectComponent> ().Reset ();
			weaponTrail.SetActive(true);
		} else {
			StartCoroutine(EnableWeapon());
		}
	}
	IEnumerator EnableWeapon() {
		yield return new WaitForSeconds (0.2f);
		var weaponTrail = MyLib.Util.FindChildRecursive(transform, "weaponTrail").gameObject;
		weaponTrail.SetActive (true);
	}
	// Update is called once per frame
	void Update () {
	
	}
}
