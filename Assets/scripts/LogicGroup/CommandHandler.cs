using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// set_group
/// input_event eventName nodeId  (actived)
/// actived
/// start_particle
/// play
/// </summary>
public class CommandHandler : MonoBehaviour {
    public class Command{
        public string[] cmd;
    }
    public delegate void CmdDelegate(Command cmd);

    //List<Command> cmds = new List<Command>();
    Dictionary<string, CmdDelegate> cmdMaps = new Dictionary<string, CmdDelegate>();

    public void AddHandler(string cmd, CmdDelegate del){
        cmdMaps.Add(cmd, del);
    }
    public void AddCommand(string cmd) {
        var c = new Command(){cmd=cmd.Split(' ')};
        if(cmdMaps.ContainsKey(c.cmd[0])) {
            cmdMaps[c.cmd[0]](c);
        }
    }

	
}
