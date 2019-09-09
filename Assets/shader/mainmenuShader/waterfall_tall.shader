Shader "Custom/waterfall_tall" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_UVAnimX ("UV Anim X", float) = 0
		_UVAnimY ("UV Anim Y", float) = 0

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
	        };

	        uniform sampler2D _MainTex;  
	        uniform fixed _UVAnimX;
	        uniform fixed _UVAnimY;

	        v2f vert(VertIn v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;

				fixed2 uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);

				float t1 = _Time.y*_UVAnimX;
				float t2 = _Time.y*_UVAnimY;
				o.uv = uv+ fixed2(t1, t2);
				return o;
			}
			
			fixed4 frag(v2f i) : Color {
                fixed4 col;
                fixed4 baseCol = tex2D(_MainTex, i.uv);
                col = baseCol*i.color;
				return col;
			}	
	        ENDCG
		}


	} 
	FallBack "Diffuse"
}
