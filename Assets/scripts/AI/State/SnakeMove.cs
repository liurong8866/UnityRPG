using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    public class SnakeMove : MoveState
    {
        float WalkSpeed = 5;
        List<GameObject> tracePoints;
        int curTracePoint = 0;
        //一次性初始化代码
        public override void EnterState()
        {
            base.EnterState();
            aiCharacter.SetRun();
            FindNearPoint();
            WalkSpeed = GetAttr ().WalkSpeed;
        }

        void FindNearPoint()
        {
            tracePoints = GetAttr().spawnTrigger.GetComponent<SpawnTrigger>().GetTracePoint();
            var minDis = 999999.0f;
            curTracePoint = 0;
            var pos = GetAttr().transform.position;
            var c = 0;
            foreach (var p in tracePoints)
            {
                var dis = Util.XZSqrMagnitude(pos, p.transform.position);
                if (dis < minDis)
                {
                    curTracePoint = c;
                    minDis = dis;
                }
                c++;
            }

        }

        IEnumerator MoveAlongPoint()
        {
            var physic = GetAttr().GetComponent<PhysicComponent>();
            while (!quit)
            {
                if(CheckEvent()) {
                    break;
                }
                var nextTP = curTracePoint;
                var pos = GetAttr().transform.position;
                var tarPos = tracePoints [nextTP].transform.position;
                var dis = Util.XZSqrMagnitude(pos, tarPos);
                Log.AI("Move Target Distance "+tarPos+" dis "+dis +" pos "+pos);
                //足够靠近目标点了
                if (dis < 2f)
                {
                    curTracePoint++;
                    curTracePoint  %= tracePoints.Count;
                } else
                {
                    var dir = tarPos-pos;
                    dir.y = 0;
                    dir.Normalize();
                    physic.TurnTo(dir);
                    physic.MoveSpeed(dir * WalkSpeed);

                }
                yield return null;
            }
        }

        IEnumerator CheckFall(){
            while(!quit){
                var ori = GetAttr().OriginPos;
                if(GetAttr().transform.position.y < (ori.y-3)){
                    GetAttr().transform.position = GetAttr().OriginPos;    
                }
                yield return new WaitForSeconds(1);
            }
        }
        IEnumerator IdleSound()
        {
            while (!quit)
            {
                var rd = Random.Range(2, 4);
                yield return new WaitForSeconds(rd);
                var rd1 = Random.Range(1, 3);
                BackgroundSound.Instance.PlayEffectPos("batmanidle" + rd1, GetAttr().transform.position);
            }
        }

        public override IEnumerator RunLogic()
        {
            GetAttr().StartCoroutine(CheckFall());
            GetAttr().StartCoroutine(IdleSound());
            yield return GetAttr().StartCoroutine(MoveAlongPoint());
        }
    }

}