// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'

Shader "Custom/lightProjector" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_ShadowTex ("Cookie", 2D) = "" {}
		_FalloffTex ("FallOff", 2D) = "" {}
		_Coff ("coff ", float) = 5
	}
	
	
	Subshader {
		Tags {"Queue"="Transparent"}
		Pass {	
			ZWrite Off
			Fog { Color (0, 0, 0) }
			ColorMask RGB
			Blend Zero SrcColor
			Offset -1, -1
	 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct v2f {
				float4 uvShadow : TEXCOORD0;
				float4 uvFalloff : TEXCOORD1;
				float4 pos : SV_POSITION;
			};
			
			float4x4 unity_Projector;
			float4x4 unity_ProjectorClip;
			
			
			
			v2f vert (float4 vertex : POSITION)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, vertex);
				o.uvShadow = mul (unity_Projector, vertex);
				o.uvFalloff = mul (unity_ProjectorClip, vertex);
				return o;
			}
			
			fixed4 _Color;
			sampler2D _ShadowTex;
			sampler2D _FalloffTex;
			float4 _ShadowTex_ST;
			
			uniform float _Coff;
			fixed4 frag (v2f i) : SV_Target
			{
				float2 xy = i.uvShadow.xy;
				xy.x = min(max(0, xy.x), 1);
				xy.y = min(max(0, xy.y), 1);
				
				float2 newUV = xy * _ShadowTex_ST.xy + _ShadowTex_ST.zw;
				float4 uv4 = float4(newUV, i.uvShadow.z, i.uvShadow.w);
				//float4 newUV = TRANSFORM_TEX(i.uvShadow, _ShadowTex);
				
				//float4 newUV = i.uvShadow;
				
				//fixed4 texS = tex2Dproj (_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
				fixed4 texS = tex2Dproj(_ShadowTex, UNITY_PROJ_COORD(uv4));
				texS.rgb *= _Color.rgb;
				texS.a = 1.0-texS.a;
	 
				fixed4 texF = tex2Dproj (_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff));
				fixed4 res = texS * texF.a;
				return res*_Coff;
			}
			ENDCG
		}
		
		/*
		Pass {
			ZWrite Off
			Blend Zero SrcColor
			Offset -1, -1
			ColorMask RGB
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct v2f {
				float4 uvShadow : TEXCOORD0;
				float4 uvFalloff : TEXCOORD1;
				float4 pos : SV_POSITION;
			};
			
			float4x4 _Projector;
			float4x4 _ProjectorClip;
			
			v2f vert (float4 vertex : POSITION)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, vertex);
				o.uvShadow = mul (_Projector, vertex);
				o.uvFalloff = mul (_ProjectorClip, vertex);
				return o;
			}
			
			fixed4 _Color;
			sampler2D _ShadowTex;
			sampler2D _FalloffTex;
			float4 _ShadowTex_ST;
			
			uniform float _Coff;
			fixed4 frag (v2f i) : SV_Target
			{
				float2 xy = i.uvShadow.xy;
				xy.x = min(max(0, xy.x), 1);
				xy.y = min(max(0, xy.y), 1);
				
				float2 newUV = xy * _ShadowTex_ST.xy + _ShadowTex_ST.zw;
				float4 uv4 = float4(newUV, i.uvShadow.z, i.uvShadow.w);
				//float4 newUV = TRANSFORM_TEX(i.uvShadow, _ShadowTex);
				
				//float4 newUV = i.uvShadow;
				
				//fixed4 texS = tex2Dproj (_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
				fixed4 texS = tex2Dproj(_ShadowTex, UNITY_PROJ_COORD(uv4));
				texS.rgb *= _Color.rgb;
				texS.a = 1.0-texS.a;
	 
				fixed4 texF = tex2Dproj (_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff));
				fixed4 res = texS * texF.a;
				return res*_Coff*2;
			}
			
			ENDCG
		}
		*/
	}
	
	FallBack "Diffuse"
}
