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

    public enum GAFTTYPE
    {
        Planar,
        Spherical
    }

    public class GravityAffector : Affector
    {
        protected GAFTTYPE GType;
        protected MAGTYPE MType;
        protected float Magnitude;
        protected AnimationCurve MagCurve;
        protected Vector3 Dir;
        protected Transform GravityObj;
        protected bool IsAccelerate = true;
        protected bool IsSine = false;
        protected float SineFreq = 1;

        public void SetAttraction(Transform goal)
        {
            GravityObj = goal;
        }

        public GravityAffector(Transform obj, GAFTTYPE gtype, MAGTYPE mtype,bool isacc, Vector3 dir, float mag,AnimationCurve curve,EffectNode node, bool isSine, float sineFreq)
            : base(node, AFFECTORTYPE.GravityAffector)
        {
            GType = gtype;
            MType = mtype;
            Magnitude = mag;
            MagCurve = curve;
            Dir = dir;
            Dir.Normalize();
            GravityObj = obj;
            IsAccelerate = isacc;
            IsSine = isSine;
            SineFreq = sineFreq;
        }

        public override void Update(float deltaTime)
        {
            float strength = 0f;

            if(IsSine) {
                strength = Magnitude;
                float t = Node.GetElapsedTime();
				float st = Mathf.Sin(2*Mathf.PI*SineFreq*t);
				strength = st*strength;

            }else if (MType == MAGTYPE.Fixed)
                strength = Magnitude;
            else
                strength = MagCurve.Evaluate(Node.GetElapsedTime());

   
            if (GType == GAFTTYPE.Planar)
            {
                Vector3 syncDir = Node.Owner.ClientTransform.rotation * Dir;
                if (IsAccelerate)
                    Node.Velocity += syncDir * strength * deltaTime;
                else
                    Node.Position += syncDir * strength * deltaTime;
            }
            else if (GType == GAFTTYPE.Spherical)
            {
                Vector3 dir;
                dir = GravityObj.position - Node.GetOriginalPos();
                if (IsAccelerate)
                    Node.Velocity += dir * strength * deltaTime;
                else
                {
                    Node.Position += dir.normalized * strength * deltaTime;
                }  
            }
        }
    }
}
