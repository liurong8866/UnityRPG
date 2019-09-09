using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class LightMapObject : MonoBehaviour
    {
        void Awake() {
            /*
            var mf = this.GetComponent<MeshFilter>();
            var uvs = mf.sharedMesh.uv2;
            var lm = this.renderer.lightmapTilingOffset;
            var scale = new Vector2(lm.x, lm.y);
            var offset = new Vector2(lm.z, lm.w);
            for(var i = 0; i < uvs.Length; i++) {
                uvs[i] = offset+new Vector2(scale.x*uvs[i].x, scale.y*uvs[i].y);
            }
            mf.mesh.uv2 = uvs;
            */
            //this.renderer.material.SetVector("_LightMapScaleAndOffset", );

        }
    }
}