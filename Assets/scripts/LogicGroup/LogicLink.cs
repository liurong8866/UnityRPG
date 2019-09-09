using UnityEngine;
using System.Collections;
[System.Serializable]
public class LogicLink {
    public GameObject source;
    public string output;

    public GameObject target;
    public string input;
    public void Handle(){

        Debug.Log(target+" "+input);
        var ch = target.GetComponent<CommandHandler>();
        ch.AddCommand(input);
    }

}
