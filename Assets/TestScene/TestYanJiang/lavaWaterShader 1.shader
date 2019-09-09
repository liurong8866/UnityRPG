Shader "Custom/lavaWaterShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_UVAnimX ("UV Anim X", float) = -0.03
		_UVAnimY ("UV Anim Y", float) = 0.03

		_MainTex1 ("Base (RGB)", 2D) = "white" {}
		_UVAnimX1 ("UV Anim X", float) = 0.03
		_UVAnimY1 ("UV Anim Y", float) = -0.03

		_MainTex2 ("Base (RGB)", 2D) = "white" {}
		_Freq_X("wave x freq", float) = 0.015
		_Amplitude_X("wave x amplitude", float) = -0.125
		
		_Freq_Y("wave  freq", float) = 0.015
		_Amplitude_Y("wave  amplitude", float) = -0.125
	}
	

	//wave_xform
	SubShader {
		Tags { "Queue"="Transparent-2" 
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
	        	fixed2 uv1 : TEXCOORD1;
	        	fixed2 uv2 : TEXCOORD2;
	        };
	        
	        uniform sampler2D _MainTex;  
	        uniform fixed _UVAnimX;
	        uniform fixed _UVAnimY;
	         
	        uniform sampler2D _MainTex1;  
	        uniform fixed _UVAnimX1;
	        uniform fixed _UVAnimY1;

	        uniform sampler2D _MainTex2;  
	        uniform fixed _Freq_X;
	        uniform fixed _Amplitude_X;
	        uniform fixed _Freq_Y;
	        uniform fixed _Amplitude_Y;

	        v2f vert(VertIn v) 

			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				
				float t1 = _Time.y*_UVAnimX;
				float t2 = _Time.y*_UVAnimY;
				fixed2 uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				o.uv = uv;
				o.uv += fixed2(t1, t2);

				float t3 = _Time.y*_UVAnimX1;
				float t4 = _Time.y*_UVAnimY1;
				o.uv1 = uv + fixed2(t3, t4);


				float t5 = sin(_Time.y*_Freq_X*6.28)*_Amplitude_X;
				float t6 = sin(_Time.y*_Freq_Y*6.28)*_Amplitude_Y;
				o.uv2 = uv + fixed2(t5, t6);
				return o;
			}
			
			fixed4 frag(v2f i) : Color {
                fixed4 tex =  tex2D(_MainTex, i.uv);
                fixed4 tex1 = tex2D(_MainTex1, i.uv1);
                fixed4 tex2 = tex2D(_MainTex2, i.uv2);
                fixed4 col;
                col.rgb = tex.rgb*tex1.a +tex2.rgb;
                //col.a = tex1.a;
                col.a = 1;
				return col;
			}	
	        
	        ENDCG
			
		}

		
	} 
	FallBack "Diffuse"
}
