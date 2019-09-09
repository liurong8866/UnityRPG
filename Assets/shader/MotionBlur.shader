Shader "Custom/MotionBlur" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_AccumOrig ("accum", float) = 0.8
	}
	SubShader {
		ztest Always
		Cull off
		zwrite off
		Pass {
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask RGB

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

	        uniform float _AccumOrig;
	        uniform sampler2D _MainTex;

	        v2f vert(appdata_t v) {
	        	v2f o;

				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				return o;
	        }

			fixed4 frag(v2f i) : Color {
				return fixed4(tex2D(_MainTex, i.uv).rgb, _AccumOrig);
			}

			ENDCG
		}
	} 
	FallBack "Diffuse"
}
