using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using Xft;

[CustomEditor(typeof(EffectLayer))]
public class EffectLayerCustom : Editor
{
	
	public static string EditorAssets = "Assets/Xffect/Editor/EditorAssets";
	
    public static string ColorBkgMatPath = "Editor/color_bkg.mat";
	
	public static bool IsSkinLoaded = false;
	
	public static Material ColorBkgMat;
	
	public static GUISkin XSkin;
	public static GUISkin InspectorSkin;
	public static GUIStyle Xtoggle;
	public static GUIStyle Xbutton;
	public static GUIStyle XLabelField;
	public static GUIStyle XArea;
	public static GUIStyle XTexture;
	public static GUIStyle XToggle2;
	public static GUIStyle XInfoArea;
	
	
	public Texture2D GradientTex;
	
	public EffectLayer Script;
	

	
	protected Rect ColorBox;
	
	
	#region serialized property
	protected SerializedProperty StartTime;string TStartTime = "how long will this layer delay before getting start.";
	protected SerializedProperty DebugColor;string TDebugColor = "the debug color of this layer in the esitor scene view.";
	protected SerializedProperty MaxFps;
	protected SerializedProperty ClientTransform;string TClientTransform = "every node's original position is relative to this transform.";
	protected SerializedProperty SyncClient;string TSyncClient = "if you toggle this option, all the nodes' position will in the local space of the client transform.";
	protected SerializedProperty Material;string TMaterial = "specify the material of this Layer.";
	protected SerializedProperty Depth;string TDepth = "the render queue of this material, depth 0 means render queue 3000.";
	protected SerializedProperty RenderType;string TRenderType = "specify the node's render type.";
	
	//Sprite
	protected SerializedProperty SpriteType;string TSpriteType = "Billboard: A quad that will always rotate to face to camera.\nBillboardSelf: It will also face to the camera, but only rotate around its original direction. In fact, it's a Ribbon Trail which has only 2 elements.\nXZ Plane: A quad on the XZ Plane.";
	protected SerializedProperty OriPoint;string TOriPoint = "the pivot of this sprite.";
	protected SerializedProperty SpriteWidth;string TSpriteWidth = "the width of this sprite.";
	protected SerializedProperty SpriteHeight;string TSpriteHeight = "the height of this sprite.";

	
	//Ribbon Trail
	protected SerializedProperty RibbonWidth; string TRibbonWidth = "the width of the trail";
	protected SerializedProperty MaxRibbonElements; string TMaxRibbonElements = "the max num of quads in a trail is 'max elements' -1, so the more elements you set, the more smooth you get. And note this value should always > 2.";
	protected SerializedProperty RibbonLen; string TRibbonLen = "the length of the trail. If the current trail length exceed this length, the trail head will stretch and tail will shrink.";
	protected SerializedProperty FaceToObject; string TFaceToObject = "used for slash trail, make the trail face to object instead of facing to camera.";
	protected SerializedProperty FaceObject; string TFaceObject = "the object to face, can't be null";
	protected SerializedProperty UseRandomRibbon; string TUseRandomRibbon = "give the trail's size some randomness";
	protected SerializedProperty RibbonWidthMin;
	protected SerializedProperty RibbonWidthMax;
	protected SerializedProperty RibbonLenMin;
	protected SerializedProperty RibbonLenMax;
	protected SerializedProperty IsWeapon; 
	protected SerializedProperty IsScale;
	
	//Emitter
	protected SerializedProperty EmitType; string TEmitType = "the shape of the emitter";
    protected SerializedProperty BoxSize;
    protected SerializedProperty EmitPoint;
    protected SerializedProperty Radius;
    protected SerializedProperty CircleDir;
    protected SerializedProperty EmitUniform;
    protected SerializedProperty LineLengthLeft;
    protected SerializedProperty LineLengthRight;
    protected SerializedProperty MaxENodes; string TMaxENodes = "the maximum nodes the emitter can emit.";
    protected SerializedProperty IsNodeLifeLoop; string TIsNodeLifeLoop = "if check this option, then each node's life is infinite.";
    protected SerializedProperty NodeLifeMin;
    protected SerializedProperty NodeLifeMax;
    protected SerializedProperty EmitWay;
    protected SerializedProperty EmitterCurve;
    protected SerializedProperty DiffDistance;
    protected SerializedProperty EmitMesh;
    protected SerializedProperty EmitMeshType;
    protected SerializedProperty IsBurstEmit;
    protected SerializedProperty EmitDuration; string TEmitDuration = "each emition loop's duration. when the time expired, the emition loop count will -1.";
    
    protected SerializedProperty EmitRate; string TEmitRate = "emit rate of each emition.";
    protected SerializedProperty EmitLoop; string TEmitLoop = "when loop count becomes zero, the emition will stop. if this value < 0, the loop count is infinite.";
    protected SerializedProperty EmitDelay;string TEmitDelay = "specify the delay time after each loop has finished.";
    protected SerializedProperty UseRandomCircle;
    protected SerializedProperty CircleRadiusMin;
    protected SerializedProperty CircleRadiusMax;
	
    
	protected SerializedProperty DirType;
    protected SerializedProperty DirCenter;
    protected SerializedProperty OriVelocityAxis;
    protected SerializedProperty AngleAroundAxis;
    protected SerializedProperty OriSpeed; string TOriSpeed = "give each node a original speed, the velocity direction is the 'direction type' that you set.";
    protected SerializedProperty AlwaysSyncRotation; string TAlwaysSyncRotation = "each node's original direction is inherited from the client by default, but it will not be influenced by the client's rotation's change during run time, if you want always sync the direction to client, check on this option.";
    protected SerializedProperty IsRandomSpeed;
    protected SerializedProperty SpeedMin;
    protected SerializedProperty SpeedMax;
    protected SerializedProperty UseRandomDirAngle;
    protected SerializedProperty AngleAroundAxisMax;
	
	
	//gravity modifier
	protected SerializedProperty GravityAffectorEnable;
    protected SerializedProperty GravityAftType;
    protected SerializedProperty GravityMagType;
    protected SerializedProperty GravityMag;
    protected SerializedProperty GravityCurve;
    protected SerializedProperty GravityDirection;
    protected SerializedProperty GravityObject;
    protected SerializedProperty IsGravityAccelerate; string TIsGravityAccelerate = "if check on this option, the gravity force will be added to the node's velocity, or, the force will directly change the node's position. e.g, if you want every node be attracted by gravity object, but there has other modifiers on each node, you should uncheck this option to make each node close to garavity object without other influences.";
	protected SerializedProperty IsSine;
	protected SerializedProperty SineFreq;


    //sine modifier
    public SerializedProperty SineAffectorEnable;
    public SerializedProperty SineForce;
    public SerializedProperty ModifyPos;// change position or speed
    public SerializedProperty SineMaxFreq;
    public SerializedProperty SineMinFreq;
	//Bomb Affector
    protected SerializedProperty BombAffectorEnable;
    protected SerializedProperty BombObject;
    protected SerializedProperty BombType;
    protected SerializedProperty BombDecayType;
    protected SerializedProperty BombMagType;
    protected SerializedProperty BombMagnitude;
    protected SerializedProperty BombMagCurve;
    protected SerializedProperty BombAxis;
    protected SerializedProperty BombDecay;
	
	
	protected SerializedProperty AirAffectorEnable;
    protected SerializedProperty AirObject;
    protected SerializedProperty AirMagType;
    protected SerializedProperty AirMagnitude;
    protected SerializedProperty AirMagCurve;
    protected SerializedProperty AirDirection;
    protected SerializedProperty AirAttenuation;
    protected SerializedProperty AirUseMaxDistance;
    protected SerializedProperty AirMaxDistance;
    protected SerializedProperty AirEnableSpread;
    protected SerializedProperty AirSpread;
    protected SerializedProperty AirInheritVelocity; string TAirInheritVelocity = "describes how much of the air object's velocity is added to the magnitude and direction of the air field.";
    protected SerializedProperty AirInheritRotation;
	
	
	protected SerializedProperty VortexAffectorEnable;
    protected SerializedProperty VortexMagType;
    protected SerializedProperty VortexMag;
    protected SerializedProperty VortexMagMax;

    protected SerializedProperty VortexCurve;
    protected SerializedProperty VortexDirection;
    protected SerializedProperty VortexInheritRotation;
    protected SerializedProperty VortexObj;
    protected SerializedProperty IsRandomVortexDir;
	protected SerializedProperty IsVortexAccelerate; string TIsVortexAccelerate = "if check on this option, the vortex force will added to the node's velocity, or, the force will directly change the node's position.";
	protected SerializedProperty VortexAttenuation;
	protected SerializedProperty UseVortexMaxDistance;
	protected SerializedProperty VortexMaxDistance;
    protected SerializedProperty IsFixedCircle;
    protected SerializedProperty IsStandTail;
    protected SerializedProperty VortexTime;
	
	
	protected SerializedProperty JetAffectorEnable;
    protected SerializedProperty JetMagType;
    protected SerializedProperty JetMag;
    protected SerializedProperty JetCurve;
    protected SerializedProperty JetMin;
    protected SerializedProperty JetMax;
	
	
	protected SerializedProperty TurbulenceAffectorEnable;
    protected SerializedProperty TurbulenceObject;
    protected SerializedProperty TurbulenceMagType;
    protected SerializedProperty TurbulenceMagnitude;
    protected SerializedProperty TurbulenceMagCurve;
    protected SerializedProperty TurbulenceAttenuation;
    protected SerializedProperty TurbulenceUseMaxDistance;
    protected SerializedProperty TurbulenceMaxDistance;
	
	
	protected SerializedProperty DragAffectorEnable;
    protected SerializedProperty DragObj;
    protected SerializedProperty DragUseDir;
    protected SerializedProperty DragDir;
    protected SerializedProperty DragMag;
    protected SerializedProperty DragUseMaxDist;
    protected SerializedProperty DragMaxDist;
    protected SerializedProperty DragAtten;
	
	
	protected SerializedProperty ConeSize;
    protected SerializedProperty ConeAngle;
    protected SerializedProperty ConeSegment; string TConeSegment = "indicate the number of segments of the bottom circle, the more segment, the more smooth, and the value should always >= 3.";
    protected SerializedProperty UseConeAngleChange;
    protected SerializedProperty ConeDeltaAngle;
	
	
	protected SerializedProperty OriRotationMin;
    protected SerializedProperty OriRotationMax;
    protected SerializedProperty RotAffectorEnable;
    protected SerializedProperty RotateType;
    protected SerializedProperty DeltaRot;
    protected SerializedProperty RotateCurve;
	protected SerializedProperty RandomOriRot;
    
    protected SerializedProperty RotateCurveWrap;
    protected SerializedProperty RotateCurveTime;
    protected SerializedProperty RotateCurveMaxValue;
    protected SerializedProperty RotateCurve01;

	protected SerializedProperty IsTowardZ;

	protected SerializedProperty CMesh;
	
	protected SerializedProperty RandomOriScale;
	protected SerializedProperty OriScaleXMin;
    protected SerializedProperty OriScaleXMax;
    protected SerializedProperty OriScaleYMin;
    protected SerializedProperty OriScaleYMax;
    protected SerializedProperty OriScaleZ;
    protected SerializedProperty ScaleAffectorEnable;
    protected SerializedProperty ScaleType;
    protected SerializedProperty DeltaScaleX;
    protected SerializedProperty DeltaScaleY;
    protected SerializedProperty ScaleXCurve;
    protected SerializedProperty ScaleYCurve;
    
    protected SerializedProperty UseSameScaleCurve;
    protected SerializedProperty IsScaleZ;

    protected SerializedProperty RotateSpeedMin;
    protected SerializedProperty RotateSpeedMax;
    protected SerializedProperty DeltaScaleXMax;
    protected SerializedProperty DeltaScaleYMax;
	
    protected SerializedProperty ScaleWrapMode;
    protected SerializedProperty ScaleCurveTime;
    protected SerializedProperty ScaleXCurveNew;
    protected SerializedProperty ScaleYCurveNew;
    protected SerializedProperty MaxScaleCalue;
    protected SerializedProperty ScaleZCurveNew;

	
	protected SerializedProperty ColorAffectorEnable;
	protected SerializedProperty ColorGradualTimeLength;
    protected SerializedProperty ColorGradualType;
	protected SerializedProperty ColorChangeType;
	//protected SerializedProperty ColorParam;
	protected SerializedProperty ColorGradualCurve;
	protected SerializedProperty Color1;
	
	
	
	protected SerializedProperty UVAffectorEnable;
    protected SerializedProperty UVType;
    protected SerializedProperty OriTopLeftUV;
    protected SerializedProperty OriUVDimensions;
    protected SerializedProperty Cols;
    protected SerializedProperty Rows;
    protected SerializedProperty LoopCircles;
    protected SerializedProperty UVTime;
	protected SerializedProperty RandomStartFrame;
	protected SerializedProperty UVRotAffectorEnable;
    protected SerializedProperty RandomUVRotateSpeed;
    protected SerializedProperty UVRotXSpeed;
    protected SerializedProperty UVRotYSpeed;
    protected SerializedProperty UVRotXSpeedMax;
    protected SerializedProperty UVRotYSpeedMax;
	
	
	protected SerializedProperty SpriteUVStretch; string TSpriteUVStretch = "specify the uv stretch direction.";
	protected SerializedProperty StretchType; string TStretchType = "specify the uv stretch direction.";
	
	
	protected SerializedProperty UseCollisionDetection;
    protected SerializedProperty ParticleRadius;
    protected SerializedProperty CollisionType;
    protected SerializedProperty CollisionAutoDestroy;
    protected SerializedProperty EventReceiver;
    protected SerializedProperty EventHandleFunctionName;
    protected SerializedProperty CollisionGoal;
    protected SerializedProperty ColliisionPosRange;
    protected SerializedProperty CollisionLayer;

    protected SerializedProperty JumpHeight;
    
    protected SerializedProperty CollisionOffset;
    protected SerializedProperty PlaneDir;
    protected SerializedProperty PlaneOffset;
    //protected SerializedProperty CollisionCollider;
    
    protected SerializedProperty UseSubEmitters;
    protected SerializedProperty SpawnCache;
    protected SerializedProperty BirthSubEmitter;
    protected SerializedProperty CollisionSubEmitter;
    protected SerializedProperty DeathSubEmitter;
	
    
    
	
	#endregion
	
	protected static XEditorTool mXEditor;
	
	
	
	public static string TDELAY = "how long will this layer delay before getting start.";

	
	public XEditorTool XEditor
	{
		get{
			if (mXEditor == null)
			{
				mXEditor = new XEditorTool();
			}
				
			mXEditor.MyEditor = this;
			return mXEditor;
		}
	}
	public static bool LoadStyle()
	{
		if (IsSkinLoaded)
			return true;
		
		//first check if we've stored the right path
		string storedpath = EditorPrefs.GetString("xffect editor assets:");
		if (!string.IsNullOrEmpty(storedpath))
		{
			EditorAssets = storedpath;
		}
		
		string projectPath = Application.dataPath;
		if (projectPath.EndsWith ("/Assets")) {
			projectPath = projectPath.Remove (projectPath.Length-("Assets".Length));
		}
		
		//if can't load at the default path or the last stored path, then re-check it.
		if (!System.IO.File.Exists (projectPath + EditorAssets + "/Xskin.guiskin")) {
			//Initiate search
			System.IO.DirectoryInfo sdir = new System.IO.DirectoryInfo (Application.dataPath);
			
			Queue<System.IO.DirectoryInfo> dirQueue = new Queue<System.IO.DirectoryInfo>();
			dirQueue.Enqueue (sdir);
			
			bool found = false;
			while (dirQueue.Count > 0) {
				System.IO.DirectoryInfo dir = dirQueue.Dequeue ();
				if (System.IO.File.Exists (dir.FullName + "/Xskin.guiskin")) {
					string path = dir.FullName.Replace ('\\','/');
					found = true;
					//Remove data path from string to make it relative
					path = path.Replace (projectPath,"");
					
					if (path.StartsWith ("/")) {
						path = path.Remove (0,1);
					}
					
					EditorAssets = path;
					Debug.Log ("Located editor assets folder to '"+EditorAssets+"'");
					
					EditorPrefs.SetString("xffect editor assets:",EditorAssets);
					
					break;
				}
				System.IO.DirectoryInfo[] dirs = dir.GetDirectories ();
				for (int i=0;i<dirs.Length;i++) {
					dirQueue.Enqueue (dirs[i]);
				}
			}
			
			if (!found) {
				Debug.LogWarning ("Could not locate editor assets folder\nXffect");
				return false;
			}
		}
		
		XSkin = AssetDatabase.LoadAssetAtPath (EditorAssets + "/Xskin.guiskin",typeof(GUISkin)) as GUISkin;
		InspectorSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
		Xtoggle = XSkin.FindStyle ("xtoggle");
		Xbutton = XSkin.FindStyle ("xbutton");
		XLabelField = XSkin.FindStyle("xlabelfield");
		XArea = XSkin.FindStyle("xarea");
		XTexture = XSkin.FindStyle("xtexture");
		XToggle2 = XSkin.FindStyle("xtoggle2");
		
		//XInfoArea = XSkin.FindStyle("xinfoarea");
		
		XInfoArea = InspectorSkin.FindStyle("HelpBox")?? InspectorSkin.FindStyle("Box");
		
		IsSkinLoaded = true;
		
		
		//load color bkg mat
		
		ColorBkgMat = AssetDatabase.LoadAssetAtPath(XEditorTool.GetXffectPath() + ColorBkgMatPath,typeof(Material)) as Material;
		
		return true;
	}
	
	void InitSerializedProperty()
	{
		ClientTransform = serializedObject.FindProperty("ClientTransform");
		SyncClient = serializedObject.FindProperty("SyncClient");
		Material = serializedObject.FindProperty("Material");
		RenderType = serializedObject.FindProperty("RenderType");
		StartTime = serializedObject.FindProperty("StartTime");
		MaxFps = serializedObject.FindProperty("MaxFps");
		DebugColor = serializedObject.FindProperty("DebugColor");
		Depth = serializedObject.FindProperty("Depth");
		
		SpriteType = serializedObject.FindProperty("SpriteType");
		OriPoint = serializedObject.FindProperty("OriPoint");
		SpriteWidth = serializedObject.FindProperty("SpriteWidth");
		SpriteHeight = serializedObject.FindProperty("SpriteHeight");
		
		RibbonWidth = serializedObject.FindProperty("RibbonWidth");
		MaxRibbonElements = serializedObject.FindProperty("MaxRibbonElements");
		RibbonLen = serializedObject.FindProperty("RibbonLen");
		FaceToObject = serializedObject.FindProperty("FaceToObject");
		FaceObject = serializedObject.FindProperty("FaceObject");
		UseRandomRibbon = serializedObject.FindProperty("UseRandomRibbon");
		RibbonWidthMin = serializedObject.FindProperty("RibbonWidthMin");
		RibbonWidthMax = serializedObject.FindProperty("RibbonWidthMax");
		RibbonLenMin = serializedObject.FindProperty("RibbonLenMin");
		RibbonLenMax = serializedObject.FindProperty("RibbonLenMax");
		IsWeapon = serializedObject.FindProperty("IsWeapon");
		IsScale = serializedObject.FindProperty("IsScale");

		EmitType = serializedObject.FindProperty("EmitType");
		BoxSize = serializedObject.FindProperty("BoxSize");
		EmitPoint = serializedObject.FindProperty("EmitPoint");
		Radius = serializedObject.FindProperty("Radius");
		CircleDir = serializedObject.FindProperty("CircleDir");
		EmitUniform = serializedObject.FindProperty("EmitUniform");
		LineLengthLeft = serializedObject.FindProperty("LineLengthLeft");
		LineLengthRight = serializedObject.FindProperty("LineLengthRight");
		MaxENodes = serializedObject.FindProperty("MaxENodes");
		IsNodeLifeLoop = serializedObject.FindProperty("IsNodeLifeLoop");
		NodeLifeMin = serializedObject.FindProperty("NodeLifeMin");
		NodeLifeMax = serializedObject.FindProperty("NodeLifeMax");
		EmitWay = serializedObject.FindProperty("EmitWay");
		EmitterCurve = serializedObject.FindProperty("EmitterCurve");
		DiffDistance = serializedObject.FindProperty("DiffDistance");
		EmitMesh = serializedObject.FindProperty("EmitMesh");
		EmitMeshType = serializedObject.FindProperty("EmitMeshType");
        IsBurstEmit = serializedObject.FindProperty("IsBurstEmit");
		EmitDuration = serializedObject.FindProperty("EmitDuration");
		EmitRate = serializedObject.FindProperty("EmitRate");
		EmitLoop = serializedObject.FindProperty("EmitLoop");
		EmitDelay = serializedObject.FindProperty("EmitDelay");
        
        UseRandomCircle = serializedObject.FindProperty("UseRandomCircle");
        CircleRadiusMin = serializedObject.FindProperty("CircleRadiusMin");
        CircleRadiusMax = serializedObject.FindProperty("CircleRadiusMax");
		
		DirType = serializedObject.FindProperty("DirType");
		DirCenter = serializedObject.FindProperty("DirCenter");
		OriVelocityAxis = serializedObject.FindProperty("OriVelocityAxis");
		AngleAroundAxis = serializedObject.FindProperty("AngleAroundAxis");
		OriSpeed = serializedObject.FindProperty("OriSpeed");
		AlwaysSyncRotation = serializedObject.FindProperty("AlwaysSyncRotation");
		IsRandomSpeed = serializedObject.FindProperty("IsRandomSpeed");
		SpeedMin = serializedObject.FindProperty("SpeedMin");
		SpeedMax = serializedObject.FindProperty("SpeedMax");
        UseRandomDirAngle = serializedObject.FindProperty("UseRandomDirAngle");
        AngleAroundAxisMax = serializedObject.FindProperty("AngleAroundAxisMax");
        
		
		GravityAffectorEnable = serializedObject.FindProperty("GravityAffectorEnable");
		GravityAftType = serializedObject.FindProperty("GravityAftType");
		GravityMagType = serializedObject.FindProperty("GravityMagType");
		GravityMag = serializedObject.FindProperty("GravityMag");
		GravityCurve = serializedObject.FindProperty("GravityCurve");
		GravityDirection = serializedObject.FindProperty("GravityDirection");
		GravityObject = serializedObject.FindProperty("GravityObject");
		IsGravityAccelerate = serializedObject.FindProperty("IsGravityAccelerate");
		IsSine = serializedObject.FindProperty("IsSine");
		SineFreq = serializedObject.FindProperty("SineFreq");

        SineAffectorEnable = serializedObject.FindProperty("SineAffectorEnable");
        SineForce = serializedObject.FindProperty("SineForce");
        ModifyPos = serializedObject.FindProperty("ModifyPos");// change position or speed
        SineMaxFreq = serializedObject.FindProperty("SineMaxFreq");
        SineMinFreq = serializedObject.FindProperty("SineMinFreq");

 		
		BombAffectorEnable = serializedObject.FindProperty("BombAffectorEnable");
		BombObject = serializedObject.FindProperty("BombObject");
		BombType = serializedObject.FindProperty("BombType");
		BombDecayType = serializedObject.FindProperty("BombDecayType");
		BombMagType = serializedObject.FindProperty("BombMagType");
		BombMagnitude = serializedObject.FindProperty("BombMagnitude");
		BombMagCurve = serializedObject.FindProperty("BombMagCurve");
		BombAxis = serializedObject.FindProperty("BombAxis");
		BombDecay = serializedObject.FindProperty("BombDecay");
		
		AirAffectorEnable = serializedObject.FindProperty("AirAffectorEnable");
		AirObject = serializedObject.FindProperty("AirObject");
		AirMagType = serializedObject.FindProperty("AirMagType");
		AirMagnitude = serializedObject.FindProperty("AirMagnitude");
		AirMagCurve = serializedObject.FindProperty("AirMagCurve");
		AirDirection = serializedObject.FindProperty("AirDirection");
		AirAttenuation = serializedObject.FindProperty("AirAttenuation");
		AirUseMaxDistance = serializedObject.FindProperty("AirUseMaxDistance");
		AirMaxDistance = serializedObject.FindProperty("AirMaxDistance");
		AirEnableSpread = serializedObject.FindProperty("AirEnableSpread");
		AirSpread = serializedObject.FindProperty("AirSpread");
		AirInheritVelocity = serializedObject.FindProperty("AirInheritVelocity");
		AirInheritRotation = serializedObject.FindProperty("AirInheritRotation");
		
		
		VortexAffectorEnable = serializedObject.FindProperty("VortexAffectorEnable");
		VortexMagType = serializedObject.FindProperty("VortexMagType");
		VortexMag = serializedObject.FindProperty("VortexMag");
        VortexMagMax = serializedObject.FindProperty("VortexMagMax");

		VortexCurve = serializedObject.FindProperty("VortexCurve");
		VortexDirection = serializedObject.FindProperty("VortexDirection");
		VortexInheritRotation = serializedObject.FindProperty("VortexInheritRotation");
		VortexObj = serializedObject.FindProperty("VortexObj");
		IsRandomVortexDir = serializedObject.FindProperty("IsRandomVortexDir");
		IsVortexAccelerate = serializedObject.FindProperty("IsVortexAccelerate");
		VortexAttenuation = serializedObject.FindProperty("VortexAttenuation");
		UseVortexMaxDistance = serializedObject.FindProperty("UseVortexMaxDistance");
		VortexMaxDistance = serializedObject.FindProperty("VortexMaxDistance");
        IsFixedCircle = serializedObject.FindProperty("IsFixedCircle");
        IsStandTail = serializedObject.FindProperty("IsStandTail");
		VortexTime = serializedObject.FindProperty("VortexTime");

		JetAffectorEnable = serializedObject.FindProperty("JetAffectorEnable");
		JetMagType = serializedObject.FindProperty("JetMagType");
		JetMag = serializedObject.FindProperty("JetMag");
		JetCurve = serializedObject.FindProperty("JetCurve");
		JetMin = serializedObject.FindProperty("JetMin");
		JetMax = serializedObject.FindProperty("JetMax");
		
		
		TurbulenceAffectorEnable = serializedObject.FindProperty("TurbulenceAffectorEnable");
		TurbulenceObject = serializedObject.FindProperty("TurbulenceObject");
		TurbulenceMagType = serializedObject.FindProperty("TurbulenceMagType");
		TurbulenceMagnitude = serializedObject.FindProperty("TurbulenceMagnitude");
		TurbulenceMagCurve = serializedObject.FindProperty("TurbulenceMagCurve");
		TurbulenceAttenuation = serializedObject.FindProperty("TurbulenceAttenuation");
		TurbulenceUseMaxDistance = serializedObject.FindProperty("TurbulenceUseMaxDistance");
		TurbulenceMaxDistance = serializedObject.FindProperty("TurbulenceMaxDistance");
		
		
		DragAffectorEnable = serializedObject.FindProperty("DragAffectorEnable");
		DragObj = serializedObject.FindProperty("DragObj");
		DragUseDir = serializedObject.FindProperty("DragUseDir");
		DragDir = serializedObject.FindProperty("DragDir");
		DragMag = serializedObject.FindProperty("DragMag");
		DragUseMaxDist = serializedObject.FindProperty("DragUseMaxDist");
		DragMaxDist = serializedObject.FindProperty("DragMaxDist");
		DragAtten = serializedObject.FindProperty("DragAtten");
		
		
		ConeSize = serializedObject.FindProperty("ConeSize");
		ConeAngle = serializedObject.FindProperty("ConeAngle");
		ConeSegment = serializedObject.FindProperty("ConeSegment");
		UseConeAngleChange = serializedObject.FindProperty("UseConeAngleChange");
		ConeDeltaAngle = serializedObject.FindProperty("ConeDeltaAngle");
		
		CMesh = serializedObject.FindProperty("CMesh");
		
		
		OriRotationMin = serializedObject.FindProperty("OriRotationMin");
		OriRotationMax = serializedObject.FindProperty("OriRotationMax");
		RotAffectorEnable = serializedObject.FindProperty("RotAffectorEnable");
		RotateType = serializedObject.FindProperty("RotateType");
		DeltaRot = serializedObject.FindProperty("DeltaRot");
		RotateCurve = serializedObject.FindProperty("RotateCurve");
		RandomOriRot = serializedObject.FindProperty("RandomOriRot");
        RotateCurveWrap = serializedObject.FindProperty("RotateCurveWrap");
        RotateCurveTime = serializedObject.FindProperty("RotateCurveTime");
        RotateCurveMaxValue = serializedObject.FindProperty("RotateCurveMaxValue");
        RotateCurve01 = serializedObject.FindProperty("RotateCurve01");
        IsTowardZ = serializedObject.FindProperty("IsTowardZ");

		
		RandomOriScale = serializedObject.FindProperty("RandomOriScale");
		OriScaleXMin = serializedObject.FindProperty("OriScaleXMin");
		OriScaleXMax = serializedObject.FindProperty("OriScaleXMax");
		OriScaleYMin = serializedObject.FindProperty("OriScaleYMin");
		OriScaleYMax = serializedObject.FindProperty("OriScaleYMax");
		OriScaleZ = serializedObject.FindProperty("OriScaleZ");
		ScaleAffectorEnable = serializedObject.FindProperty("ScaleAffectorEnable");
		ScaleType = serializedObject.FindProperty("ScaleType");
		DeltaScaleX = serializedObject.FindProperty("DeltaScaleX");
		DeltaScaleY = serializedObject.FindProperty("DeltaScaleY");
		ScaleXCurve = serializedObject.FindProperty("ScaleXCurve");
		ScaleYCurve = serializedObject.FindProperty("ScaleYCurve");
		UseSameScaleCurve = serializedObject.FindProperty("UseSameScaleCurve");

        
        ScaleWrapMode = serializedObject.FindProperty("ScaleWrapMode");
        ScaleCurveTime = serializedObject.FindProperty("ScaleCurveTime");
        MaxScaleCalue = serializedObject.FindProperty("MaxScaleCalue");
        ScaleXCurveNew = serializedObject.FindProperty("ScaleXCurveNew");
        ScaleYCurveNew = serializedObject.FindProperty("ScaleYCurveNew");
        ScaleZCurveNew = serializedObject.FindProperty("ScaleZCurveNew");
        
        IsScaleZ = serializedObject.FindProperty("IsScaleZ");


        RotateSpeedMin = serializedObject.FindProperty("RotateSpeedMin");
        RotateSpeedMax = serializedObject.FindProperty("RotateSpeedMax");
        DeltaScaleXMax = serializedObject.FindProperty("DeltaScaleXMax");
        DeltaScaleYMax = serializedObject.FindProperty("DeltaScaleYMax");
		
		
		ColorAffectorEnable = serializedObject.FindProperty("ColorAffectorEnable");
		ColorGradualTimeLength = serializedObject.FindProperty("ColorGradualTimeLength");
		ColorGradualType = serializedObject.FindProperty("ColorGradualType");
		ColorChangeType = serializedObject.FindProperty("ColorChangeType");
		ColorGradualCurve = serializedObject.FindProperty("ColorGradualCurve");
		Color1 = serializedObject.FindProperty("Color1");
		
		
		UVAffectorEnable = serializedObject.FindProperty("UVAffectorEnable");
		UVType = serializedObject.FindProperty("UVType");
		OriTopLeftUV = serializedObject.FindProperty("OriTopLeftUV");
		OriUVDimensions = serializedObject.FindProperty("OriUVDimensions");
		Cols = serializedObject.FindProperty("Cols");
		Rows = serializedObject.FindProperty("Rows");
		LoopCircles = serializedObject.FindProperty("LoopCircles");
		UVTime = serializedObject.FindProperty("UVTime");
		RandomStartFrame = serializedObject.FindProperty("RandomStartFrame");
		UVRotAffectorEnable = serializedObject.FindProperty("UVRotAffectorEnable");
		RandomUVRotateSpeed = serializedObject.FindProperty("RandomUVRotateSpeed");
		UVRotXSpeed = serializedObject.FindProperty("UVRotXSpeed");
		UVRotYSpeed = serializedObject.FindProperty("UVRotYSpeed");
		UVRotXSpeedMax = serializedObject.FindProperty("UVRotXSpeedMax");
		UVRotYSpeedMax = serializedObject.FindProperty("UVRotYSpeedMax");
		
		SpriteUVStretch = serializedObject.FindProperty("SpriteUVStretch");
		StretchType = serializedObject.FindProperty("StretchType");
		
		
		UseCollisionDetection = serializedObject.FindProperty("UseCollisionDetection");
		ParticleRadius = serializedObject.FindProperty("ParticleRadius");
		CollisionType = serializedObject.FindProperty("CollisionType");
		CollisionAutoDestroy = serializedObject.FindProperty("CollisionAutoDestroy");
		EventReceiver = serializedObject.FindProperty("EventReceiver");
		EventHandleFunctionName = serializedObject.FindProperty("EventHandleFunctionName");
		CollisionGoal = serializedObject.FindProperty("CollisionGoal");
		ColliisionPosRange = serializedObject.FindProperty("ColliisionPosRange");
		CollisionLayer = serializedObject.FindProperty("CollisionLayer");
        
        CollisionOffset = serializedObject.FindProperty("CollisionOffset");
        PlaneDir = serializedObject.FindProperty("PlaneDir");
        PlaneOffset = serializedObject.FindProperty("PlaneOffset");
        JumpHeight = serializedObject.FindProperty("JumpHeight");
		//CollisionCollider = serializedObject.FindProperty("CollisionCollider");
        
        UseSubEmitters = serializedObject.FindProperty("UseSubEmitters");
        SpawnCache = serializedObject.FindProperty("SpawnCache");
        BirthSubEmitter = serializedObject.FindProperty("BirthSubEmitter");
        CollisionSubEmitter = serializedObject.FindProperty("CollisionSubEmitter");
        DeathSubEmitter = serializedObject.FindProperty("DeathSubEmitter");
	}
	
    
    void DrawSubEmitter()
    {
        XEditor.BeginXArea("Sub Emitters",XArea, XEditorTool.MinHeight,XEditorTool.EArea.CheckBox, UseSubEmitters);
     
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("xffect cache:",EffectLayerCustom.XLabelField,GUILayout.Width(XEditorTool.LabelWidth));
        SpawnCache.objectReferenceValue = EditorGUILayout.ObjectField(SpawnCache.objectReferenceValue,typeof(XffectCache),true);
        EditorGUILayout.EndHorizontal();
        
        XEditor.DrawText("Birth:","",BirthSubEmitter);
        XEditor.DrawText("Death:","",DeathSubEmitter);
        XEditor.DrawText("Collision:","",CollisionSubEmitter);
        
        XEditor.EndXArea();
 }
 	/*TODO: Add More Affector Function
	void DrawAddAffector() {
			XEditor.BeginXArea("AddEffector", XArea, XEditorTool.MinHeight, XEditorTool.EArea.CheckBox, )
	}
	*/
	void DrawCollision()
	{
		XEditor.BeginXArea("Collision",XArea, XEditorTool.MinHeight,XEditorTool.EArea.CheckBox, UseCollisionDetection);
		
		XEditor.DrawFloat("node radius:","",ParticleRadius);
		XEditor.DrawToggle("auto destroy","",CollisionAutoDestroy);
		
		XEditor.DrawSeparator();
		

		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("collision type:",""),XLabelField,GUILayout.Width(XEditorTool.LabelWidth));
        CollisionType.enumValueIndex = (int)(COLLITION_TYPE)EditorGUILayout.EnumPopup((COLLITION_TYPE)CollisionType.enumValueIndex);
		EditorGUILayout.EndHorizontal();
		
		if ((COLLITION_TYPE)CollisionType.enumValueIndex == COLLITION_TYPE.Sphere)
		{
			XEditor.DrawTransform("collision goal:","",CollisionGoal);
			XEditor.DrawFloat("goal radius:","",ColliisionPosRange);
            XEditor.DrawFloat("collision offset:","",CollisionOffset);
		}
		else if ((COLLITION_TYPE)CollisionType.enumValueIndex == COLLITION_TYPE.CollisionLayer)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("layer mask:",EffectLayerCustom.XLabelField,GUILayout.Width(XEditorTool.LabelWidth));
			CollisionLayer.intValue = EditorGUILayout.LayerField(CollisionLayer.intValue);
			EditorGUILayout.EndHorizontal();
            XEditor.DrawFloat("collision offset:","",CollisionOffset);
		}
		else
        {
            XEditor.DrawVector3Field("plane dir:","",PlaneDir);
            XEditor.DrawVector3Field("plane offset:","",PlaneOffset);
        }
		XEditor.DrawSeparator();
		XEditor.DrawFloat("JumpHeight: ", "", JumpHeight);
		
		XEditor.DrawTransform("event receiver:","",EventReceiver);
		
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("event handle function:",EffectLayerCustom.XLabelField,GUILayout.Width(XEditorTool.LabelWidth));
        EventHandleFunctionName.stringValue = EditorGUILayout.TextField(EventHandleFunctionName.stringValue);
        EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.Space();
		
		XEditor.EndXArea();
	}
	
	void DrawUVConfig()
	{
		
		XEditor.BeginXArea("UV Config",XArea, XEditorTool.MinHeight,XEditorTool.EArea.AlwaysToggle, null,1f);
		
		if (RenderType.intValue != 2 && RenderType.intValue != 3)
		{
			XEditor.DrawVector2Field("top left uv:","",OriTopLeftUV);
		}
		else
		{
			GUI.contentColor = Color.yellow;
			XEditor.DrawVector2Field("bottom left uv:","",OriTopLeftUV);
			GUI.contentColor = Color.white;
		}
		
		XEditor.DrawVector2Field("uv dimensions:","",OriUVDimensions);
		
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("uv change type:",""),XLabelField,GUILayout.Width(XEditorTool.LabelWidth));
        UVType.intValue = (int)(UVTYPE)EditorGUILayout.EnumPopup((UVTYPE)UVType.intValue);
		EditorGUILayout.EndHorizontal();
		
		UVAffectorEnable.boolValue = false;
		UVRotAffectorEnable.boolValue = false;
		
		if (UVType.intValue == 1)
		{
			XEditor.DrawInt("x tile:","",Cols);
			XEditor.DrawInt("y tile:","",Rows);
			XEditor.DrawFloat("time(-1 is node life):","",UVTime);
			XEditor.DrawInt("loop(-1 is infinite):","",LoopCircles);
			UVAffectorEnable.boolValue = true;
		}
		else if (UVType.intValue == 2)
		{
			if (XEditor.DrawToggle("random speed?","",RandomUVRotateSpeed))
			{
				XEditor.DrawFloat("speed X min:","",UVRotXSpeed);
				XEditor.DrawFloat("speed X max:","",UVRotXSpeedMax);
				
				XEditor.DrawFloat("speed Y min:","",UVRotYSpeed);
				XEditor.DrawFloat("speed Y max:","",UVRotYSpeedMax);
			}
			else
			{
				XEditor.DrawFloat("speed X:","",UVRotXSpeed);
				XEditor.DrawFloat("speed Y:","",UVRotYSpeed);
			}
			UVRotAffectorEnable.boolValue = true;
		}
		
		if (UVType.intValue != 0)
		{
			XEditor.DrawSeparator();
			XEditor.DrawToggle("random start frame?","",RandomStartFrame);
		}
		
		EditorGUILayout.Space();
		
		XEditor.EndXArea();
	}

	void DrawColorConfig()
	{
		
		XEditorTool.XArea m = XEditor.BeginXArea("Color Config",XArea, XEditorTool.MinHeight,XEditorTool.EArea.AlwaysToggle, null,1f);
		
		
		
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("color change type:",""),XLabelField,GUILayout.Width(XEditorTool.LabelWidth));
        ColorChangeType.enumValueIndex = (int)(COLOR_CHANGE_TYPE)EditorGUILayout.EnumPopup((COLOR_CHANGE_TYPE)ColorChangeType.enumValueIndex);
        EditorGUILayout.EndHorizontal();
		
		if ((COLOR_CHANGE_TYPE)ColorChangeType.enumValueIndex == COLOR_CHANGE_TYPE.Constant)
		{
			ColorAffectorEnable.boolValue = false;
			XEditor.DrawColor("color:","",Color1);
		}
		else
		{
			if((COLOR_CHANGE_TYPE)ColorChangeType.enumValueIndex == COLOR_CHANGE_TYPE.Gradient)
			{
				XEditor.DrawInfo("Tips: you can edit the gradient color while updating xffect in editor");
				XEditor.DrawFloat("time(-1 is node life):","",ColorGradualTimeLength);
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(new GUIContent("gradient mode:",""),XLabelField,GUILayout.Width(XEditorTool.LabelWidth));
				ColorGradualType.enumValueIndex = (int)(COLOR_GRADUAL_TYPE)EditorGUILayout.EnumPopup((COLOR_GRADUAL_TYPE)ColorGradualType.enumValueIndex);
				EditorGUILayout.EndHorizontal();
				if ((COLOR_GRADUAL_TYPE)ColorGradualType.enumValueIndex == COLOR_GRADUAL_TYPE.CURVE)
				{
					XEditor.DrawCurve("t curve:","",ColorGradualCurve,true);
				}
			}
			

			
			ColorAffectorEnable.boolValue = true;
			if (m.Open)
			{
				XEditorTool.RefreshGradientTex(ref GradientTex,Script.ColorParam,this);
			}
			
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Edit",GUILayout.Width(XEditorTool.LabelWidth)))
			{
				XGradientEditor.Show(Script.ColorParam,GradientTex,this);
			}
			
			//use a dummy label field to get the rect.
			EditorGUILayout.LabelField("",EffectLayerCustom.XLabelField,GUILayout.ExpandWidth(true));
			if (Event.current.type == EventType.Repaint )
			{
				ColorBox = GUILayoutUtility.GetLastRect();
			}
			EditorGUILayout.EndHorizontal();
			if (m.Open)
				EditorGUI.DrawPreviewTexture(ColorBox, GradientTex,EffectLayerCustom.ColorBkgMat);
		}
		

		
		XEditor.EndXArea();
	}
	
	void DrawScaleConfig()
	{
		XEditor.BeginXArea("Scale Config",XArea, XEditorTool.MinHeight,XEditorTool.EArea.AlwaysToggle, null,1f);
		
		if (XEditor.DrawToggle("random start scale?","",RandomOriScale))
		{
			XEditor.DrawFloat("start scale x min:","",OriScaleXMin);
			XEditor.DrawFloat("start scale x max:","",OriScaleXMax);
			XEditor.DrawFloat("start scale y min:","",OriScaleYMin);
			XEditor.DrawFloat("start scale y max:","",OriScaleYMax);
		}
		else
		{
			XEditor.DrawFloat("start scale x:","",OriScaleXMin);
			XEditor.DrawFloat("start scale y:","",OriScaleYMin);
			OriScaleXMax.floatValue = OriScaleXMin.floatValue;
			OriScaleYMax.floatValue = OriScaleYMin.floatValue;
		}
		XEditor.DrawFloat("start Scale z:", "", OriScaleZ);
		
		
		XEditor.DrawSeparator();
		
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("scale change type:",""),XLabelField,GUILayout.Width(XEditorTool.LabelWidth));
        ScaleType.enumValueIndex = (int)(RSTYPE)EditorGUILayout.EnumPopup((RSTYPE)ScaleType.enumValueIndex);
        EditorGUILayout.EndHorizontal();
		
		if ((RSTYPE)ScaleType.enumValueIndex == RSTYPE.NONE)
		{
			ScaleAffectorEnable.boolValue = false;
		}
		else if ((RSTYPE)ScaleType.enumValueIndex == RSTYPE.SIMPLE)
		{
			ScaleAffectorEnable.boolValue = true;
			XEditor.DrawFloat("speed X:","",DeltaScaleX);
			XEditor.DrawFloat("speed Y:","",DeltaScaleY);
		}
		else if ((RSTYPE)ScaleType.enumValueIndex == RSTYPE.CURVE)
		{
			ScaleAffectorEnable.boolValue = true;
            XEditor.DrawInfo("this curve editor is deprecated, please use 'CURVE01' instead.");
			if (XEditor.DrawToggle("same curve?","",UseSameScaleCurve))
			{
				XEditor.DrawCurve("scaleXY curve:","",ScaleXCurve);
			}
			else
			{
				XEditor.DrawCurve("scaleX curve:","",ScaleXCurve);
				XEditor.DrawCurve("scaleY curve:","",ScaleYCurve);
			}
		}
        else if ((RSTYPE)ScaleType.enumValueIndex == RSTYPE.RANDOM)
        {
            ScaleAffectorEnable.boolValue = true;
            XEditor.DrawFloat("speed X min:","",DeltaScaleX);
            XEditor.DrawFloat("speed X max:","",DeltaScaleXMax);
            XEditor.DrawSeparator();
            XEditor.DrawFloat("speed Y min:","",DeltaScaleY);
            XEditor.DrawFloat("speed Y max:","",DeltaScaleYMax);
        }
        else
        {
            ScaleAffectorEnable.boolValue = true;
            
            bool sameCurve = XEditor.DrawToggle("same curve?","",UseSameScaleCurve);
            
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("wrap mode:",""),XLabelField,GUILayout.Width(XEditorTool.LabelWidth));
            ScaleWrapMode.enumValueIndex = (int)(WRAP_TYPE)EditorGUILayout.EnumPopup((WRAP_TYPE)ScaleWrapMode.enumValueIndex);
            EditorGUILayout.EndHorizontal();
            XEditor.DrawFloat("time(-1 is node life):","",ScaleCurveTime);
            XEditor.DrawFloat("max value:","indicate the max value of the curve here",MaxScaleCalue);
            if (sameCurve)
            {
                XEditor.DrawCurve01("scaleXY curve:","",ScaleXCurveNew);
            }
            else
            {
                XEditor.DrawCurve01("scaleX curve:","",ScaleXCurveNew);
                XEditor.DrawCurve01("scaleY curve:","",ScaleYCurveNew);
            }
        }

        if(XEditor.DrawToggle("scale Z curve?", "", IsScaleZ)) {
        	XEditor.DrawCurve01("scale Z curve", "", ScaleZCurveNew);
        }

		
		EditorGUILayout.Space();
		
		XEditor.EndXArea();
	}
	
	
	void DrawRotationConfig()
	{
		XEditor.BeginXArea("Rotation Config",XArea, XEditorTool.MinHeight,XEditorTool.EArea.AlwaysToggle, null,3f);
		
		if (XEditor.DrawToggle("random start rotation?","",RandomOriRot))
		{
			XEditor.DrawInt("start rotation min:","",OriRotationMin);
			XEditor.DrawInt("start rotation max:","",OriRotationMax);
		}
		else
		{
			XEditor.DrawInt("start rotation:","",OriRotationMin);
			OriRotationMax.intValue = OriRotationMin.intValue;
		}
		
		XEditor.DrawSeparator();
		
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("rotation change type:",""),XLabelField,GUILayout.Width(XEditorTool.LabelWidth));
        RotateType.enumValueIndex = (int)(RSTYPE)EditorGUILayout.EnumPopup((RSTYPE)RotateType.enumValueIndex);
        EditorGUILayout.EndHorizontal();
		
		if ((RSTYPE)RotateType.enumValueIndex == RSTYPE.NONE)
		{
			RotAffectorEnable.boolValue = false;
		}
		else if ((RSTYPE)RotateType.enumValueIndex == RSTYPE.SIMPLE)
		{
			RotAffectorEnable.boolValue = true;
			XEditor.DrawFloat("speed:","",DeltaRot);
			if (RenderType.intValue == 0 && SpriteType.intValue == 1)
			{
				XEditor.DrawInfo("warning: billboard self sprite is not recommended to rotate, it might not work properly.");
			}
		}
		else if((RSTYPE)RotateType.enumValueIndex == RSTYPE.CURVE)
		{
			RotAffectorEnable.boolValue = true;
            
            XEditor.DrawInfo("this curve editor is deprecated, please use 'CURVE01' instead.");
            
			XEditor.DrawCurve("angle by curve:","",RotateCurve);
			if (RenderType.intValue == 0 && SpriteType.intValue == 1)
			{
				XEditor.DrawInfo("warning: billboard self sprite is not recommended to rotate, it might not work properly.");
			}
		}
        else if ((RSTYPE)RotateType.enumValueIndex == RSTYPE.RANDOM)
        {
            RotAffectorEnable.boolValue = true;
            XEditor.DrawFloat("speed min:","",RotateSpeedMin);
            XEditor.DrawFloat("speed max:","",RotateSpeedMax);
        }
		else
        {
            RotAffectorEnable.boolValue = true;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("wrap mode:",""),XLabelField,GUILayout.Width(XEditorTool.LabelWidth));
            RotateCurveWrap.enumValueIndex = (int)(WRAP_TYPE)EditorGUILayout.EnumPopup((WRAP_TYPE)RotateCurveWrap.enumValueIndex);
            EditorGUILayout.EndHorizontal();
            XEditor.DrawFloat("time(-1 is node life):","",RotateCurveTime);
            XEditor.DrawFloat("max value:","indicates this curve's max value",RotateCurveMaxValue);
            XEditor.DrawCurve01("curve:","",RotateCurve01);
        }
        XEditor.DrawToggle("IsTowardZ", "", IsTowardZ);

		XEditor.EndXArea();
	}
	
	void DrawCustomMesh()
	{
		XEditor.BeginXArea("CustomMesh Config",XArea, XEditorTool.MinHeight,XEditorTool.EArea.AlwaysToggle, null);
		
		XEditor.DrawMesh("custom mesh:","",CMesh);
		
		XEditor.EndXArea();
	}
	
	void DrawConeConfig()
	{
		XEditor.BeginXArea("Cone",XArea, XEditorTool.MinHeight,XEditorTool.EArea.AlwaysToggle, null);
		
		XEditor.DrawVector2Field("size:","",ConeSize);
		XEditor.DrawInt("segment(>=3)",TConeSegment,ConeSegment);
		
		if(XEditor.DrawToggle("angle change?","",UseConeAngleChange))
		{
			XEditor.DrawCurve("angle change curve:","",ConeDeltaAngle);
		}
        else
        {
            XEditor.DrawFloat("angle:","",ConeAngle);
        }
		
		XEditor.EndXArea();
			
	}
	
	void DrawDragModifier()
	{
		XEditor.BeginXArea("Drag Modifier",XArea, XEditorTool.MinHeight,XEditorTool.EArea.CheckBox, DragAffectorEnable,2f);
		
		XEditor.DrawTransform("position:","",DragObj);
		
		if (XEditor.DrawToggle("use direction?","",DragUseDir))
		{
			XEditor.DrawVector3Field("direction:","",DragDir);
		}
		
		if (XEditor.DrawToggle("limit in max distance?","",DragUseMaxDist))
		{
			XEditor.DrawFloat("max distance:","",DragMaxDist);
		}
		
		XEditor.DrawFloat("magnitude:","",DragMag);
		
		XEditor.DrawFloat("attenuation:","",DragAtten);
		
		XEditor.EndXArea();
	}
	
	void DrawTurbulenceAffector()
	{
		XEditor.BeginXArea("Turbulence Modifier",XArea, XEditorTool.MinHeight,XEditorTool.EArea.CheckBox, TurbulenceAffectorEnable);
		XEditor.DrawTransform("position:","",TurbulenceObject);
		XEditor.DrawEnumMagType("magnitude type:","",TurbulenceMagType);
		
		if ((MAGTYPE)TurbulenceMagType.enumValueIndex == MAGTYPE.Fixed)
		{
			XEditor.DrawFloat("magnitude:","",TurbulenceMagnitude);
		}
		else
		{
			XEditor.DrawCurve("magnitude curve:","",TurbulenceMagCurve);
		}
		
		if (XEditor.DrawToggle("use max distance:","",TurbulenceUseMaxDistance))
		{
			XEditor.DrawFloat("max distance:","",TurbulenceMaxDistance);
		}
		
		XEditor.DrawSlider("attenuation:","",TurbulenceAttenuation,0f,1f);
		
		XEditor.EndXArea();
	}
	
	void DrawAccelerateAffector()
	{
		
		XEditor.BeginXArea("Acceleration Modifier",XArea, XEditorTool.MinHeight,XEditorTool.EArea.CheckBox, JetAffectorEnable);
		XEditor.DrawEnumMagType("magnitude type:","",JetMagType);
		
		if ((MAGTYPE)JetMagType.enumValueIndex == MAGTYPE.Fixed)
		{
			XEditor.DrawFloat("magnitude:","",JetMag);
		}
		else if((MAGTYPE)JetMagType.enumValueIndex == MAGTYPE.Curve)
		{
			XEditor.DrawCurve("magnitude curve:","",JetCurve);
		}
		else if((MAGTYPE) JetMagType.enumValueIndex == MAGTYPE.RANDOM) {
			XEditor.DrawFloat("min", "", JetMin);
			XEditor.DrawFloat("max", "", JetMax);
		}
		
		
		XEditor.EndXArea();
	}
	
	void DrawVortexAffector()
	{
		XEditor.BeginXArea("Vortex Modifier",XArea, XEditorTool.MinHeight,XEditorTool.EArea.CheckBox, VortexAffectorEnable,1f);
		
		XEditor.DrawTransform("position:","",VortexObj);
		
		if (!XEditor.DrawToggle("random direction?","",IsRandomVortexDir))
		{
			XEditor.DrawVector3Field("direction:","",VortexDirection);
			
			XEditor.DrawToggle("inherit rotation?","",VortexInheritRotation);
		}
		
		XEditor.DrawEnumMagType("magnitude type:","",VortexMagType);
		
		if ((MAGTYPE)VortexMagType.enumValueIndex == MAGTYPE.Curve)
		{
			XEditor.DrawCurve("magnitude curve:","",VortexCurve);
			XEditor.DrawFloat("magnitude", "", VortexMag);
		}
        else if((MAGTYPE)VortexMagType.enumValueIndex == MAGTYPE.RANDOM)
		{
			XEditor.DrawFloat("magnitude Min:","",VortexMag);
            XEditor.DrawFloat("magnitude Max:","",VortexMagMax);
        }else {
            XEditor.DrawFloat("magnitude:","",VortexMag);
        }
		
        XEditor.DrawToggle("accelerate?",TIsVortexAccelerate,IsVortexAccelerate);
        
        if (IsVortexAccelerate.boolValue)
        {
            if (XEditor.DrawToggle("use max distance?","",UseVortexMaxDistance))
            {
                XEditor.DrawFloat("max distance:","",VortexMaxDistance);
            }
            XEditor.DrawSlider("attenuation:","",VortexAttenuation,0f,1f);
        }
        else
        {
            XEditor.DrawToggle("fixed circle track?","",IsFixedCircle);
            XEditor.DrawFloat("Vortex Time", "", VortexTime);
        }

        //XEditor.DrawToggle("stand tail", "", IsStandTail);

		XEditor.EndXArea();
	}
	
	void DrawAirAffector()
	{
		
		XEditor.BeginXArea("Air Modifier",XArea, XEditorTool.MinHeight,XEditorTool.EArea.CheckBox, AirAffectorEnable);
		
		XEditor.DrawInfo("note you should set the air object's position different to node's emit pos.");
		
		XEditor.DrawTransform("air object:","",AirObject);
		XEditor.DrawVector3Field("air direction:","",AirDirection);
		XEditor.DrawEnumMagType("magnitude type:","",AirMagType);
		
		if ((MAGTYPE)AirMagType.enumValueIndex == MAGTYPE.Fixed)
		{
			XEditor.DrawFloat("magnitude:","",AirMagnitude);
		}
		else
		{
			XEditor.DrawCurve("magnitude curve:","",AirMagCurve);
		}
		
		if (XEditor.DrawToggle("limit in distance?","",AirUseMaxDistance))
		{
			XEditor.DrawFloat("air max distance:","",AirMaxDistance);
		}
		
		XEditor.DrawFloat("attenuation:","",AirAttenuation);
		
		if (XEditor.DrawToggle("enable spread:","",AirEnableSpread))
		{
			XEditor.DrawSlider("air spread:","",AirSpread,0f,1f);
		}
		
		XEditor.DrawSlider("inherit velocity:",TAirInheritVelocity,AirInheritVelocity,0f,1f);
		
		XEditor.DrawToggle("inherit rotation:","",AirInheritRotation);
		
		XEditor.EndXArea();
		
	}
	
	
	
	void DrawBombAffector()
	{
		
		XEditor.BeginXArea("Bomb Modifier",XArea, XEditorTool.MinHeight,XEditorTool.EArea.CheckBox, BombAffectorEnable);
		
		XEditor.DrawTransform("bomb pos:","",BombObject);
		
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("bomb type:",""),XLabelField,GUILayout.Width(XEditorTool.LabelWidth));
        BombType.enumValueIndex = (int)(BOMBTYPE)EditorGUILayout.EnumPopup((BOMBTYPE)BombType.enumValueIndex);
        EditorGUILayout.EndHorizontal();
		
		if ((BOMBTYPE)BombType.enumValueIndex != BOMBTYPE.Spherical)
			XEditor.DrawVector3Field("bomb axis:","",BombAxis);
		
		XEditor.DrawEnumMagType("magnitude type:","",BombMagType);
		if ((MAGTYPE)BombMagType.enumValueIndex == MAGTYPE.Fixed)
		{
			XEditor.DrawFloat("magnitude:","",BombMagnitude);
		}
		else
		{
			XEditor.DrawCurve("magnitude curve:","",BombMagCurve);
		}
		
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("decay type:",""),XLabelField,GUILayout.Width(XEditorTool.LabelWidth));
        BombDecayType.enumValueIndex = (int)(BOMBDECAYTYPE)EditorGUILayout.EnumPopup((BOMBDECAYTYPE)BombDecayType.enumValueIndex);
        EditorGUILayout.EndHorizontal();
		
		if ((BOMBDECAYTYPE)BombDecayType.enumValueIndex != BOMBDECAYTYPE.None)
		{
			XEditor.DrawFloat("decay distance:","",BombDecay);
		}
		
		XEditor.EndXArea();
		
	}
	
	void DrawAffectors()
	{
		XEditor.BeginXArea("Modifier",XArea,XEditorTool.MinHeight,XEditorTool.EArea.AlwaysToggle,null);
		DrawGravityAffector();
		
		DrawSineAffector();

		DrawBombAffector();
		
		DrawAirAffector();
		
		DrawVortexAffector();
		
		DrawAccelerateAffector();
		
		DrawTurbulenceAffector();
		
		DrawDragModifier();
		
		XEditor.EndXArea();
	}
	
	void DrawSineAffector() {

        XEditor.BeginXArea("Sine Modifier", XArea, XEditorTool.MinHeight, XEditorTool.EArea.CheckBox, SineAffectorEnable);
        XEditor.DrawVector3Field("force", "", SineForce);
		XEditor.DrawToggle("modify Pos:", "", ModifyPos);
        XEditor.DrawFloat("SineMaxFreq", "", SineMaxFreq);
        XEditor.DrawFloat("SineMinFreq", "", SineMinFreq);

		XEditor.EndXArea();
	}

	void DrawGravityAffector()
	{
		
		XEditor.BeginXArea("Gravity Modifier",XArea, XEditorTool.MinHeight,XEditorTool.EArea.CheckBox, GravityAffectorEnable);

		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("gravity type:",XLabelField,GUILayout.Width(XEditorTool.LabelWidth));
        GravityAftType.enumValueIndex = (int)(GAFTTYPE)EditorGUILayout.EnumPopup((GAFTTYPE)GravityAftType.enumValueIndex);
        EditorGUILayout.EndHorizontal();
		XEditor.DrawTransform("gravity object:","",GravityObject);
		
		if ((GAFTTYPE)GravityAftType.enumValueIndex == GAFTTYPE.Planar)
		{
			XEditor.DrawVector3Field("direction:","",GravityDirection);

		}
		XEditor.DrawEnumMagType("magnitude type:","",GravityMagType);
		if ((MAGTYPE)GravityMagType.enumValueIndex == MAGTYPE.Fixed)
		{
			XEditor.DrawFloat("magnitude:","",GravityMag);
		}
		else
		{
			XEditor.DrawCurve("magnitude curve:","",GravityCurve);
		}
		
		
		XEditor.DrawToggle("is accelerate?",TIsGravityAccelerate,IsGravityAccelerate);
		if(XEditor.DrawToggle("isSine", "", IsSine)) {
			XEditor.DrawFloat("SineFreq", "", SineFreq);
		}
		
		XEditor.EndXArea();
		
	}
	
	void DrawDirectionConfig()
	{
		XEditor.BeginXArea("Direction Config",XArea,XEditorTool.MinHeight,XEditorTool.EArea.AlwaysToggle,null);
		
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("direction type:",XLabelField,GUILayout.Width(XEditorTool.LabelWidth));
        DirType.enumValueIndex = (int)(DIRECTION_TYPE)EditorGUILayout.EnumPopup((DIRECTION_TYPE)DirType.enumValueIndex);
        EditorGUILayout.EndHorizontal();
		
		if ((DIRECTION_TYPE)DirType.enumValueIndex == DIRECTION_TYPE.Planar)
		{
			XEditor.DrawVector3Field("original direction:","",OriVelocityAxis);
		}
		else if ((DIRECTION_TYPE)DirType.enumValueIndex == DIRECTION_TYPE.Sphere)
		{
			XEditor.DrawTransform("direction center:","",DirCenter);
		}
		else if ((DIRECTION_TYPE)DirType.enumValueIndex == DIRECTION_TYPE.Cone)
		{
            XEditor.DrawVector3Field("cone axis:","",OriVelocityAxis);
            if (XEditor.DrawToggle("random angle?","",UseRandomDirAngle))
            {
                XEditor.DrawInt("angle min:","",AngleAroundAxis);
                XEditor.DrawInt("angle max:","",AngleAroundAxisMax);
            }
            else
            {
                XEditor.DrawInt("angle:","",AngleAroundAxis);
            }
		}
		else if ((DIRECTION_TYPE)DirType.enumValueIndex == DIRECTION_TYPE.Cylindrical)
		{
			XEditor.DrawVector3Field("cylindrical axis:","",OriVelocityAxis);
			XEditor.DrawTransform("direction center:","",DirCenter);
		}
		
		XEditor.DrawToggle("inherit client rotation?",TAlwaysSyncRotation,AlwaysSyncRotation);
		
		XEditor.DrawSeparator();
		
		if(XEditor.DrawToggle("random speed?","",IsRandomSpeed))
		{
			XEditor.DrawFloat("speed min:","",SpeedMin);
			XEditor.DrawFloat("speed max:","",SpeedMax);
		}
		else
		{
			XEditor.DrawFloat("original speed:",TOriSpeed,OriSpeed);
		}
		XEditor.EndXArea();
	}
	
	void DrawEmitterConfig()
	{
		XEditor.BeginXArea("Emitter Config",XArea,XEditorTool.MinHeight,XEditorTool.EArea.AlwaysToggle,null);
		
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("emitter shape:",TEmitType),XLabelField,GUILayout.Width(XEditorTool.LabelWidth));
        EmitType.intValue = (int)(EMITTYPE)EditorGUILayout.EnumPopup((EMITTYPE)EmitType.intValue);
        EditorGUILayout.EndHorizontal();
		
		if (EmitType.intValue == 0)
		{
			XEditor.DrawVector3Field("emit pos:","",EmitPoint);
		}
		else if (EmitType.intValue == 1)
		{
			XEditor.DrawVector3Field("box center:","",EmitPoint);
			XEditor.DrawVector3Field("box size:","",BoxSize);
		}
		else if (EmitType.intValue == 2)
		{
			XEditor.DrawVector3Field("sphere center:","",EmitPoint);
			XEditor.DrawFloat("sphere radius:","",Radius);
		}
		else if (EmitType.intValue == 3)
		{
			XEditor.DrawVector3Field("circle center:","",EmitPoint);
			XEditor.DrawVector3Field("circle direction:","",CircleDir);
            if (XEditor.DrawToggle("random radius?","",UseRandomCircle))
            {
                XEditor.DrawFloat("radius min:","",CircleRadiusMin);
                XEditor.DrawFloat("radius min:","",CircleRadiusMax);
            }
            else
            {
                XEditor.DrawFloat("circle radius:","",Radius);
                XEditor.DrawToggle("emit uniformly?","",EmitUniform);                
            }

		}
		else if (EmitType.intValue == 4)
		{
			XEditor.DrawVector3Field("line center:","",EmitPoint);
			
			XEditor.DrawFloat("left length:","",LineLengthLeft);
			
			XEditor.DrawFloat("right length:","",LineLengthRight);
		}
		else if (EmitType.intValue == 5)
		{
			XEditor.DrawMesh("mesh:","",EmitMesh);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("mesh emit pos:",XLabelField,GUILayout.Width(XEditorTool.LabelWidth));
			EmitMeshType.intValue = (int)(MESHEMIT_TYPE)EditorGUILayout.EnumPopup((MESHEMIT_TYPE)EmitMeshType.intValue);
			EditorGUILayout.EndHorizontal();
			XEditor.DrawToggle("emit uniformly?","",EmitUniform);
		}
		
		XEditor.DrawSeparator();
		
		XEditor.DrawInt("max nodes:", TMaxENodes,MaxENodes);
		XEditor.DrawToggle("is node life infinite?", TIsNodeLifeLoop,IsNodeLifeLoop);
		if (!IsNodeLifeLoop.boolValue)
		{
			XEditor.DrawFloat("node life min:","",NodeLifeMin);
			XEditor.DrawFloat("node life max:","",NodeLifeMax);
		}
		
        XEditor.DrawSeparator();
        
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("emit method:",XLabelField,GUILayout.Width(XEditorTool.LabelWidth));
		EmitWay.enumValueIndex = (int)(EEmitWay)EditorGUILayout.EnumPopup((EEmitWay)EmitWay.enumValueIndex);
		EditorGUILayout.EndHorizontal();
		
		if ((EEmitWay)EmitWay.enumValueIndex == EEmitWay.ByDistance)
		{
			XEditor.DrawFloat("diff distance:", "",DiffDistance);
		}
		else if ((EEmitWay)EmitWay.enumValueIndex == EEmitWay.ByRate)
		{
			//XEditor.DrawSlider("chance:",TChanceToEmit,ChanceToEmit,1,100);
            if (XEditor.DrawToggle("burst?","burst emit at start",IsBurstEmit))
            {
                XEditor.DrawFloat("burst count:",TEmitRate, EmitRate);
            }
            else
            {
                XEditor.DrawFloat("emit duration:", TEmitDuration,EmitDuration);
                XEditor.DrawFloat("emit rate:",TEmitRate, EmitRate);
                XEditor.DrawInt("loop count(-1 infinite):",TEmitLoop,EmitLoop);
                XEditor.DrawFloat("delay after each loop:",TEmitDelay,EmitDelay);     
            }
		}
		else
		{
			XEditor.DrawCurve("emit curve:","",EmitterCurve);
		}
		
		XEditor.EndXArea();
	}
	
	void DrawRibbonTrail()
	{
		XEditor.BeginXArea("Ribbon Trail",XArea,XEditorTool.MinHeight,XEditorTool.EArea.AlwaysToggle,null);

		if (XEditor.DrawToggle("random start size?",TUseRandomRibbon,UseRandomRibbon))
		{
			XEditor.DrawFloat("width min:","",RibbonWidthMin);
			XEditor.DrawFloat("width max:","",RibbonWidthMax);
			
			XEditor.DrawFloat("length min:","",RibbonLenMin);
			XEditor.DrawFloat("length max:","",RibbonLenMax);
		}
		else
		{
			XEditor.DrawFloat("width:",TRibbonWidth,RibbonWidth);
			XEditor.DrawFloat("trail length:",TRibbonLen,RibbonLen);
		}
		
		XEditor.DrawSeparator();
		
		XEditor.DrawInt("max elements(>=3):",TMaxRibbonElements,MaxRibbonElements);
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(new GUIContent("uv stretch:",TStretchType),XLabelField,GUILayout.Width(XEditorTool.LabelWidth));
		StretchType.intValue = (int)(STRETCH_TYPE)EditorGUILayout.EnumPopup((STRETCH_TYPE)StretchType.intValue);
		EditorGUILayout.EndHorizontal();
		
		if (XEditor.DrawToggle("slash trail?",TFaceToObject,FaceToObject))
		{
			XEditor.DrawTransform("face to object:",TFaceObject,FaceObject);
		}
		
		if(XEditor.DrawToggle("isWeapon", "", IsWeapon)) {

		}
		if(XEditor.DrawToggle("isScale", "", IsScale)) {

		}
		
		XEditor.EndXArea();
	}
	
	
	void DrawSpriteConfig()
	{
		XEditor.BeginXArea("Sprite",XArea,XEditorTool.MinHeight,XEditorTool.EArea.AlwaysToggle,null);
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("sprite type:",TSpriteType),XLabelField,GUILayout.Width(XEditorTool.LabelWidth));
        SpriteType.intValue = (int)(STYPE)EditorGUILayout.EnumPopup((STYPE)SpriteType.intValue);
        EditorGUILayout.EndHorizontal();

		if (SpriteType.intValue == 1 || SpriteType.intValue == 2)
		{
			XEditor.DrawInfo("the heading direction is based on 'Direction Config'");
		}
		
		if (SpriteType.intValue == 1)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(new GUIContent("uv stretch:",TSpriteUVStretch),XLabelField,GUILayout.Width(XEditorTool.LabelWidth));
			SpriteUVStretch.intValue = (int)(STRETCH_TYPE)EditorGUILayout.EnumPopup((STRETCH_TYPE)SpriteUVStretch.intValue);
			EditorGUILayout.EndHorizontal();
		}
		
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("pivot",TOriPoint),XLabelField,GUILayout.Width(XEditorTool.LabelWidth));
        OriPoint.intValue = (int)(ORIPOINT)EditorGUILayout.EnumPopup((ORIPOINT)OriPoint.intValue);
        EditorGUILayout.EndHorizontal();
		
		XEditor.DrawFloat("width:",TSpriteWidth,SpriteWidth);
		XEditor.DrawFloat("height:",TSpriteHeight,SpriteHeight);
		
		
		XEditor.EndXArea();
	}
	
	
	
	
	void OnEnable()
	{
		Script = target as EffectLayer;
		InitSerializedProperty();
		LoadStyle();
	}
	
	
	void DrawMainConfig()
	{
		XEditor.BeginXArea("Effect Layer",XArea,XEditorTool.MinHeightBig,XEditorTool.EArea.Texture,null);
		
		XEditor.DrawMaterial("material",TMaterial,Material);
		XEditor.DrawInt("depth:",TDepth,Depth);
		
		EditorGUILayout.Space();
		
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("render type:",TRenderType),XLabelField,GUILayout.Width(XEditorTool.LabelWidth));
        RenderType.intValue = (int)(RENDER_TYPE)EditorGUILayout.EnumPopup((RENDER_TYPE)RenderType.intValue);
        EditorGUILayout.EndHorizontal();
		
		XEditor.DrawFloat("delay(s):",TStartTime,StartTime);
		XEditor.DrawColor("debug color:",TDebugColor,DebugColor);
		XEditor.DrawFloat("Fps", "", MaxFps);
		
		XEditor.DrawTransform("client:",TClientTransform,ClientTransform);
		XEditor.DrawToggle("sync pos to client?",TSyncClient,SyncClient);

	
		
		XEditor.EndXArea();
	}
	
	
	public override void OnInspectorGUI ()
	{
		serializedObject.Update();
		
		DrawMainConfig();
		
		if (RenderType.intValue == 0)
			DrawSpriteConfig();
		else if (RenderType.intValue == 1)
			DrawRibbonTrail();
		else if (RenderType.intValue == 2)
			DrawConeConfig();
		else
			DrawCustomMesh();
		
		DrawEmitterConfig();
		
		DrawDirectionConfig();
		
		DrawUVConfig();
		
		if (RenderType.intValue != 1)
		{
			DrawRotationConfig();
			DrawScaleConfig();
		}
		
		DrawColorConfig();
		
		DrawAffectors();
		
		DrawCollision();
		
        DrawSubEmitter();
        
		serializedObject.ApplyModifiedProperties();
	}
}
