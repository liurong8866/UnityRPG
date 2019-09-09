Shader "Custom/TestShader" {
	Properties {
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		Pass {
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

	        v2f vert(VertIn v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				return o;
			}

			fixed4 frag(v2f i) : Color {
				return fixed4(1, 1, 1, 1);
			}
	        
		    ENDCG	
		}
	} 

	FallBack "Diffuse"
}
