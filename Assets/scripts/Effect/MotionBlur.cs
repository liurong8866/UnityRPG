using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class MotionBlur : MonoBehaviour
    {
        private RenderTexture accumTex;
        private Material material;
        private Camera clearCamera;

        private Material material2;
        void Awake()
        {
            accumTex = GraphInit.Instance.accumTexture;
            var sh = Shader.Find("Custom/MotionBlur");
            material = new Material(sh);
            var sh2 = Shader.Find("Custom/NormalImg");
            material2 = new Material(sh2);

            /*
            var go = new GameObject("MotionClearCamera");
            clearCamera = go.AddComponent<Camera>();
            go.transform.parent = transform;
            clearCamera.clearFlags= CameraClearFlags.SolidColor;
            clearCamera.backgroundColor = new Color(0, 0, 0, 0);
            clearCamera.enabled = false;
            clearCamera.targetTexture = accumTex;
            clearCamera.cullingMask = 0;
            */
        }

        private bool first = false;
        void OnEnable() {
            //clearCamera.Render();
            first = true;
        }

        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if(first) {
                first = false;
                Graphics.Blit(src, accumTex);
            }

            material.SetTexture("_MainTex", accumTex);
            material.SetFloat("_AccumOrig", 1.0f-GraphInit.Instance.blurAmount);
            accumTex.MarkRestoreExpected();
            Graphics.Blit(src, accumTex, material);
            Graphics.Blit(src, accumTex, material2);
            Graphics.Blit(accumTex, dest);
        }
    }

}