using UnityEngine;
using System.Collections;

namespace MyLib
{
    /// <summary>
    /// 在0.5s中释放12次对象 位置是从2 到 12  释放的半径 和 释放的 角度范围限定 曲线产生一条直线
    /// </summary>
    public class UnitSpawner : MonoBehaviour
    {
        SkillLayoutRunner runner;

        /// <summary>
        /// 子弹向前
        /// 子弹由中心 向四周 
        /// </summary>
        public enum Direction
        {
            Forward,
            OutwardFromCenter,
        }

        /// <summary>
        /// 子弹初始偏移位置 
        /// </summary>
        public Vector3 Position = Vector3.zero;
        /// <summary>
        /// 发射子弹的数量 
        /// </summary>
        public int count = 1;
        public float duration = 0.5f;
        public Direction direction = Direction.Forward;

        public float Angle = 15;

        public float MaxMagnitude = 1;
        public AnimationCurve MaxRadius = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        public AnimationCurve MinRadius = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

        public bool UseMaxOnly = true;

        public GameObject particle;
        public MissileData Missile;
        //产生怪物的id
        public int MonsterId = -1;
        public float delay = 0;

        public enum ReleaseOrder
        {
            ByRandom,
            Clockwise,
        }

        public ReleaseOrder releaseOrder = ReleaseOrder.ByRandom;

        /// <summary>
        /// 初始化Runner
        /// </summary>
        void Start()
        {
            runner = transform.parent.GetComponent<SkillLayoutRunner>();
            if (runner.stateMachine.attacker != null)
            {
                var spawnT = runner.stateMachine.attacker.GetComponent<NpcAttribute>().spawnTrigger;
                if (spawnT != null)
                {
                    var chest = spawnT.GetComponent<SpawnChest>();
                    if (chest != null)
                    {
                        MonsterId = chest.MonsterID;
                    }
                }
            }
            StartCoroutine(UpdateUnitSpawn());
        }

        /// <summary>
        /// 生成子弹同时附着上当前Runner所对应事件的Affix 
        /// </summary>
        /// <param name="deg">Deg.</param>
        void MakeMissile(float deg)
        {
            Log.AI("bullet degree " + deg);

            var b = new GameObject("bullet_" + Missile.name);
            var bullet = b.AddComponent<Bullet>();
            bullet.OffsetPos = Position;
            GameObject attacker = null;
            if (runner != null)
            {
                bullet.skillData = runner.stateMachine.skillFullData.skillData;
                attacker = runner.stateMachine.attacker;
                bullet.attacker = runner.stateMachine.attacker;
                bullet.runner = runner;
            }

            bullet.missileData = Missile;
            bullet.transform.parent = ObjectManager.objectManager.transform;

            if (runner.stateMachine.forwardSet)
            {
                var playerForward = Quaternion.Euler(new Vector3(0, 0 + runner.transform.rotation.eulerAngles.y, 0));
                var bulletForward = Quaternion.Euler(new Vector3(0, deg + runner.transform.eulerAngles.y, 0));
                bullet.transform.localPosition = runner.transform.localPosition + playerForward * Position;
                bullet.transform.localRotation = bulletForward;
            } else
            {
                var playerForward = Quaternion.Euler(new Vector3(0, 0 + attacker.transform.rotation.eulerAngles.y, 0));
                var bulletForward = Quaternion.Euler(new Vector3(0, deg + attacker.transform.eulerAngles.y, 0));
                bullet.transform.localPosition = attacker.transform.localPosition + playerForward * Position;
                bullet.transform.localRotation = bulletForward;
            }
        }

        void MakeMonster()
        {
            Affix af = null;
            if (runner.Event.affix.target == Affix.TargetType.Pet)
            {
                af = runner.Event.affix;
            }
            
            var pos = gameObject.transform.position + runner.stateMachine.InitPos; 
            if (runner.stateMachine.attacker != null)
            {
                var npc = runner.stateMachine.attacker.GetComponent<NpcAttribute>();
                if (npc.spawnTrigger != null)
                {
                    var c = npc.spawnTrigger.transform.childCount;
                    if (c > 0)
                    {
                        var rd = Random.Range(0, c);
                        var child = npc.spawnTrigger.transform.GetChild(rd);
                        pos = child.transform.position;
                        Log.AI("CreateMonster Use Dynamic Pos " + pos + " index " + rd);
                    }
                }
            }
            ObjectManager.objectManager.CreatePet(MonsterId, runner.stateMachine.attacker, af, 
                pos); 
        }

        /// <summary>
        /// 子弹生成 
        /// </summary>
        /// <returns>The unit spawn.</returns>
        IEnumerator UpdateUnitSpawn()
        {
            if (delay > 0)
            {
                yield return new WaitForSeconds(delay);
            }
            float passTime = 0;
            int lastFrame = -1;
            float initDeg = -Angle / 2.0f;
            float diffDeg = 0;
            if (count > 1)
            {
                diffDeg = Angle * 1.0f / (count - 1);
            }

            while (lastFrame < (count - 1))
            {
                float radius = 0;
                float rate = 0;
                if (duration == 0)
                {
                    rate = 1;
                } else
                {
                    rate = passTime / duration;
                }
                int frame = (int)(rate * count);
                if (UseMaxOnly)
                {
                    radius = MaxRadius.Evaluate(rate);
                } else
                {
                    radius = Random.Range(MinRadius.Evaluate(rate), MaxRadius.Evaluate(rate));
                }

                radius *= MaxMagnitude;

                if (frame > lastFrame && lastFrame < (count - 1))
                {
                    lastFrame++;
                    if (particle != null)
                    {
                        var par = Instantiate(particle) as GameObject;
                        par.transform.parent = transform;
                        var rot = Quaternion.Euler(new Vector3(0, Random.Range(0, Angle), 0));
                        par.transform.localPosition = rot * (new Vector3(0, 0, radius));
                        par.transform.localRotation = Quaternion.identity;
                        par.transform.localScale = Vector3.one;
                        par.GetComponent<XffectComponent>().enabled = true;
                        //Missle挂在Layout下面来决定什么时候摧毁Missile
                    } else if (Missile != null)
                    {
                        Log.Ani("Missile spawn");
                        if (direction == Direction.Forward)
                        {
                            MakeMissile(0);
                        } else if (direction == Direction.OutwardFromCenter)
                        {
                            MakeMissile(initDeg + diffDeg * lastFrame);
                        }
                    } else if (MonsterId != -1)
                    {
                        MakeMonster();	
                    }
                }

                passTime += Time.deltaTime;
                yield return null;
            }
        }

    }
}
