Shader "Custom/waterStreaks" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_UVAnimX ("UV Anim X", float) = 0
		_UVAnimY ("UV Anim Y", float) = 0
	}
	SubShader {
		Tags { "Queue"="Transparent" 
				"IgnoreProjector"="True"
				"RenderType"="Transparent" }
				
		Pass {
			LOD 200
			Blend One One 
			//Blend SrcAlpha OneMinusSrcAlpha 
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
	   			fixed4 vertColor;
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
                col.rgb = tex.rgb*2*(i.vertColor);
                col.a = tex.a;
				//col.a *= 0.5f;
				return col;
			}	
	        
	        
	        ENDCG
		}
	} 
	FallBack "Diffuse"
}
