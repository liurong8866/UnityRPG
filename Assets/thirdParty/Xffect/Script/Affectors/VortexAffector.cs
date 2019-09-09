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
	public class VortexAffector : Affector
	{
		AnimationCurve VortexCurve;
		protected Vector3 Direction;
		protected Transform VortexObj;
		protected MAGTYPE MType;
		protected bool InheritRotation;
		protected bool IsStandTail = false;

		float Magnitude;
        float MagnitudeMax;
        
        protected bool IsFirst = true;
        protected float OriginalRadius = 0f;
        protected float VortexTime = -1;
        
        public override void Reset ()
        {
            IsFirst = true;
            OriginalRadius = 0f;
        }
        float realMag = 1;
        public VortexAffector (Transform obj, MAGTYPE mtype, float mag, float magMax, AnimationCurve vortexCurve, Vector3 dir, bool inhRot, EffectNode node, float vt)
            : base(node, AFFECTORTYPE.VortexAffector)
		{
			VortexCurve = vortexCurve;
			Direction = dir;
			InheritRotation = inhRot;
			VortexObj = obj;
			MType = mtype;
			Magnitude = mag;
            MagnitudeMax = magMax;

			VortexTime = vt;
			//ver 1.2.1
			if (node.Owner.IsRandomVortexDir) {
				Direction.x = Random.Range (-1f, 1f);
				Direction.y = Random.Range (-1f, 1f);
				Direction.z = Random.Range (-1f, 1f);
			}
			Direction.Normalize ();
            IsFirst = true;
            if(MType == MAGTYPE.RANDOM) {
                realMag = Random.Range(Magnitude, MagnitudeMax);
            }
		}

		public override void Update (float deltaTime)
		{
			Vector3 diff;

			diff = Node.GetOriginalPos() - VortexObj.position;
            
            Vector3 direction = Direction;
             if (InheritRotation)
                 direction = Node.Owner.ClientTransform.rotation * direction;
            
            if (IsFirst)
            {
                IsFirst = false;
                OriginalRadius = (diff - Vector3.Project(diff,direction)).magnitude;
            }

			float distanceSqr = diff.sqrMagnitude;

			if (distanceSqr < 1e-06f)
				return;
			//Vortex is Trail 
			if(IsStandTail) {
				float segParam = Vector3.Dot (direction, diff);
				diff -= segParam * direction;

				Vector3 deltaV = Vector3.zero;
				if (diff == Vector3.zero) {
					deltaV = diff;
				} else {
					deltaV = Vector3.Cross (direction, diff).normalized;
				}
				float time = Node.GetElapsedTime ();
				float magnitude;
				if (MType == MAGTYPE.Curve) {
					float totalTime = VortexTime;
					if(VortexTime == -1) {
						totalTime = Node.GetRealLife();
					}
					magnitude = VortexCurve.Evaluate (time/totalTime) * Magnitude;
                }else if(MType == MAGTYPE.Fixed) {
					magnitude =  Magnitude;
                }else {
                    magnitude = realMag;
                }

				if (Node.Owner.VortexAttenuation < 1e-04f)
				{
					deltaV *= magnitude * deltaTime;
				}
				else
				{
					deltaV *= magnitude * deltaTime / Mathf.Pow(Mathf.Sqrt(distanceSqr),Node.Owner.VortexAttenuation);
				}
				//Adjust Node Position by deltaV
				//Node.
			}
			else if (!Node.Owner.UseVortexMaxDistance || (distanceSqr <= Node.Owner.VortexMaxDistance * Node.Owner.VortexMaxDistance)) 
			{
				float segParam = Vector3.Dot (direction, diff);
				diff -= segParam * direction;

				Vector3 deltaV = Vector3.zero;
				if (diff == Vector3.zero) {
					deltaV = diff;
				} else {
					deltaV = Vector3.Cross (direction, diff).normalized;
				}
				float time = Node.GetElapsedTime ();
				float magnitude;
				if (MType == MAGTYPE.Curve) {
					float totalTime = VortexTime;
					if(VortexTime == -1) {
						totalTime = Node.GetRealLife();
					}

					magnitude = VortexCurve.Evaluate (time/totalTime)*Magnitude;
                }else if(MType == MAGTYPE.Fixed) {
                    magnitude =  Magnitude;
                }else {
                    magnitude = realMag;
                }
				
				if (Node.Owner.VortexAttenuation < 1e-04f)
				{
					deltaV *= magnitude * deltaTime;
				}
				else
				{
					deltaV *= magnitude * deltaTime / Mathf.Pow(Mathf.Sqrt(distanceSqr),Node.Owner.VortexAttenuation);
				}
				
				if (Node.Owner.IsVortexAccelerate)
                {
                    Node.Velocity += deltaV;
                }	
				else
                {
                    if (Node.Owner.IsFixedCircle)
                    {
                        Vector3 dist = Node.GetOriginalPos() + deltaV - VortexObj.position;
                        Vector3 proj = Vector3.Project(dist,direction);
                        Vector3 fixedDir = dist - proj;
                        if (Node.Owner.SyncClient)
                        {
                            Node.Position = fixedDir.normalized * OriginalRadius + proj;
                        } 
                        else
                        {
                            Node.Position = Node.GetRealClientPos() + fixedDir.normalized * OriginalRadius + proj;
                        }    
                    }
                    else
                    {
                        Node.Position += deltaV;
                    } 
                }
					
			}


		}
	}
}
