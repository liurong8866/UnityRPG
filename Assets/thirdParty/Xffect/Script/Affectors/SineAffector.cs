//----------------------------------------------
//            Xffect Editor
// Copyright © 2012- Shallway Studio
// http://shallway.net
//----------------------------------------------

using UnityEngine;
using System.Collections;
using Xft;

namespace Xft
{
    ///沿着本地的forward 方向 sine震动 时间频率和空间频率  振幅
	/// 震动是变化 X 坐标
    public class SineAffector : Affector
    {
        /*
        protected float Magnitude;
        protected float SineFreqTime = 1;
        protected float SineFreqDist = 1;

        public SineAffector(float mag, float sineFreqTime, float sineFreqDist, EffectNode node)
            : base(node, AFFECTORTYPE.SineAffector)
        {
            Magnitude = mag;
            SineFreqTime = sineFreqTime;
            SineFreqDist = sineFreqDist;
        }
        */
        protected Vector3 SineForce;
        protected bool ModifyPos;
        protected float SineMaxFreq;
        protected float SineMinFreq;
        float SineFreq = 1;
        public SineAffector(Vector3 force, bool modifyPos, float maxFreq, float minFreq, EffectNode node)
            :base(node, AFFECTORTYPE.SineAffector)
        {
            SineForce = force;
            ModifyPos = modifyPos;
            SineMaxFreq = maxFreq;
            SineMinFreq = minFreq;
            SineFreq = SineMinFreq;
        }
        public override void Update(float deltaTime)
        {
            float strength = 0;
            strength = 1;
            float t = Node.GetElapsedTime();
            float st = Mathf.Sin(2*Mathf.PI*SineFreq*t);
            strength = st*strength;
            Vector3 syncDir = Node.Owner.ClientTransform.rotation * SineForce;
            if(ModifyPos) {
                Node.Position += syncDir*strength*deltaTime;
            }else {
                Node.Velocity += syncDir*strength*deltaTime;
            }


        }
    }
}
