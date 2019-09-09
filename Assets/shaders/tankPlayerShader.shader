Shader "Custom/tankPlayerShader" {
	Properties {
	}
	
	SubShader {
		Tags { 
			"Queue"="Geometry+5" 
			"IgnoreProjector"="True"
		 }

		Pass {
			Name "BASE"
			
			LOD 200
			CGPROGRAM
			
			#pragma vertex vert 
	        #pragma fragment frag
	        #include "UnityCG.cginc"
	        
			uniform fixed4 _HighLightDir;
			uniform fixed4 _Ambient;
			uniform fixed4 _LightDiffuseColor;

			struct VertIn {
	        	float4 vertex : POSITION;
	        	float4 texcoord : TEXCOORD0;
	        	float4 color : COLOR;
	        	float3 normal : NORMAL;
	        };
	        struct v2f {
	        	float4 pos : SV_POSITION;
	        	float2 uv : TEXCOORD0;
	        	float4 colour : TEXCOORD1;
	        };
	        
	        uniform sampler2D _MainTex;
	        
	        v2f vert(VertIn v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				o.colour = v.color;

				//fixed3 viewNor = normalize(mul((float3x3)_Object2World, v.normal));
				//fixed3 lightDir = -normalize(_HighLightDir);
				//o.colour = (_Ambient+saturate(dot(lightDir, viewNor))*_LightDiffuseColor)*v.color;

	          	//o.colour = *_Color+ _Color*;
	          	//o.colour.a = 1;
				return o;
			}
			
			fixed4 frag(v2f i) : Color {
				//return tex2D(_MainTex, i.uv)*i.colour;
				return i.colour;
			}
	        ENDCG
		}

	} 
	FallBack "Diffuse"
}
