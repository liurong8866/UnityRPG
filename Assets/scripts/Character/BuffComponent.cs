using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MyLib
{
    public class BuffComponent : MonoBehaviour
    {
        List<IEffect> effectList = new List<IEffect>();

        void Update()
        {
            foreach (IEffect ef in effectList)
            {
                if (!ef.IsDie)
                {
                    ef.OnUpdate();
                }
            }
            for (int i = 0; i < effectList.Count;)
            {
                var ef = effectList [i];
                if (ef.IsDie)
                {
                    ef.OnDie();
                    effectList.RemoveAt(i);
                } else
                {
                    i++;
                }
            }
        }

        public bool CheckHasSuchBuff(Affix.EffectType effectType)
        {
            for (int i = 0; i < effectList.Count; i++)
            {
                if (effectList [i].affix.effectType == effectType)
                {
                    return true;
                }
            } 
            return false;
        }

        public void RemoveBuff(Affix.EffectType type)
        {
            foreach (var a in effectList)
            {
                if (a.affix.effectType == type)
                {
                    a.IsDie = true;
                    break;
                }
            }
        }

        public bool AddBuff(Affix affix, Vector3 attackerPos = default(Vector3))
        {
            if (affix != null)
            {
                Log.Sys("AddBuff is " + gameObject.name + " " + affix.effectType);
                //只保留最旧的Buff
                if (affix.keepOld)
                {
                    for (int i = 0; i < effectList.Count; i++)
                    {
                        if (effectList [i].affix.effectType == affix.effectType)
                        {
                            return false;
                        }
                    }
                }

                var eft = BuffManager.buffManager.GetBuffInstance(affix.effectType);
                var buff = (IEffect)Activator.CreateInstance(eft);
                buff.Init(affix, gameObject);
                buff.attackerPos = attackerPos;

                if (affix.IsOnlyOne)
                {
                    for (int i = 0; i < effectList.Count; i++)
                    {
                        if (effectList [i].affix.effectType == affix.effectType)
                        {
                            effectList [i].IsDie = true;
                        }
                    }
                }
                effectList.Add(buff);
                buff.OnActive();
                return true;
            }
            return false;
        }

        public int GetArmor()
        {
            int addArmor = 0;
            foreach (IEffect ef in effectList)
            {
                addArmor += ef.GetArmor();
            }
            return addArmor;
        }

        public float GetSpeedCoff()
        {
            float speedCoff = 1;
            foreach (IEffect ef in effectList)
            {
                speedCoff *= ef.GetSpeedCoff();
            }
            return speedCoff;
        }

        public int GetCriticalRate()
        {
            int rate = 0;
            foreach (IEffect ef in effectList)
            {
                rate += ef.GetCriticalRate();
            }
            return rate;
        }

        public bool CanUseSkill()
        {
            foreach (var ef in effectList)
            {
                if (!ef.CanUseSkill())
                {
                    return false;
                }
            }
            return true;
        }


        void OnDisable()
        {
            foreach (IEffect ef in effectList)
            {
                ef.OnDie();
            }
        }
    }

}