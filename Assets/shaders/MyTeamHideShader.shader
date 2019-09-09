Shader "Custom/MyTeamHideShader" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_Ambient ("Ambient Color", Color) = (0.588, 0.588, 0.588, 1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}

	SubShader {
		Tags { 
			"Queue"="Geometry+5" 
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
		 }
		 Pass {
			Name "Overlay"
			zwrite off
			ztest always  
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert 
	        #pragma fragment frag
	        #include "UnityCG.cginc" 
	        
	        struct v2f {
	        	float4 pos : SV_POSITION;
	        	float2 uv : TEXCOORD0;
	        };
	        
	        uniform sampler2D _MainTex;
	        uniform fixed4 _OverlayColor;
	        
	        v2f vert(appdata_base v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
	          	return o;
			}
			fixed4 frag(v2f i) : Color {
				float4 col = tex2D(_MainTex, i.uv);
				col.a = 0.5;
				return col;
			}
			
	        ENDCG
		}
	}
	FallBack "Diffuse"
}
