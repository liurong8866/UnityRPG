using UnityEngine;
using System.Collections;
using MyLib;

#if UNITY_EDITOR
public class RemoveMonsterInZone : MonoBehaviour
{
    [ButtonCallFunc()]public bool Show;

    public void ShowMethod()
    {
        var pro = transform.Find("properties");
        foreach (Transform t in pro)
        {
            var spawnChest = t.GetComponent<SpawnChest>();
            if(spawnChest != null) {
                spawnChest.ResetMethod();
                spawnChest.UpdateEditor();
            }
        }
    }

    [ButtonCallFunc()]
    public bool Remove;
    public void RemoveMethod()
    {
        var pro = transform.Find("properties");
        foreach (Transform t in pro)
        {
            var trigger = t.GetComponent<SpawnTrigger>();
            var spawnNpc = t.GetComponent<SpawnNpc>();
            var spawnChest = t.GetComponent<SpawnChest>();
            if (trigger != null || spawnNpc != null || spawnChest != null)
            {

                if (trigger != null)
                {
                    trigger.ClearChildren();
                } else
                {
                    for (int i = 0; i < t.childCount;)
                    {
                        if (t.GetChild(i).name.Contains("Child"))
                        {
                            i++;
                        } else
                        {
                            GameObject.DestroyImmediate(t.GetChild(i).gameObject);
                        }
                    }
                }
            }
        }

        pro = transform.Find("npcs");
        if (pro != null)
        {
            foreach (Transform t in pro)
            {
                var trigger = t.GetComponent<SpawnTrigger>();
                var spawnNpc = t.GetComponent<SpawnNpc>();
                if (trigger != null || spawnNpc != null)
                {
                    for (int i = 0; i < t.childCount; i++)
                    {
                        GameObject.DestroyImmediate(t.GetChild(i).gameObject);

                    }
                }
            }
        }
    }

    [ButtonCallFunc()]
    public bool
        Reset;

    public void ResetMethod()
    {
        var pro = transform.Find("properties");
        foreach (Transform t in pro)
        {
            var trigger = t.GetComponent<SpawnTrigger>();
            if (trigger != null)
            {
                trigger.reset = true;
                trigger.UpdateEditor();
                trigger.UpdateEditor();
            }
        }
    }

    // Use this for initialization
    void Start()
    {
    
    }
    
    // Update is called once per frame
    void Update()
    {
    
    }
}

#endif