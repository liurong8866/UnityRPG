﻿Shader "Custom/waterWall" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_AnimTex("Animation Texture Pass", 2D) = "white" {}	

	}

	SubShader {
		Tags { "Queue"="Transparent" 
				"IgnoreProjector"="True"
				"RenderType"="Transparent" }
		Pass {
			Blend One One  
			zwrite off

			LOD 200
			
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
	        uniform sampler2D _AnimTex;
	        v2f vert(VertIn v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				return o;
			}
			fixed4 frag(v2f i) : Color {
                fixed4 tex =  tex2D(_MainTex, i.uv);
                fixed4 tex2 =  tex2D(_AnimTex, i.uv);
				 
                fixed4 col;
                col.rgb = tex.rgb*tex2;
                
                col.a = tex.a;
				return col;
			}


			ENDCG
		}
	} 
	FallBack "Diffuse"
}