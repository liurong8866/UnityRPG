using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class LightMapInit : MonoBehaviour
    {
        public Texture2D unityLightMap;
        public float lightMapScale;

        public static LightMapInit Instance;

        void Awake()
        {
            Instance = this;
        }
        void Start() {
            InitLightMapMethod();
        }

        [ButtonCallFunc()] public bool InitLightMap;

        public void InitLightMapMethod()
        {
            //Shader.SetGlobalTexture("_UnityLightMap", unityLightMap);
           // Shader.SetGlobalFloat("_LightMapScale", lightMapScale);
            var lm = new LightmapData();
            lm.lightmapFar = unityLightMap;
            LightmapSettings.lightmaps = new LightmapData[]{
                lm
            };
        }
	
    }

}