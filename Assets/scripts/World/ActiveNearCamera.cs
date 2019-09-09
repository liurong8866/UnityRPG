using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    /// <summary>
    ///挂载世界上检测新激活的对象，并且移除旧的被激活对象 
    /// </summary>
    public class ActiveNearCamera : MonoBehaviour
    {
        HashSet<Collider> oldActive = new HashSet<Collider>();

        void Awake() {
            MyEventSystem.myEventSystem.RegisterEvent(MyEvent.EventType.ChangeScene, OnEvent);
        }
        void OnEvent(MyEvent e){
            if(e.type == MyEvent.EventType.ChangeScene){
                oldActive.Clear();
            }
        }
        // Use this for initialization
        void Start()
        {
            StartCoroutine(CheckActive());
        }

        IEnumerator CheckActive()
        {
            while (true)
            {
                GameObject player = null;
                while (player == null)
                {
                    player = ObjectManager.objectManager.GetMyPlayer();
                    yield return null;
                }
                while (player != null)
                {
                    var newHit = Physics.OverlapSphere(player.transform.position, 15, 1<<(int)GameLayer.SceneProps);
                    Log.Sys("newHit: "+newHit.Length);
                    var newhash = new HashSet<Collider>();
                    newhash.UnionWith(newHit);
                    List<Collider> diff = new List<Collider>();
                    foreach(var n in oldActive ){
                        if(!n || n == null) {
                            continue;
                        }
                        if(n.gameObject == null) {
                            continue;
                        }
                        if(!newhash.Contains(n))
                        {
                            n.gameObject.GetComponent<NearTrigger>().SetConnectActive(false);
                        }
                    }
                    foreach(var n in newhash){
                        if(!oldActive.Contains(n)){
                            n.gameObject.GetComponent<NearTrigger>().SetConnectActive(true);
                        }
                    }
                    oldActive = newhash;

                    yield return new WaitForSeconds(2);
                }
            }
        }
    
    }

}