using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class MatchRoom : MonoBehaviour
    {
        private NetMatchScene nm;

        void Start()
        {
            nm = GetComponent<NetMatchScene>();
        }

        public void SyncAvatarInfo(AvatarInfo newInfo, AvatarInfo oldInfo)
        {
            if (newInfo.HasIsMaster)
            {
                oldInfo.IsMaster = newInfo.IsMaster;
            }
            if (newInfo.HasTeamColor)
            {
                oldInfo.TeamColor = newInfo.TeamColor;
            }
        }

        public AvatarInfo GetMyInfo()
        {
            var myId = nm.myId;
            var roomInfo = nm.roomInfo;
            foreach (var p in roomInfo.PlayersList)
            {
                if (p.Id == myId)
                {
                    return p;
                }
            }
            return null;
        }

        public bool IsMeMaster()
        {
            var myId = nm.myId;
            var roomInfo = nm.roomInfo;
            foreach (var p in roomInfo.PlayersList)
            {
                if (p.Id == myId)
                {
                    return p.IsMaster;
                }
            }
            return false;
        }

        private int needPlayer = 2;
        public bool GetPlayerFull()
        {
            return nm.roomInfo.PlayersCount >= needPlayer;
        }

        public int GetPlayerNum() {
            return nm.roomInfo.PlayersCount;
        }
    }

}