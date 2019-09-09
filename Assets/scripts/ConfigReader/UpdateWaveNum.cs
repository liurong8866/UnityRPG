
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;

/*
 * Set Level Monster SpawnTrigger WaveNum To it's Name 's WaveNum
 * 
 * For Example : 
 * 		SpawnTrigger name is wave0
 * 		wavenum = 0
 */ 
namespace MyLib
{
	public class UpdateWaveNum : MonoBehaviour
	{
		[ButtonCallFunc()]
		public bool
			resetWave;

		public void resetWaveMethod ()
		{
			foreach (Transform t in transform) {
				var sp = t.GetComponent<SpawnTrigger> ();
				if (sp != null) {
					sp.UpdateEditor ();
				}
			}
		}
		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}
	}
}
