using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class SyncPosWithTarget : MonoBehaviour
    {
        public GameObject target;
        Vector3 localPos;

        void Start()
        {
            localPos = transform.localPosition;
        }
    
        // Update is called once per frame
        void Update()
        {
            if (target != null)
            {
                transform.position = target.transform.position + localPos;
            }
        }
    }

}