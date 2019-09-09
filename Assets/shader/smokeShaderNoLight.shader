Shader "Custom/smokeShaderNoLight" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_UVAnimX ("UV Anim X", float) = 0
		_UVAnimY ("UV Anim Y", float) = 0
		
		_MaskMap ("Mask Map", 2D) = "white" {}
	}
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType" = "Transparent" }
		LOD 200
		
		Pass {	
			Name "BASE"
			LOD 200
			Lighting Off
			Blend SrcAlpha OneMinusSrcAlpha
			zwrite off
			ztest on
			
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
	        	fixed2 uv2 : TEXCOORD1;
	        };
	        
	        uniform sampler2D _MainTex;
			uniform sampler2D _MaskMap;
			
			
			uniform fixed _UVAnimX;
	        uniform fixed _UVAnimY;
	        
			v2f vert(VertIn v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				float t1 = _Time.y*_UVAnimX;
				float t2 = _Time.y*_UVAnimY;
				o.uv2 = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				o.uv = o.uv2+ fixed2(t1, t2);
				
				
				return o;
			}
			
			fixed4 frag(v2f i) : Color {
				fixed4 col =  tex2D(_MainTex, i.uv);
	        	fixed4 retCol;
	        	
				fixed4 maskCol = tex2D(_MaskMap, i.uv2);
				retCol.rgb = col.rgb * maskCol.rgb;
				retCol.a = col.a;
				//retCol.a = col.a*maskCol.a;
				return retCol;
			}	

			ENDCG
	        
		}
	} 
	FallBack "Diffuse"
}
