using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
public class TestPhysic : MonoBehaviour {
    public float mul = 2;
    [ButtonCallFunc()]public bool ChangeP;
    public void ChangePMethod() {
        MyLib.TankPhysicComponent.Multi = mul;
    }

    public float changeS2 = 2;
    [ButtonCallFunc()]public bool ChangeS;
    public void ChangeSMethod() {
        MyLib.TowerAutoCheck.maxRotateChange = changeS2;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

}

#endif