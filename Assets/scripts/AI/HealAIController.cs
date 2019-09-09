using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    public class AddHPComponent : MonoBehaviour
    {
        List<GameObject> healObjects = new List<GameObject>();
        void Awake()
        {
            var sp = gameObject.AddComponent<SphereCollider>();
            sp.isTrigger = true;
            sp.center = new Vector3(0, 2, 0);
            sp.radius = 2.5f;
        }
         void AddHP(GameObject go)
        {
            GameInterface_Skill.AddSkillBuff(go, 151, Vector3.zero);
            healObjects.Add(go);
        }

        void RemoveBuff(GameObject go)
        {
            GameInterface_Skill.RemoveSkillBuff(go, 151);
            healObjects.Remove(go);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == GameTag.Player)
            {
                var attr = NetworkUtil.GetAttr(other.gameObject);
                if (attr != null)
                {
                    AddHP(attr.gameObject);
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.tag == GameTag.Player)
            {
                var attr = NetworkUtil.GetAttr(other.gameObject);
                if (attr != null)
                {
                    RemoveBuff(attr.gameObject);
                }
            }
        }
    }

    [RequireComponent(typeof(MonsterSync))]
    [RequireComponent(typeof(MonsterSyncToServer))]
    [RequireComponent(typeof(BlockPhysic))]
    public class HealAIController : AIBase
    {
        void Awake()
        {
            attribute = GetComponent<NpcAttribute>();
            attribute.ShowBloodBar = false;
            ai = new ChestCharacter();
            ai.attribute = attribute;

            ai.AddState(new ChestIdle());
            ai.AddState(new NotMoveDead());
            ai.AddState(new MonsterKnockBack());
            Util.SetLayer(gameObject, GameLayer.IgnoreCollision2);

            var g = new GameObject("AddHP");
            g.transform.parent = transform;
            Util.InitGameObject(g);
            g.AddComponent<AddHPComponent>();
        }
        void Start() {
            ai.ChangeState(AIStateEnum.IDLE);
        }
    }
}