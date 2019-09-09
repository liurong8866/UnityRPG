using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class MeshUVTest : MonoBehaviour
    {

        [ButtonCallFunc()]public bool GetUV;

        public void GetUVMethod()
        {
            var mf = this.GetComponent<MeshFilter>();
            var uv = mf.sharedMesh.uv2;
            foreach (var u in uv)
            {
                Debug.Log(u);
            }

            Debug.LogError("uv1");
            var uv1 = mf.sharedMesh.uv2;
            foreach(var u1 in uv) {
                Debug.Log(u1);
            }
        }
    }

}