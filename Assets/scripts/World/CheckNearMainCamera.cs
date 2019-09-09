using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class NearTrigger : MonoBehaviour
    {
        public GameObject connectObject;
        public void SetConnectActive(bool a){
            connectObject.SetActive(a);
        }
    }
/// <summary>
/// 检测场景物件如果不是靠近镜头则移除掉粒子效果 这样提高系统效率
    /// 采用物理Physics.OverlapSphere 来管理对象
    /// 创建一个同位置的物体
    /// 场景物件不会移动

/// </summary>
    public class CheckNearMainCamera : MonoBehaviour
    {
        //EnterScene
        //ExitScene
        void Awake()
        {
            var go = new GameObject("NearMainCameraTrigger");
            go.transform.parent = gameObject.transform.parent;
            Util.InitGameObject(go);
            go.transform.position = gameObject.transform.position;

            var col = go.AddComponent<SphereCollider>();
            var tri = go.AddComponent<NearTrigger>();
            tri.connectObject = gameObject;

            col.radius = 2;
            col.isTrigger = true;
            go.layer = (int)GameLayer.SceneProps;
            gameObject.SetActive(false);
        }
    }

}