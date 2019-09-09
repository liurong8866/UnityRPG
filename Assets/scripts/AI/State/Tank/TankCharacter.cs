using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class TankCharacter : AICharacter
    {
        public override void SetRun()
        {
            /*
            var runName = "run";
            if (WorldManager.worldManager.IsPeaceLevel())
            {
                runName = "run1";
            } else
            {
                runName = "run2";
            }
            if (!CheckAni(runName))
            {
                runName = "run";
            }
            if (!CheckAni(runName))
            {
                runName = "walk";
            }
            Log.AI("RunAnimation name " + runName);
            PlayAni(runName, GetAttr().ObjUnitData.WalkAniSpeed, WrapMode.Loop);
            */
        }

        public override void PlayAni(string name, float speed, WrapMode wm)
        {
            /*
            var ani = GetAttr().animation;
            ani [name].speed = speed;
            ani [name].wrapMode = wm;
            GetAttr().GetComponent<AnimationController>().PlayAnimation(ani [name]);
            */
        }

        public override void SetAni(string name, float speed, WrapMode wm)
        {
            /*
            var ani = GetAttr().animation;
            ani [name].speed = speed;
            ani [name].wrapMode = wm;
            GetAttr().GetComponent<AnimationController>().CrossfadeAnimation(ani [name], 0.2f);
            */
        }

        public override void SetIdle()
        {
            /*
            var peace = WorldManager.worldManager.IsPeaceLevel();
            var idleName = "idle";
            if (peace)
            {
                if (CheckAni("stand"))
                {
                    idleName = "stand";
                }
            } 
            PlayAni(idleName, 1, WrapMode.Loop);
            */
        }

    }
}