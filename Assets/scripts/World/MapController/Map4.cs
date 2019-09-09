using UnityEngine;
using System.Collections;
using System;

namespace MyLib
{

    public class Map4 :CScene
    {
        public override bool ShowTeamColor
        {
            get
            {
                return true;
            }
        }



        public override bool IsNet
        {
            get
            { 
                return true;
            }
        }


        private NetworkScene netScene;

        protected override void Awake()
        {
            base.Awake();
            netScene = gameObject.AddComponent<NetworkScene>();
        }

        public override void BroadcastMsg(CGPlayerCmd.Builder cmd)
        {
            netScene.BroadcastMsg(cmd);
        }

    }

}
