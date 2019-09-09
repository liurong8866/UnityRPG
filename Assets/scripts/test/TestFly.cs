using UnityEngine;
using System.Collections;

public class TestFly : MonoBehaviour {
	public float speed = 20;
	public float turnRate = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += transform.forward * Time.deltaTime * speed;
		transform.localRotation = Quaternion.Euler (new Vector3(0, transform.localRotation.eulerAngles.y+Random.Range(-turnRate, turnRate)*Time.deltaTime, 0));
	}
}
