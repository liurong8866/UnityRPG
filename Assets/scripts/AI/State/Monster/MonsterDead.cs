using UnityEngine;
using System.Collections;

namespace MyLib
{

    public class MonsterDead : DeadState
    {
        float KnockFlyTime = 0.3f;
        float FlySpeed = 6;
        float StopKnockTime = 0.2f;

        public override void EnterState()
        {
            base.EnterState();
            var rd = Random.Range(1, 3);
            BackgroundSound.Instance.PlayEffect("bloodexplode" + rd);
            var rd1 = Random.Range(1, 3);
            BackgroundSound.Instance.PlayEffect("burrowerdeath" + rd1);
            var playerId = ObjectManager.objectManager.GetMyLocalId();
            var monId = GetAttr().GetLocalId();
            var evt = new DeadExpEvent();
            evt.monId = monId;
            evt.playerId = playerId;
            evt.exp = GetAttr().ObjUnitData.XP;
            var playerLevel = ObjectManager.objectManager.GetMyAttr().Level;
            var num = (playerLevel - GetAttr().ObjUnitData.Level) / 5;
            if (num > 0)
            {
                evt.exp = evt.exp >> num;
            }

            MyEventSystem.myEventSystem.PushEvent(evt);
            DropGoods.Drop(GetAttr());
        }

        IEnumerator KnockDie()
        {
            GameObject attacker = GetEvent().attacker;
            Vector3 moveDirection = Vector3.zero;
            if (attacker != null)
            {
                moveDirection = GetAttr().transform.position - attacker.transform.position;
            } else
            {
                moveDirection = -GetAttr().transform.forward;
            }
            moveDirection.y = 0;

            var physic = GetAttr().GetComponent<PhysicComponent>();
            float curFlySpeed = 0;
            float passTime = 0;
            while (passTime < KnockFlyTime)
            {
                curFlySpeed = Mathf.Lerp(curFlySpeed, FlySpeed, 5 * Time.deltaTime);
                var movement = moveDirection * curFlySpeed;
                physic.MoveSpeed(movement);
                passTime += Time.deltaTime;
                yield return null;
            }

            float stopTime = 0;
            while (stopTime < StopKnockTime)
            {
                curFlySpeed = Mathf.Lerp(curFlySpeed, 0, 5 * Time.deltaTime);
                var movement = moveDirection * curFlySpeed;
                physic.MoveSpeed(movement);
                stopTime += Time.deltaTime;
                yield return null;
            }

            var deathBlood = GameObject.Instantiate(Resources.Load<GameObject>("particles/deathblood")) as GameObject;
            deathBlood.transform.parent = ObjectManager.objectManager.transform;
            deathBlood.transform.localPosition = GetAttr().transform.localPosition + Vector3.up * 0.1f;
            deathBlood.transform.localRotation = Quaternion.identity;
            deathBlood.transform.localScale = Vector3.one;
            NGUITools.AddMissingComponent<RemoveSelf>(deathBlood);
        }



        public override IEnumerator RunLogic()
        {
            GetAttr().ShowDead();

            //GetAttr ().GetComponent<BloodBar> ().enabled = false;
            var deathBlood = Resources.Load<GameObject>("particles/swordhit");

            GameObject g = GameObject.Instantiate(deathBlood) as GameObject;
            g.transform.parent = SaveGame.saveGame.EffectMainNode.transform;
            g.transform.position = GetAttr().transform.position;
            if (CheckAni("die"))
            {
                GetAttr().GetComponent<Animation>().CrossFade("die");
            }

            if (CheckAni("death"))
            {
                GetAttr().GetComponent<Animation>().CrossFade("death");
            }

            yield return GetAttr().StartCoroutine(KnockDie());

            yield return new WaitForSeconds(2);
            yield return GetAttr().StartCoroutine(Util.SetBurn(GetAttr().gameObject));
            yield return null;
			
            ObjectManager.objectManager.DestroyByLocalId(GetAttr().GetComponent<KBEngine.KBNetworkView>().GetLocalId());
        }
    }



}