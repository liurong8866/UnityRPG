
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
    /*
 * 配置的数据均为百分比数据  需要从相关的曲线数据中获取时机的数据
 * 例如HP = 100 表示100% 的生命值， 根据Level 在 Health_monster 表格中查到对应Level怪兽的数据 接着按照百分比调整即可 
 */
    public class UnitData
    {
        MonsterFightConfigData config = null;
        RoleUpgradeConfigData playerConfig = null;
        public RoleJobDescriptionsData jobConfig = null;
        NpcConfigData npcConfig = null;

        public NpcConfigData NpcConfig
        {
            get
            {
                return npcConfig;
            }
        }

        public MonsterFightConfigData Config
        {
            get
            {
                return config;
            }
        }

        public bool IsElite
        {
            get
            {
                return config.id > 1000;
                //return false;
            }
        }

        List<int> elites = null;

        public List<int> EliteIds
        {
            get
            {
                var mid = config.id;
                if (elites == null)
                {
                    var ret = new List<int>();
                    if (mid < 1000)//Not Elite Then Find Elite
                    {
                        var elite = GMDataBaseSystem.database.SearchId<MonsterFightConfigData>(GameData.MonsterFightConfig, mid + 1000);
                        if (elite != null)
                        {
                            ret.Add(elite.id);
                        }
                        for (int i = 1; i < 10; i++)
                        {
                            var elite1 = GMDataBaseSystem.database.SearchId<MonsterFightConfigData>(GameData.MonsterFightConfig, 10 * (mid + 1000) + i);
                            if (elite1 != null)
                            {
                                ret.Add(elite1.id);
                            } else
                            {
                                break;
                            }
                        }
                    }
                    elites = ret;
                }
                return elites;
            }
        }

        public int ID
        {
            get
            {
                if (IsPlayer)
                {
                    return jobConfig.id;
                } else
                {
                    return config.id;
                }
            }
        }

        public float MoveSpeed
        {
            get
            {
                if (IsPlayer)
                {
                    return jobConfig.moveSpeed / 10.0f;
                } else
                {
                    return config.moveSpeed / 10.0f;
                }
            }
        }

        public int Level
        {
            get
            {
                if (config != null)
                {
                    return config.level;
                }
                return playerConfig.level;
            }
        }

        public string SpawnEffect
        {
            get
            {
                return config.spawnParticle;
            }
        }

        SimpleJSON.JSONArray skList = null;

        public SimpleJSON.JSONArray GetSkillList()
        {
            if (skList == null)
            {
                Log.AI("MonsterConfig " + config.name + " " + config.skillList);
                var p = SimpleJSON.JSON.Parse(config.skillList);
                if (p != null)
                {
                    skList = p.AsArray;
                    int min = 0;
                    foreach (SimpleJSON.JSONNode j in skList)
                    {
                        if (string.IsNullOrEmpty(j ["ignore"].Value))
                        {
                            j ["min"].AsInt = min;
                            j ["max"].AsInt = min + j ["chance"].AsInt;
                            min += j ["chance"].AsInt;
                        }
                    }
                } else
                {
                    skList = new SimpleJSON.JSONArray();
                }
            }
            return skList;
        }

        public bool AttachToMaster
        {
            get
            {
                return config.attachToMaster;
            }
        }

        public string AITemplate
        {
            get
            {
                if (npcConfig != null)
                {
                    return npcConfig.LogicTemplate;
                }
                return config.LogicTemplate;
            }
        }

        public class Treasure
        {
            public ItemData itemData;
            public int min;
            public int max;
            public int Weight = 0;
        }

        public string name
        {
            get
            {
                if (npcConfig != null)
                {
                    return npcConfig.name;
                }

                if (IsPlayer)
                {
                    return jobConfig.job;
                } else
                {
                    return config.name;
                }
            }
        }

        //人物升级需要经验
        public long MaxExp
        {
            get
            {
                if (playerConfig != null)
                {
                    return playerConfig.exp; 
                }
                return 0;
            }
        }

        public string ModelName
        {
            get
            {
                if (npcConfig != null)
                {
                    return npcConfig.model;
                }
                if (IsPlayer)
                {
                    return jobConfig.ModelName;
                } else
                {
                    return config.model;
                }
            }
        }

        ///<summary>
        /// 人物自身属性都是静态的，装备只提供加成不会影响静态的基础
        /// </summary> 
        public int HP
        {
            get
            {
                if (npcConfig != null)
                {
                    return 1;
                }
                if (IsPlayer)
                {
                    return playerConfig.maxHp;
                } else
                {
                    return config.hp;
                }
            }
        }

        public int MP
        {
            get
            {
                if (IsPlayer)
                {
                    return playerConfig.maxMp;
                } else
                {
                    //throw new System.Exception();
                    return 0;
                }
            }
        }

        public int Damage
        {
            get
            {
                if (IsPlayer)
                {
                    return playerConfig.attack;
                } else
                {
                    return config.attack;
                }
            }
        }

        //怪物给的经验值
        public int XP
        {
            get
            {
                if (IsPlayer)
                {
                    throw new System.NotImplementedException();
                }

                return config.exp;
            }
        }

        public int Armor
        {
            get
            {
                if (IsPlayer)
                {
                    return playerConfig.defense;
                } else
                {
                    return config.defense;
                }
            }
        }

        //TODO:怪物死亡后掉落物品 机制
        public List<Treasure> TreasureData
        {
            get
            {
                return new List<Treasure>();
            }
        }

        bool IsPlayer = false;

        public bool GetIsPlayer()
        {
            return IsPlayer;
        }

        public int CriticalHit
        {
            get
            {
                if (IsPlayer)
                {
                    return jobConfig.criticalHit;
                }
                return config.criticalHit;
            }
        }
        /*
         * 碰撞体配置在模型上面
         */

        public string TextureReplace
        {
            get
            {
                if (npcConfig != null)
                {
                    return "";
                }
                if (!IsPlayer)
                {
                    return config.textureReplace;
                } else
                {
                    return "";
                }
            }
        }

        //TODO:增加怪物技能的配置信息
        public List<MyLib.SkillData> Skills
        {
            get
            {
                return null;
            }
        }

        public int ApproachDistance
        {
            get
            {
                return config.warnRange;
            }
        }

        public int AttackRange
        {
            get
            {
                if (IsPlayer)
                {
                    return jobConfig.attackRange;
                } else
                {
                    return config.attackRange;
                }
            }
        }

        //默认攻击技能
        //远程装备上装备之后 装备技能
        //还是职业属性技能
        public int baseSkill
        {
            get
            {
                if (IsPlayer)
                {
                    return jobConfig.baseSkill;
                } else
                {
                    return config.baseSkill;
                }
            }
        }

        //远程技能的子弹 配置到技能里面

        public float WalkAniSpeed
        {
            get
            {
                return 1;
            }
        }

        public float AttackAniSpeed
        {
            get
            {
                return jobConfig.AttackAniSpeed / 1000.0f;
            }
        }

        public Job job
        {
            get
            {
                return (Job)jobConfig.id;
            }
        }

        public List<List<float>>  GetRandomDrop(float modify)
        {
            var dropList = config.drop;
            var drop = Util.ParseConfig(dropList);
            //var lastRd = 0.0f;
            //id rate num
            List<List<float>> allDrops = new List<List<float>>();
            foreach (var d in drop)
            {
                var rd = Random.Range(0, 1.0f);
                Log.Sys("random " + rd + " last " + " d " + d [1]);

                //100% 掉落
                if (d.Count >= 4)
                {
                    allDrops.Add(d);
                } else
                {
                    if (rd < d [1] * modify)
                    {
                        allDrops.Add(d);
                    }
                }
            }
            return allDrops;
        }

        public string GetDefaultWardrobe()
        {
            return jobConfig.DefaultWardrobe;
        }

        public bool IsNpc()
        {
            return npcConfig != null;
        }

        public UnitData(NpcConfigData n)
        {
            npcConfig = n;
        }

        /// <summary>
        /// 0 Monster
        /// 1 Player 
        /// 2 Npc
        /// </summary>
        /// <param name="isPlayer">If set to <c>true</c> is player.</param>
        /// <param name="mid">Middle.</param>
        /// <param name="level">Level.</param>
        public UnitData(bool isPlayer, int mid, int level)
        {
            IsPlayer = isPlayer;
            Log.Important("Init Unit Data is " + isPlayer + " " + mid + " " + level);
            if (!isPlayer)
            {
                config = GMDataBaseSystem.SearchIdStatic<MonsterFightConfigData>(GameData.MonsterFightConfig, mid);

            } else
            {
                jobConfig = GMDataBaseSystem.SearchIdStatic<RoleJobDescriptionsData>(GameData.RoleJobDescriptions, mid);
                RoleUpgradeConfigData lastInfo = null;
                foreach (RoleUpgradeConfigData r in GameData.RoleUpgradeConfig)
                {
                    if (r.job == mid && r.level == level)
                    {
                        playerConfig = r;
                        break;
                    }
                    if (r.job == mid)
                    {
                        lastInfo = r;
                    }
                }

                //等级超过了配置区间
                if (playerConfig == null)
                {
                    playerConfig = lastInfo;
                }

                Log.Important("jobConfig " + jobConfig + " playerConfig " + playerConfig);
            }
        }

    }

}