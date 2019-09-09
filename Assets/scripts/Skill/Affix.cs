using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    /// <summary>
    /// Buff的配置参数Key列表 
    /// </summary>
    public enum PairEnum {
        Percent = 0,
        Abs = 1,
        Rot = 2,
    }
    [System.Serializable]
    public struct Pair
    {
        public PairEnum key;
        public string value;
    }

    [System.Serializable]
    public class Affix
    {
        //效果类型
        public enum EffectType
        {
            None,
            SummonDuration, //召唤物持续时间 
            DefenseAdd, //防御增加
            KnockBack, //击退怪物
            AddHPMP, //增加HP和MP
            Fushi, //腐蚀降低防御力
            ReduceHP, //持续降低HP
            Frozen, //冰冻降低移动攻击速度 50% 不能叠加
            FanTan, //反弹伤害
            Ghost, //鬼魂化
            IgnoreCol, //忽略一切的碰撞
            DamageNearBy, //献祭燃烧附近的敌人
            Relive, //灰鼠复活buff 死亡时候释放这个buff
            GoldSteal,
            Stunned , //眩晕 参考冰冻 和 KnockBack
            ShiXue, //嗜血Buff 多次攻击后触发增强
            OneTimeCriticalHit,
            Blind, //玩家致盲
            Silent, //随机沉默
            WeaponBuff,//武器Buff
            AddMP, //自动回复魔法的Buff
            AddHP, //自动回复HP
        }


        public EffectType effectType = EffectType.None;
        public float Duration = 10;//Buff Time 

        public GameObject UnitTheme;//buff 期间的单位粒子效果
        public Vector3 ThemePos = Vector3.zero;

        //TODO:防御增加的数值 从数值表中读取
        //根据技能等级 以10%的比例上升 或者相对于某条数值曲线 上面取值
        public int addDefense = 0;

        public enum TargetType
        {
            Self,
            Pet,
            Enemy,
        }

        //附加buff到攻击者还是宠物身上
        public TargetType target = TargetType.Self;
        public List<Pair> affixParameters;

        public string GetPara(PairEnum key){
            foreach(var kv in affixParameters){
                if(kv.key == key){
                    return kv.value;
                }
            }
            Debug.LogError("NotFindAffixPara "+effectType+" key "+key);
            return null;
        }

        /// <summary>
        /// 燃烧不能叠加 
        /// </summary>
        public bool IsOnlyOne = false;

        /// <summary>
        /// buff周期性触发伤害的粒子效果 
        /// </summary>
        public GameObject fireParticle;

        /// <summary>
        /// 眩晕只保持最旧的一个效果 新的效果不能添加 
        /// </summary>
        public bool keepOld = false;

        public GameObject buffParticle;
    }
}
