using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    public class Ghost : IEffect
    {
        SkinnedMeshRenderer[] renders;
        List<Material> materials;

        public override void Init (Affix af, GameObject o)
        {
            base.Init (af, o);
            type = Affix.EffectType.Ghost;
            renders = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
            materials = new List<Material>();
            //Copy
            foreach(var r in renders) {
                materials.Add(r.sharedMaterial);
                Log.Sys("CopySharedMaterial "+r.sharedMaterial);
            }
            //Instance
            foreach(var r in renders) {
                r.material.shader = Shader.Find("Custom/GhostShader");
            }
            obj.layer = (int)GameLayer.IgnoreCollision;
        }

        public override int GetArmor ()
        {
            return 100000;
        }

        public override void OnDie ()
        {
            base.OnDie ();
            int c = 0;
            foreach(var r in renders){
                var mat = r.material;
                var oldMat = materials[c++];
                //New Material Not Save WithOld Destroy
                if(mat != oldMat) {
                    r.sharedMaterial = oldMat;
                    Log.Sys("Material is "+r.sharedMaterial);
                    r.material = r.sharedMaterial;
                    GameObject.Destroy(mat);
                }else {
                    mat.shader = Shader.Find("Custom/npcShader");
                }
            }
            obj.layer = (int)GameLayer.Npc;
        }
    }

}