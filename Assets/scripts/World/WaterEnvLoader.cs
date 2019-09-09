using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class WaterEnvLoader : MonoBehaviour
    {
        void Awake()
        {
            Log.Sys("WaterEnvLoader");
        }

        void Start()
        {
            LoadWater();
        }

        void LoadWater()
        {
            var sceneId = WorldManager.worldManager.GetActive().def.id;
            LevelConfigData.Init();
            var configLists = LevelConfigData.LevelLayout [sceneId];
            var first = configLists [0];

            Log.Sys("LoadWater " + first.type);
            if (LevelConfigData.envConfig.ContainsKey(first.type))
            {
                Log.Sys("LoadWater " + first.type);
                var d = LevelConfigData.envConfig [first.type];
                if (!string.IsNullOrEmpty(d.waterBottom))
                {
                    var bottom = Resources.Load<GameObject>(d.waterBottom);
                    var b = GameObject.Instantiate(bottom) as GameObject;
                    Log.Sys("WaterObj " + b);
                }

                if (!string.IsNullOrEmpty(d.waterFace))
                {
                    var bottom2 = Resources.Load<GameObject>(d.waterFace);
                    var b2 = GameObject.Instantiate(bottom2) as GameObject;
                    b2.transform.localPosition = new Vector3(0, d.offY, 0);
                }
                if (!string.IsNullOrEmpty(d.skyBox))
                {
                    var skybox = Resources.Load<GameObject>(d.skyBox);
                    var s2 = GameObject.Instantiate(skybox) as GameObject;
                    s2.transform.localPosition = Vector3.zero;
                }
                if (d.useFog)
                {
                    RenderSettings.fog = d.useFog;
                    RenderSettings.fogColor = d.fogColor;
                    RenderSettings.fogStartDistance = d.fogStart;
                    RenderSettings.fogEndDistance = d.fogEnd;
                    RenderSettings.fogMode = d.fogMode;
                    RenderSettings.fogDensity = d.fogDensity;
                    Camera.main.farClipPlane = d.cameraDist;

                    GraphInit.Instance.lightCoff = d.lightCoff;
                    GraphInit.Instance.ambient = d.ambient;
                    GraphInit.Instance.InitNowMethod();

                } else
                {
                    RenderSettings.fog = d.useFog;
                }

                if (d.hasRain)
                {
                    gameObject.AddComponent<RainSystem>();
                    GraphInit.Instance.lightCoff = d.rainLightCoff;
                    GraphInit.Instance.ambient = d.rainAmbient;
                    GraphInit.Instance.InitNowMethod();
                }else {
                    
                }
                if (d.hasLightning)
                {
                    gameObject.AddComponent<LightningSystem>();
                }
            }
        }

    }

}