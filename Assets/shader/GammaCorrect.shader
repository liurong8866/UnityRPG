Shader "Custom/GammaCorrect" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
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
	        uniform float _Gamma;

	        v2f vert(appdata_t v) {
	        	v2f o;

				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				return o;
	        }

			fixed4 frag(v2f i) : Color {
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 retCol;
				retCol.rgb = pow(col.rgb, _Gamma);
				retCol.a = col.a;
				return retCol;
			}

			ENDCG
		}


	} 
	FallBack "Diffuse"
}
