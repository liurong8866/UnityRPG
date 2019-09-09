Shader "Custom/waveAll" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_ScrollX1("", float) = -0.1
		_ScrollY1("", float) = -0.3
		
		_ScrollX2("", float) = 0.15
		_ScrollY2("", float) = -0.2
		
		_Mask ("Mask", 2D) = "white" {}
		
	}
	
	SubShader {
		Tags { "Queue"="Transparent+1" 
				"IgnoreProjector"="True"
				"RenderType"="Transparent" }
		Pass {
			
			LOD 200
			Blend SrcAlpha OneMinusSrcAlpha 
			Zwrite off
			
			CGPROGRAM
			#pragma vertex vert 
	        #pragma fragment frag
	        #include "UnityCG.cginc"
	        
	        struct VertIn {
	        	float4 vertex : POSITION;
	        	float4 texcoord : TEXCOORD0;
	        };
	        
	        struct v2f {
	        	fixed4 pos : SV_POSITION;
	        	fixed2 uv : TEXCOORD0;
	        	fixed2 uv1 : TEXCOORD1;
	        	fixed2 uv2 : TEXCOORD2;
	   		};
	        
	        uniform sampler2D _MainTex;  
	        uniform sampler2D _Mask;
	        uniform fixed _ScrollX1;
	        uniform fixed _ScrollY1;
	        
	        uniform fixed _ScrollX2;
	        uniform fixed _ScrollY2;
	        
	         
	        v2f vert(VertIn v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				
				float t1 = _Time.y*_ScrollX1;
				float t2 = _Time.y*_ScrollY1;
				
				float t3 = _Time.y*_ScrollX2;
				float t4 = _Time.y*_ScrollY2;
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				
				o.uv1 = o.uv+fixed2(t1, t2);
				o.uv2 = o.uv+fixed2(t3, t4);
				//o.uv1 = o.uv;
				//o.uv2 = o.uv;
				return o;
			}
			
			fixed4 frag(v2f i) : Color {
                fixed4 tex =  tex2D(_MainTex, i.uv1);
                fixed4 wave1 = tex2D(_MainTex, i.uv2);
                fixed4 tex3 =  tex2D(_Mask, i.uv);
				 
                fixed4 col;
                col.rgb = tex.rgb*wave1.rgb*tex3.rgb;
                
                col.a = tex.a*tex3.a;
				return col;
			}	
	        
	        ENDCG
		}
	} 
	FallBack "Diffuse"
}
