Shader "Custom/waterFallShader" {
	Properties {
		_Color ("Ambient", Color) = (0.588, 0.588, 0.588, 1)
		//_Diffuse ("Ambient", Color) = (0.588, 0.588, 0.588, 1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_UVAnimX ("UV Anim X", float) = 0
		_UVAnimY ("UV Anim Y", float) = 0
		
		_Mask ("Mask", 2D) = "white" {}
		_Spray ("spray ", 2D) = "white" {}
		_UVAnimX1 ("UV Anim X", float) = 0
		_UVAnimY1 ("UV Anim Y", float) = 0
	}
	
	SubShader {
		Tags { "Queue"="Transparent+1" 
				"IgnoreProjector"="True"
				"RenderType"="Transparent" }
		LOD 200
		Pass {
			Blend SrcAlpha OneMinusSrcAlpha 
			ztest off
			zwrite off
			//ztest always
			//zwrite off
			
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
	        uniform fixed4 _Color;
	        uniform sampler2D _MainTex;  
	        uniform fixed _UVAnimX;
	        uniform fixed _UVAnimY;
	         
	        uniform sampler2D _Mask;
	        
	        v2f vert(VertIn v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				
				float t1 = _Time.y*_UVAnimX;
				float t2 = _Time.y*_UVAnimY;
	
				o.uv1 = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				o.uv = o.uv1+fixed2(t1, t2);	
				return o;
			}
	        //Mask alpha Value at begin and end
	        fixed4 frag(v2f i) : Color {
                fixed4 tex =  tex2D(_MainTex, i.uv);
                fixed4 tex2 = tex2D(_Mask, i.uv1);
                fixed4 col;
                col.rgb = tex.rgb*_Color;
                //col.rgb = tex2.rgb;
                col.a = tex.a*tex2.a;
				//col.a = tex2.a;
				return col;
			}
	        
	        ENDCG
		}
		
		Pass {
			Blend SrcAlpha OneMinusSrcAlpha 
			//zwrite off
			
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
	   		
	        uniform fixed4 _Color;
	        uniform sampler2D _Spray;  
	        uniform fixed _UVAnimX1;
	        uniform fixed _UVAnimY1;
	         
	        uniform sampler2D _Mask;
	        
	        v2f vert(VertIn v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				
				float t1 = _Time.y*_UVAnimX1;
				float t2 = _Time.y*_UVAnimY1;
	
				o.uv1 = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				o.uv = o.uv1+fixed2(t1, t2);	
				return o;
			}
	        //Mask alpha Value at begin and end
	        fixed4 frag(v2f i) : Color {
                fixed4 tex =  tex2D(_Spray, i.uv);
                fixed4 tex2 = tex2D(_Mask, i.uv1);
                fixed4 col;
                col.rgb = tex.rgb*_Color;
                //col.rgb = tex2.rgb;
                col.a = tex.a*tex2.a;
				//col.a = tex2.a;
				return col;
			}
	        
	        ENDCG
		}
	} 
	FallBack "Diffuse"
}
