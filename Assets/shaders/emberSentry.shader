Shader "Custom/emberSentry" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_IllumMap ("Illum Map", 2D) = "white" {}
		
		_LightDir ("World light Dir", Vector) = (-1, -1, 1, 0)
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		 
		Pass {
			Tags {"LightMode" = "Vertex"}
			Name "Base"
			LOD 200
			
			Material {
				Ambient [_Color]
				Diffuse [_Color]
			}
			Lighting Off
			SetTexture [_MainTex] {
				Combine texture, texture
			}
			
			SetTexture [_IllumMap] {
				combine texture + previous
			}
		}
		
		
	} 
	
	FallBack "Diffuse"
}
