Shader "Custom/waterWaveShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Freq_X("wave x freq", float) = 0.07
		_Amplitude_X("wave x amplitude", float) = -0.2
		
		_Freq_Y("wave y freq", float) = 0.06
		_Amplitude_Y("wave y amplitude", float) = -0.24
	
	
		_AnimTex("Animation Texture Pass", 2D) = "white" {}	
		_Freq_X1("wave x freq", float) = 0.02
		_Amplitude_X1("wave x amplitude", float) = 0.05
		
		_Freq_Y1("wave y freq", float) = 0.02
		_Amplitude_Y1("wave y amplitude", float) = 0.05
	}
	

	//wave_xform
	SubShader {
		Tags { "Queue"="Transparent" 
				"IgnoreProjector"="True"
				"RenderType"="Transparent" }
		Pass {
			
			LOD 200
			Blend SrcAlpha OneMinusSrcAlpha 
			
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
	        };
	        
	        uniform sampler2D _MainTex;  
	        uniform fixed _Freq_X;
	        uniform fixed _Amplitude_X; 
	         
	        uniform fixed _Freq_Y;
	        uniform fixed _Amplitude_Y; 
	         
	         
	        v2f vert(VertIn v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				
				float t1 = sin(_Time.y*_Freq_X*6.28)*_Amplitude_X;
				float t2 = sin(_Time.y*_Freq_Y*6.28)*_Amplitude_Y;
	
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				o.uv += fixed2(t1, t2);
				
				return o;
			}
			
			fixed4 frag(v2f i) : Color {
                fixed4 tex =  tex2D(_MainTex, i.uv);
                fixed4 col;
                col.rgb = tex.rgb;
                col.a = tex.a;
				return col;
			}	
	        
	        
	        ENDCG
		}
		
		
		Pass {
			LOD 200
			blend one one
			zwrite off
			lighting off
			
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
	        };
	        
	        uniform sampler2D _AnimTex;  
	        uniform fixed _Freq_X1;
	        uniform fixed _Amplitude_X1; 
	         
	        uniform fixed _Freq_Y1;
	        uniform fixed _Amplitude_Y1; 
	         
	         
	        v2f vert(VertIn v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				
				float t1 = sin(_Time.y*_Freq_X1*6.28)*_Amplitude_X1;
				float t2 = sin(_Time.y*_Freq_Y1*6.28)*_Amplitude_Y1;
	
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				o.uv += fixed2(t1, t2);
				
				return o;
			}
			
			fixed4 frag(v2f i) : Color {
                fixed4 tex =  tex2D(_AnimTex, i.uv);
                fixed4 col;
                col.rgb = tex.rgb;
                col.a = tex.a;
				return col;
			}	
	        
	        ENDCG
		}
		
	} 
	FallBack "Diffuse"
}
