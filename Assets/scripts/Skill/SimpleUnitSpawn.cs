using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class SimpleUnitSpawn : MonoBehaviour
    {
        SkillLayoutRunner runner;
        public int MonsterId = -1;

        void Start()
        {
            runner = transform.parent.GetComponent<SkillLayoutRunner>();
            UpdateUnitSpawn();
        }

        void MakeMonster(){
             Affix af = null;
            if(runner.Event.affix.target == Affix.TargetType.Pet) {
                af = runner.Event.affix;
            }

            Vector3 pos = gameObject.transform.position;
            if(runner.triggerEvent != null && runner.triggerEvent.type == MyEvent.EventType.EventMissileDie) {
                pos = runner.triggerEvent.missile.transform.position;
            }
            ObjectManager.objectManager.CreatePet(MonsterId, runner.stateMachine.attacker, af, 
                                                  pos); 
        }


        void  UpdateUnitSpawn()
        {
            if(MonsterId != -1){
                MakeMonster();  
            }
        }

    }

}