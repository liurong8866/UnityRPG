
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
using System.Collections.Generic;


[System.Serializable]
public class DataPair {
	public int x;
	public float y;
}
public class GraphData : MonoBehaviour {
	public int InitData = 0;
	public int AddPerLevel = 0;
	[ButtonCallFunc()]
	public bool ResetData;
	public void ResetDataMethod() {
		int c = 1;
		foreach (DataPair d in data) {
			d.x = c;
			d.y = InitData+(c-1)*AddPerLevel;
			c++;
		}
	}

	public List<DataPair> data;
	// Use this for initialization
	void Start () {
	
	}

	public float GetData(int key) {
		for(int i = 0; i < data.Count; i++) {
			if(data[i].x == key)
				return data[i].y;
		}

		Debug.LogError ("not read Data for level "+this+" "+key);
		return -1;
	}
	// Update is called once per frame
	void Update () {
	
	}
}
