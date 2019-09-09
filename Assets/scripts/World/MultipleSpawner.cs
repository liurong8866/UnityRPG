using UnityEngine;
using System.Collections;
using System;

namespace MyLib
{
    [System.Serializable]
    public class MultipleSpawner
    {
        public bool Mark;
        public void SpawnChild(SpawnTrigger trigger)
        {
            var allChild = trigger.GetChildPoint();
            foreach(var c in allChild) {
                ObjectManager.objectManager.CreatePet(trigger.MonsterID, trigger.FirstMonster, (Affix)null, 
                                                       c.transform.position); 
            }

        }
    }

}