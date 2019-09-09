using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class MotionCamera : MonoBehaviour
    {
        public static MotionCamera Instance;

        private RenderTexture accumTex;
        private Material material;
        private Shader test;
        void Awake() {
            Instance = this;

            GetComponent<Camera>().enabled = false;
            accumTex = GraphInit.Instance.accumTexture;
            var sh = Shader.Find("Custom/MotionBlur");
            material = new Material(sh);
            test = Shader.Find("Custom/TestShader");
            gameObject.SetActive(false);
        }

        private NpcAttribute nat;
        void Update() {
            if(nat != null) {
                nat.SetMotionLayer(); 
                GetComponent<Camera>().RenderWithShader(test, null);
                nat.SetNormalLayer();
            }
        }


        private bool first = false;
        void OnEnable() {
            first = true;

            var player = ObjectManager.objectManager.GetMyPlayer();
            if (player != null)
            {
                nat = player.GetComponent<NpcAttribute>();
            }
        }

        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if(first) {
                Graphics.Blit(src, accumTex);
                first = false;
            }

            material.SetTexture("_MainTex", accumTex);
            material.SetFloat("_AccumOrig", 1.0f-GraphInit.Instance.blurAmount);
            accumTex.MarkRestoreExpected();
            Graphics.Blit(src, accumTex, material);
            //Graphics.Blit(accumTex, dest);
        }

        void OnDisable()
        {
            var player = ObjectManager.objectManager.GetMyPlayer();
            if (player != null)
            {
                nat = player.GetComponent<NpcAttribute>();
            }
            if(nat != null) {
                nat.SetNormalLayer();
            }
        }
    }

}