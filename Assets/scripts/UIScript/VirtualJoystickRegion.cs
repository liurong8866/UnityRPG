
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/

/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
/// <summary>
/// Coded by Bruno Xavier L'.
/// If you use this, stop by and say thanks: http://forum.unity3d.com/threads/116076-VJR-(Virtual-Joystick-Region)-Sample?p=773620#post773620
/// </summary>
using UnityEngine;
namespace MyLib
{
	public class VirtualJoystickRegion : IUserInterface
	{
		public static Vector2 VJRvector;	// Joystick's controls in Screen-Space.
		static Vector2 _vjrNormals;
		public static Vector2 VJRnormals{
			get {
				return _vjrNormals;
			}
		}// Joystick's normalized controls.
		public static bool VJRdoubleTap;	// Player double tapped this Joystick.
		public Color activeColor;			// Joystick's color when active.
		public Color inactiveColor;			// Joystick's color when inactive.

		public Texture2D joystick2D;		// Joystick's Image.
		public Texture2D background2D;		// Background's Image.
		private GameObject backOBJ;			// Background's Object.
		private GUITexture joystick;		// Joystick's GUI.
		private GUITexture background;		// Background's GUI.
		private Vector2 origin;				// Touch's Origin in ScreBen-Space.
		private Vector2 position;			// Pixel Position in Screen-Space.
		private int size;					// Screen's smaller side.
		private float length;				// The maximum distance the Joystick can be pushed.
		private bool gotPosition;			// Joystick has a position.
		private int fingerID;				// ID of finger touching this Joystick.
		private int lastID;					// ID of last finger touching this Joystick.
		private float tapTimer;				// Double-tap's timer.
		private bool enable;				// VJR external control.

		public float JoyWidth = 350;
		public float JoyHeight = 350;

		//
	
		public void DisableJoystick ()
		{
			enable = false;
			ResetJoystick ();
		}

		public void EnableJoystick ()
		{
			enable = true;
			ResetJoystick ();
		}
	
		//
		public static VirtualJoystickRegion VJR;
		Vector2 initPos;
		//bool touchYet = false;
	
        private float lastResetTime = 0;

		private void ResetJoystick ()
		{
            useMouse = false;
			VJRvector = new Vector2 (0, 0);
			_vjrNormals = VJRvector;
			lastID = fingerID;
			fingerID = -1;
			tapTimer = 0.150f;
			joystick.color = inactiveColor;
			position = origin;
			gotPosition = false;
            ResetOrigin();
		}

        private void ResetOrigin(){
            position = new Vector2 ((Screen.width / 3) / 2, (Screen.height / 3) / 2);
            origin = position;
        }
	
        /// <summary>
        ///限制虚拟摇杆的位置和中心的位置差 
        /// </summary>
        /// <returns>The radius.</returns>
        /// <param name="midPoint">Middle point.</param>
        /// <param name="endPoint">End point.</param>
        /// <param name="maxDistance">Max distance.</param>
		private Vector2 GetRadius (Vector2 midPoint, Vector2 endPoint, float maxDistance)
		{
			Vector2 distance = endPoint;
			if (Vector2.Distance (midPoint, endPoint) > maxDistance) {
				distance = endPoint - midPoint;
				distance.Normalize ();
				return (distance * maxDistance) + midPoint;
			}
			return distance;
		}
	
        bool useMouse = false;
        /// <summary>
        /// Get Origin Operation Position 
        /// </summary>
		private void GetPosition ()
		{
            useMouse = false;
            Vector3 mousePos = Input.mousePosition;
            //Mouse 和手机上的Finger冲突了
#if UNITY_EDITOR
            if(Input.GetMouseButtonDown(0)) {
                if(mousePos.x < Screen.width/3 && mousePos.y < Screen.height/3) {
                    useMouse = true;
                    position = mousePos;
                    origin = position;
                    joystick.texture = joystick2D; 
                    joystick.color = activeColor;
                    background.texture = background2D;
                    background.color = activeColor;
                    gotPosition = true;
                    return;
                }
            }
#endif

			foreach (Touch touch in Input.touches) {
				fingerID = touch.fingerId;
				if (fingerID >= 0 && fingerID < Input.touchCount) {
					if (Input.GetTouch (fingerID).position.x < Screen.width / 3 && Input.GetTouch (fingerID).position.y < Screen.height / 3 && Input.GetTouch (fingerID).phase == TouchPhase.Began) {
						position = Input.GetTouch (fingerID).position;
						origin = position;
						joystick.texture = joystick2D; 
						joystick.color = activeColor;

						background.texture = background2D;
						background.color = activeColor;
						if (fingerID == lastID && tapTimer > 0) {
							VJRdoubleTap = true;
						}
						gotPosition = true;
                        break;
					}
				}
			}
		}
	
        /// <summary>
        /// Limit Origin In ProperPosition 
        /// </summary>
		private void GetConstraints ()
		{
			if (origin.x < (background.pixelInset.width / 2) + 25) {
				origin.x = (background.pixelInset.width / 2) + 25;
			}
			if (origin.y < (background.pixelInset.height / 2) + 25) {
				origin.y = (background.pixelInset.height / 2) + 25;
			}
			if (origin.x > Screen.width / 3) {
				origin.x = Screen.width / 3;
			}
			if (origin.y > Screen.height / 3) {
				origin.y = Screen.height / 3;
			}
		}
	
        /// <summary>
        /// 0.8 操控满足0.8f就可以最大速度了 
        /// </summary>
        /// <returns>The controls.</returns>
        /// <param name="pos">Position.</param>
        /// <param name="ori">Ori.</param>
		private Vector2 GetControls (Vector2 pos, Vector2 ori)
		{
			Vector2 vector = new Vector2 ();
			if (Vector2.Distance (pos, ori) > 0) {
				vector = new Vector2 ((pos.x - ori.x)*3f, (pos.y - ori.y)*3f);
			}
			return vector;
		}
	
		//
		void Destroy() {
			VJR = null;
		}
		Vector2 GetRealSize() {
			var rate = Mathf.Min(Screen.width/1024.0f, Screen.height/768.0f);
			Vector2 v = new Vector2(JoyWidth, JoyHeight)*rate;
			return v;
		}
		private void Awake ()
		{
			Log.GUI ("Init VJR Awake");
			VJR = this;

			gameObject.transform.localScale = new Vector3 (0, 0, 0);
			gameObject.transform.position = new Vector3 (0, 0, 999);
			if (Screen.width > Screen.height) {
				size = Screen.height;
			} else {
				size = Screen.width;
			}
			VJRvector = new Vector2 (0, 0);
			joystick = gameObject.AddComponent <GUITexture>() as GUITexture;
			joystick.texture = joystick2D;
			joystick.color = inactiveColor;

			backOBJ = new GameObject ("VJR-Joystick Back");
			backOBJ.transform.localScale = new Vector3 (0, 0, 0);
			background = backOBJ.AddComponent <GUITexture>() as GUITexture;
			background.texture = background2D;
			background.color = inactiveColor;
			var rsz = GetRealSize();
			var pxin = new Rect (0, 0,  (int)rsz.x, (int)rsz.y);
			background.pixelInset = pxin;

			fingerID = -1;
			lastID = -1;
			VJRdoubleTap = false;
			tapTimer = 0;
            var rate = Mathf.Min(Screen.width/1024.0f, Screen.height/768.0f);
			length = 80 * rate;
			position = new Vector2 ((Screen.width / 3) / 2, (Screen.height / 3) / 2);
			origin = position;
			gotPosition = false;
			EnableJoystick ();
			enable = true;
		}

        void HandleMouse() {
            if(gotPosition == true) {
                if(Input.GetMouseButton(0)) {
                    position = Input.mousePosition;
                    position = GetRadius (origin, position, length);
                    VJRvector = GetControls (position, origin);
                    _vjrNormals = new Vector2 (VJRvector.x / length, VJRvector.y / length);
                    if(_vjrNormals.sqrMagnitude >= 1) {
                        _vjrNormals.Normalize();
                    }
                }
            }

            if(!gotPosition) {
                if (background.color != inactiveColor) {
                    background.color = inactiveColor;
                }
            }
            var rsz = GetRealSize();
            background.pixelInset = new Rect (origin.x - (rsz.x / 2), origin.y - (rsz.y / 2), rsz.x, rsz.y);
            joystick.pixelInset = new Rect (position.x - (joystick.pixelInset.width / 2), position.y - (joystick.pixelInset.height / 2), size / 11, size / 11);
        }
        void HandleTouch() {
            if(gotPosition == true) {
                foreach (Touch touch in Input.touches) {
                    if (touch.fingerId == fingerID) {
                        position = touch.position;
                        position = GetRadius (origin, position, length);
                        VJRvector = GetControls (position, origin);
                        _vjrNormals = new Vector2 (VJRvector.x / length, VJRvector.y / length);
                        if(_vjrNormals.sqrMagnitude >= 1) {
                            _vjrNormals.Normalize();
                        }
                        if (Input.GetTouch (fingerID).position.x > (Screen.width / 3) + background.pixelInset.width
                            || Input.GetTouch (fingerID).position.y > (Screen.height / 3) + background.pixelInset.height) {
                            ResetJoystick ();
                        }
                        break;
                    }
                }

            }

            if(gotPosition) {
                if (Input.GetTouch (fingerID).phase == TouchPhase.Ended || Input.GetTouch (fingerID).phase == TouchPhase.Canceled) {
                    ResetJoystick ();
                }
            }

            if (gotPosition == false) {
                if (background.color != inactiveColor) {
                    background.color = inactiveColor;
                }
            }

            var rsz = GetRealSize();
            background.pixelInset = new Rect (origin.x - (rsz.x / 2), origin.y - (rsz.y / 2), rsz.x, rsz.y);
            joystick.pixelInset = new Rect (position.x - (joystick.pixelInset.width / 2), position.y - (joystick.pixelInset.height / 2), size / 11, size / 11);
        }

		private void Update ()
		{
			if (tapTimer > 0) {
				tapTimer -= Time.deltaTime;
			}

            if(useMouse && !Input.GetMouseButton(0)) {
                ResetJoystick();
            }
            if(!useMouse) {
                if (fingerID > -1 && fingerID >= Input.touchCount) {
                    ResetJoystick ();
                }
            }

            if(enable) {
                if(gotPosition == false) {
                    GetPosition ();
                    GetConstraints ();
                }

                if(useMouse) {
                    HandleMouse();
                }else {
                    HandleTouch();
                }

            }else {
                background.pixelInset = new Rect (0, 0, 0, 0);
                joystick.pixelInset = new Rect (0, 0, 0, 0);
            }

		}
	}
}