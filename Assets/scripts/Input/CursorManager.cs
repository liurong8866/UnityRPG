using UnityEngine;
using System.Collections;

namespace MyLib
{
    /// <summary>
    ///键盘输入监听器 
    /// </summary>
	public class CursorManager : MonoBehaviour
	{
		static CursorManager _cursor;
		public static CursorManager cursorManager{
			get {
				if(_cursor == null) {
					var g = new GameObject();
					_cursor = g.AddComponent<CursorManager>();
					DontDestroyOnLoad(g);
					g.name = "CursorManager";
				}
				return _cursor;
			}
		}

		// Use this for initialization
		void Start ()
		{
			
		}
	
		// Update is called once per frame
		void Update ()
		{
            if(Input.GetKeyDown("f10")) {
                WindowMng.windowMng.PushView("UI/GMCmd");
            }

			if (Input.touchCount > 0) {
				Log.GUI(" Capture Touch Event ");
			}
			if (Input.GetMouseButtonDown (0)) {
				Log.GUI(" Capture Mouse Event ");
				Log.GUI("Mouse Pos "+Input.mousePosition);

				Log.GUI( "Screen "+Screen.width+" "+Screen.height);
			}
			float v = Input.GetAxisRaw ("Vertical");
			float h = Input.GetAxisRaw ("Horizontal");
			if (Mathf.Abs (v) < 0.1f && Mathf.Abs (h) < 0.1f && VirtualJoystickRegion.VJR != null) {
				Vector2 vec = VirtualJoystickRegion.VJRnormals;
				h = vec.x;
				v = vec.y;
			}

			if (ObjectManager.objectManager != null) {
				var evt = new MyEvent (MyEvent.EventType.MovePlayer);
				evt.localID = ObjectManager.objectManager.GetMyLocalId ();
				evt.vec2 = new Vector2 (h, v);
				//MyEventSystem.myEventSystem.PushEvent (evt);
				MyEventSystem.myEventSystem.PushLocalEvent(evt.localID, evt);
			}
		}
	}
}

