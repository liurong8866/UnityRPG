using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class LightSourceCamera : MonoBehaviour
    {
        void Awake() {
            this.GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
        }
       
    }
}
