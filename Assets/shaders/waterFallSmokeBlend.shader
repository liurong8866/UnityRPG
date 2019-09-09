Shader "Custom/waterFallSmokeBlend" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	Category {
		Tags { "Queue"="Transparent+2" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off Lighting Off ZWrite Off ZTest Off Fog { Color (0,0,0,0) }
		
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
	
}
