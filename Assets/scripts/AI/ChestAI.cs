using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class ChestAI : AIBase
    {
        void Awake() {
            attribute = GetComponent<NpcAttribute>();
            ai = new ChestCharacter ();
            ai.attribute = attribute;
            ai.AddState(new ChestIdle());
            ai.AddState(new ChestDead());
        }
        // Use this for initialization
        void Start()
        {
            ai.ChangeState (AIStateEnum.IDLE);
            var par = Instantiate(Resources.Load<GameObject>("particles/drops/generic_item")) as GameObject;
            par.transform.parent = transform;
            par.transform.localPosition = Vector3.zero;
        }
	
        protected override void OnDestroy() {
            base.OnDestroy();
            if (attribute.IsDead) {
                Util.ClearMaterial (gameObject);
            }
        }
    }

}