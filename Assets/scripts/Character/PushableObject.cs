using UnityEngine;
using System.Collections;

namespace MyLib
{
    //配置障碍物的 半径大小和碰撞提类型
    //或者自动加载障碍物 怪物类型
    public class PushableObject : MonoBehaviour
    {
        void Start() {
            var sphere = gameObject.AddComponent<SphereCollider>();
            sphere.isTrigger = true;
            sphere.radius = 1;
            sphere.center = new Vector3(0, 1, 0);
        }

        //为玩家增加Buff
        void OnTriggerEnter(Collider other)
        {
            if (NetworkUtil.IsNetMaster())
            {
                if (other.tag == GameTag.Player)
                {
                    //击退技能
                    var pos = other.transform.position;
                    var otherGo = other.gameObject;

                    var skill = Util.GetSkillData(140, 1);
                    var skillInfo = SkillLogic.GetSkillInfo(skill);
                    var evt = skillInfo.eventList[0];
                    var ret = gameObject.GetComponent<BuffComponent>().AddBuff(evt.affix, pos);
                    if(ret) {
                        NetDateInterface.FastAddBuff(evt.affix, otherGo, gameObject, skill.Id, evt.EvtId);
                    }
                }
            }
        }

    }

}