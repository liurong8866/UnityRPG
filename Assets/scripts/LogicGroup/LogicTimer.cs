using UnityEngine;
using System.Collections;

public class LogicTimer : LogicNode
{
    public bool loop = false;
    public float time = 0.95f;
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        StartCoroutine(LoopTimer());
    }

    IEnumerator LoopTimer()
    {

        while (true)
        {
            yield return new WaitForSeconds(time);

            if(logicGroup != null) {
                logicGroup.GetComponent<CommandHandler>().AddCommand(string.Format("input_event actived {0}", myId));
            }
            if (!loop)
            {
                break;
            }
        }

    }
}
