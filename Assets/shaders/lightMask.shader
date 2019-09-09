Shader "Custom/lightMask" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	Category {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend Zero OneMinusSrcAlpha, Zero One
		Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
		
		BindChannels {
			Bind "Color", color
			Bind "Vertex", vertex
			Bind "TexCoord", texcoord
		}
		
		SubShader {
			Pass {
				SetTexture [_MainTex] {
					combine texture * primary
				}
			}
		}
		
	} 
	FallBack "Diffuse"
}
