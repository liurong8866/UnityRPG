using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class MotionMainCamera : MonoBehaviour
    {
        Texture accumTexture;
        Material motionBlend;
        void Awake() {
        }
        void Start() {
            accumTexture = GraphInit.Instance.accumTexture;
            var sh = Shader.Find("Custom/motionBlendShader");
            motionBlend = new Material(sh);
        }

        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            motionBlend.SetTexture("_MainTex", src);
            motionBlend.SetTexture("_MotionTex", accumTexture);
            Graphics.Blit(src, dest, motionBlend);
        }

	
    }
}
