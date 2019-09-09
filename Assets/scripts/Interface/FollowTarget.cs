using UnityEngine;
using System.Collections;

namespace MyLib
{
    /// <summary>
    /// blood And Harm Number 
    /// </summary>
	public class FollowTarget : MonoBehaviour
	{
		public GameObject target;
		Transform mTrans;
		Camera gameCamera;
		Camera uiCamera;

		void Awake() {
			mTrans = transform;
		}
		// Use this for initialization
		void Start ()
		{
			gameCamera = NGUITools.FindCameraForLayer (target.gameObject.layer);
			uiCamera = NGUITools.FindCameraForLayer (gameObject.layer);
			var pos = gameCamera.WorldToViewportPoint (target.transform.position);
			transform.position = uiCamera.ViewportToWorldPoint (pos);

			pos = mTrans.localPosition;
			pos.x = Mathf.FloorToInt(pos.x);
			pos.y = Mathf.FloorToInt(pos.y);
			pos.z = 0f;
			mTrans.localPosition = pos;
		}
		void Update() {
            if(target == null){
                return;
            }
			var offsetPos = target.transform.position + Vector3.up;
			var pos = gameCamera.WorldToViewportPoint (offsetPos);
			transform.position = uiCamera.ViewportToWorldPoint (pos);
			pos = mTrans.localPosition;
			pos.x = Mathf.FloorToInt(pos.x);
			pos.y = Mathf.FloorToInt(pos.y);
			pos.z = 0f;
			mTrans.localPosition = pos;
		}
	}
}
