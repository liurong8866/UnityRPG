Shader "Xffect/glow_per_obj/blend" {
Properties {
	_MainTex ("Main Texture", 2D) = "white" {}
	_GlowTex ("Glow Texture", 2D) = "white" {}
}

Category {
	Tags { "XftEffect"="GlowPerObj" }
	ZTest Always
	Cull Off Lighting Off ZWrite Off Fog { Mode Off }
	

	// ---- Fragment program cards
	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _GlowTex;
			
			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};
			
			float4 _MainTex_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}

			
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 mcolor = tex2D(_MainTex, i.texcoord);
				fixed4 gcolor = tex2D(_GlowTex, i.texcoord);
				
				//additive blend
				//fixed4 ret = mcolor + mcolor * gcolor;
				
				fixed4 ret = mcolor + gcolor;
				
				return ret;
			}
			ENDCG 
		}
	} 	
}
}
