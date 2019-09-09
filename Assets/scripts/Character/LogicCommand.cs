
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/

/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    /*
 * Object Command List
 */
    public class ObjectCommand
    {
        public enum ENUM_OBJECT_COMMAND
        {
            INVALID = -1,
            OC_MOVE,
            OC_UPDATE_IMPACT,
            //Buff Update
            OC_USE_SKILL,
            OC_SUPER_MOVE,
        }

        public int skillId;
        public int skillLevel;

        public ENUM_OBJECT_COMMAND commandID;
        public int logicCount;
        public uint startTime;

        public Vector3 targetPos;
        public int dir;

        public GCPushUnitAddBuffer buffInfo;

        public ObjectCommand()
        {
        }


        public ObjectCommand(ENUM_OBJECT_COMMAND cmd)
        {
            commandID = cmd;
        }
    }



    /*
	 * Process Network Command 
	 * Change Character State
	 */
    /// <summary>
    /// 执行网络命令在特定的玩家 或者 怪兽身上
    /// 将网络命令转化成本地命令 在对象身上执行
    /// </summary>
    public class LogicCommand : MonoBehaviour
    {
        NpcAttribute attribute;
        MoveController mvController;

        List<ObjectCommand> commandList = new List<ObjectCommand>();
        ObjectCommand currentLogicCommand = null;
        float logicSpeed = 1;

        /*
		 * NetWork Command Input
		 */
        public void PushCommand(ObjectCommand cmd)
        {
            Log.Important("Push Command What " + cmd.commandID+" target "+cmd.targetPos+" dir "+cmd.dir);
            commandList.Add(cmd);
        }

        bool DoLogicCommand(ObjectCommand cmd)
        {
            Log.AI("DoLogicCommad " + cmd.commandID);
            currentLogicCommand = cmd;

            bool ret = false;
            switch (cmd.commandID)
            {
                case ObjectCommand.ENUM_OBJECT_COMMAND.OC_MOVE:
                    StartCoroutine(Move());
                    break;
                case ObjectCommand.ENUM_OBJECT_COMMAND.OC_USE_SKILL:
                    EnterUseSkill(cmd);
                    break;
                default:
                    Debug.LogError("LogicCommand:: UnImplement Command " + cmd.commandID.ToString());
                    break;
            }
            return ret;
        }

        //所有命令执行函数需要注意命令执行结束的时候 设定当前命令为null
        void EnterUseSkill(ObjectCommand cmd)
        {
            //判断是否可以使用技能
            var msg = new MyAnimationEvent.Message(MyAnimationEvent.MsgType.DoSkill);
            msg.skillData = Util.GetSkillData(cmd.skillId, cmd.skillLevel);
            GetComponent<MyAnimationEvent>().InsertMsg(msg);
            currentLogicCommand = null;
        }


        /*
         * 监督执行移动命令当有新的移动命令注入的时候当前的移动命令则失效 
         * 异常拯救函数
		 */
        IEnumerator Move()
        {
            var samplePos = transform.position; //new List<Vector3>(){transform.position};
            var lastSameTime = 0f;
            while (currentLogicCommand != null)
            {
                Vector3 mypos = transform.position;

                var lastOne = samplePos;
                var diff = Util.XZSqrMagnitude(lastOne, mypos);
                if (diff < 1)
                {
                    if(lastSameTime == 0) {
                        lastSameTime = Time.time;
                    }
                } else
                {
                    lastSameTime = 0;
                    samplePos = mypos;
                }


                Vector3 tarPos = currentLogicCommand.targetPos;
                float dx = tarPos.x - mypos.x;
                float dz = tarPos.z - mypos.z;
                var dy = tarPos.y-mypos.y;
                if(Mathf.Abs(dy) > 0.5f) {
                    transform.position = tarPos;
                }

                Vector2 vdir = new Vector2(dx, dz);
                if (vdir.sqrMagnitude < 0.1f)
                {
                    mvController.vcontroller.inputVector.x = 0;
                    mvController.vcontroller.inputVector.y = 0;
                    break;
                }

                //超过1s卡在某个位置 和目标位置相差超过1距离 使用超能移动 和目标相距3长时间
                if (lastSameTime != 0 && (Time.time - lastSameTime > 1) && vdir.sqrMagnitude >= 2)
                {
                    transform.position = tarPos;
                    break;
                }

                vdir.Normalize();

                Vector2 mRight = new Vector2(mvController.camRight.x, mvController.camRight.z);
                Vector2 mForward = new Vector2(mvController.camForward.x, mvController.camForward.z);

                float hval = Vector2.Dot(vdir, mRight);
                float vval = Vector2.Dot(vdir, mForward);

                mvController.vcontroller.inputVector.x = hval;
                mvController.vcontroller.inputVector.y = vval;
                yield return null;
            }

            //简单实现
            if(currentLogicCommand != null) {
                transform.localRotation = Quaternion.Euler(new Vector3(0, currentLogicCommand.dir, 0));
            }
            //再次检测Move位置状态，如果不正常则重新开始
            currentLogicCommand = null;
        }


        /*
		 * 执行当前的移动命令
         *  若上一个命令是移动命令则替换
         *  若上一个命令是技能命令则等待
		 */
        IEnumerator CommandHandle()
        {
            while (true)
            {
                if (commandList.Count > 0)
                {
                    var peekCmd = commandList [0];
                    if (peekCmd.commandID == ObjectCommand.ENUM_OBJECT_COMMAND.OC_MOVE)
                    {
                        if (currentLogicCommand != null && currentLogicCommand.commandID == ObjectCommand.ENUM_OBJECT_COMMAND.OC_MOVE)
                        {
                            currentLogicCommand = peekCmd;
                            commandList.RemoveAt(0);
                        }
                    }

                    if (currentLogicCommand == null)
                    {
                        var cmd = commandList [0];
                        commandList.RemoveAt(0);
						
                        int logicCommandCount = commandList.Count;
                        logicSpeed = logicCommandCount * 0.5f + 1.0f;
                        DoLogicCommand(cmd);
                    }
                }
                yield return null;
            }
        }

        void Start()
        {
            attribute = GetComponent<NpcAttribute>();
            mvController = GetComponent<MoveController>();
            //mvController.vcontroller = new VirtualController();

            StartCoroutine(CommandHandle());
        }
    }
}
