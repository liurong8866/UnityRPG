using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class MissileSpawn : MonoBehaviour
    {

        public Vector3 Position = Vector3.zero;
        public int count = 3;
        public float offSetX = 0.5f;
        public MissileData Missile;

        SkillLayoutRunner runner;

        void Start()
        {
            runner = transform.parent.GetComponent<SkillLayoutRunner>();
            StartCoroutine(UpdateUnitSpawn());
        }

        void MakeMissile(int deg)
        {
            Log.AI("bullet degree " + deg);
            var offX = new float[]{
                0,
                -offSetX,
                offSetX,
            };
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

            bullet.transform.localPosition = attacker.transform.localPosition + new Vector3(offX[deg], 0, 0);
            bullet.transform.localRotation = attacker.transform.localRotation ;
        }

        IEnumerator UpdateUnitSpawn()
        {
            for (int i = 0; i < count; i++)
            {
                MakeMissile(i);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

}