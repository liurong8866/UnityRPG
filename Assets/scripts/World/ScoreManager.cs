using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    public class ScoreData
    {
        public int killed = 0;
        public int beKilled = 0;
    }

    /// <summary>
    /// 统计得分
    /// 倒计时
    /// 显示游戏结束面板 
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        public int leftTime = 300;
        public Dictionary<int, ScoreData> score = new Dictionary<int, ScoreData>();
        LeftTimeUI ltui;
        ScoreUI scoreUI;

        public static ScoreManager Instance;
        void Awake() {
            Instance = this;
        }
        void Start()
        {
            StartCoroutine(CountDown());
        }

        public void NetAddScore(int attacker, int enemy) {
            if(!score.ContainsKey(attacker)) {
                score[attacker] = new ScoreData();
            }
            score[attacker].killed++;

            if(!score.ContainsKey(enemy)) {
                score[enemy] = new ScoreData();
            }
            score[enemy].beKilled++;

            scoreUI.SetData(score);
        }

        public void NetSyncTime(int lt) {
            leftTime = lt;
        }
        public void NetworkGameOver()
        {
            Log.Sys("NetworkGameOver: "+leftTime);
            leftTime = 0;
            ltui.SetLabel("游戏结束，请点击退出按钮");
            var active = WorldManager.worldManager.GetActive();
            //不能操控也别接受控制命令了 也不发送网络命令了
            active.state = SceneState.GameOver;

            var player = ObjectManager.objectManager.GetMyPlayer();
            var ai = player.GetComponent<AIBase>().GetAI();
            ai.ChangeStateForce(AIStateEnum.STOP);
        }


        IEnumerator CountDown()
        {
            while(WorldManager.worldManager.station != WorldManager.WorldStation.Enter) {
                yield return null;
            }

            var uiRoot = WindowMng.windowMng.GetMainUI();
            var lt = NGUITools.AddChild(uiRoot, Resources.Load<GameObject>("UI/LeftTimeUI"));
            ltui = lt.GetComponent<LeftTimeUI>();
            var sui  = NGUITools.AddChild(uiRoot, Resources.Load<GameObject>("UI/ScoreUI"));
            scoreUI = sui.GetComponent<ScoreUI>();

            var rtp = Util.FindChildRecursive(uiRoot.transform, "RightTop");
            rtp.gameObject.SetActive(false);

            while (leftTime > 0)
            {
                ltui.SetLabel("" + leftTime);
                if (NetworkUtil.IsNetMaster())
                {
                    NetDateInterface.SyncTime(leftTime);
                }
                leftTime--;
                yield return new WaitForSeconds(1);
            }
            if (NetworkUtil.IsNetMaster())
            {
                NetDateInterface.GameOver();
                NetworkGameOver();
            }
        }
    }
}