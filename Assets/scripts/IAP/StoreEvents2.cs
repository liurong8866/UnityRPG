using UnityEngine;
using System.Collections;
using SimpleJSON;
using MyLib;

public class StoreEvents2 : MonoBehaviour {
    void Awake() {
        DontDestroyOnLoad(gameObject);
    }

	public void OnEvent(string evt) {
		Debug.LogError("Events "+evt);
        if(evt == "EVENT_BUY_SUCCESS") {
            GameInterface_Backpack.inTransaction = false;
            Util.ShowMsg("充值成功，"+GameInterface_Backpack.lastCharge.num);
            PlayerData.AddJingShi(GameInterface_Backpack.lastCharge.num);
        }else if(evt == "EVENT_BUY_FAIL") {
            GameInterface_Backpack.inTransaction = false;
            Util.ShowMsg("充值失败，"+GameInterface_Backpack.lastCharge.num);
        }else {
            Util.ShowMsg("获取产品列表");
        }
	}

    void Start() {
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

}
