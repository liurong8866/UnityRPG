using UnityEngine;
using System.Collections;

namespace MyLib
{
    /// <summary>
    /// 同步服务器上的怪物属性和操作 
    /// </summary>
    public class MonsterSync : MonoBehaviour
    {
        AIBase aibase;

        public void DoNetworkDamage(GCPlayerCmd cmd)
        {
            var eid = cmd.DamageInfo.Enemy;
            var attacker = ObjectManager.objectManager.GetPlayer(cmd.DamageInfo.Attacker);
            if (attacker != null)
            {
                gameObject.GetComponent<MyAnimationEvent>().OnHit(attacker, cmd.DamageInfo.Damage, cmd.DamageInfo.IsCritical);
            }
        }

        public void NetworkBuff(GCPlayerCmd cmd)
        {
            //var attacker = ObjectManager.objectManager.GetPlayer(cmd.BuffInfo.Attacker);
            //if(attacker != null) {
            var sk = Util.GetSkillData(cmd.BuffInfo.SkillId, 1);
            var skConfig = SkillLogic.GetSkillInfo(sk);
            var evt = skConfig.GetEvent(cmd.BuffInfo.EventId);
            if (evt != null)
            {
                var pos = cmd.BuffInfo.AttackerPosList;
                var px = pos [0] / 100.0f;
                var py = pos [1] / 100.0f;
                var pz = pos [2] / 100.0f;
                gameObject.GetComponent<BuffComponent>().AddBuff(evt.affix, new Vector3(px, py, pz));
            }
            //}
        }

        private void NetworkMove(EntityInfo info)
        {
            if (aibase == null)
            {
                aibase = gameObject.GetComponent<AIBase>();
            }
            if (aibase != null)
            {
                var aiState = aibase.GetAI().state;
                if (aiState != null && aiState.type == AIStateEnum.IDLE)
                {
                    var curPos = transform.position;
                    var tarPos = NetworkUtil.FloatPos(info.X, info.Y, info.Z);
                    var vdir = tarPos - curPos;
                    if (vdir.sqrMagnitude >= 2)
                    {
                        transform.position = tarPos;
                    }
                }
            }
        }

        public void SyncAttribute(EntityInfo info) {
            var attr = gameObject.GetComponent<NpcAttribute>();
            NetworkMove(info);
            if (info.HasHP)
            {
                attr.SetHPNet(info.HP);
            }
        }
        public void SyncAttribute(GCPlayerCmd cmd)
        {
            var info = cmd.EntityInfo;
            SyncAttribute(info);
        }
    }

}