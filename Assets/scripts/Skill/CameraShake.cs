using UnityEngine;
using System.Collections;
namespace MyLib {
	public class CameraShake : MonoBehaviour {
		public CameraShakeData shakeData;
		public float duration = 0.2f;
		public Vector3 direction = Vector3.right;
		public float magnitude = 0.4f;

		bool isActive = true;
		bool startYet = false;

		public bool autoRemove = false;
		void Update() {
			if (isActive) {
				if(!startYet) {
					startYet = true;
					StartCoroutine(ShakeCamera());
				}
			}
		}

		IEnumerator ShakeCamera() {
			Log.Sys ("CameraShakeNow ");
			if (CameraController.cameraController.CheckCanShake ()) {
				var startEvt = new MyEvent (MyEvent.EventType.ShakeCameraStart);
				startEvt.ShakeDir = direction;
				MyEventSystem.myEventSystem.PushEvent (startEvt);

				float passTime = 0;
				while (passTime < duration) {
					var rate = passTime / duration;
					var value = shakeData.shakeCurve.Evaluate (rate);
					var evt = new MyEvent (MyEvent.EventType.ShakeCamera);
					evt.floatArg = value * magnitude;
					MyEventSystem.myEventSystem.PushEvent (evt);
					passTime += Time.deltaTime;
					yield return null;
				}
				MyEventSystem.myEventSystem.PushEvent (MyEvent.EventType.ShakeCameraEnd);
			}
			Log.Sys ("Shake Over");
			if (autoRemove) {
				GameObject.Destroy(gameObject);
			}
		}
	}
}
