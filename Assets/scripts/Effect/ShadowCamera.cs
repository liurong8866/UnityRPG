using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class ShadowCamera : MonoBehaviour
    {
        public Vector3 CamPos;
        private Shader shader;
        private NpcAttribute nat;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            GetComponent<Camera>().enabled = false;
            shader = Shader.Find("Custom/shadowMap2");
        }

        void Update()
        {
            transform.position = CameraController.Instance.transform.position + CamPos;
            //if (nat == null)
            {
                //var player = ObjectManager.objectManager.GetMyPlayer();
                var allP = ObjectManager.objectManager.GetAllPlayer();
                while (allP.MoveNext())
                {
                    var c = allP.Current;
                    var np = c.gameObject.GetComponent<NpcAttribute>();
                    np.SetShadowLayer();

                }

                GetComponent<Camera>().RenderWithShader(shader, null);

                allP = ObjectManager.objectManager.GetAllPlayer();
                while (allP.MoveNext())
                {
                    var c = allP.Current;
                    var np = c.gameObject.GetComponent<NpcAttribute>();
                    np.SetNormalLayer();
                }

                /*
                if (player != null)
                {
                    nat = player.GetComponent<NpcAttribute>();
                }
                */
            }
            /*
            if (nat != null)
            {
                nat.SetShadowLayer();
                camera.RenderWithShader(shader, null);
                nat.SetNormalLayer();
            }
            */

        }
    }
}