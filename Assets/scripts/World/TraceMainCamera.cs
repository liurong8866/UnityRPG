
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

/*
 * LightMap Camera Trace main Camera 
 * And Set All Material Camera Offset Value 
 */ 
namespace MyLib
{
	public class TraceMainCamera : MonoBehaviour
	{
		public static TraceMainCamera traceMainCamera;
		void Awake() {
			traceMainCamera = this;
			DontDestroyOnLoad (gameObject);
		}
		/*
	 * Floor Shader Camera Pos should be same with this value
	 */ 
		public Vector3 Offset;
		public List<Material> Materials;
		// Use this for initialization
		void Start ()
		{
			Transform lm = transform.Find ("lightMask");
			float sz = GetComponent<Camera>().orthographicSize / 5;
			lm.localScale = new Vector3 (sz, 1, sz);

			//Adjust All Material's CameraPos according to Offset
			foreach (var m in Materials) {
				if(m != null) {
					m.SetVector ("_CamPos", new Vector4 (Offset.x, Offset.y, Offset.z, 0));
					m.SetFloat ("_CameraSize", GetComponent<Camera>().orthographicSize);
				}
			}
		}
	
		// Update is called once per frame
		void Update ()
		{
			transform.position = Camera.main.transform.position + Offset;
		}
	}
}
