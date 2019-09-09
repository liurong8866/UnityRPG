using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class BombSpawn : MonoBehaviour
    {
        SkillLayoutRunner runner;
        public BombData bombData;
        public Vector3 Position;

        void Start()
        {
            runner = transform.parent.GetComponent<SkillLayoutRunner>();
            MakeBomb();	
        }

        void MakeBomb()
        {
            var b = new GameObject("bomb_" + bombData.name);
            var bomb = b.AddComponent<Bomb>();
            bomb.OffsetPos = Position;

            GameObject attacker = null;
            if (runner != null)
            {
                bomb.skillData = runner.stateMachine.skillFullData.skillData;
                attacker = runner.stateMachine.attacker;
                bomb.runner = runner;
                bomb.attacker = attacker;
            }
            bomb.bombData = bombData;

            bomb.transform.parent = ObjectManager.objectManager.transform;
            var playerForward = Quaternion.Euler(new Vector3(0, 0 + attacker.transform.rotation.eulerAngles.y, 0));

            bomb.transform.localPosition = attacker.transform.localPosition + playerForward * Position;
            bomb.transform.localRotation = playerForward;
        }
    }

}