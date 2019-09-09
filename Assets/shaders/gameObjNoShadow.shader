// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/gameObjNoShadow" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_Ambient ("Ambient Color", Color) = (0.588, 0.588, 0.588, 1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		
	    _LightDir ("World light Dir", Vector) = (-1, -1, 1, 0)
	
		_HighLightDir ("Hight Light Dir", Vector) = (-1, -1, 1, 0)	
		_LightDiffuseColor ("Light Diffuse Color", Color) = (1, 1, 1, 1)
		
	}
	SubShader {
		Pass {
			Name "BASE"
			Tags { "RenderType"="Opaque" }
			LOD 200
			CGPROGRAM
			
			#pragma vertex vert 
	        #pragma fragment frag
	        #include "UnityCG.cginc"
	        
	        uniform fixed4 _Color;
			uniform fixed4 _HighLightDir;
			uniform fixed4 _Ambient;
			uniform fixed4 _LightDiffuseColor;
			
	        struct v2f {
	        	float4 pos : SV_POSITION;
	        	float2 uv : TEXCOORD0;
	        	float4 colour : TEXCOORD1;
	        };
	        
	        uniform sampler2D _MainTex;
	        
	        v2f vert(appdata_base v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				fixed3 viewNor = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
				fixed3 lightDir = -normalize(_HighLightDir);
				
	          	o.colour = _Ambient*_Color+ _Color*saturate(dot(lightDir, viewNor))*_LightDiffuseColor;
	          	o.colour.a = 1;
				return o;
			}
			
			fixed4 frag(v2f i) : Color {
				return tex2D(_MainTex, i.uv)*i.colour;
			}
	        ENDCG
		}
		
	} 
	  
	FallBack "Diffuse"
}
