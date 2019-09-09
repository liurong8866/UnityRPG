//----------------------------------------------
//            Xffect Editor
// Copyright © 2012- Shallway Studio
// http://shallway.net
//----------------------------------------------
using UnityEngine;
using System.Collections;
using Xft;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Xft
{
	public enum STYPE
{
    BILLBOARD,
    BILLBOARD_SELF,
    XZ,
    BILLBOARD_Y,

	BILLBOARD_UP,
}

public enum ORIPOINT
{
    CENTER,
    LEFT_UP,
    LEFT_BOTTOM,
    RIGHT_BOTTOM,
    RIGHT_UP,
    BOTTOM_CENTER,
    TOP_CENTER,
    LEFT_CENTER,
    RIGHT_CENTER
}

public enum EMITTYPE
{
    POINT,
    BOX,
    SPHERE,
    CIRCLE,
    LINE,
    Mesh
}

public enum RSTYPE
{
    NONE,
    SIMPLE,
    CURVE,
        CURVE01,
        RANDOM,
}

public enum MAGTYPE
{
    Fixed,
    Curve,
	RANDOM,
}


public enum COLLITION_TYPE
{
    Sphere,
    CollisionLayer,
        Plane,
}

public enum COLOR_GRADUAL_TYPE
{
    CLAMP,
    LOOP,
    REVERSE,
		CURVE,
}
    
    public enum WRAP_TYPE
    {
        CLAMP,
        LOOP,
        REVERSE,
    }

public enum DIRECTION_TYPE
{
    Planar,
    Cone,
    Sphere,
    Cylindrical
}

	public enum RENDER_TYPE
	{
		Sprite,
		RibbonTrail,
		Cone,
		CustomMesh,
	}
	
	public enum MESHEMIT_TYPE
	{
		ByVertex,
		ByTriangle,
	}
	
	public enum COLOR_CHANGE_TYPE
	{
		Constant,
		Gradient,
		Random,
	}
	
	public enum UVTYPE
	{
		None,
		TextureSheetAnimation,
		Scroll,
	}
	
	public enum STRETCH_TYPE
	{
		Vertical,
		Horizontal,
	}
	
}
public class EffectLayer : MonoBehaviour
{
 
    public int VersionIdentifier1 = 222;
    
    public VertexPool Vertexpool;
    //Main Config
    public Transform ClientTransform;
    public bool SyncClient;
    public Material Material;
    public int RenderType; //0 sprite 1 ribbon 2 cone, 3 custom mesh
    public float StartTime = 0f;
    /*deprecated This value is obsolete, default to 60*/
	public float MaxFps = 60f;
    public Color DebugColor = Color.white;
    public int Depth = 0;

    //Sprite Config
    public int SpriteType;
    public int OriPoint;
    public float SpriteWidth = 1;
    public float SpriteHeight = 1;
	
    public int SpriteUVStretch = 0;
	
	
	public bool RandomOriScale = false;
    public bool RandomOriRot = false;
    public bool IsTowardZ = false;
	
    //Rotation Config
    public int OriRotationMin;
    public int OriRotationMax;
    public bool RotAffectorEnable = false;
    public RSTYPE RotateType = RSTYPE.NONE;
    public float DeltaRot;
    public AnimationCurve RotateCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 360));
    
    public WRAP_TYPE RotateCurveWrap;
    public float RotateCurveTime = 1f;
    public float RotateCurveMaxValue = 1f;
    public AnimationCurve RotateCurve01 = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    
    public float RotateSpeedMin = 0f;
    public float RotateSpeedMax = 0f;
    

    //Scale Config
    public float OriScaleXMin = 1f;
    public float OriScaleXMax = 1f;
    public float OriScaleYMin = 1f;
    public float OriScaleYMax = 1f;
    public float OriScaleZ = 1f; //Mesh Scale Z Direction

    public bool ScaleAffectorEnable = false;
    public RSTYPE ScaleType = RSTYPE.NONE;
    public float DeltaScaleX = 0f;
    public float DeltaScaleY = 0f;
    public bool IsScaleZ = false;
    public AnimationCurve ScaleZCurveNew = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
    
    //deprecated.
    public AnimationCurve ScaleXCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 5));
    public AnimationCurve ScaleYCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 5));
    
    public WRAP_TYPE ScaleWrapMode;
    public float ScaleCurveTime = -1f;
    public float MaxScaleCalue = 1f;
    public AnimationCurve ScaleXCurveNew = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
    public AnimationCurve ScaleYCurveNew = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
    
    public bool UseSameScaleCurve = false;
 
    public float DeltaScaleXMax = 0f;
    public float DeltaScaleYMax = 0f;
    
    //Color Config
    public bool ColorAffectorEnable = false;
    /*deprecated*/public int ColorAffectType = 0;
    public float ColorGradualTimeLength = -1f;
    public COLOR_GRADUAL_TYPE ColorGradualType = COLOR_GRADUAL_TYPE.CLAMP;
	public Color Color1 = Color.white;
    /*deprecated*/public Color Color2;
    /*deprecated*/public Color Color3;
    /*deprecated*/public Color Color4;
    /*deprecated*/public Color Color5;
	
	public COLOR_CHANGE_TYPE ColorChangeType = COLOR_CHANGE_TYPE.Constant;
	public ColorParameter ColorParam;
	public AnimationCurve ColorGradualCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

	
    //RibbonTrail Config
    public float RibbonWidth = 1f;
    public int MaxRibbonElements = 8;
    public float RibbonLen = 15f;
	/*deprecated This value is obsolete, default to 0*/
    public float TailDistance = 0f;
    public int StretchType = 0;
    public bool FaceToObject = false;
    public Transform FaceObject;

	public bool IsWeapon = false;
    public bool IsScale = false;

    //ver 1.2.1
    public bool UseRandomRibbon = false;
    public float RibbonWidthMin = 1f;
    public float RibbonWidthMax = 1f;
    public float RibbonLenMin = 15f;
    public float RibbonLenMax = 15f;


    //Cone Config
    public Vector2 ConeSize = Vector2.one;
    public float ConeAngle;
    public int ConeSegment = 4;
    public bool UseConeAngleChange = false;
    public AnimationCurve ConeDeltaAngle = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 60));
    
    //Custom Mesh Config
    public Mesh CMesh;

    //Emitter Config
    public int EmitType; // 0:point 1:box, 2: sphere surface, 3: circle, 4: line. 5: mesh
    public Vector3 BoxSize;
	/*deprecated This value is obsolete, default to true*/
    //public bool BoxInheritRotation = true;
    public Vector3 EmitPoint;
    public float Radius;
    public bool UseRandomCircle = false;
    public float CircleRadiusMin = 1f;
    public float CircleRadiusMax = 10f;
    public Vector3 CircleDir = Vector3.up;
    public bool EmitUniform = false;
    public float LineLengthLeft = -1f;
    public float LineLengthRight = 1f;
    public int MaxENodes = 1;
    public bool IsNodeLifeLoop = true;
    public float NodeLifeMin = 1;
    public float NodeLifeMax = 1;
    
    public EEmitWay EmitWay = EEmitWay.ByRate;
    public AnimationCurve EmitterCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 10));
    
    //for downward compatibility
    //public bool IsEmitByDistance = false;
    
    public float DiffDistance = 0.1f;
    public Mesh EmitMesh;
    public int EmitMeshType;
 
    
    public bool IsBurstEmit = false;
    public float ChanceToEmit = 100f;//deprecated.
    public float EmitDuration = 100f;
    public float EmitRate = 20;
    public int EmitLoop = -1;
    public float EmitDelay = 0f;

    //Original Direction Config
    
    //discarded
    //public bool IsRandomDir;
    
    public DIRECTION_TYPE DirType = DIRECTION_TYPE.Planar;
    public Transform DirCenter;
    public Vector3 OriVelocityAxis;
    public int AngleAroundAxis;
    public bool UseRandomDirAngle = false;
    public int AngleAroundAxisMax;
    
    public float OriSpeed;
    //public bool AlongVelocity = false;
    public bool AlwaysSyncRotation = false;
    public bool IsRandomSpeed = false;
    public float SpeedMin = 0f;
    public float SpeedMax = 0f;


    //public int AffectorIndex;

    //Jet Affector
    public bool JetAffectorEnable = false;
    public MAGTYPE JetMagType = MAGTYPE.Fixed;
    public float JetMag;
    public AnimationCurve JetCurve;
    public float JetMin = 0;
    public float JetMax = 0;
    //public float JetMin;
    //public float JetMax;

    //Vortex Affector
    public bool VortexAffectorEnable = false;
    public MAGTYPE VortexMagType = MAGTYPE.Fixed;
    public float VortexMag = 1f;
    public float VortexMagMax = 1f;

    public AnimationCurve VortexCurve;
    public Vector3 VortexDirection = Vector3.up;
    public bool VortexInheritRotation = true;
    public Transform VortexObj;
    public bool IsFixedCircle = false;
    public float VortexTime = -1;

    //ver 1.2.1
    public bool IsRandomVortexDir = false;
	//2012.9.22
	public bool IsVortexAccelerate = false;
	public float VortexAttenuation = 0f;
	public bool UseVortexMaxDistance = false;
	public float VortexMaxDistance = 0f;

    //UVRotationAffector
    public bool UVRotAffectorEnable = false;
    public bool RandomUVRotateSpeed = false;
    public float UVRotXSpeed = 0;
    public float UVRotYSpeed = 0;
    public float UVRotXSpeedMax = 0;
    public float UVRotYSpeedMax = 0;

    //Gravity Affector
    public bool GravityAffectorEnable = false;
    public GAFTTYPE GravityAftType = GAFTTYPE.Planar;
    public MAGTYPE GravityMagType = MAGTYPE.Fixed;
    public float GravityMag;
    public AnimationCurve GravityCurve;
    public Vector3 GravityDirection = Vector3.up;
    public Transform GravityObject;
    public bool IsGravityAccelerate = true;
    public bool IsSine = false;
    public float SineFreq = 1;


    public bool SineAffectorEnable = false;
    public Vector3 SineForce = Vector3.up;
    public bool ModifyPos = true;// change position or speed
    public float SineMaxFreq = 5;
    public float SineMinFreq = 1;

    //AirField Affector
    public bool AirAffectorEnable = false;
    public Transform AirObject;
    public MAGTYPE AirMagType = MAGTYPE.Fixed;
    public float AirMagnitude;
    public AnimationCurve AirMagCurve;
    public Vector3 AirDirection;
    public float AirAttenuation;
    public bool AirUseMaxDistance;
    public float AirMaxDistance;
    public bool AirEnableSpread;
    public float AirSpread;
    public float AirInheritVelocity;
    public bool AirInheritRotation;

    //Bomb Affector
    public bool BombAffectorEnable = false;
    public Transform BombObject;
    public BOMBTYPE BombType = BOMBTYPE.Spherical;
    public BOMBDECAYTYPE BombDecayType = BOMBDECAYTYPE.None;
    public MAGTYPE BombMagType = MAGTYPE.Fixed;
    public float BombMagnitude;
    public AnimationCurve BombMagCurve;
    public Vector3 BombAxis;
    public float BombDecay;

    //TurbulenceFieldAffector
    public bool TurbulenceAffectorEnable = false;
    public Transform TurbulenceObject;
    public MAGTYPE TurbulenceMagType = MAGTYPE.Fixed;
    public float TurbulenceMagnitude;
    public AnimationCurve TurbulenceMagCurve;
    public float TurbulenceAttenuation;
    public bool TurbulenceUseMaxDistance;
    public float TurbulenceMaxDistance;
    
    //DragAffector
    public bool DragAffectorEnable = false;
    public Transform DragObj;
    public bool DragUseDir = false;
    public Vector3 DragDir = Vector3.up;
    public float DragMag = 10f;
    public bool DragUseMaxDist = false;
    public float DragMaxDist = 50f;
    public float DragAtten = 0f;

    //UV Config
    public bool UVAffectorEnable = false;
    
    public int UVType = 0;   //0: none 1: UVAnimation, 2: UVRotate
    public Vector2 OriTopLeftUV = Vector2.zero;
    public Vector2 OriUVDimensions = Vector2.one;
    protected Vector2 UVTopLeft;
    protected Vector2 UVDimension;
    public int Cols = 1;
    public int Rows = 1;
    public int LoopCircles = -1;
    public float UVTime = -1;
    /*deprecated*/public string EanPath = "none";
    /*deprecated*/public int EanIndex = 0;
    public bool RandomStartFrame = false;




    //Collision Config
    public bool UseCollisionDetection = false;
    public float ParticleRadius = 1f;
    public COLLITION_TYPE CollisionType = COLLITION_TYPE.Sphere;
    public bool CollisionAutoDestroy = true;
    public Transform EventReceiver;
    public string EventHandleFunctionName=" ";
    public Transform CollisionGoal;
    public float ColliisionPosRange;
    public LayerMask CollisionLayer;

	public float JumpHeight = 0.3f;
    
    public float CollisionOffset = 0f;
    
    public Vector3 PlaneDir = Vector3.up;
    public Vector3 PlaneOffset = Vector3.zero;
    
    protected Plane mCollisionPlane;
    
    public Plane CollisionPlane
    {
        get{
            return mCollisionPlane;
        }
    }
    
    //public Collider CollisionCollider = null;
 
    
    
    //sub emitters
    public bool UseSubEmitters = false;
    public XffectCache SpawnCache = null;
    public string BirthSubEmitter;
    public string CollisionSubEmitter;
    public string DeathSubEmitter;
    

    public Emitter emitter;


    public EffectNode[] AvailableENodes;
    public EffectNode[] ActiveENodes;
    public int AvailableNodeCount;
    public Vector3 LastClientPos;

    public  Camera MyCamera;
    
    
    public XffectComponent Owner;
    
    
    public bool mStopped = false;
    
    public void FindMyCamera()
    {
        int layerMask = 1 << gameObject.layer;
        Camera[] cameras = GameObject.FindObjectsOfType(typeof(Camera)) as Camera[];
        for (int i = 0, imax = cameras.Length; i < imax; ++i)
        {
            Camera cam = cameras[i];

            if ((cam.cullingMask & layerMask) != 0)
            {
                MyCamera = cam;
                return;
            }
        }
        //Debug.LogError("can't find properly camera for this object's layer!");
    }


    public void InitCollision()
    {
        if (!UseCollisionDetection)
            return;
  
        mCollisionPlane = new Plane(PlaneDir.normalized,transform.position + PlaneOffset);

        if (CollisionType == COLLITION_TYPE.CollisionLayer || CollisionType == COLLITION_TYPE.Plane )
            return;

        if (CollisionGoal == null /* || CollisionGoal.gameObject.active == false*/)
        {
            Debug.LogWarning("please set the collision goal!");
        }
    }

    protected List<Affector> InitAffectors(EffectNode node)
    {
        List<Affector> AffectorList = new List<Affector>();

        if (UVAffectorEnable)
        {
            UVAnimation uvAnim = new UVAnimation();
            if (UVType == 1)
            {
                float perWidth = OriUVDimensions.x / Cols;
                float perHeight = Mathf.Abs(OriUVDimensions.y / Rows);
                Vector2 cellSize = new Vector2(perWidth, perHeight);
                uvAnim.BuildUVAnim(OriTopLeftUV, cellSize, Cols, Rows, Cols * Rows);
            }
            UVDimension = uvAnim.UVDimensions[0];
            UVTopLeft = uvAnim.frames[0];

            if (uvAnim.frames.Length != 1)
            {
                uvAnim.loopCycles = LoopCircles;
                Affector aft = new UVAffector(uvAnim, UVTime, node, RandomStartFrame);
                AffectorList.Add(aft);
            }
        }
        else
        {
            UVDimension = OriUVDimensions;
            UVTopLeft = OriTopLeftUV;
        }

        if(IsTowardZ) {
			//Debug.Log("toward z rotate affector "+IsTowardZ);
            Affector aft;
            aft = new RotateAffector(DeltaRot, node, IsTowardZ);
            AffectorList.Add(aft);
        }

        if (RotAffectorEnable && (RotateType != RSTYPE.NONE))
        {
            Affector aft;
            if (RotateType == RSTYPE.NONE)
                aft = new RotateAffector(DeltaRot, node, IsTowardZ);
            else
                aft = new RotateAffector(RotateType, node, IsTowardZ);
            AffectorList.Add(aft);
        }

        if (ScaleAffectorEnable && ScaleType != RSTYPE.NONE)
        {
            Affector aft;
            
            if (ScaleType == RSTYPE.NONE)
                aft = new ScaleAffector(DeltaScaleX, DeltaScaleY, node);
            else
                aft = new ScaleAffector(ScaleType, node);
            
            AffectorList.Add(aft);
        }
        if (ColorAffectorEnable /*&& ColorAffectType != 0*/)
        {
            ColorAffector aft = new ColorAffector(this, node);
            AffectorList.Add(aft);
        }
        if (JetAffectorEnable)
        {
            Affector aft = new JetAffector(JetMag,JetMagType,JetCurve,node, JetMin, JetMax);
            AffectorList.Add(aft);
        }
        if (VortexAffectorEnable)
        {
            Affector aft;
            aft = new VortexAffector(VortexObj, VortexMagType, VortexMag, VortexMagMax, VortexCurve, VortexDirection, VortexInheritRotation, node, VortexTime);

            AffectorList.Add(aft);
        }
        if (UVRotAffectorEnable)
        {
            Affector aft;
            
            float xscroll = UVRotXSpeed;
            float yscroll = UVRotYSpeed;
            if (RandomUVRotateSpeed)
            {
                xscroll = Random.Range(UVRotXSpeed,UVRotXSpeedMax);
                yscroll = Random.Range(UVRotYSpeed,UVRotYSpeedMax);
            }
            
            aft = new UVRotAffector(xscroll, yscroll, node);
            AffectorList.Add(aft);
        }


        if (GravityAffectorEnable)
        {
            Affector aft;
            aft = new GravityAffector(GravityObject, GravityAftType, GravityMagType, IsGravityAccelerate,GravityDirection, GravityMag, GravityCurve, node, IsSine, SineFreq);
            AffectorList.Add(aft);

            if (GravityAftType == GAFTTYPE.Spherical && GravityObject == null)
            {
                Debug.LogWarning("Gravity Object is missing, automatically set to effect layer self:" + gameObject.name);
                GravityObject = transform;
            }

        }
        
        if(SineAffectorEnable) {
            Affector aft;
            aft = new Xft.SineAffector(SineForce, ModifyPos, SineMaxFreq, SineMinFreq, node);
            AffectorList.Add(aft);
        }


        if (AirAffectorEnable)
        {
            Affector aft = new AirFieldAffector(AirObject, AirDirection, AirMagType, AirMagnitude, AirMagCurve, AirAttenuation, AirUseMaxDistance,
                AirMaxDistance, AirEnableSpread, AirSpread, AirInheritVelocity, AirInheritRotation, node);
            AffectorList.Add(aft);
        }
        if (BombAffectorEnable)
        {
            Affector aft = new BombAffector(BombObject, BombType, BombMagType, BombDecayType, BombMagnitude, BombMagCurve, BombDecay, BombAxis, node);
            AffectorList.Add(aft);
        }
        if (TurbulenceAffectorEnable)
        {
            Affector aft = new TurbulenceFieldAffector(TurbulenceObject, TurbulenceMagType, TurbulenceMagnitude, TurbulenceMagCurve, TurbulenceAttenuation, TurbulenceUseMaxDistance, TurbulenceMaxDistance, node);
            AffectorList.Add(aft);
        }
        if (DragAffectorEnable)
        {
            Affector aft = new DragAffector(DragObj,DragUseDir,DragDir,DragMag,DragUseMaxDist,DragMaxDist,DragAtten,node);
            AffectorList.Add(aft);
        }
        return AffectorList;
    }

    //fixed 2012.5.24
    //if set client by out caller. each node should update too.
    public void SetClient(Transform client)
    {
        ClientTransform = client;
        for (int i = 0; i < MaxENodes; i++)
        {
            EffectNode node = ActiveENodes[i];
            if (node == null)
                node = AvailableENodes[i];
            node.ClientTrans = client;
        }
    }

    protected void Init()
    {
        //added 2012.6.24
        InitCollision();
        
        Owner = transform.parent.gameObject.GetComponent<XffectComponent>();
        if (Owner == null)
            Debug.LogError("you must set EffectLayer to be XffectComponent's child.");

        //fixed 2012.6.2. ignoring the red errors.
        if (ClientTransform == null)
        {
            Debug.LogWarning("effect layer: "+ gameObject.name + " haven't assign a client transform, automaticly set to itself.");
            ClientTransform = transform;
        }
        AvailableENodes = new EffectNode[MaxENodes];
        ActiveENodes = new EffectNode[MaxENodes];
        for (int i = 0; i < MaxENodes; i++)
        {
            EffectNode n = new EffectNode(i, ClientTransform, SyncClient, this);
            List<Affector> afts = InitAffectors(n);
            n.SetAffectorList(afts);
            if (RenderType == 0)
                n.SetType(SpriteWidth, SpriteHeight, (STYPE)SpriteType, (ORIPOINT)OriPoint, SpriteUVStretch, 60f);
            else if (RenderType == 1)
            {
                float rwidth = RibbonWidth;
                float rlen = RibbonLen;
                if (UseRandomRibbon)
                {
                    rwidth = Random.Range(RibbonWidthMin, RibbonWidthMax);
                    rlen = Random.Range(RibbonLenMin, RibbonLenMax);
                }
                n.SetType(FaceToObject, FaceObject, rwidth, MaxRibbonElements, rlen, ClientTransform.position + EmitPoint, StretchType, 60f, IsWeapon, IsScale);
            }
            else if (RenderType == 2)
            {
                n.SetType(ConeSize,ConeSegment,ConeAngle, transform.rotation * OriVelocityAxis,0,60f,UseConeAngleChange,ConeDeltaAngle);
            }  
            else if (RenderType == 3)
            {
                Vector3 dir = Vector3.zero;
                if (OriVelocityAxis == Vector3.zero)
                {
                    OriVelocityAxis = Vector3.up;
                }
                
                dir = transform.rotation * OriVelocityAxis;
                
                n.SetType(CMesh,dir,60f);
            }
                
            AvailableENodes[i] = n;
        }
        AvailableNodeCount = MaxENodes;
        emitter = new Emitter(this);
        
        mStopped = false;
    }


    public VertexPool GetVertexPool()
    {
        return Vertexpool;
    }


    public int GetActiveNodeCount()
    {
        return ActiveENodes.Length;
    }

    public void RemoveActiveNode(EffectNode node)
    {
        if (AvailableNodeCount == MaxENodes)
            Debug.LogError("out index!");
        if (ActiveENodes[node.Index] == null) //already removed
            return;
        ActiveENodes[node.Index] = null;
        AvailableENodes[node.Index] = node;
        AvailableNodeCount++;
    }

    public void AddActiveNode(EffectNode node)
    {
        if (AvailableNodeCount == 0)
            Debug.LogError("out index!");
        if (AvailableENodes[node.Index] == null) //already added
            return;
        ActiveENodes[node.Index] = node;
        AvailableENodes[node.Index] = null;
        AvailableNodeCount--;
    }


    protected void AddNodes(int num)
    {
        int added = 0;
        for (int i = 0; i < MaxENodes; i++)
        {
            if (added == num)
                break;
            EffectNode node = AvailableENodes[i];
            if (node != null)
            {
                AddActiveNode(node);
                added++;
                
                //activate on birth subemitter.
                if (UseSubEmitters && !string.IsNullOrEmpty(BirthSubEmitter))
                {             
                    XffectComponent sub = SpawnCache.GetEffect(BirthSubEmitter);
                    if (sub == null)
                    {
                        return;
                    }
                    
                    node.SubEmitter = sub;
                    sub.Active();
                }


                emitter.SetEmitPosition(node);
                float nodeLife = 0;
                float realNodeLife = 0;
                if (IsNodeLifeLoop) {
                    nodeLife = -1;
                    realNodeLife = Random.Range(NodeLifeMin, NodeLifeMax); 
                }
                else {
                    nodeLife = Random.Range(NodeLifeMin, NodeLifeMax);
                    realNodeLife = nodeLife;
                }
                Vector3 oriDir = emitter.GetEmitRotation(node);
                float speed = OriSpeed;
                if (IsRandomSpeed)
                {
                    speed = Random.Range(SpeedMin, SpeedMax);
                }
                node.Init(oriDir.normalized, speed, nodeLife, Random.Range(OriRotationMin, OriRotationMax),
                    Random.Range(OriScaleXMin, OriScaleXMax), Random.Range(OriScaleYMin, OriScaleYMax),Color1, UVTopLeft, UVDimension, OriScaleZ, realNodeLife);
            }
            else
                continue;
        }
    }
    
    
    public void ResetCamera(Camera cam)
    {
        if (ActiveENodes == null)
            return;
        for (int i = 0; i < MaxENodes; i++)
        {
            EffectNode node = ActiveENodes[i];
            if (node != null)
            {
                node.ResetCamera(cam);
            }
            else
            {
                node = AvailableENodes[i];
                node.ResetCamera(cam);
            }
        }
    }
    
    public void Reset()
    {
        if (ActiveENodes == null)
            return;
        for (int i = 0; i < MaxENodes; i++)
        {
            EffectNode node = ActiveENodes[i];
            if (node != null)
            {
                node.Reset();
                RemoveActiveNode(node);
            }
        }
        emitter.Reset();
        
        mStopped = false;
    }

    public void FixedUpdateCustom(float deltaTime)
    {
#if UNITY_EDITOR
        if (UseSubEmitters && !EditorApplication.isPlaying)
        {
            Debug.LogWarning("Effect Layer:" + gameObject.name + " has sub emitters, it can't be updated in editor mode, please play the game to see the results.");
            return;
        }
#endif 
        
        //Version 2.2.0, update the camera when new level is loaded.
        if (MyCamera == null)
        {
            FindMyCamera();
            ResetCamera(MyCamera);
        }
        
        int needToAdd = emitter.GetNodes(deltaTime);
        AddNodes(needToAdd);
        for (int i = 0; i < MaxENodes; i++)
        {
            EffectNode node = ActiveENodes[i];
            if (node == null)
                continue;
            node.Update(deltaTime);
        }
    }

    public void StartCustom()
    {
        FindMyCamera();
        Init();
        LastClientPos = ClientTransform.position;
    }

    void OnDrawGizmos()
    {
        if (ClientTransform == null)
            return;

        Gizmos.color = DebugColor;

        float unitRadius = 0f;
        if (RenderType == 0)
            unitRadius = (SpriteWidth + SpriteHeight) / 6;
        else if (RenderType == 1)
            unitRadius = RibbonWidth / 3;
        else
            unitRadius = (ConeSize.x + ConeSize.y) / 6;
        unitRadius = Mathf.Clamp(unitRadius, 0, 1);
        //Visual Debug

        if (EmitType == 0 || EmitType == 3)
        {

            Gizmos.DrawWireSphere(ClientTransform.position + EmitPoint, unitRadius);
        }

        if (EmitType == 1)
        {
            Gizmos.DrawWireCube(ClientTransform.position + EmitPoint, BoxSize);
        }
        //else if (EmitType == 2)
        //{
        //    Gizmos.DrawWireSphere(ClientTransform.position + EmitPoint, Radius);
        //}
        else if (EmitType == 4)
        {
            //Gizmos.DrawSphere(ClientTransform.position + EmitPoint, Radius);
            Vector3 left = ClientTransform.position + EmitPoint + ClientTransform.forward * LineLengthLeft;
            Vector3 right = ClientTransform.position + EmitPoint + ClientTransform.forward * LineLengthRight;
            Gizmos.DrawLine(left, right);
        }
        else if (EmitType == 5)
        {
            //Graphics.DrawMeshNow(EmitMesh, Matrix4x4.identity);
        }

        if (OriVelocityAxis != Vector3.zero)
        {
            Gizmos.DrawLine(ClientTransform.position + EmitPoint, ClientTransform.position + EmitPoint + ClientTransform.rotation * OriVelocityAxis * unitRadius * 15);
        }
        
        if (UseCollisionDetection && CollisionType == COLLITION_TYPE.Plane)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(ClientTransform.position + PlaneOffset, new Vector3(unitRadius*50f,0f,unitRadius*50f));
            Gizmos.color = Color.white;
        }
    }

    //added. 2012.6.3
    //Xffect LifeTime < 0， 且又是EmitByRate的话，会自动判断是否已经没有活动节点，没有则自动Deactive()。
    public bool EmitOver(float deltaTime)
    {
        if (ActiveENodes == null)
        {
            return false;
        }
        if (AvailableNodeCount == MaxENodes)
        {
            if (EmitWay == EEmitWay.ByRate)
            {
                if (emitter.EmitLoop == 0)
                    return true;
            }
            else if (EmitWay == EEmitWay.ByCurve)
            {
                if (emitter.CurveEmitDone)
                    return true;
            }
            else if (EmitWay == EEmitWay.ByDistance)
            {
                if (mStopped)
                    return true;
            }
        }

        return false;
    }
 
    public void StopEmit()
    {
        mStopped = true;
        
        if (IsNodeLifeLoop)
        {
            for (int i = 0; i < MaxENodes; i++)
            {
                EffectNode node = ActiveENodes[i];
                if (node == null)
                    continue;
                node.Stop();
            }
        }
        emitter.StopEmit();
    }
    
    public void SetCollisionGoalPos(Transform pos)
    {
        if (UseCollisionDetection == false)
        {
            Debug.LogWarning(gameObject.name + "is not set to collision detect mode, please check it");
            return;
        }
        CollisionGoal = pos;
    }

    public void SetArractionAffectorGoal(Transform goal)
    {
        if (GravityAffectorEnable == false || GravityAftType == GAFTTYPE.Planar)
        {
            Debug.LogWarning(gameObject.name + "has no attraction affector, please check it");
            return;
        }
        foreach (EffectNode en in AvailableENodes)
        {
            List<Affector> affectorList = en.GetAffectorList();
            foreach (Affector aft in affectorList)
            {
                if (aft.Type == AFFECTORTYPE.GravityAffector)
                {
                    GravityAffector atft = (GravityAffector)aft;
                    atft.SetAttraction(goal);
                }
            }
        }

    }
    
    
    public void SetScale(Vector2 scale)
    {
        for (int i = 0; i < MaxENodes; i++)
        {
            EffectNode node = ActiveENodes[i];
            if (node == null)
                node = AvailableENodes[i];
            node.Scale = scale;
        }
    }

    // for out caller use.
    public EffectNode EmitByPos(Vector3 pos)
    {
        int added = 0;
        EffectNode ret = null;
        for (int i = 0; i < MaxENodes; i++)
        {
            if (added == 1)
                break;
            EffectNode node = AvailableENodes[i];
            if (node != null)
            {
                AddActiveNode(node);
                added++;

                node.SetLocalPosition(pos);
                float nodeLife = 0;
                float realLife = 0;
                if (IsNodeLifeLoop) {
                    nodeLife = -1;
                    realLife = Random.Range(NodeLifeMin, NodeLifeMax);
                }
                else {
                    nodeLife = Random.Range(NodeLifeMin, NodeLifeMax);
                    realLife = nodeLife;
                }
                Vector3 oriDir = emitter.GetEmitRotation(node);

                
                node.Init(oriDir.normalized, OriSpeed, nodeLife, Random.Range(OriRotationMin, OriRotationMax),
                    Random.Range(OriScaleXMin, OriScaleXMax), Random.Range(OriScaleYMin, OriScaleYMax), Color1, UVTopLeft, UVDimension, OriScaleZ, realLife);
                
                ret = node;
            }
            else
                continue;
        }
        return ret;
    }

	}
