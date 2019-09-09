using UnityEngine;
using System.Collections;
using SimpleJSON;
using System;
using MyLib;

#if UNITY_EDITOR
using UnityEditor;
using Xft;
#endif

/// <summary>
///导入粒子效果之后 需要点击一下 各个属性 粒子的属性才能生效 
/// </summary>
public class MakeParticle : MonoBehaviour
{
    public string configFile;
    [ButtonCallFunc()]
    public bool
        Make;

    
    #if UNITY_EDITOR
    public void MakeMethod()
    {
        var md = AssetDatabase.LoadAssetAtPath("Assets/Config/" + configFile + ".json", typeof(TextAsset)) as TextAsset;
        var jarr = SimpleJSON.JSON.Parse(md.text).AsArray;
        var xffectObj = DoCreateXffectObject();
        xffectObj.name = configFile.Split(char.Parse(".")) [0];

        foreach (SimpleJSON.JSONNode n in jarr)
        {
            var layer = DoCreateLayer(xffectObj);

            var jobj = GetProp(n);
            var name = jobj ["NAME"].Value;
            layer.name = name;
            var effectLayer = layer.GetComponent<EffectLayer>();
            var tex = jobj ["TEXTURE"].Value;
            var renderStyle = jobj ["RENDER STYLE"].Value;
            Shader useShader;
            if (renderStyle == "Additive")
            {
                useShader = Shader.Find("Mobile/Particles/Additive");
            } else if (renderStyle == "Modulate")
            {
                useShader = Shader.Find("Mobile/Particles/Multiply");
            } else
            {
                useShader = Shader.Find("Mobile/Particles/Alpha Blended");
            }

            if (!string.IsNullOrEmpty(jobj ["U RATE"].Value))
            {
                var uRate = System.Convert.ToSingle(jobj ["U RATE"].Value);
                var vRate = System.Convert.ToSingle(jobj ["V RATE"].Value);
                effectLayer.UVRotXSpeed = uRate;
                effectLayer.UVRotYSpeed = vRate;
                effectLayer.UVRotAffectorEnable = true;
                effectLayer.RandomUVRotateSpeed = false;
                effectLayer.UVType = 2;
                effectLayer.RandomStartFrame = true;
            }

            var texPath = tex.Replace("media", "Assets")
                .Replace("MEDIA", "Assets")
                .Replace(".dds", ".mat").Replace(".DDS", ".mat").Replace("\\", "/");

            var texName = texPath.Replace(".mat", ".png");
            Debug.Log("load Material " + texPath + "  " + texName);

            var mat = AssetDatabase.LoadAssetAtPath(texPath, typeof(Material)) as Material;
            Debug.Log("load Material " + mat);

            if (mat == null)
            {
                mat = new Material(useShader);
                mat.SetTexture("_MainTex", AssetDatabase.LoadAssetAtPath(texName, typeof(Texture)) as Texture);
                AssetDatabase.CreateAsset(mat, texPath);
                AssetDatabase.ImportAsset(texPath);
            }

            effectLayer.Material = mat;

            SetProp(effectLayer, jobj);
            //Modifier
            var child = GetChildren(n);
            var childArr = child ["children"].AsArray;
            foreach (JSONNode c in childArr)
            {
                var modData = c ["children"] [0].AsObject;
                var modType = modData ["DESCRIPTOR"].Value;
                if (modType == "Color")
                {
                    var overTime = modData ["COLOR OVER TIME"].Value;
                    float[] param;
                    if (string.IsNullOrEmpty(overTime))
                    {
                        param = new float[]{ 0, 1, 1, 1, 1 };
                    } else
                    {
                        param = ConvertToFloat(overTime.Split(','));
                    }
                    SetColor(effectLayer, param);
                } else if (modType == "Emitter")
                {
                    Debug.Log("Emit Config " + modData.ToString());
                    SetEmit(effectLayer, modData);

                } else if (modType == "Texture Rotate")
                {

                    Debug.Log("Rotate Config " + modData.ToString());
                    SetRotate(effectLayer, modData);
                } else if (modType == "Scale")
                {
                    SetScale(effectLayer, modData);
                } else if (modType == "Texture Animation")
                {
                    SetAni(effectLayer, modData);
                } else if (modType == "Gravity Well")
                {
                    SetGravity(effectLayer, modData);
                } else if (modType == "Vortext")
                {
                    SetVortext(effectLayer, modData);
                } else if (modType == "Sine Force")
                {
                    SetSine(effectLayer, modData);
                } else if (modType == "Linear Force")
                {
                    SetLinear(effectLayer, modData);
                } else if (modType == "Geometry Rotator")
                {
                    SetGeometryRotate(effectLayer, modData);
                }

                if (c ["children"].Count > 1)
                {
                    Debug.Log(" Set Embedded Modifier " + c ["children"].Count);
                    SetLayerModifier(effectLayer, c ["children"] [1] ["children"].AsArray);
                }

            }

        }
    }

    void SetLayerModifier(EffectLayer effectLayer, JSONArray modifier)
    {
        foreach (JSONNode c in modifier)
        {
            var modData = c ["children"] [0].AsObject;
            var modType = modData ["DESCRIPTOR"].Value;
            if (modType == "Color")
            {
                var overTime = modData ["COLOR OVER TIME"].Value;
                var param = ConvertToFloat(overTime.Split(','));
                SetColor(effectLayer, param);
            } else if (modType == "Emitter")
            {
                SetEmit(effectLayer, modData);
                
            } else if (modType == "Texture Rotate")
            {
                Debug.Log("Rotate Config " + modData.ToString());
                SetRotate(effectLayer, modData);
            } else if (modType == "Scale")
            {
                SetScale(effectLayer, modData);
            } else if (modType == "Texture Animation")
            {
                SetAni(effectLayer, modData);
            } else if (modType == "Gravity Well")
            {
                SetGravity(effectLayer, modData);
            } else if (modType == "Vortext")
            {
                SetVortext(effectLayer, modData);
            } else if (modType == "Sine Force")
            {
                SetSine(effectLayer, modData);
            } else if (modType == "Linear Force")
            {
                SetLinear(effectLayer, modData);
            }
        }
    }

    void SetLinear(EffectLayer effectLayer, JSONClass modData)
    {
        effectLayer.GravityAffectorEnable = true;
        var dir = Vector3.up;
        if (modData ["FORCES"] != null)
        {
            var forces = ConvertToFloat(modData ["FORCES"].Value);
            dir = new Vector3(forces [0], forces [1], forces [2]);
        } else
        {
            dir = new Vector3(modData ["FORCESX"].AsFloat, modData ["FORCESY"].AsFloat, modData ["FORCESZ"].AsFloat);
        }
        effectLayer.GravityDirection = dir.normalized;
        effectLayer.GravityMag = dir.magnitude * velScale;
    }

    void SetSine(EffectLayer effectLayer, JSONClass modData)
    {
        effectLayer.SineAffectorEnable = true;
        var fx = modData ["FORCESX"].AsFloat;
        var fy = modData ["FORCESY"].AsFloat;
        var fz = modData ["FORCESZ"].AsFloat;

        effectLayer.SineForce = new Vector3(fx, fy, fz);
        effectLayer.ModifyPos = true;
        effectLayer.SineMinFreq = modData ["MIN"].AsFloat;
        effectLayer.SineMaxFreq = modData ["MAX"].AsFloat;

    }

    void SetVortexCurve(EffectLayer effectLayer, float[] curve)
    {

        int count = (curve.Length - 1) / 2;
        float x = 0;
        float value = 0;
        //float max = 1;
        var ks = new Keyframe[count];

        int c = 0;
        for (int i = 1; i < curve.Length; i++)
        {
            var mod = (i - 1) % 2;
            if (mod == 0)
            {
                x = curve [i];
            } else if (mod == 1)
            {
                value = curve [i];
                ks [c] = new Keyframe(x, value);
                c++;
            }
        }
        effectLayer.VortexCurve = new AnimationCurve(ks);
    }

    void SetVortext(EffectLayer effectLayer, JSONClass modData)
    {
        effectLayer.VortexAffectorEnable = true;

        var dirx = modData ["DIRECTIONX"].AsFloat;
        var diry = modData ["DIRECTIONY"].AsFloat;
        var dirz = modData ["DIRECTIONZ"].AsFloat;
        if (diry == 0)
        {
            diry = 1;
        }
        effectLayer.VortexDirection = new Vector3(dirx, diry, dirz);

        var rateData = modData ["RATE"] == null ? modData ["rate"] : modData ["RATE"];
        if (rateData != null)
        {
            var rate = ConvertToFloat(rateData.Value);
            var rateType = (int)rate [0];
            Log.Sys("VortexCurveType " + rateType);
            if (rateType == 0)
            {
                effectLayer.VortexMag = rate [1];
            } else if (rateType == 3)
            {
                effectLayer.VortexMagType = MAGTYPE.Curve;
                SetVortexCurve(effectLayer, rate);
            }
        }
        var posx = modData ["POSITIONX"].AsFloat;
        var posy = modData ["POSITIONY"].AsFloat;
        var posz = modData ["POSITIONZ"].AsFloat;
        var g = new GameObject(effectLayer.name + "_vortexCenter");
        g.transform.parent = effectLayer.transform.parent;
        Util.InitGameObject(g);
        g.transform.localPosition = new Vector3(posx, posy, posz);

        effectLayer.VortexObj = g.transform;
        effectLayer.IsVortexAccelerate = true;
    }

    void SetGravity(EffectLayer effectLayer, JSONClass modData)
    {
        Debug.Log("SetGravity " + modData.ToString());
        var gra = modData ["GRAVITY"].AsFloat;
        bool enable = true;
        if (modData ["ENABLE"].Value != "")
        {
            enable = modData ["ENABLE"].AsBool;
        }

        effectLayer.GravityAffectorEnable = enable;
        effectLayer.IsGravityAccelerate = false;
        effectLayer.GravityMag = gra;
     
        var px = modData ["POSITIONX"].AsFloat;
        var py = modData ["POSITIONY"].AsFloat;
        var pz = modData ["POSITIONZ"].AsFloat;
        var g = new GameObject(effectLayer.name + "_gravityWell");
        g.transform.parent = effectLayer.transform.parent;
        effectLayer.GravityObject = g.transform;
        Util.InitGameObject(g);
        g.transform.localPosition = new Vector3(-px, py, pz);
        effectLayer.GravityAftType = GAFTTYPE.Spherical;
    }

    float velScale = 1;

    void SetProp(EffectLayer effectLayer, JSONClass modData)
    {
        var fw = modData ["FRAMES WIDE"].AsInt;
        var fh = modData ["FRAMES HIGH"].AsInt;
        if (fw > 0)
        {
            Debug.Log("Set UV");
            effectLayer.UVType = 1;
            effectLayer.Cols = fw;
            effectLayer.Rows = Mathf.Max(1, fh);
            
        }

        var renderType = modData ["RENDER TYPE"].Value;
        if (renderType == "Billboard Up" || renderType == "Billboard Up Camera" || renderType == "Billboard Forward")
        {
            effectLayer.RenderType = 0;
            effectLayer.SpriteType = (int)Xft.STYPE.BILLBOARD_UP;
        } else if (renderType == "EntityWorld")
        {
            Debug.LogError(" Model Particle " + effectLayer.name);
            effectLayer.RenderType = 3;
        } else if (renderType == "RibbonTrail")
        {
            Debug.LogError("RibbomTrail");
            effectLayer.RenderType = 1;
        } else if (renderType == "Sphere")
        {
            effectLayer.RenderType = 3;
            //effectLayer.CMesh = ;
            Debug.LogError("手动设置Sphere 粒子模型");
        }

        var isLight = modData ["IS LIGHT"].AsBool;
        if (isLight)
        {
            effectLayer.gameObject.layer = (int)GameLayer.Light;

        }
        if (modData ["ORIGIN"].Value == "Bottom Center")
        {
            effectLayer.OriPoint = (int)ORIPOINT.BOTTOM_CENTER;
        } else if (modData ["ORIGIN"].Value == "Center Left")
        {
            effectLayer.OriPoint = (int)ORIPOINT.LEFT_CENTER;
        }

        if (modData ["POSITIONX"].Value != "")
        {
            var px = modData ["POSITIONX"].AsFloat;
            var py = modData ["POSITIONY"].AsFloat;
            var pz = modData ["POSITIONZ"].AsFloat;
            effectLayer.transform.localPosition = new Vector3(-px, py, pz);
        }
        var vs = modData ["VELOCITY SCALE"];
        if (vs != null)
        {
            velScale = vs.AsFloat;
        } else
        {
            velScale = 1;
        }

        if (effectLayer.RenderType == 1)
        {
            var segments = modData ["SEGMENTS"];
            var width = modData ["WIDTH"];
            effectLayer.MaxRibbonElements = (int)segments.AsDouble;
            effectLayer.RibbonWidth = (float)width.AsDouble;
        }
    }

    void SetRandomScaleCurve(EffectLayer effectLayer, float[] scale)
    {
        int count = (scale.Length - 1) / 3;
        float x = 0;
        float value = 0;
        float max = 1;

        for (int i = 1; i < scale.Length; i++)
        {
            var mod = (i - 1) % 3;
            if (mod == 0)
            {
                x = scale [i];
            } else if (mod == 1)
            {
                value = scale [i];
                if (Mathf.Abs(value) > max)
                {
                    max = Mathf.Abs(value);
                }
            }
        }

        effectLayer.MaxScaleCalue = max;
        var ks = new Keyframe[count];
        int c = 0;
        for (int i = 1; i < scale.Length; i++)
        {
            var mod = (i - 1) % 3;
            if (mod == 0)
            {
                x = scale [i];
            } else if (mod == 1)
            {
                value = scale [i];
                Debug.Log("Add Scale Node " + value / max);
                ks [c] = new Keyframe(x, value / max);
                c++;

            }
        }
        effectLayer.ScaleXCurveNew = new AnimationCurve(ks);
        effectLayer.ScaleWrapMode = WRAP_TYPE.LOOP;
    }

    void SetScaleXCurve(EffectLayer effectLayer, float[] scale)
    {
        int scaType = (int)scale [0];
        if (scaType == 4)
        {
            SetRandomScaleCurve(effectLayer, scale);
        } else
        {
            int count = (scale.Length - 1) / 2;
            float x = 0;
            float value = 0;
            float max = 1;
            for (int i = 1; i < scale.Length; i++)
            {
                if ((i - 1) % 2 == 0)
                {
                    x = scale [i];
                } else
                {
                    value = scale [i];
                    if (Mathf.Abs(value) > max)
                    {
                        max = Mathf.Abs(value);
                    }
                }
            }
            effectLayer.MaxScaleCalue = max;
            var ks = new Keyframe[count];
            int c = 0;
            for (int i = 1; i < scale.Length; i++)
            {
                if ((i - 1) % 2 == 0)
                {
                    x = scale [i];
                } else
                {
                    value = scale [i];
                    Debug.Log("Add Scale Node " + value / max);
                    ks [c] = new Keyframe(x, value / max);
                    c++;

                }
            }
            effectLayer.ScaleXCurveNew = new AnimationCurve(ks);
            effectLayer.ScaleWrapMode = WRAP_TYPE.LOOP;
        }
    
    }

    void AdjustScaleX(EffectLayer effectLayer, float[] scale)
    {
        int scaType = (int)scale [0];
        if (scaType == 4)
        {
            SetRandomScaleCurve(effectLayer, scale);
        } else
        {
            int count = (scale.Length - 1) / 2;
            float x = 0;
            float value = 0;
            float max = 1;
            /*
            for (int i = 1; i < scale.Length; i++)
            {
                if ((i - 1) % 2 == 0)
                {
                    x = scale [i];
                } else
                {
                    value = scale [i];
                    if (Mathf.Abs(value) > max)
                    {
                        max = Mathf.Abs(value);
                    }
                }
            }
            */
            max = effectLayer.MaxScaleCalue;
            //effectLayer.MaxScaleCalue = max;
            var ks = new Keyframe[count];
            int c = 0;
            for (int i = 1; i < scale.Length; i++)
            {
                if ((i - 1) % 2 == 0)
                {
                    x = scale [i];
                } else
                {
                    value = scale [i];
                    Debug.Log("Add Scale Node " + value / max);
                    ks [c] = new Keyframe(x, value / max);
                    c++;

                }
            }
            effectLayer.ScaleXCurveNew = new AnimationCurve(ks);
            effectLayer.ScaleWrapMode = WRAP_TYPE.LOOP;
        }
    }

    void SetScaleYCurve(EffectLayer effectLayer, float[] scale, float[] scaleX)
    {
        int scaType = (int)scale [0];
        if (scaType == 4)
        {
            SetRandomScaleCurve(effectLayer, scale);
        } else
        {
            int count = (scale.Length - 1) / 2;
            float x = 0;
            float value = 0;
            float max = 1;
            for (int i = 1; i < scale.Length; i++)
            {
                if ((i - 1) % 2 == 0)
                {
                    x = scale [i];
                } else
                {
                    value = scale [i];
                    if (Mathf.Abs(value) > max)
                    {
                        max = Mathf.Abs(value);
                    }
                }
            }
            //保持X的MaxValue不要调整
            if (max > effectLayer.MaxScaleCalue)
            {
                effectLayer.MaxScaleCalue = max;
                AdjustScaleX(effectLayer, scaleX);
            } else
            {
                max = effectLayer.MaxScaleCalue;
            }
            var ks = new Keyframe[count];
            int c = 0;
            for (int i = 1; i < scale.Length; i++)
            {
                if ((i - 1) % 2 == 0)
                {
                    x = scale [i];
                } else
                {
                    value = scale [i];
                    Debug.Log("Add Scale Node " + value / max);
                    ks [c] = new Keyframe(x, value / max);
                    c++;

                }
            }
            effectLayer.ScaleYCurveNew = new AnimationCurve(ks);
            effectLayer.ScaleWrapMode = WRAP_TYPE.LOOP;
        }
    }

    void SetScale(EffectLayer effectLayer, JSONClass modData)
    {
        bool fix = false;
        if (!string.IsNullOrEmpty(modData ["FIXED"].Value))
        {
            fix = modData ["FIXED"].AsBool;
        }
        Debug.Log("SetScale: " + modData.ToString());
        effectLayer.ScaleType = RSTYPE.CURVE01;
        if (string.IsNullOrEmpty(modData ["X"].Value))
        {
            var f = new float[]{ 2, 0, 1, 1, 1 };
            SetScaleXCurve(effectLayer, f);
        } else
        {
            var scaleX = ConvertToFloat(modData ["X"].Value);
            SetScaleXCurve(effectLayer, scaleX);
        }
        //fix 确保XY 比例不变
        if (!fix)
        {
            if (string.IsNullOrEmpty(modData ["Y"].Value))
            {
                //var f = new float[]{ 2, 0, 1, 1, 1 };
                //SetScaleYCurve(effectLayer, f);
                effectLayer.UseSameScaleCurve = true;
            } else
            {
                var scaleX = ConvertToFloat(modData ["X"].Value);
                var scaleY = ConvertToFloat(modData ["Y"].Value);
                SetScaleYCurve(effectLayer, scaleY, scaleX);
            }
        } else
        {
            effectLayer.UseSameScaleCurve = true;
        }
        /*
        if (fix)
        {
            effectLayer.ScaleType = RSTYPE.CURVE01;
            effectLayer.UseSameScaleCurve = true;

            var scale = ConvertToFloat(modData ["X"].Value);
            int scaType = (int)scale [0];
            if (scaType == 4)
            {
                SetRandomScaleCurve(effectLayer, scale);
            } else
            {
                //scale[0]  2  curve  3 straight
                int count = (scale.Length - 1) / 2;
                float x = 0;
                float value = 0;
                float max = 1;
                for (int i = 1; i < scale.Length; i++)
                {
                    if ((i - 1) % 2 == 0)
                    {
                        x = scale [i];
                    } else
                    {
                        value = scale [i];
                        if (Mathf.Abs(value) > max)
                        {
                            max = Mathf.Abs(value);
                        }
                    }
                }
                effectLayer.MaxScaleCalue = max;
                var ks = new Keyframe[count];
                int c = 0;
                for (int i = 1; i < scale.Length; i++)
                {
                    if ((i - 1) % 2 == 0)
                    {
                        x = scale [i];
                    } else
                    {
                        value = scale [i];
                        Debug.Log("Add Scale Node " + value / max);
                        ks [c] = new Keyframe(x, value / max);
                        c++;

                    }
                }
                effectLayer.ScaleXCurveNew = new AnimationCurve(ks);
                effectLayer.ScaleWrapMode = WRAP_TYPE.LOOP;
            }
        }
        */
    }

    void SetGeometryRotate(EffectLayer effectLayer, JSONClass modData)
    {
        var rotX = modData ["AXIS OF ROTATIONX"].AsFloat;
        var rotY = modData ["AXIS OF ROTATIONY"].AsFloat;
        var rotZ = modData ["AXIS OF ROTATIONZ"].AsFloat;
        var speed = modData ["ROTATION SPEED"].Value;
        var speedList = ConvertToFloat(speed);
        if (((int)speedList [0]) == 0)
        {
            var min = speedList [1] * 90;
            effectLayer.RotateType = RSTYPE.SIMPLE;
            effectLayer.RotateSpeedMin = min;
            effectLayer.RotateSpeedMax = min;
        }

        effectLayer.OriVelocityAxis = new Vector3(-rotX, rotY, rotZ);
    }

    //float oldRotateSpeed = 10;

    void SetRotate(EffectLayer effectLayer, JSONClass modData)
    {
        var rotValue = modData ["STARTING ROTATION"].Value;
        if (rotValue != "")
        {
            var rotate = ConvertToFloat(rotValue);

            if ((int)(rotate [0]) == 1)
            {
                Debug.Log("rate value " + rotate [0]);
                effectLayer.RandomOriRot = true;
                effectLayer.OriRotationMin = (int)rotate [1];
                effectLayer.OriRotationMax = (int)rotate [2];
            }
        }
        float[] speed = new float[]{ 0, 10 };
        if (modData ["ROTATION SPEED"].Value == "")
        {
            //oldRotateSpeed = 10;
        } else
        {
            speed = ConvertToFloat(modData ["ROTATION SPEED"].Value);
        }

        if ((int)(speed [0]) == 1)
        {
            float min = speed [1];
            float max = speed [2];
            effectLayer.RotateType = RSTYPE.RANDOM;
            effectLayer.RotateSpeedMin = min;
            effectLayer.RotateSpeedMax = max;
        } else if ((int)(speed [0]) == 0)
        {
            float min = speed [1];
            effectLayer.RotateType = RSTYPE.SIMPLE;
            effectLayer.RotateSpeedMin = min;
            effectLayer.RotateSpeedMax = min;
        } else if (((int)speed [0]) == 2)
        {
            effectLayer.RotateType = RSTYPE.CURVE01;
            effectLayer.RotateCurveWrap = WRAP_TYPE.LOOP;
            effectLayer.RotateCurveTime = -1;
            var max = 360;
            effectLayer.RotateCurveMaxValue = max;
            var ks = new Keyframe[(speed.Length - 1) / 2];

            for (int i = 0; i < (speed.Length - 1) / 2; i++)
            {
                var t = speed [i * 2 + 1];
                var angle = speed [i * 2 + 2];
                ks [i] = new Keyframe(t, angle * 1.0f / max);
            }
            Log.GUI("RotateCurve " + ks.Length);
            effectLayer.RotateCurve01 = new AnimationCurve(ks);

        }
    }

    float lastSpeed = 1;
    bool useRandomStartAni = false;

    void SetAni(EffectLayer effectLayer, JSONClass modData)
    {
        var fnum = effectLayer.Cols * effectLayer.Rows;
        var speedStr = modData ["ANIMATION SPEED"].Value;
        float[] speed = new float[]{ 0, lastSpeed };
        if (speedStr == "")
        {
        } else
        {
            speed = ConvertToFloat(speedStr);
        }
        lastSpeed = speed [1];
        var uvTime = fnum * 1.0f / speed [1];
        effectLayer.UVTime = uvTime;

        var use = modData ["USE RANDOM STARTING FRAME"].Value;
        if (use == "")
        {
        } else
        {
            useRandomStartAni = modData ["USE RANDOM STARTING FRAME"].AsBool;
        }
        effectLayer.RandomStartFrame = useRandomStartAni;
    }

    void SetEmit(EffectLayer effectLayer, JSONClass modData)
    {
        Log.Sys("SetEmit " + effectLayer.name + " modData " + modData.ToString());
        var forever = modData ["NO EXPIRATION"].AsBool;
        var rateStr = modData ["EMIT RATE"].Value;
        var rate = new float[]{ 0, 15 };//有15个 虫子在水面 waterskip
        if (string.IsNullOrEmpty(rateStr))
        {
            
        } else
        {
            rate = ConvertToFloat(rateStr.Split(','));
        }


        int rateType = (int)rate [0];
        int rateMin = (int)rate [1];
        int rateNum = 1;
        if (rateType == 0)
        {
            effectLayer.EmitRate = rateMin;
            effectLayer.MaxENodes = rateMin;
            rateNum = rateMin; 
            /*
            if (rateMin == 1)
            {
                effectLayer.IsBurstEmit = true;
            } else
            {
            }
            */

        } else
        {
            int rateMax = (int)rate [2];
            effectLayer.EmitRate = rateMax;
            effectLayer.MaxENodes = rateMax;
            rateNum = rateMax;
        }

        if (forever)
        {
            effectLayer.IsNodeLifeLoop = true;
            effectLayer.IsBurstEmit = true;

        } else if (rateType == 0 && rateMin == 1)
        {
            effectLayer.IsNodeLifeLoop = true;
            effectLayer.IsBurstEmit = true;
        } else
        {
            effectLayer.IsNodeLifeLoop = false;
            effectLayer.IsBurstEmit = false;
        }
        var numLoops = modData ["NUM LOOPS"];
        bool hasLoops = false;
        if (modData ["NUM LOOPS"] == null)
        {
            effectLayer.IsBurstEmit = false;
            effectLayer.EmitLoop = -1;
        } else if (modData ["NUM LOOPS"].AsInt == 1)
        {
            effectLayer.IsBurstEmit = true;
            effectLayer.EmitLoop = 1;
        } else if (numLoops.AsInt == 0)
        {
            hasLoops = true;
            effectLayer.IsBurstEmit = false;
            effectLayer.EmitLoop = 1;
        }

        {
            Log.Sys("Particle Life:");
            float[] life;
            if (string.IsNullOrEmpty(modData ["PARTICLE LIFE"]))
            {
                life = new float[]{ 1, 5, 10 };
            } else
            {
                life = ConvertToFloat(modData ["PARTICLE LIFE"].Value);
            }
            int lifeType = (int)life [0];
            float lifeTime = 1;
            if (lifeType == 1)
            {
                effectLayer.NodeLifeMin = life [1];
                effectLayer.NodeLifeMax = life [2];
                effectLayer.EmitDuration = 1;
                effectLayer.EmitLoop = -1;
                effectLayer.EmitDelay = 0;

            } else if (lifeType == 0)
            {
                effectLayer.NodeLifeMin = life [1];
                effectLayer.NodeLifeMax = life [1];
                effectLayer.EmitDuration = 1;
                effectLayer.EmitLoop = -1;
                effectLayer.EmitDelay = 0;

            }

            Log.Sys("NumLoops " + numLoops);
            
            lifeTime = effectLayer.NodeLifeMax;
            effectLayer.MaxENodes = (int)Mathf.Max(rateNum, rateNum * lifeTime);
        }
        if (hasLoops && numLoops.AsInt == 0)
        {
            effectLayer.IsBurstEmit = false;
            effectLayer.EmitLoop = 1;
        }

        var duration = modData ["EMIT DURATION"];
        if (duration == null)
        {
        } else
        {
            var dur = ConvertToFloat(duration.Value);
            if (dur [0] == 0)
            {
                if (dur [1] == 0)
                {
                    effectLayer.IsBurstEmit = true;
                } else
                {
                    effectLayer.EmitDuration = dur [1];
                }
            } else
            {
            }
        }

        if(rateMin == 1 && forever) {
            effectLayer.IsBurstEmit = true;
        }

        var emitType = modData ["TYPE OF EMITTER"].Value;
        if (emitType == "Circle")
        {
            effectLayer.EmitType = 3;
            effectLayer.UseRandomCircle = true;
            float[] maxRadius;
            if (string.IsNullOrEmpty(modData ["MAX RADIUS"]))
            {
                maxRadius = new float[]{ 0, 3 };
            } else
            {
                maxRadius = ConvertToFloat(modData ["MAX RADIUS"].Value.Split(','));
            }
            effectLayer.CircleRadiusMax = maxRadius [1];
            if (modData ["MIN RADIUS"].Value == "")
            {
                effectLayer.CircleRadiusMin = effectLayer.CircleRadiusMax;
            } else
            {
                effectLayer.CircleRadiusMin = ConvertToFloat(modData ["MIN RADIUS"].Value.Split(',')) [1];

            }
        } else if (emitType == "SphereSurface")
        {
            effectLayer.EmitType = 2;
            effectLayer.Radius = modData ["RADIUS"].AsFloat;
            effectLayer.DirType = DIRECTION_TYPE.Sphere;

        } else if (emitType == "Box")
        {
            effectLayer.EmitType = 1;
            var width = modData ["WIDTH_"].AsFloat;
            var height = modData ["HEIGHT_"].AsFloat;
            var depth = modData ["DEPTH_"].AsFloat;
            effectLayer.BoxSize = new Vector3(width, height, depth);
        }
            
        var scaleModData = modData ["SCALE ON LAUNCH"].Value;
        Debug.Log("scale mod data " + scaleModData);
        float[] scaleData = new float[]{ 0, 1 };
        if (scaleModData == "")
        {
        } else
        {
            scaleData = ConvertToFloat(scaleModData);
            //Avoid Start Scale 0
            if (scaleData [1] == 0)
            {
                scaleData [1] = 1;
            }
        }

        var vs = modData ["VISUAL SCALE"].Value;
        float vscale = 1;
        if (vs != "")
        {
            vscale = Convert.ToSingle(vs);
        }

        var scaType = (int)scaleData [0];
        if (scaType == 1)
        {
            effectLayer.RandomOriScale = true;
            effectLayer.OriScaleXMin = scaleData [1] * vscale;
            effectLayer.OriScaleXMax = scaleData [2] * vscale;
            effectLayer.OriScaleYMin = scaleData [1] * vscale;
            effectLayer.OriScaleYMax = scaleData [2] * vscale;
        } else if (scaType == 0)
        {
            effectLayer.RandomOriScale = false;
            effectLayer.OriScaleXMin = effectLayer.OriScaleXMax = scaleData [1] * vscale;
            effectLayer.OriScaleYMin = effectLayer.OriScaleYMax = scaleData [1] * vscale;
        }
        Log.Sys("scaleData is type " + scaType + " vscale " + vscale + " scaleData " + scaleData [1] + " res " + effectLayer.OriScaleXMin);

        var posx = modData ["POSITIONX"].AsFloat;
        var posy = modData ["POSITIONY"].AsFloat;
        var posz = modData ["POSITIONZ"].AsFloat;
        var pos = modData ["POSITION"];
        if (pos != null)
        {
            var p = ConvertToFloat(pos.Value);
            effectLayer.EmitPoint = new Vector3(p [0], p [1], p [2]);
        } else
        {
            effectLayer.EmitPoint = new Vector3(posx, posy, posz);
        }
        //Width * Height
        if (modData ["WIDTH"].Value != "")
        {
            var width = ConvertToFloat(modData ["WIDTH"].Value);
            if ((int)width [0] == 0)
            {
                effectLayer.SpriteWidth = width [1];
            }
        }
        if (modData ["HEIGHT"].Value != "")
        {
            var width = ConvertToFloat(modData ["HEIGHT"].Value);
            if ((int)width [0] == 0)
            {
                effectLayer.SpriteHeight = width [1];
            }
        }


        float[] velocity;
        if (string.IsNullOrEmpty(modData ["VELOCITY"]))
        {
            velocity = new float[]{ 0, 15 };
        } else
        {
            velocity = ConvertToFloat(modData ["VELOCITY"].Value);
        }
        var vtype = (int)velocity [0];
        if (vtype == 0)
        {
            effectLayer.OriSpeed = velocity [1] * velScale;
            effectLayer.OriVelocityAxis = Vector3.up;
        } else if (vtype == 1)
        {
            effectLayer.OriSpeed = velocity [1] * velScale;
            effectLayer.SpeedMin = velocity [1] * velScale;
            effectLayer.SpeedMax = velocity [2] * velScale;
            effectLayer.IsRandomSpeed = true;
            effectLayer.OriVelocityAxis = Vector3.up;

        }
        if (modData ["ANGLE"].Value != "")
        {
            var angle = ConvertToFloat(modData ["ANGLE"].Value);
            if (angle.Length > 0)
            {
                if ((int)angle [0] == 1)
                {
                    var minDeg = angle [1];
                    var maxDeg = angle [2];
                    effectLayer.DirType = DIRECTION_TYPE.Cone;
                    effectLayer.UseRandomDirAngle = true;
                    effectLayer.AngleAroundAxis = (int)minDeg;
                    effectLayer.AngleAroundAxisMax = (int)maxDeg;

                    effectLayer.SpriteType = (int)Xft.STYPE.BILLBOARD_SELF;
                } else if ((int)(angle [0]) == 0)
                {
                    var minDeg = angle [1];
                    if (minDeg > 0)
                    {
                        effectLayer.DirType = DIRECTION_TYPE.Cone;
                        effectLayer.UseRandomDirAngle = false;
                        effectLayer.AngleAroundAxis = (int)0;
                        effectLayer.AngleAroundAxisMax = (int)minDeg;
                        effectLayer.SpriteType = (int)Xft.STYPE.BILLBOARD_SELF;
                        effectLayer.UseRandomDirAngle = true;
                    }

                }
            }
        }



    }

    void SetColor(EffectLayer layer, float[] f)
    {
        layer.ColorChangeType = COLOR_CHANGE_TYPE.Gradient;
        layer.ColorParam = new ColorParameter();
        layer.ColorParam.Colors.Clear();
        layer.ColorAffectorEnable = true;
        layer.ColorGradualType = COLOR_GRADUAL_TYPE.LOOP;
        float time = 0;
        float r, g, b, a;
        r = g = b = a = 0;
        for (int i = 0; i < f.Length; i++)
        {
            switch (i % 5)
            {
                case 0:
                    time = f [i];
                    break;
                case 1:
                    r = f [i];
                    break;
                case 2:
                    g = f [i];
                    break;
                case 3:
                    b = f [i];
                    break;
                case 4:
                    a = f [i];
                    layer.ColorParam.AddColorKey(time, new Color(r, g, b, a));
                
                    break;

            }

        }
    }

    float[] ConvertToFloat(string s)
    {
        return ConvertToFloat(s.Split(','));
    }

    float[] ConvertToFloat(string[] s)
    {
        if (s.Length == 0)
        {
            return new float[0];
        }
        float[] f = new float[s.Length];
        int i = 0;
        foreach (string c in s)
        {
            f [i] = Convert.ToSingle(c);
            i++;
        }
        return f;
    }

    SimpleJSON.JSONClass GetProp(JSONNode obj)
    {
        return obj ["children"] [0].AsObject;
    }

    JSONClass GetChildren(JSONNode obj)
    {
        return obj ["children"] [1].AsObject;
    }

    static GameObject DoCreateXffectObject()
    {
        GameObject go = new GameObject("XffectObj");
        go.transform.localScale = Vector3.one;
        go.transform.rotation = Quaternion.identity;
        go.AddComponent<XffectComponent>();
        return go;
    }

    static GameObject DoCreateLayer(GameObject go)
    {
        GameObject layer = new GameObject("EffectLayer");
        EffectLayer efl = (EffectLayer)layer.AddComponent<EffectLayer>();
        layer.transform.parent = go.transform;

        efl.transform.localPosition = Vector3.zero;
        //fixed 2012.6.25. default to effect layer object.
        efl.ClientTransform = efl.transform;
        efl.GravityObject = efl.transform;
        efl.BombObject = efl.transform;
        efl.TurbulenceObject = efl.transform;
        efl.AirObject = efl.transform;
        efl.VortexObj = efl.transform;
        efl.DirCenter = efl.transform;
        efl.DragObj = efl.transform;

        efl.Material = AssetDatabase.LoadAssetAtPath(GetXffectPath() + DefaultMatPath, typeof(Material)) as Material;
        return layer;
    }

    static public string GetXffectPath()
    {
        Shader temp = Shader.Find("Xffect/PP/radial_blur_mask");
        string assetPath = AssetDatabase.GetAssetPath(temp);
        int index = assetPath.LastIndexOf("Xffect");
        string basePath = assetPath.Substring(0, index + 7);
        
        return basePath;
    }

    // Use this for initialization
    void Start()
    {
    
    }
    
    // Update is called once per frame
    void Update()
    {
    
    }

    public static string DefaultMatPath = "Examples/Materials/default.mat";
    #endif
}
