using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class TowerAutoCheck : MonoBehaviour
    {
        public static float maxRotateChange = 3.0f;

        void Start()
        {
            StartCoroutine(AutoEnemy());
        }

        //根据配置锁定目标 开火
        //最近攻击目标
        //目标惯性持续目标多久 目标脱离攻击范围多久仍然持续攻击
        //
        IEnumerator AutoTarget(Transform trans, GameObject enemy)
        {
            var attr = trans.GetComponent<NpcAttribute>();
            var passTime = 0.0f;
            while (true)
            {
                var dir = enemy.transform.position - trans.position;
                dir.y = 0;
                var curDir = transform.forward;
                curDir.y = 0;
                var forwardDir = dir;
                var diffDir = Quaternion.FromToRotation(curDir, forwardDir);
                var diffY = diffDir.eulerAngles.y;

                if (diffY > 180)
                {
                    diffY = diffY - 360;
                }
                if (diffY < -180)
                {
                    diffY = 360 + diffY;
                }
                var dy = Mathf.Clamp(diffY, -maxRotateChange, maxRotateChange);
                var curRot = transform.rotation.eulerAngles.y;
                var tarDir = Quaternion.Euler(new Vector3(0, curRot + dy, 0));
                transform.rotation = tarDir;
                yield return new WaitForFixedUpdate() ;
                passTime += Time.deltaTime;
                if (passTime >= 1)
                {
                    var tempEnemy = SkillLogic.FindNearestEnemy(trans.gameObject);
                    if (tempEnemy != enemy)
                    {
                        break;
                    }
                    passTime = 0;
                }
            }
        }


        IEnumerator AdjustTower(NpcAttribute attr)
        {
            var passTime = 0.0f;
            var trans = attr.transform;
            while (true)
            {
                var curDir = transform.forward;
                curDir.y = 0;
                var forwardDir = trans.forward;
                var diffDir = Quaternion.FromToRotation(curDir, forwardDir);
                var diffY = diffDir.eulerAngles.y;

                if (diffY > 180)
                {
                    diffY = diffY - 360;
                }
                if (diffY < -180)
                {
                    diffY = 360 + diffY;
                }

                var dy = Mathf.Clamp(diffY, -maxRotateChange, maxRotateChange);
                var curRot = transform.rotation.eulerAngles.y;
                var tarDir = Quaternion.Euler(new Vector3(0, curRot + dy, 0));
                transform.rotation = tarDir;
                //yield return null;
                yield return new WaitForFixedUpdate() ;

                if(Mathf.Abs(diffY) <= 0.1f) {
                    break;
                }

                passTime += Time.deltaTime;
                if (passTime >= 1)
                {
                    var tempEnemy = SkillLogic.FindNearestEnemy(trans.gameObject);
                    if (tempEnemy != null)
                    {
                        break;
                    }
                    passTime = 0;
                }
            }
        }

        //自适应炮塔调整
        IEnumerator AutoEnemy()
        {
            var attr = NetworkUtil.GetAttr(gameObject);
            var phy = attr.GetComponent<GeneraPhysic>();
            var trans = attr.transform;
            while (true)
            {
                var enemy = SkillLogic.FindNearestEnemy(trans.gameObject);
                if (enemy != null)
                {
                    yield return StartCoroutine(AutoTarget(trans, enemy));
                } else
                {
                    yield return StartCoroutine(AdjustTower(attr));
                }
                yield return new WaitForSeconds(0.3f);
            }
        }
    }
}
