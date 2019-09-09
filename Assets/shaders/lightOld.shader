Shader "Custom/lightOld" {
	Properties {
		_Color ("Color", Color) = (1, 1, 1, 1)
		_Ambient ("Ambient", Color) = (0.588, 0.588, 0.588, 1)
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
			//Pass {
				//Name "BLEND"
				
				Blend One One	
				
				LOD 200
				
				CGPROGRAM
				#pragma surface surf Lambert

				sampler2D _MainTex;
				fixed4 _Ambient;
				float4 _Color;

				struct Input {
					float2 uv_MainTex;
				};

				void surf (Input IN, inout SurfaceOutput o) {
					half4 c = tex2D (_MainTex, IN.uv_MainTex);
					//o.Albedo = c.rgb;
					//o.Alpha = c.a;
					o.Emission = c.rgb*_Color;//+0.5f;
				}
				ENDCG
			
			//}
		}
	}
		 
	FallBack "Diffuse"
}
