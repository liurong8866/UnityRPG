using UnityEngine;
using System.Collections;
using System;

namespace MyLib
{

    public class Map5 :CScene
    {
        public override bool ShowTeamColor
        {
            get
            {
                return true;
            }
        }

        public override bool IsEnemy(GameObject a, GameObject b)
        {
            var aattr = NetworkUtil.GetAttr(a);
            var battr = NetworkUtil.GetAttr(b);
            if (aattr != null && battr != null)
            {
                return a != b && b.tag == GameTag.Player && aattr.TeamColor != battr.TeamColor;
            }
            return false;
        }

        public override bool IsNet
        {
            get
            { 
                return true;
            }
        }

        public override bool IsRevive
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

        void Start()
        {
            gameObject.AddComponent<ScoreManager>();
            netScene.InitMap();
        }

        public override void BroadcastMsg(CGPlayerCmd.Builder cmd)
        {
            if (state == SceneState.InGame)
            {
                netScene.BroadcastMsg(cmd);
            }
        }

    }

}
