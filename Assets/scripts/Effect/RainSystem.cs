using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class RainSystem : MonoBehaviour
    {
        //public GameObject drop;
        //public GameObject splash;
        // Use this for initialization
        void Start()
        {
            StartCoroutine(RainTracePlayer());
        }
        private GameObject rainObj;

        void OnDestroy() {
            GameObject.Destroy(rainObj);
        }

        IEnumerator RainTracePlayer() {
            var drop = Resources.Load<GameObject>("particles/puddle_drop");
            var splash = Resources.Load<GameObject>("particles/puddle_splash");
            var foreRain = Resources.Load<GameObject>("weather/rain");

            var drop2 = GameObject.Instantiate(drop) as GameObject;
            var splash2 = GameObject.Instantiate(splash) as GameObject;
            var forRain2 = GameObject.Instantiate(foreRain) as GameObject;
            rainObj = forRain2;

            forRain2.transform.parent = Camera.main.transform;
            forRain2.transform.localPosition = new Vector3(0, -1.6f, 0.8f);
            forRain2.transform.localRotation = Quaternion.Euler(new Vector3(56.9f, -19.2f, -16f));
            forRain2.transform.localScale = new Vector3(2, 2, 2);

            var player = ObjectManager.objectManager.GetMyPlayer();
            while(player == null) {
                player = ObjectManager.objectManager.GetMyPlayer();
                yield return null;
            }

            while(true) {
                drop2.transform.position = player.transform.position;
                splash2.transform.position = player.transform.position+new Vector3(0, 0.1f, 0);
                yield return null;
            }
        }
	
    }

}