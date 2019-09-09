Shader "Custom/alphaScrollAddBlend" {
	Properties {
		_Color ("Ambient", Color) = (0.588, 0.588, 0.588, 1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_UVAnimX ("UV Anim X", float) = 0
		_UVAnimY ("UV Anim Y", float) = 0
	}
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector" = "true" "RenderType"="Transparent"}
		LOD 200
		
			
		Pass {	
			Blend One One
			Cull Off Lighting Off
			ztest off
			zwrite off

			CGPROGRAM
			#pragma vertex vert 
	        #pragma fragment frag
	        #include "UnityCG.cginc"
	        
	        struct VertIn {
	        	float4 vertex : POSITION;
	        	float4 texcoord : TEXCOORD0;
	        	float4 color : COLOR;
	        };
	        
	        struct v2f {
	        	fixed4 pos : SV_POSITION;
	        	fixed2 uv : TEXCOORD0;
	   			fixed4 vertColor : TEXCOORD1;
	        };
	        
	        uniform sampler2D _MainTex;  
	        uniform fixed _UVAnimX;
	        uniform fixed _UVAnimY;
	         
	        v2f vert(VertIn v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				
				float t1 = _Time.y*_UVAnimX;
				float t2 = _Time.y*_UVAnimY;
	
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				o.uv += fixed2(t1, t2);
				
				o.vertColor = v.color; 
				return o;
			}
			
			fixed4 frag(v2f i) : Color {
                fixed4 tex =  tex2D(_MainTex, i.uv);//*i.vertColor;
                fixed4 col;
                col.rgb = tex.rgb*(i.vertColor);
                col.a = tex.a;
				return col;
			}	
	        
	        ENDCG
		}
	} 
	FallBack "Diffuse"
}
