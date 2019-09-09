using UnityEngine;
using System.Collections;

namespace MyLib
{
	public class GraphInit : MonoBehaviour
	{
        public static GraphInit Instance;
        void Awake() {
            Instance = this;
        }

        public Texture lightMap;
        public Texture specMap;
        public Texture cloudNoise;

        public Vector3 ambient = Vector3.one;
        public Texture lightMask;
        public float lightCoff;
        public float specCoff;
        public float specSize;
		
        public float specFreqX;
        public float specAmpX;
        public float specFreqY;
        public float specAmpY;

        public Vector3 rimColor;
        public float rimPower;

        public AnimationCurve lightningCurve;
        public float lightningTime = 1;
        public float lightningMagnitude = 1;

        public RenderTexture skillLight;
        public float skillLightCoff = 1;

        public RenderTexture shadowMap;
        public Vector3 lightDir;

        public Color shadowColor;
        public float shinness;
        public Color specColor;

        public float noiseScale;
        public float blurAmount = 0.8f;
        public RenderTexture accumTexture;

        public RenderTexture motionCamera;

        public float gamma = 1;
        public Color lightDiffuseColor = new Color(223/255.0f, 248/255.0f, 255/255.0f, 1);

        void InitAll() {
            var lc = Resources.Load<GameObject>("LightCamera").GetComponent<Camera>();
            var lightCamera = lc.GetComponent<LightCamera>();
            var sc = Resources.Load<GameObject>("levelPublic/ShadowCamera").GetComponent<ShadowCamera>();
            //New Shader lightMapxxx need these Set
            //var lc = GameObject.FindGameObjectWithTag("LightCamera").camera;
            
            var camSize = lc.orthographicSize;
            Shader.SetGlobalTexture("_LightMap", lightMap);
            Shader.SetGlobalTexture("_SpecMap", specMap);
            Shader.SetGlobalTexture("_CloudNoise", cloudNoise);

            Shader.SetGlobalVector("_CamPos", lightCamera.CamPos);
            Shader.SetGlobalFloat("_CameraSize", camSize);
            
            Shader.SetGlobalVector("_AmbientCol", ambient);
            Shader.SetGlobalTexture("_LightMask", lightMask);
            Shader.SetGlobalFloat("_LightCoff", lightCoff);
            Shader.SetGlobalFloat("_SpecCoff", specCoff);
            Shader.SetGlobalFloat("_SpecSize", specSize );

            Shader.SetGlobalFloat("_SpecFreqX", specFreqX);
            Shader.SetGlobalFloat("_SpecFreqY", specFreqY);
            Shader.SetGlobalFloat("_SpecAmpX", specAmpX );
            Shader.SetGlobalFloat("_SpecAmpY", specAmpY );

            Shader.SetGlobalVector("_RimColor2", rimColor);
            Shader.SetGlobalFloat("_RimPower2", rimPower );
            Shader.SetGlobalFloat("_SkillLightCoff",  skillLightCoff );

            Shader.SetGlobalColor ("_OverlayColor", new Color(68/255.0f, 227/255.0f, 237/255.0f, 0.5f));
            Shader.SetGlobalColor ("_ShadowColor", shadowColor);// new Color (28/255.0f, 25/255.0f, 25/255.0f, 0.5f));
            Shader.SetGlobalVector ("_LightDir", lightDir);
            Shader.SetGlobalVector ("_HighLightDir", new Vector3 (-1, -1, -1));
            Shader.SetGlobalColor ("_LightDiffuseColor", lightDiffuseColor);
            
            Shader.SetGlobalColor ("_GhostColor", new Color(68/255.0f, 227/255.0f, 68/255.0f, 0.5f));

            Shader.SetGlobalTexture("_ShadowMap", shadowMap);
            Shader.SetGlobalVector("_ShadowCamPos", sc.CamPos);
            Shader.SetGlobalFloat("_ShadowCameraSize", sc.GetComponent<Camera>().orthographicSize);

            Shader.SetGlobalFloat("_Shinness", shinness);
            Shader.SetGlobalColor ("_SpecColor", specColor);
            Shader.SetGlobalFloat("_NoiseScale", noiseScale);
            Shader.SetGlobalFloat("_Gamma", gamma);

            var res = Screen.currentResolution;
            Log.GUI ("Screen Attribute resolution "+res.width + " "+res.height+" "+res.refreshRate);
            Log.GUI ("Screen Attribute dpi "+Screen.dpi);
            Log.GUI ("Screen Attribute height "+Screen.height);
            Log.GUI ("Screen Attribute width "+Screen.width); 
        }
        // Use this for initialization
		void Start ()
		{
            InitAll();
		}
	
        public Color testAmbient;
        [ButtonCallFunc()]
        public bool InitAmbient;
        public void InitAmbientMethod() {
            Shader.SetGlobalVector("_AmbientCol", testAmbient);
            Shader.SetGlobalFloat("_LightCoff", lightCoff);
        }

        [ButtonCallFunc()]
        public bool InitNow;
        public void InitNowMethod() {
            InitAll();
        }

        public bool IsBlind = false;
        public void SetBlind(bool b) {
            IsBlind = b;
            if(IsBlind) {
                Blind();
            }else {
                Clear();
            }
        }

        //当前Room的Light和Props关闭
        private void Blind() {
            Shader.SetGlobalVector("_AmbientCol", Vector3.zero);
            var zone = BattleManager.battleManager.Zones;
            var cz = BattleManager.battleManager.currentZone;
            if(cz >= 0 && cz < zone.Count) {
                //var z = zone[cz];
                var z = GameObject.Find("Root_"+cz);
                if(z != null) {
                    var props = z.transform.Find("Props");
                    var light = z.transform.Find("Light");
                    if(props != null && light != null) {
                        props.gameObject.SetActive(false);
                        light.gameObject.SetActive(false);
                    }
                }
            }
        }

        private void Clear() {
            Shader.SetGlobalVector("_AmbientCol", ambient);
            var zone = BattleManager.battleManager.Zones;
            var cz = BattleManager.battleManager.currentZone;
            if(cz >= 0 && cz < zone.Count) {
                var z = GameObject.Find("Root_"+cz);
                if(z != null) {
                    var props = z.transform.Find("Props");
                    var light = z.transform.Find("Light");
                    if(props != null && light != null) {
                        props.gameObject.SetActive(true);
                        light.gameObject.SetActive(true);
                    }
                }
            }
        }

        public bool useMotion = false;
        [ButtonCallFunc()]public bool UseMotionBlur;
        public void UseMotionBlurMethod() {
            /*
            if(useMotion) {
                Camera.main.GetComponent<MotionMainCamera>().enabled = true;
                MotionCamera.Instance.gameObject.SetActive(true);
            }else {
                Camera.main.GetComponent<MotionMainCamera>().enabled = false;
                MotionCamera.Instance.gameObject.SetActive(false);
            }
            */
            if(useMotion) {
                Camera.main.GetComponent<MotionBlur>().enabled = true;
                Camera.main.GetComponent<CameraController>().enabled = false;
            }else {
                Camera.main.GetComponent<MotionBlur>().enabled = false;
                Camera.main.GetComponent<CameraController>().enabled = true;
            }
        }

        public Texture heatTex;
        public float DistortFactor;
        public float RiseFactor;
        public float _Radius;
        public float _ClipArg;

        public bool useHeat = true ;
        [ButtonCallFunc()] public bool heat;
        public void heatMethod() {
            if(useHeat) {
                Camera.main.GetComponent<HeatHaze>().enabled = true;
            }else {
                Camera.main.GetComponent<HeatHaze>().enabled =  false;
            }
        }
	}
}
