Shader "Custom/ToonShader" {
	Properties {
		_Color ("Color", Color) = (1, 1, 1, 1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" 
		}
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

	        uniform fixed4 _Color;

	         
	        v2f vert(VertIn v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				fixed2 uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				o.uv = uv;
				return o;
			}

			fixed4 frag(v2f i) : Color {
				return _Color;
			}
			
			ENDCG
		}
		
	} 
	FallBack "Diffuse"
}
