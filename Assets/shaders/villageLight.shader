Shader "Custom/villageLight" {
	Properties {
		_Enhence ("enhence coff", float) = 1
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	
	Category{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Cull Off Lighting Off ZWrite Off Ztest Always Fog {Mode Off}
			
			
		BindChannels {
			Bind "Color", color
			Bind "Vertex", vertex
			Bind "TexCoord", texcoord
		}
	
		SubShader {		
			Blend One One	
			lighting off
			LOD 200
			Pass {
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				struct v2f {
		        	fixed4 pos : SV_POSITION;
		        	fixed2 uv : TEXCOORD0;
		        };
		        v2f vert(appdata_base v) 
				{
					v2f o;
					o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
					o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);		
					return o;
				} 
				uniform sampler2D _MainTex; 
				uniform fixed _Enhence;
				fixed4 frag(v2f i) : Color {
		            fixed4 col =  tex2D(_MainTex, i.uv);	
					return col*_Enhence;
				}
				ENDCG
			}
			/*
			CGPROGRAM
			#pragma surface surf Lambert

			sampler2D _MainTex;
			fixed _Enhence;

			struct Input {
				float2 uv_MainTex;
			};

			void surf (Input IN, inout SurfaceOutput o) {
				half4 c = tex2D (_MainTex, IN.uv_MainTex);
				o.Emission = c.rgb*_Enhence;
			}
			ENDCG
			*/
		}
	}
	FallBack "Diffuse"
}
