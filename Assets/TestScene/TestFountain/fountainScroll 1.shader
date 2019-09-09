Shader "Custom/fountainScroll" {

	Properties {
		_Mask ("Mask (RGB)", 2D) = "white" {}
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_UVAnimX ("UV Anim X", float) = 0
		_UVAnimY ("UV Anim Y", float) = 0


		_Mask2 ("Mask2 (RGB)", 2D) = "white" {}
		_MainTex2 ("Base2 (RGB)", 2D) = "white" {}
		_UVAnimX2 ("UV Anim X", float) = 0
		_UVAnimY2 ("UV Anim Y", float) = 0
	}

	SubShader {
		Tags { "Queue"="Transparent" 
				"IgnoreProjector"="True"
				"RenderType"="Transparent" }
		LOD 200
		Pass {
			
			LOD 200
			Blend SrcAlpha OneMinusSrcAlpha 
			zwrite off
			Lighting off
			
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
	        };

	        uniform sampler2D _Mask;  
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
				o.uv1 = o.uv+fixed2(t1, t2);
				
				return o;
			}
			
			fixed4 frag(v2f i) : Color {
                fixed4 tex1 =  tex2D(_Mask, i.uv);
                fixed4 tex2 =  tex2D(_MainTex, i.uv1);
                fixed4 col;
                col.rgb = tex1.rgb*tex2.rgb;
                col.a = tex1.a*tex2.a;
				return col;
			}	
	        
	        
	        ENDCG
		}

		Pass {
			
			LOD 200
			Blend SrcAlpha OneMinusSrcAlpha 
			zwrite off
			Lighting off
			
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
	        };

	        uniform sampler2D _Mask2;  
	        uniform sampler2D _MainTex2;  
	        uniform fixed _UVAnimX2;
	        uniform fixed _UVAnimY2;

	         
	         
	        v2f vert(VertIn v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				
				float t1 = _Time.y*_UVAnimX2;
				float t2 = _Time.y*_UVAnimY2;
	
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				o.uv1 = o.uv+fixed2(t1, t2);
				
				return o;
			}
			
			fixed4 frag(v2f i) : Color {
                fixed4 tex1 =  tex2D(_Mask2, i.uv);
                fixed4 tex2 =  tex2D(_MainTex2, i.uv1);
                fixed4 col;
                col.rgb = tex1.rgb*tex2.rgb;
                col.a = tex1.a*tex2.a;
				return col;
			}	
	        
	        
	        ENDCG
		}
	
	} 
	FallBack "Diffuse"
}
