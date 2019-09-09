//----------------------------------------------
//            Xffect Editor
// Copyright © 2012- Shallway Studio
// http://shallway.net
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Xft;

namespace Xft
{
 
    public class CollisionParam
    {
        protected GameObject m_collideObject;
        protected Vector3 m_collidePos;
        public EffectNode effectNode;

        
        public GameObject CollideObject
        {
            get {return m_collideObject;}
            set {m_collideObject = value;}
        }
        
        public Vector3 CollidePos
        {
            get {return m_collidePos;}
            set {m_collidePos = value;}
        }
        
        public CollisionParam(GameObject obj, Vector3 pos, EffectNode ef)
        {
            m_collideObject = obj;
            m_collidePos = pos;
            effectNode = ef;
        }
    }
    
    public class EffectNode
    {

        //constructor
        protected int Type;  //1 sprite, 2 ribbon trail, 3 cone, 4, custom mesh.
        public int Index;
        public Transform ClientTrans;
        public bool SyncClient;
        public EffectLayer Owner;

        //internal using
        protected Vector3 CurDirection;
        protected Vector3 LastWorldPos = Vector3.zero;
        protected Vector3 CurWorldPos;
        protected float ElapsedTime;
        public Sprite Sprite;    //1
        public RibbonTrail Ribbon; //2
        public Cone Cone;   //3
        public CustomMesh CusMesh;   //4

        //affect by affector
        public Vector3 Position;
        public Vector2 LowerLeftUV;
        public Vector2 UVDimensions;
        public Vector3 Velocity;
        public Vector3 Scale;
        public float RotateAngle;
        public Color Color;
  
        bool isCollision = false;
        public bool isBounce = false;

		public bool IsSine = false;
		public float SineFreqTime = 1;
		public float SineFreqDist = 1;
		public float SineMagnitude = 1;

        public XffectComponent SubEmitter = null;
        
        //reset
        protected List<Affector> AffectorList;
        protected BounceAffector bounceAffector;

        protected Vector3 OriDirection;
        protected float LifeTime;
        protected float RealLifeTime;
        protected int OriRotateAngle;
        protected float OriScaleX;
        protected float OriScaleY;
        protected float OriScaleZ;

        protected bool SimpleSprite = false;

        //added 2012 6.3 for collision use

        protected bool IsCollisionEventSended = false;
        protected Vector3 LastCollisionDetectDir = Vector3.zero;

        public EffectNode(int index, Transform clienttrans, bool sync, EffectLayer owner)
        {
            Index = index;
            ClientTrans = clienttrans;
            SyncClient = sync;
            Owner = owner;
            LowerLeftUV = Vector2.zero;
            UVDimensions = Vector2.one;
            Scale = Vector3.one;
            RotateAngle = 0;
            Color = Color.white;
        }

        public void SetAffectorList(List<Affector> afts)
        {
            AffectorList = afts;
        }

        public List<Affector> GetAffectorList()
        {
            return AffectorList;
        }

        public void Init(Vector3 oriDir, float speed, float life, int oriRot, float oriScaleX, float oriScaleY, Color oriColor, Vector2 oriLowerUv, Vector2 oriUVDimension, float oriScaleZ, float realLife)
        {
            //OriDirection = ClientTrans.TransformDirection(oriDir);
            OriDirection = oriDir;
            LifeTime = life;
            RealLifeTime = realLife;
            OriRotateAngle = oriRot;
            OriScaleX = oriScaleX;
            OriScaleY = oriScaleY;
            OriScaleZ = oriScaleZ;
            
            Color = oriColor;
            ElapsedTime = 0f;
            // direction is synced to client rotation, except sphere dir
            if (Owner.DirType != DIRECTION_TYPE.Sphere)
                Velocity = Owner.transform.rotation * OriDirection * speed;
            else
                Velocity = OriDirection * speed;
            LowerLeftUV = oriLowerUv;
            UVDimensions = oriUVDimension;
            IsCollisionEventSended = false;

            if (Type == 1)
            {
                Sprite.SetUVCoord(LowerLeftUV, UVDimensions);
                Sprite.SetColor(oriColor);
                if (SimpleSprite)
                {
                    //Simple sprite 只更新一次。
                    Update(0f);
                    Sprite.Transform();
                }
            }

            else if (Type == 2)
            {
                Ribbon.SetUVCoord(LowerLeftUV, UVDimensions);
                Ribbon.SetColor(oriColor);
                Ribbon.SetHeadPosition(GetRealClientPos() + Position + OriDirection.normalized * Owner.TailDistance);
                Ribbon.ResetElementsPos();
            }
            else if (Type == 3)
            {
                Cone.SetUVCoord(LowerLeftUV, UVDimensions);
                Cone.SetColor(oriColor);
                Cone.SetRotation(oriRot);
            }
            else if (Type == 4)
            {
                
                CusMesh.SetColor(oriColor);
                CusMesh.SetRotation(oriRot);
                CusMesh.SetScale(oriScaleX,oriScaleY, OriScaleZ);
                CusMesh.ScaleZ = oriScaleZ;
                CusMesh.SetUVCoord(LowerLeftUV,UVDimensions);
            }

            //set  sprite direction
            if (Type == 1)
            {
                //direction is synced to client rotation, except Sphere direction.
                if (Owner.DirType != DIRECTION_TYPE.Sphere)
                    Sprite.SetRotationTo(Owner.ClientTransform.rotation * OriDirection);
                else
                    Sprite.SetRotationTo(OriDirection);
                
            }
        }
        
        
        public void ResetCamera(Camera cam)
        {
            if (Type == 1)
                Sprite.MainCamera = cam;
            else if (Type == 2)
                Ribbon.MainCamera = cam;
        }
        
        public float GetElapsedTime()
        {
            return ElapsedTime;
        }

        public float GetLifeTime()
        {
            return LifeTime;
        }
  
        public float GetRealLife() {
            return RealLifeTime;
        }
        
        public void SetLocalPosition(Vector3 pos)
        {
            //fixed 2012.6.3   ribbon trail need to reset the head.
            if (Type == 2)
            {
                if (!SyncClient)
                    Ribbon.OriHeadPos = pos;
                else
                    Ribbon.OriHeadPos = GetRealClientPos() + pos;
                //Ribbon.ResetElementsPos();
            }

            // collison may get through. use dir to detect.
            //if (Owner.UseCollisionDetection && Owner.CollisionType != COLLITION_TYPE.CollisionLayer)
                //LastCollisionDetectDir = Vector3.zero;

            Position = pos;
        }
        public Vector3 GetLocalPosition()
        {
            return Position;
        }
  
        public Vector3 GetRealClientPos()
        {
            Vector3 mscale = Vector3.one * Owner.Owner.Scale;
            Vector3 clientPos = Vector3.zero;
            clientPos.x = ClientTrans.position.x / mscale.x;
            clientPos.y = ClientTrans.position.y / mscale.y;
            clientPos.z = ClientTrans.position.z / mscale.z;
            return clientPos;
        }
        
        //back to original scale pos.
        public Vector3 GetOriginalPos()
        {
            Vector3 ret;
            Vector3 clientPos = GetRealClientPos();
            if (!SyncClient)
                ret = Position - clientPos + ClientTrans.position;
            else
                ret = Position + ClientTrans.position;
            return ret;
        }

        public Vector3 GetWorldPos()
        {
            return CurWorldPos;
        }


        //added to optimize grid effect..if simpe no Transform() every circle.
        protected bool IsSimpleSprite()
        {
            bool ret = false;
            if (Owner.SpriteType == (int)STYPE.XZ && Owner.OriVelocityAxis == Vector3.zero&& Owner.ScaleAffectorEnable == false && Owner.RotAffectorEnable == false &&
                Owner.OriSpeed < 1e-04 && Owner.GravityAffectorEnable == false&& Owner.AirAffectorEnable == false && Owner.TurbulenceAffectorEnable == false && Owner.BombAffectorEnable == false&&Owner.UVRotAffectorEnable == false&&
                Mathf.Abs(Owner.OriRotationMax - Owner.OriRotationMin) < 1e-04 && Mathf.Abs(Owner.OriScaleXMin - Owner.OriScaleXMax) < 1e-04 &&
                Mathf.Abs(Owner.OriScaleYMin - Owner.OriScaleYMax) < 1e-04)
            {
                ret = true;
                //Debug.Log("detected simple sprite which not transformed!");
            }
            return ret;
        }

        //NOTICE:SetType is called before Init()!!and called only once!like constructor.

        //sprite
        public void SetType(float width, float height, STYPE type, ORIPOINT orip, int uvStretch, float maxFps)
        {
            Type = 1;
            SimpleSprite = IsSimpleSprite();
            //SimpleSprite = false;
            Sprite = Owner.GetVertexPool().AddSprite(width, height, type, orip, Owner.MyCamera, uvStretch, maxFps,SimpleSprite);
            Sprite.Owner = this;
        }
        //ribbon trail
        public void SetType(bool useFaceObj, Transform faceobj,float width, int maxelemnt, float len, Vector3 pos, int stretchType, float maxFps, bool isWeapon, bool isScale)
        {
            Type = 2;
            Ribbon = Owner.GetVertexPool().AddRibbonTrail(Owner.MyCamera, useFaceObj, faceobj, width, maxelemnt, len, pos, stretchType, maxFps, isWeapon, this, isScale);
            Ribbon.Owner = this;
        }

        //fence
        public void SetType(Vector2 size, int numSegment, float angle, Vector3 dir, int uvStretch, float maxFps, bool usedelta, AnimationCurve deltaAngle)
        {
            Type = 3;
            Cone = Owner.GetVertexPool().AddCone(size, numSegment, angle, dir, uvStretch, maxFps, usedelta,deltaAngle, this);
        }
        
        //custom mesh
        public void SetType(Mesh mesh, Vector3 dir, float maxFps)
        {
            Type = 4;
            CusMesh = Owner.GetVertexPool().AddCustomMesh(mesh,dir,maxFps,this);
        }
  

        
        public void Reset()
        {
            isCollision = false;
            isBounce = false;

            //activate on death subemitter.
            if (Owner.UseSubEmitters && !string.IsNullOrEmpty(Owner.DeathSubEmitter))
            {
                XffectComponent sub = Owner.SpawnCache.GetEffect(Owner.DeathSubEmitter);
                if (sub == null)
                {
                    return;
                }
                sub.Active();
                sub.transform.position = CurWorldPos;
            }
            
            //Position = Vector3.up * -9999;
			//if (Owner.SpriteType == (int)STYPE.BILLBOARD_UP) {
			//	Position = Quaternion.Euler (new Vector3 (0, Owner.transform.rotation.eulerAngles.y, 0)) * Owner.EmitPoint + Owner.transform.position;
			//} else {
			Position = Owner.EmitPoint + Owner.transform.position;
			//}

            Velocity = Vector3.zero;
            ElapsedTime = 0f;
   
            
            CurWorldPos = Owner.transform.position;
            
            LastWorldPos = CurWorldPos;

            IsCollisionEventSended = false;

            //foreach (Affector aft in AffectorList)
            //{
            //    aft.Reset();
            //}V

            //do not use foreach in your script!
            for (int i = 0; i < AffectorList.Count; i++)
            {
                Affector aft = AffectorList[i];
                aft.Reset();
            }
   
            Scale = Vector3.one;
            
            if (Type == 1)
            {
                Sprite.SetRotation( OriRotateAngle);
                Sprite.SetPosition(Position);
                Sprite.SetColor(Color.clear);
                Sprite.Update(true,0f);
                //TODO:should reset in ScaleAffector.
                Scale = Vector3.one;
            }
            else if (Type == 2)
            {
                Vector3 headpos;
                if (Owner.AlwaysSyncRotation)
                    headpos = ClientTrans.rotation * (GetRealClientPos() + Owner.EmitPoint);
                else
                    headpos = GetRealClientPos() + Owner.EmitPoint;
                Ribbon.SetHeadPosition(headpos /*+ OriDirection.normalized * Owner.TailDistance*/);
                Ribbon.Reset();
                Ribbon.SetColor(Color.clear);
                Ribbon.UpdateVertices(Vector3.zero);
            }
            else if (Type == 3)
            {
                Cone.SetRotation(OriRotateAngle);
                Cone.SetColor(Color.clear);
                Cone.SetPosition(Position);
                Scale = Vector3.one;
                Cone.ResetAngle();
                Cone.Update(true,0f);
            }
            else if (Type == 4)
            {
                CusMesh.SetColor(Color.clear);
                CusMesh.SetRotation(OriRotateAngle);
                CusMesh.Update(true,0f);
            }
                
            
            if (Owner.UseSubEmitters && SubEmitter != null && XffectComponent.IsActive(SubEmitter.gameObject))
            {
                SubEmitter.StopEmit();
            }
            
        }

        public void Remove()
        {
            Owner.RemoveActiveNode(this);
        }
  
        public void Stop()
        {
            Reset();
            Remove();
        }

        public void UpdateSprite(float deltaTime)
        {
            //added 2012.7.6
            if (Owner.AlwaysSyncRotation)
            {
                if (Owner.DirType != DIRECTION_TYPE.Sphere)
                    Sprite.SetRotationTo(Owner.transform.rotation * OriDirection);
            }

            Sprite.SetScale(Scale.x * OriScaleX, Scale.y * OriScaleY);
            if (Owner.ColorAffectorEnable)
                Sprite.SetColor(Color);
            if (Owner.UVAffectorEnable || Owner.UVRotAffectorEnable)
                Sprite.SetUVCoord(LowerLeftUV, UVDimensions);

            Sprite.SetRotation((float)OriRotateAngle + RotateAngle);
            Sprite.SetPosition(CurWorldPos);
            Sprite.Update(false,deltaTime);
        }

        public void UpdateRibbonTrail(float deltaTime)
        {
            Ribbon.SetHeadPosition(CurWorldPos);
            if (Owner.UVAffectorEnable || Owner.UVRotAffectorEnable)
                Ribbon.SetUVCoord(LowerLeftUV, UVDimensions);


            Ribbon.SetColor(Color);
            Ribbon.Update(deltaTime);
        }

        public void UpdateCone(float deltaTime)
        {
            Cone.SetScale(Scale.x * OriScaleX, Scale.y * OriScaleY);
            if (Owner.ColorAffectorEnable)
                Cone.SetColor(Color);
            if (Owner.UVAffectorEnable || Owner.UVRotAffectorEnable)
                Cone.SetUVCoord(LowerLeftUV, UVDimensions);
            Cone.SetRotation((float)OriRotateAngle + RotateAngle);
            Cone.SetPosition(CurWorldPos);
            Cone.Update(false, deltaTime);
        }
        
        public void UpdateCustomMesh(float deltaTime)
        {
            CusMesh.SetScale(Scale.x * OriScaleX, Scale.y * OriScaleY, Scale.z*OriScaleZ);
            if (Owner.ColorAffectorEnable)
                CusMesh.SetColor(Color);
            
            if (Owner.UVAffectorEnable || Owner.UVRotAffectorEnable)
                CusMesh.SetUVCoord(LowerLeftUV, UVDimensions);
            CusMesh.SetRotation((float)OriRotateAngle + RotateAngle);
            CusMesh.SetPosition(CurWorldPos);
            CusMesh.Update(false, deltaTime);
        }

        public void CollisionDetection()
        {
            if (!Owner.UseCollisionDetection || IsCollisionEventSended)
                return;

            bool collided = false;
   
            GameObject collideObject = null;
            
            if (Owner.CollisionType == COLLITION_TYPE.Sphere && Owner.CollisionGoal != null)
            {
                Vector3 diff = CurWorldPos  + Velocity.normalized * Owner.CollisionOffset - Owner.CollisionGoal.position;
                float range = Owner.ColliisionPosRange + Owner.ParticleRadius;
                if (diff.sqrMagnitude <= range * range /*|| //fixed, don't check get through.
                    Vector3.Dot(diff, LastCollisionDetectDir) < 0*/)
                {
                    collided = true;
                    collideObject = Owner.CollisionGoal.gameObject;
                }
                LastCollisionDetectDir = diff;
            }
            else if (Owner.CollisionType == COLLITION_TYPE.CollisionLayer)
            {
                int layer = 1 << Owner.CollisionLayer;
                
                RaycastHit hit;
                Vector3 p1 = GetOriginalPos() + Velocity.normalized * Owner.CollisionOffset;
                if (Physics.SphereCast(p1, Owner.ParticleRadius, Velocity.normalized, out hit, Owner.ParticleRadius,layer))
                {
                    collided = true;
                    collideObject = hit.collider.gameObject;
                }
            }
            else if (Owner.CollisionType == COLLITION_TYPE.Plane)
            {
				//Debug.Log("particle pos "+CurWorldPos+" nor "+Owner.PlaneDir.normalized+" "+Owner.ParticleRadius);
                //Debug.Log("plane "+Owner.CollisionPlane.distance+" "+Owner.CollisionPlane.normal+" pos ");
                //var realPos = CurWorldPos + Owner.transform.position;
                //var realPos = CurWorldPos;
                var realPos = this.Position;
                var np = realPos-Owner.PlaneDir.normalized * Owner.ParticleRadius;
                /*
                if(realPos.y <= 0) {
                    Debug.Log("Collide: "+Owner.CollisionPlane.distance+" w "+CurWorldPos+" pos "+Owner.transform.position
                        +" np "+np+" emitPoint "+Owner.EmitPoint);
                    collided = true;
                    collideObject = Owner.gameObject;
                }
                */

                if (!Owner.CollisionPlane.GetSide(np))
                {
                    Debug.Log("Collide: "+Owner.CollisionPlane.distance+" w "+CurWorldPos+" pos "+Owner.transform.position
                        +" np "+np+" emitPoint "+Owner.EmitPoint);
                    //var testP = new Plane(Vector3.up, Owner.transform.position);
                    //var side = testP.GetSide(np);
                    //Debug.Log("TestP: "+testP.distance+" n "+testP.normal+" np "+np+" si "+side);

                    collided = true;
                    collideObject = Owner.gameObject;

                }
            }
            else
            {
                Debug.LogError("invalid collision type!");
            }

            if (collided)
            {
                if (Owner.EventHandleFunctionName != "" && Owner.EventReceiver != null)
                {
                    //Owner.EventReceiver.SendMessage(Owner.EventHandleFunctionName, Owner.CollisionGoal);
                    Owner.EventReceiver.SendMessage(Owner.EventHandleFunctionName, new CollisionParam(collideObject,GetOriginalPos(), this) );
                    isCollision = true;
                }
                    
                IsCollisionEventSended = true;
                if (Owner.CollisionAutoDestroy)
                {
                    //distroy.
                    ElapsedTime = Mathf.Infinity;
                }
                

                //activate on collision subemitter.
                if (Owner.UseSubEmitters && !string.IsNullOrEmpty(Owner.CollisionSubEmitter))
                {
                    XffectComponent sub = Owner.SpawnCache.GetEffect(Owner.CollisionSubEmitter);
                    if (sub == null)
                    {
                        return;
                    }
                    sub.Active();
                    sub.transform.position = CurWorldPos;
                }
                
            }
        }
        public void SetBounce() {
            isBounce = true;
            bounceAffector = new BounceAffector(this);
        }

        public class BounceAffector : Affector {
            float curTime;
            //Vector3 dir;
            Vector3 initVelocity;
            Vector3 initPos;
            float jumpHeight;
            float friction;
			float initRotate;
			Vector3 curPos;

            Vector3 ownerPos;
            public BounceAffector(EffectNode node)
            : base(node, AFFECTORTYPE.GravityAffector)
            {
                friction = 2f;
                curTime = 1/2.75f;
                //dir = Node.Velocity;
				initVelocity = new Vector3(Node.Velocity.x, 0, Node.Velocity.z);
                ownerPos = Node.Owner.transform.position;
                initPos = Node.Position;//+ownerPos;
                Debug.Log("OwnerPos MePos: "+ownerPos+" mePos "+Node.Position);
				curPos = initPos;
                jumpHeight = Node.Owner.JumpHeight+initPos.y;
				initRotate = Node.RotateAngle;
                Node.Velocity = initVelocity;
            }

            float bounceTime(float time)
            {
                if (time < 1 / 2.75)
                {
                    return 7.5625f * time * time;
                }
                else if (time < 2 / 2.75)
                {
                    time -= 1.5f / 2.75f;
                    return 7.5625f * time * time + 0.75f;
                }
                else if(time < 2.5 / 2.75)
                {
                    time -= 2.25f / 2.75f;
                    return 7.5625f * time * time + 0.9375f;
                }

                time -= 2.625f / 2.75f;
                return 7.5625f * time * time + 0.984375f;
            }
            public override void Update(float deltaTime) {
                curTime += deltaTime;
                curTime = Mathf.Min(curTime, 1);
				//Debug.Log("update bounce time "+curTime);
                float nt = bounceTime(curTime);

                float hei = Mathf.Lerp(jumpHeight, initPos.y, nt);
                curPos.y = hei;

                //Debug.Log("update bounce");
                float reduction = friction * deltaTime;
                if(reduction < 1f) {
                    initVelocity *= 1-reduction;
                }else {
                    initVelocity = Vector3.zero;
                }

                Node.Position = curPos+initVelocity*deltaTime;//+ownerPos;
                //Node.Position = initPos;
				Node.RotateAngle = initRotate;
                //Debug.Log("NodeNewPos: "+Node.Position);
            }   
        }

        void UpdateBounce(float deltaTime) {
            bounceAffector.Update(deltaTime);
        }

        public void Update(float deltaTime)
        {
            ElapsedTime += deltaTime;

            if(isCollision && isBounce) 
            {
				//Update color only not velocity and position
				for (int i = 0; i < AffectorList.Count; i++)
				{
					Affector aft = AffectorList[i];
					aft.Update(deltaTime);
				}
                UpdateBounce(deltaTime);
                //Position += Velocity * deltaTime;
				if (SyncClient)
				{
					//NOTICE: only sync position.
					CurWorldPos = Position + GetRealClientPos(); 
				}
				else
				{
					CurWorldPos = Position;
				}
            }   
            else
            {
                for (int i = 0; i < AffectorList.Count; i++)
                {
                    Affector aft = AffectorList[i];
                    aft.Update(deltaTime);
                }

                Position += Velocity * deltaTime;

				if (SyncClient)
				{
					//NOTICE: only sync position.
					CurWorldPos = Position + GetRealClientPos(); 
				}
				else
				{
					CurWorldPos = Position;
				}

                CollisionDetection();
            }
            
            //sync subemitters position.
            if (Owner.UseSubEmitters && SubEmitter != null && XffectComponent.IsActive(SubEmitter.gameObject))
            {
                SubEmitter.transform.position = CurWorldPos;
            }
            
            
            if (Type == 1)
                UpdateSprite(deltaTime);
            else if (Type == 2)
                UpdateRibbonTrail(deltaTime);
            else if (Type == 3)
                UpdateCone(deltaTime);
            else if (Type == 4)
                UpdateCustomMesh(deltaTime);

            LastWorldPos = CurWorldPos;

            if (ElapsedTime > LifeTime && LifeTime > 0)
            {
                Reset();
                Remove();
            }
        }
    }
}