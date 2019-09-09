Shader "Custom/HeatHaze" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_HeatTex ("Base (RGB)", 2D) = "white" {}
		_DistortFactor ("accum", float) = 1
		_RiseFactor ("accum", float) = 1
		_Radius ("accum", float) = 1
		_ClipArg ("accum", float) = 0.2

	}
	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert 
		    #pragma fragment frag
		    #pragma fragmentoption ARB_precision_hint_fastest
		    #include "UnityCG.cginc"

	        struct appdata_t {
	        	float4 vertex : POSITION;
	        	float4 texcoord : TEXCOORD0;
	        };
	        struct v2f {
	        	fixed4 pos : SV_POSITION;
	        	fixed2 uv : TEXCOORD0;
	        };

	        uniform sampler2D _MainTex;
	        uniform sampler2D _HeatTex;
	        uniform float _DistortFactor;
	        uniform float _RiseFactor;
	        uniform float _Radius;
	        uniform float _ClipArg;

	        v2f vert(appdata_t v) {
	        	v2f o;

				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				return o;
	        }

			fixed4 frag(v2f i) : Color {
				fixed2 distorCoord = i.uv;
				distorCoord.t = _Time.y*_RiseFactor;
				fixed4 distortMapValue = tex2D(_HeatTex, distorCoord);
				fixed2 distortPosOffset = distortMapValue.xy;
				distortPosOffset -= fixed2(0.5, 0.5);
				distortPosOffset *= 2.0;
				distortPosOffset *= _DistortFactor;

				fixed2 center = i.uv-fixed2(0.5, 0.5);
				center *= 2.0;

				fixed rate = max(0, _Radius-length(center));
				fixed alpha = lerp(0, 1, min(1, rate/_ClipArg));
				distortPosOffset *= alpha;

				fixed2 distortedTexCoord = i.uv+distortPosOffset;
				fixed4 c = tex2D(_MainTex, distortedTexCoord);
				return c;
			}

			ENDCG
		}
		
	} 
	FallBack "Diffuse"
}
