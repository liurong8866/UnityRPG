
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/

/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    /// <summary>
    /// 技能数据为完全静态数据，不存在其他因素可以影响技能的数据
    /// </summary>
    public class SkillData
    {
        public enum SkillConstId
        {
            Bomb = 3,
            Jump = 4,
            KnockBack = 140,
            AddMP = 150,
        }

        public SkillConfigData skillConfig;
        SkillEffectConfigData effectConfig;

        public string Sound
        {
            get
            {
                return skillConfig.effectSound;
            }
        }

        public string HitSound
        {
            get
            {
                return skillConfig.hitSound;
            }
        }
        //下一级技能学习 的条件数据
        SkillLearnConfigData learnConfig;

        public int Id
        {
            get
            {
                return skillConfig.id;
            }
        }

        public int Level
        {
            get
            {
                if (effectConfig != null)
                {
                    return effectConfig.skillLevel;
                }
                return 0;
            }
        }

        public int sheet
        {
            get
            {
                return skillConfig.sheet;
            }
        }

        public string icon
        {
            get
            {
                return skillConfig.icon;
            }
        }

        public enum CastType
        {
            Always = 0,
            Stuck,
        }

        public string template
        {
            get
            {
                return skillConfig.template;
            }
        }

        public enum SkillType
        {
            None = 0,
            ActiveAttack,
            ActiveBuff,
            ActiveHeal,
            TriggerSkill,
            PassiveSkill,
            AttachBuff,
        }

        public enum DamageType
        {
            Physic = 0,
            Water,
            Fire,
            Wind,
            Soil,
        }

        public enum DamageShape
        {
            None = 0,
            Line = 1,
            Circle = 2,
        }

        public string SkillName
        {
            get
            {
                return skillConfig.name;
            }
        }

        public float AttackAniTime
        {
            get
            {
                return skillConfig.animationTime / 1000.0f;
            }
        }

        public string AnimationName
        {
            get
            {
                return skillConfig.animationName;
            }
        }

        static Dictionary<int, int> idToMaxLev = new Dictionary<int, int>();

        public int MaxLevel
        {
            get
            {
                //return skillConfig.maxLevel;
                var skId = skillConfig.id;
                if (idToMaxLev.ContainsKey(skId))
                {
                    return idToMaxLev [skId];
                } else
                {
                    int maxLev = 0;
                    foreach (var sk in GameData.SkillEffectConfig)
                    {
                        if (sk.skillId == skId)
                        {
                            maxLev = Mathf.Max(maxLev, sk.skillLevel);
                        }
                    }
                    idToMaxLev [skId] = maxLev;
                    return maxLev;
                }
            }
        }

    

        public int LevelRequired
        {
            get
            {
                if (learnConfig != null)
                {
                    return learnConfig.restrictLevel + (Level - 1) * learnConfig.addLevel;
                }
                return 99999;
            }
        }

        public int ManaCost
        {
            get
            {
                if (effectConfig == null)
                {
                    return 0;
                }
                return effectConfig.mp;
            }
        }

        //GoTo Next Level
        public int SpCost
        {
            get
            {
                return learnConfig.skillPoint;
            }
        }

        public  DamageType damageType
        {
            get
            {
                return DamageType.Physic;
                //return (DamageType)skillConfig.damageType;
            }
        }

        public int WeaponDamagePCT
        {
            get
            {
                if (effectConfig == null)
                {
                    return 100;
                } else
                {
                    return effectConfig.damageRatio;
                }
            }
        }


        public string SkillDes
        {
            get
            {
                return skillConfig.introduce;
            }
        }





        /*
         * UI display  6row * 3 col
         */
        //TODO: 技能显示 在 技能面板上面的位置
        public int row = 1;
        public int column = 1;
        public bool IsActive = true;


        public SkillData(int skillId, int level)
        {
            skillConfig = GMDataBaseSystem.SearchIdStatic<SkillConfigData>(GameData.SkillConfig, skillId);

            foreach (SkillEffectConfigData sd in GameData.SkillEffectConfig)
            {
                if (sd.skillId == skillId && sd.skillLevel == level)
                {
                    effectConfig = sd;
                    break;
                }
            }
            foreach (SkillLearnConfigData sd in GameData.SkillLearnConfig)
            {
                if (sd.skillId == skillId)
                {
                    learnConfig = sd;
                    break;
                }
            }
            if (learnConfig == null)
            {
                Log.Sys("Skill Learn Config Not exist " + skillId + " " + level);
            }
            if (effectConfig == null)
            {
                Log.Sys("SkillData Init Error " + skillId + " " + level);
            }

            Log.Important("skill Id and skill Effect " + skillConfig + " " + effectConfig);

        }
    }

}