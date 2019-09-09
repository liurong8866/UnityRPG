using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MyLib
{
    [System.Serializable]
    public class LayoutConfig
    {
        public GameObject g;
        public Vector3 pos;
        public Vector3 scale = Vector3.one;
        public float rotY = 0;

    }

    /// <summary>
    /// 参考SpawnTrigger 运行时加载相关组件资源而不是将组件资源嵌入到GameObject中 
    /// 
    /// 使用方法：
    ///   1：配置工程中的prefab 到parts中
    ///   2：点击UpdateModel
    ///   3：调整model位置
    ///   4：点击CollectPos
    ///   5：点击RemoveModel
    ///   6：将其保存为一个Prefab到工程中
    /// </summary>
    public class ComplexLayout : MonoBehaviour
    {
        public List<LayoutConfig> parts = new List<LayoutConfig>();

        [ButtonCallFunc()]
        public bool UpdateModel;

        public void UpdateModelMethod()
        {
            ClearChildren();
            AddChildren();
        }

        [ButtonCallFunc()]
        public bool CollectPos;

        public void CollectPosMethod()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                parts [i].pos = child.localPosition;
                parts [i].scale = child.localScale;
                parts [i].rotY = child.localRotation.eulerAngles.y;
            } 
        }

        [ButtonCallFunc()]
        public bool Remove;

        public void RemoveMethod()
        {
            ClearChildren();
        }

        void ClearChildren()
        {
            for (int i = 0; i < transform.childCount;)
            {
                GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        void AddChildren()
        {
            foreach (var g in parts)
            {

                if (g.g != null)
                {
                    var nb = GameObject.Instantiate(g.g) as GameObject;
                    var oldRot = nb.transform.localRotation;
                    nb.transform.parent = transform;
                    Util.InitGameObject(nb);
                    nb.transform.localRotation = oldRot;
                    if (g.rotY != 0)
                    {
                        nb.transform.localRotation = Quaternion.Euler(new Vector3(oldRot.eulerAngles.x, g.rotY, oldRot.eulerAngles.z));
                    }

                    nb.transform.localPosition = g.pos;
                    if (g.scale.x != 0)
                    {
                        nb.transform.localScale = g.scale;
                    } else
                    {
                        nb.transform.localScale = Vector3.one;
                    }
                }
            }
        }

        /// <summary>
        /// 游戏中调用 
        /// </summary>
        void Awake()
        {
            UpdateModelMethod();
        }

        //收集Prefab的位置和Scale以及GameObject
        [ButtonCallFunc()]public bool CollectObj;

        #if UNITY_EDITOR
        public void CollectObjMethod()
        {
            parts.Clear();
            WriteObj(transform);   
        }

        void WriteObj(Transform tran)
        {
            foreach (Transform t in tran)
            {
                if (t.childCount == 0)
                {
                    var pb = PrefabUtility.GetPrefabParent(t.gameObject) as GameObject;
                    var pl = new LayoutConfig()
                    {
                        g = pb,
                        pos = t.transform.localPosition,
                        scale = t.transform.localScale,
                    };
                    //par2.Add(pl);
                    parts.Add(pl);

                } else
                {
                    WriteObj(t);
                }
            }

            Debug.Log("Par2Num: " + parts.Count);
            //parts = par2;
        }
        #endif
    }

}