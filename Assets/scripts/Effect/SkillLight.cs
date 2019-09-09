using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class SkillLightCamera : MonoBehaviour
    {
        void Awake()
        {
            var mc = Camera.main;
            var ca = gameObject.AddComponent<Camera>();
            ca.cullingMask = 1 << (int)GameLayer.SkillLight;
            ca.clearFlags = CameraClearFlags.SolidColor;
            ca.backgroundColor = new Color(0, 0, 0, 0);
            ca.orthographic = false;
            ca.fieldOfView = Camera.main.fieldOfView;
            ca.nearClipPlane = mc.nearClipPlane;
            ca.farClipPlane = mc.farClipPlane;
            ca.targetTexture = GraphInit.Instance.skillLight;
            ca.transform.parent = mc.transform;
            Util.InitGameObject(gameObject);
        }
    }

    /// <summary>
    /// 合并场景中的SkillLight 到PostEffect中 
    /// </summary>
    public class SkillLight : MonoBehaviour
    {
        Texture skillLight;
        Material skillMat;

        void Awake()
        {
            var g = new GameObject("SkillLightCamera");
            g.AddComponent<SkillLightCamera>();


        }

        void Start()
        {
            skillLight = GraphInit.Instance.skillLight;
            var sh = Shader.Find("Custom/skillBlendShader");
            skillMat = new Material(sh);
        }

        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            skillMat.SetTexture("_MainTex", src);
            skillMat.SetTexture("_SkillTex", skillLight);
            Graphics.Blit(src, dest, skillMat);
        }

    }

}