using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class HeatHaze : MonoBehaviour
    {
        private Material material;
        void Awake()
        {
            var sh = Shader.Find("Custom/HeatHaze");
            material = new Material(sh);
        }

        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            material.SetTexture("_MainTex", src);
            material.SetTexture("_HeatTex", GraphInit.Instance.heatTex);
            material.SetFloat("_DistortFactor", GraphInit.Instance.DistortFactor);
            material.SetFloat("_RiseFactor", GraphInit.Instance.RiseFactor);
            material.SetFloat("_Radius", GraphInit.Instance._Radius);
            material.SetFloat("_ClipArg", GraphInit.Instance._ClipArg);
            Graphics.Blit(src, dest, material);
        }
    }

}