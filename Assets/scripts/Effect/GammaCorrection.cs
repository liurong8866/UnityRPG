using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class GammaCorrection : MonoBehaviour
    {
        private Material material;

        void Awake()
        {
            var sh = Shader.Find("Custom/GammaCorrect");
            material = new Material(sh);
        }


        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            material.SetTexture("_MainTex", src);
            Graphics.Blit(src, dest, material);
        }

    }

}