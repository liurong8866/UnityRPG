using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class BlockPhysic : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            StartCoroutine(AdjustPos());
        }

        IEnumerator AdjustPos()
        {
            var ch = GetComponent<CharacterController>();
            var p = 0.0f;
            var mg = new Vector3(0, -20, 0);
            while(p < 1.0f) {
                ch.Move(mg);
                yield return new WaitForFixedUpdate();
                p += Time.fixedDeltaTime;
            }
        }

    }
}