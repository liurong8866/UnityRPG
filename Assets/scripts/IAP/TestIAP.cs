using UnityEngine;
using System.Collections;
using SimpleJSON ;

public class TestIAP : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	void OnGUI() {
		if(GUI.Button(new Rect(200, 200, 100, 100), "GetList")) {
			//Debug.LogError("itemList "+MyStoreAsset.Instance.allItems.Count);
			var proj = new SimpleJSON.JSONArray();
			var n = new JSONData("item6");
			proj.Add(n);
			n = new JSONData("item18");
			proj.Add(n);
			n = new JSONData("item60");
			proj.Add(n);
			var str = proj.ToString();
			Debug.LogError(str);
			SimpleIAP.GetInstance().LoadProducst(str);
		}
		if(GUI.Button(new Rect(100, 100, 100, 100), "BuyItem")) {
			//Soomla.Store.SoomlaStore.BuyMarketItem("item8", "buy item8");
			SimpleIAP.GetInstance().ChargeItem("item6");
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
