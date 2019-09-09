Shader "Custom/waterfall_bottomsplash" {
	Properties {
		_MainTex2 ("MainTex", 2D) = "white" {}
		_Mask2 ("Mask2", 2D) = "white" {}
		_Mask3 ("Mask3", 2D) = "white" {}
		_UVAnimX1 ("UV Anim X1", float) = 0
		_UVAnimY1 ("UV Anim Y1", float) = 0
		_UVAnimX2 ("UV Anim X2", float) = 0
		_UVAnimY2 ("UV Anim Y2", float) = 0
		_UVAnimX3 ("UV Anim X3", float) = 0
		_UVAnimY3 ("UV Anim Y3", float) = 0
		_UVAnimX4 ("UV Anim X4", float) = 0
		_UVAnimY4 ("UV Anim Y4", float) = 0
	}
	SubShader {
		Tags { "Queue"="Transparent" 
				"IgnoreProjector"="True"
				"RenderType"="Transparent" }

				/*
		Pass {
			Name "Base"
			LOD 200
			Blend SrcAlpha OneMinusSrcAlpha 
			zwrite off
			
			CGPROGRAM
			#pragma vertex vert 
	        #pragma fragment frag
	        #include "UnityCG.cginc"
	        
	        struct VertIn {
	        	float4 vertex : POSITION;
	        	float4 color : COLOR;
	        };
	        
	        struct v2f {
	        	fixed4 pos : SV_POSITION;
	        	fixed4 color : TEXCOORD0;
	        };
	        
	        v2f vert(VertIn v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				return o;
			}
			
			fixed4 frag(v2f i) : Color {
                fixed4 col;
                col = i.color;
				return col;
			}	
	        ENDCG
		}
		*/

		Pass {
			Name "Mask"
			LOD 200
			Blend SrcAlpha OneMinusSrcAlpha 
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
	        	fixed4 color : TEXCOORD0;
	        	fixed2 uv : TEXCOORD1; 
	        	fixed2 uv1 : TEXCOORD2; 
	        	fixed2 uv2 : TEXCOORD3; 
	        	fixed2 uv3 : TEXCOORD4;
	        };

	        uniform sampler2D _MainTex2;  
	        uniform sampler2D _Mask2;  
	        uniform sampler2D _Mask3;  
	        uniform fixed _UVAnimX1;
	        uniform fixed _UVAnimY1;
	        uniform fixed _UVAnimX2;
	        uniform fixed _UVAnimY2;
	        uniform fixed _UVAnimX3;
	        uniform fixed _UVAnimY3;
	        uniform fixed _UVAnimX4;
	        uniform fixed _UVAnimY4;

	        v2f vert(VertIn v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;

				fixed2 uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);

				float t1 = _Time.y*_UVAnimX1;
				float t2 = _Time.y*_UVAnimY1;
				o.uv1 = uv+ fixed2(t1, t2);

				float t3 = _Time.y*_UVAnimX2;
				float t4 = _Time.y*_UVAnimY2;
				o.uv2 = uv+ fixed2(t3, t4);

				float t5 = _Time.y*_UVAnimX3;
				float t6 = _Time.y*_UVAnimY3;
				o.uv3 = uv + fixed2(t5, t6);

				float t7 = _Time.y*_UVAnimX4;
				float t8 = _Time.y*_UVAnimY4;
				o.uv = uv+ fixed2(t7, t8);

				return o;
			}
			
			fixed4 frag(v2f i) : Color {
                fixed4 col;
                fixed4 baseCol = tex2D(_MainTex2, i.uv);
                fixed4 maskCol = tex2D(_Mask2, i.uv1);
                fixed4 maskCol2 = tex2D(_Mask3, i.uv2);
                fixed4 maskCol3 = tex2D(_Mask3, i.uv3);

                col = maskCol*maskCol2*maskCol3*baseCol*i.color;
				return col;
			}	
	        ENDCG
	    }
		


	} 
	FallBack "Diffuse"
}
