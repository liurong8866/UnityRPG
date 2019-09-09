using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class WolfSpawn : MonoBehaviour
    {
        public int MonsterId = -1;
        public int num = 0;
        SkillLayoutRunner runner;
        // Use this for initialization
        void Start()
        {
            runner = transform.parent.GetComponent<SkillLayoutRunner>();
            StartCoroutine(MakeMonster());    
        }

        IEnumerator  MakeMonster()
        {
            Affix af = null;
            if (runner.Event.affix.target == Affix.TargetType.Pet)
            {
                af = runner.Event.affix;
            }

            for (int i = 0; i < num; i++)
            {
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

                yield return new WaitForSeconds(0.1f);
            }
        }
    
    }

}