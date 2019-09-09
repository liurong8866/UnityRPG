
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

public class ShadowComponent : MonoBehaviour {
	GameObject shadowPlane;
	/*
	 * Npc Attribute Create ShadowPlane for Monster or Player
	 */ 
	void CreateShadowPlane() {
		if (shadowPlane == null) {
			GameObject p = GameObject.CreatePrimitive (PrimitiveType.Plane);
            DestroyImmediate (p.GetComponent<Collider>());
			shadowPlane = p;
			p.name = "shadowPlane";
			p.transform.parent = transform;
			p.transform.localScale = Vector3.one;
			p.transform.localRotation = Quaternion.identity;
			p.GetComponent<Renderer>().enabled = false;
			p.transform.localPosition = Vector3.zero;
		
			foreach (Transform c in transform) {
				if (c.GetComponent<Renderer>() != null) {
					SetShadowPlane sp = NGUITools.AddMissingComponent<SetShadowPlane> (c.gameObject);
					sp.plane = p;
				}
			}
		}
	}
    private bool inLock = false;
    private float lockY = 0;
    public void LockShadowPlane() {
        inLock = true;
        lockY = shadowPlane.transform.position.y;
    }
    public void UnLockShadowPlane() {
        inLock = false;
        shadowPlane.transform.localPosition = Vector3.zero;
    }
    void Update() {
        if(inLock) {
            var oldP = shadowPlane.transform.position;
            oldP.y = lockY;
            shadowPlane.transform.position = oldP; 
        }
    }

	public void HideShadow() {
		shadowPlane.transform.localPosition = Vector3.up*-100;
	}

	void Awake() {
		CreateShadowPlane ();
	}
	/*
	 *  
	 */
	public void AdjustLightPos(Vector3 pos) {
		var me = transform.GetComponentsInChildren<MeshRenderer>();
		foreach(MeshRenderer r in me) {
			r.material.SetVector("_LightDir", transform.position-pos);
		}
	}

	/*
	 * Add Shadow Plane for new Equipment like:Armor Chest Weapon
	 */ 
	public void SetShadowPlane(GameObject g) {
		SetShadowPlane sp = NGUITools.AddMissingComponent<SetShadowPlane>(g);
		sp.plane = shadowPlane;
	}
}
