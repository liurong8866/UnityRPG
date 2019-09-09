// Shader created with Shader Forge Beta 0.28 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.28;sub:START;pass:START;ps:flbk:,lico:0,lgpr:1,nrmq:0,limd:0,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,hqsc:False,hqlp:False,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:True,ufog:False,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32101,y:32613|emission-322-OUT,clip-297-OUT;n:type:ShaderForge.SFN_Tex2d,id:2,x:33102,y:32751,ptlb:mainTex,ptin:_mainTex,tex:7a933b8854e334964ba2f50d16cbe989,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:21,x:33301,y:33235,ptlb:cloud,ptin:_cloud,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Power,id:22,x:32724,y:33053|VAL-238-OUT,EXP-337-OUT;n:type:ShaderForge.SFN_Multiply,id:25,x:32455,y:33061|A-22-OUT,B-26-OUT;n:type:ShaderForge.SFN_Vector1,id:26,x:32617,y:33201,v1:15;n:type:ShaderForge.SFN_Vector3,id:27,x:32467,y:33298,v1:1,v2:0.3,v3:0.1;n:type:ShaderForge.SFN_Multiply,id:28,x:32290,y:33218|A-25-OUT,B-27-OUT;n:type:ShaderForge.SFN_Multiply,id:182,x:32437,y:32637|A-2-RGB,B-28-OUT;n:type:ShaderForge.SFN_ComponentMask,id:190,x:32776,y:32835,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-446-OUT;n:type:ShaderForge.SFN_Clamp,id:238,x:33012,y:33304|IN-21-RGB,MIN-239-OUT,MAX-240-OUT;n:type:ShaderForge.SFN_Vector1,id:239,x:33092,y:32994,v1:0;n:type:ShaderForge.SFN_Vector1,id:240,x:33092,y:33048,v1:0.7;n:type:ShaderForge.SFN_Power,id:297,x:32558,y:32778|VAL-190-OUT,EXP-302-OUT;n:type:ShaderForge.SFN_Vector1,id:302,x:32776,y:32743,v1:0.15;n:type:ShaderForge.SFN_Lerp,id:322,x:32376,y:32361|A-2-RGB,B-182-OUT,T-646-OUT;n:type:ShaderForge.SFN_Slider,id:323,x:32793,y:32423,ptlb:timeLerp,ptin:_timeLerp,min:0,cur:0.3589372,max:1;n:type:ShaderForge.SFN_Lerp,id:337,x:33124,y:32512|A-338-OUT,B-339-OUT,T-323-OUT;n:type:ShaderForge.SFN_Vector1,id:338,x:33291,y:32468,v1:0;n:type:ShaderForge.SFN_Vector1,id:339,x:33333,y:32546,v1:12;n:type:ShaderForge.SFN_Power,id:446,x:32814,y:33467|VAL-238-OUT,EXP-449-OUT;n:type:ShaderForge.SFN_ConstantLerp,id:449,x:33120,y:33494,a:0,b:14|IN-323-OUT;n:type:ShaderForge.SFN_Negate,id:645,x:32673,y:32064|IN-650-OUT;n:type:ShaderForge.SFN_Add,id:646,x:32673,y:31921|A-647-OUT,B-645-OUT;n:type:ShaderForge.SFN_Vector1,id:647,x:32971,y:31925,v1:1;n:type:ShaderForge.SFN_Add,id:648,x:33076,y:32036|A-323-OUT,B-649-OUT;n:type:ShaderForge.SFN_Vector1,id:649,x:33179,y:32198,v1:-1;n:type:ShaderForge.SFN_Power,id:650,x:32875,y:32047|VAL-648-OUT,EXP-651-OUT;n:type:ShaderForge.SFN_Vector1,id:651,x:32613,y:32237,v1:2;proporder:2-21-323;pass:END;sub:END;*/

Shader "Shader Forge/burnShader" {
    Properties {
        _mainTex ("mainTex", 2D) = "white" {}
        _cloud ("cloud", 2D) = "white" {}
        _timeLerp ("timeLerp", Range(0, 1)) = 0.3589372
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform sampler2D _mainTex; uniform float4 _mainTex_ST;
            uniform sampler2D _cloud; uniform float4 _cloud_ST;
            uniform float _timeLerp;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float2 node_663 = i.uv0;
                float3 node_238 = clamp(tex2D(_cloud,TRANSFORM_TEX(node_663.rg, _cloud)).rgb,0.0,0.7);
                float node_323 = _timeLerp;
                clip(pow(pow(node_238,lerp(0,14,node_323)).r,0.15) - 0.5);
////// Lighting:
////// Emissive:
                float4 node_2 = tex2D(_mainTex,TRANSFORM_TEX(node_663.rg, _mainTex));
                float3 emissive = lerp(node_2.rgb,(node_2.rgb*((pow(node_238,lerp(0.0,12.0,node_323))*15.0)*float3(1,0.3,0.1))),(1.0+(-1*pow((node_323+(-1.0)),2.0))));
                float3 finalColor = emissive;
/// Final Color:
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCollector"
            Tags {
                "LightMode"="ShadowCollector"
            }
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCOLLECTOR
            #define SHADOW_COLLECTOR_PASS
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcollector
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform sampler2D _cloud; uniform float4 _cloud_ST;
            uniform float _timeLerp;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_COLLECTOR;
                float4 uv0 : TEXCOORD5;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_SHADOW_COLLECTOR(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float2 node_664 = i.uv0;
                float3 node_238 = clamp(tex2D(_cloud,TRANSFORM_TEX(node_664.rg, _cloud)).rgb,0.0,0.7);
                float node_323 = _timeLerp;
                clip(pow(pow(node_238,lerp(0,14,node_323)).r,0.15) - 0.5);
                SHADOW_COLLECTOR_FRAGMENT(i)
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Cull Off
            Offset 1, 1
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform sampler2D _cloud; uniform float4 _cloud_ST;
            uniform float _timeLerp;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float4 uv0 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float2 node_665 = i.uv0;
                float3 node_238 = clamp(tex2D(_cloud,TRANSFORM_TEX(node_665.rg, _cloud)).rgb,0.0,0.7);
                float node_323 = _timeLerp;
                clip(pow(pow(node_238,lerp(0,14,node_323)).r,0.15) - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
