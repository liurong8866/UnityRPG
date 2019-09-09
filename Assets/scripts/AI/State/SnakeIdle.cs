using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class SnakeIdle : IdleState
    {

        bool birthYet = false;
        float directionChangeInterval = 3;
        float RunSpeed = 5;

        //一次性初始化代码
        public override void EnterState()
        {
            base.EnterState();
            SetAttrState(CharacterState.Idle);
            aiCharacter.SetIdle();
        }

        IEnumerator Birth()
        {
            if (CheckAni("spawn"))
            {
                SetAttrState(CharacterState.Birth);
                
                PlayAni("spawn", 2, WrapMode.Once);
                
                if (GetAttr().ObjUnitData.SpawnEffect != "")
                {
                    GameObject g = GameObject.Instantiate(Resources.Load<GameObject>(GetAttr().ObjUnitData.SpawnEffect)) as GameObject;
                    g.transform.parent = SaveGame.saveGame.EffectMainNode.transform;
                    g.transform.position = GetAttr().transform.position;
                    NGUITools.AddMissingComponent<RemoveSelf>(g);
                }
                //yield return GetAttr ().StartCoroutine (Util.WaitForAnimation (GetAttr ().animation));
                yield return new WaitForSeconds(1f);
                
                SetAttrState(CharacterState.Idle);
                SetAni("idle", 1, WrapMode.Loop);
            } else
            {
                SetAni("idle", 1, WrapMode.Once);
                GameObject g = null;
                if (GetAttr().ObjUnitData.SpawnEffect != "")
                {
                    g = GameObject.Instantiate(Resources.Load<GameObject>(GetAttr().ObjUnitData.SpawnEffect)) as GameObject;
                } else
                {
                    g = GameObject.Instantiate(Resources.Load<GameObject>("particles/playerskills/impsummon")) as GameObject;
                }
                g.transform.parent = SaveGame.saveGame.EffectMainNode.transform;
                g.transform.position = GetAttr().transform.position;
                NGUITools.AddMissingComponent<RemoveSelf>(g);
                //yield return GetAttr ().StartCoroutine (Util.WaitForAnimation (GetAttr ().animation));
                yield return new WaitForSeconds(1f);
                
                SetAttrState(CharacterState.Idle);
                SetAni("idle", 1, WrapMode.Loop);
            }
            birthYet = true;
            var rd = Random.Range(1, 3);
            BackgroundSound.Instance.PlayEffectPos("batmanspawn" + rd, GetAttr().transform.position);
        }

        public override IEnumerator RunLogic()
        {
            if (!birthYet)
            {
                yield return GetAttr().StartCoroutine(Birth());
            }
            aiCharacter.ChangeState(AIStateEnum.MOVE);
            Log.AI("State Logic Over " + type);
        }

    }

}