using UnityEngine;
using System.Collections;
using System;

namespace MyLib
{
    /// <summary>
    /// 玩家或者Boss的技能相关的的组件
    /// </summary>
    public class SkillInfoComponent : MonoBehaviour
    {
        SkillData activeSkill;
        NpcAttribute attribute;


        void Start()
        {
            attribute = GetComponent<NpcAttribute>();
        }

        public int GetDefaultSkillId()
        {
            return attribute.ObjUnitData.baseSkill;
        }

        //TODO: 根据怪物还是 玩家获取对应技能的等级数据
        public int GetDefaultSkillLevel()
        {
            return 1;
        }

        public SkillFullInfo GetDefaultSkill()
        {
            var sd = Util.GetSkillData(GetDefaultSkillId(), GetDefaultSkillLevel());
            var sk = new SkillFullInfo(sd);
            return sk;
        }

        //检测是否是玩家的普通攻击 没有技能数据则为默认技能
        public bool IsDefaultSkill(SkillData sd)
        {
            if (sd == null)
            {
                return true;
            }

            var isPlayer = attribute.ObjUnitData.GetIsPlayer();
            if (isPlayer)
            {
                return GetDefaultSkillId() == sd.Id;
            }
            return false;
        }

        public void SetDefaultActive()
        {
            activeSkill = GetDefaultSkill().skillData;
        }

        /// <summary>
        ///得到怪物的死亡随机技能 
        /// </summary>
        /// <returns>The dead skill.</returns>
        public SkillData  GetDeadSkill()
        {
            var skList = attribute.ObjUnitData.GetSkillList();
            foreach (SimpleJSON.JSONNode j in skList)
            {
                if (!string.IsNullOrEmpty(j ["death"].Value))
                {
                    if (j ["death"].AsBool)
                    {
                        activeSkill = Util.GetSkillData(j ["id"].AsInt, j ["level"].AsInt);
                        Log.AI("Set Death Skill Active " + activeSkill.SkillName);
                        return activeSkill;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 获得一个当前存在条件约束的可以使用的技能 
        /// 闪烁技能
        /// Random
        /// ignore
        /// </summary>
        /// <returns>The random skill.</returns>
        public SkillData GetRandomSkill()
        {
            Log.AI("SetRandomActive " + gameObject.name);
            var rd = UnityEngine.Random.Range(0, 100);
            var skList = attribute.ObjUnitData.GetSkillList();
            Log.AI("skList is " + skList.Count);
            foreach (SimpleJSON.JSONNode j in skList)
            {
                if (!string.IsNullOrEmpty(j ["ignore"].Value))
                {
                    if (rd < j ["chance"].AsInt)
                    {
                        var tp = Type.GetType("MyLib.Skill" + j ["id"].AsInt);
                        if (tp == null)
                        {
                        } else
                        {
                            var t = typeof(NGUITools);
                            var m = t.GetMethod ("AddMissingComponent");
                            var geMethod = m.MakeGenericMethod (tp);
                            var sk = geMethod.Invoke (null, new object[]{gameObject}) as SkillObj;

                            //技能有额外的条件需要检测
                            //var sk = (SkillObj)Activator.CreateInstance(tp);
                            var ret = sk.CheckCondition(gameObject);
                            Log.AI("CheckSkillCondition " + j ["id"].Value);
                            if (ret)
                            {
                                return Util.GetSkillData(j ["id"].AsInt, j ["level"].AsInt);
                            }
                        }
                    }
                }
            }
            return null;
        }


        /// <summary>
        ///怪物随机一个技能 
        /// </summary>
        public void SetRandomActive()
        {
            Log.AI("SetRandomActive " + gameObject.name);
            var rd = UnityEngine.Random.Range(0, 100);
            var skList = attribute.ObjUnitData.GetSkillList();
            Log.AI("skList is " + skList.Count);
            foreach (SimpleJSON.JSONNode j in skList)
            {
                if (string.IsNullOrEmpty(j ["ignore"].Value) && rd >= j ["min"].AsInt && rd < j ["max"].AsInt)
                {
                    var tp = Type.GetType("MyLib.Skill" + j ["id"].AsInt);
                    if (tp == null)
                    {
                        activeSkill = Util.GetSkillData(j ["id"].AsInt, j ["level"].AsInt);
                        Log.AI("Set Random Skill Active " + activeSkill.SkillName);
                        return;
                    } else
                    {
                        //技能有额外的条件需要检测
                        var sk = (SkillObj)Activator.CreateInstance(tp);
                        var ret = sk.CheckCondition(gameObject);
                        Log.AI("CheckSkillCondition " + j ["id"].Value);
                        if (ret)
                        {
                            activeSkill = Util.GetSkillData(j ["id"].AsInt, j ["level"].AsInt);
                            Log.AI("Set Random Skill Active " + activeSkill.SkillName);
                            return;
                        }
                    }
                }
            }
            SetDefaultActive();
        }

        public void SetActiveSkill(SkillData skillData)
        {
            activeSkill = skillData;
        }

        //获取当前使用的技能
        public SkillFullInfo GetActiveSkill()
        {
            return new SkillFullInfo(activeSkill);
        }

        //获取技能的客户端表现逻辑配置
        //TODO: 获取宠物的技能
        public SkillDataConfig GetSkillConfig()
        {
            return SkillLogic.GetSkillInfo(activeSkill);
        }
    }

}