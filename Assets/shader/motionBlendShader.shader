Shader "Custom/motionBlendShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_MotionTex ("Base (RGB)", 2D) = "white" {}
	}
	
	SubShader {
		Pass {
			ztest always
			zwrite off

			CGPROGRAM
			#pragma vertex vert 
		    #pragma fragment frag
		    #include "UnityCG.cginc"
		    struct VertIn {
	        	float4 vertex : POSITION;
	        	float4 texcoord : TEXCOORD0;
	        };
	        struct v2f {
	        	fixed4 pos : SV_POSITION;
	        	fixed2 uv : TEXCOORD0;
	   		};
	        uniform sampler2D _MainTex;
	        uniform sampler2D _MotionTex;

	        v2f vert(VertIn v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				return o;
			}

			fixed4 frag(v2f i) : Color {
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 moCol = tex2D(_MotionTex, i.uv);
				fixed4 newCol;
				//newCol.rgb = col.rgb*(1-moCol.a)+moCol.rgb;//*moCol.a;
				newCol.rgb = moCol.rgb;
				newCol.a = col.a;
				return newCol;
			}
	        
		    ENDCG
		}
		
	} 
	FallBack "Diffuse"
}
