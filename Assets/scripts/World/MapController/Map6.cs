using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class Map6 : CScene
    {
        public override bool IsNet
        {
            get
            {
                return true;
            }
        }

        //和服务器建立连接之后 连接不会断开不会因为切换场景而删除掉这个map只负责网络连接
        private NetMatchScene netScene;

        protected override void Awake()
        {
            base.Awake();
            var go = new GameObject("NetMatchScene");
            netScene = go.AddComponent<NetMatchScene>();
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