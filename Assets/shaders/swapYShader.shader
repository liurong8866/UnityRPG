Shader "Custom/swapYShader" {
	Properties {
		_MainTex ("Particle Texture", 2D) = "white" {}
	}

	Category {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha One
		Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
		
		
		
		BindChannels {
			Bind "Color", color
			Bind "Vertex", vertex
			Bind "TexCoord", texcoord
		}
		
		SubShader {
			Pass {
				Name "BASE"
				CGPROGRAM
				#pragma vertex vert 
	        	#pragma fragment frag
	        	#include "UnityCG.cginc"
	        	
	        	struct v2f {
		        	fixed4 pos : SV_POSITION;
		        	fixed2 uv : TEXCOORD0;
		        };
	        	uniform sampler2D _MainTex;
	        
		        v2f vert(appdata_base v) 
				{
					v2f o;
					o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
					fixed ny = 1-v.texcoord.y;
					fixed nx = 1-v.texcoord.x;
					
					o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, fixed2(nx, ny));
					//o.v = 1-o.v;
					return o;
				}
				
				fixed4 frag(v2f i) : Color {
					return tex2D(_MainTex, i.uv);
					
				}
	        	ENDCG
			
			}
			/*
			Pass {
				SetTexture [_MainTex] {
					combine texture * primary
				}
			}
			*/
		}
	}
}
