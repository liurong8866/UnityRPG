Shader "Custom/GhostShader" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_Ambient ("Ambient Color", Color) = (0.588, 0.588, 0.588, 1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	
	SubShader {
		Tags { 
			"Queue"="Geometry+5" 
			"IgnoreProjector"="True"
		 }
		 
		 Pass {
			Name "Overlay"
			//Blend SrcAlpha OneMinusSrcAlpha
			Blend One One
			//zwrite off
			//ztest greater
			CGPROGRAM
			#pragma vertex vert 
	        #pragma fragment frag
	        #include "UnityCG.cginc" 
	        
	        struct v2f {
	        	float4 pos : SV_POSITION;
	        	float2 uv : TEXCOORD0;
	        };
	        
	        uniform sampler2D _MainTex;
	        uniform fixed4 _GhostColor;
	        
	        v2f vert(appdata_base v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
	          	return o;
			}
			fixed4 frag(v2f i) : Color {
				return tex2D(_MainTex, i.uv)*_GhostColor;
			}
			
	        ENDCG
			
		}
		
	} 
	FallBack "Diffuse"
}
