using UnityEngine;
using System.Collections;

namespace MyLib
{
    /// <summary>
    ///主镜头后期阴影混合特效 
    /// </summary>
    public class ShadowBlend : MonoBehaviour
    {
        void Awake()
        {
            GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;

        }
        [ButtonCallFunc()]public bool SetMatrix;
        public void SetMatrixMethod() {
            Update();
        }
        void Update() {
            var mat = GetComponent<Camera>().projectionMatrix * GetComponent<Camera>().worldToCameraMatrix ;
            Shader.SetGlobalMatrix("_MainCameraWorldToProj", mat);
        }

        //光源照射的玩家阴影的场景深度
        private RenderTexture shadowMap;
        private Material shadowBlendMat;

        void Start()
        {
            shadowMap = GraphInit.Instance.shadowMap;
            var sh = Shader.Find("Custom/shadowBlend");
            shadowBlendMat = new Material(sh);
        }

        /*
        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            shadowBlendMat.SetTexture("_MainTex", src);
            shadowBlendMat.SetTexture("_ShadowMap", shadowMap);
            Graphics.Blit(src, dest, shadowBlendMat);
        }
        */

    }
}