Shader "Custom/backAddtive" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend One One
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
