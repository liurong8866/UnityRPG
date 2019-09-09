// Upgrade NOTE: replaced '_LightMatrix0' with 'unity_WorldToLight'
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/ambient" {

Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader {
		Tags {
			"RenderType"="Opaque"
		}
		
		Pass {
			Name "BASE"
			Tags {  		
				"LightMode" = "ForwardBase"
			}
			LOD 200
			CGPROGRAM
			#pragma multi_compile_fwdbase
			#pragma vertex vert 
	        #pragma fragment frag
	        #define UNITY_PASS_FORWARDBASE
	        #include "UnityCG.cginc"
	        #include "AutoLight.cginc"
	        
	        struct v2f {
	        	fixed4 pos : SV_POSITION;
	        	fixed2 uv : TEXCOORD0;
	        };
	        
	        uniform sampler2D _MainTex;  
	        v2f vert(appdata_base v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				
				return o;
			}
			
			fixed4 frag(v2f i) : Color {
                return tex2D(_MainTex, i.uv) * UNITY_LIGHTMODEL_AMBIENT;
			}	        
	        
	        ENDCG
			
		}
		Pass {
            Name "ForwardAdd"
            Tags {
                "LightMode"="ForwardAdd"
            }
            //Blend One One
            
            LOD 200
			CGPROGRAM
			#pragma multi_compile_fwdbase
			#pragma vertex vert 
	        #pragma fragment frag
	        #define UNITY_PASS_FORWARDADD
	        #include "UnityCG.cginc"
	        #include "AutoLight.cginc"
	        
	        struct v2f {
	        	fixed4 pos : SV_POSITION;
	        	fixed2 uv : TEXCOORD0;
	        	float4 posWorld : TEXCOORD1;
	        	float3 normalDir : TEXCOORD2;
	        	//LIGHTING_COORDS(3,4)
	        	float3 _LightCoord : TEXCOORD3;
	        };
	        
	        uniform sampler2D _MainTex;
	        uniform fixed4 _LightColor0;
	        
	        uniform sampler2D _LightTexture0;
			uniform float4x4 unity_WorldToLight;

	        v2f vert(appdata_base v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
	          	o._LightCoord = mul(unity_WorldToLight, mul(unity_ObjectToWorld, v.vertex)).xyz;
				return o;
			}
			
			fixed4 frag(v2f i) : Color {
				i.normalDir = normalize(i.normalDir);
			
				float3 normalDirection =  i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
    	
				//float attenuation = LIGHT_ATTENUATION(i);
                float attenuation = (tex2D(_LightTexture0, dot(i._LightCoord, i._LightCoord).rr).UNITY_ATTEN_CHANNEL);
                
                float3 attenColor = attenuation * _LightColor0.xyz;
                
                
                
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor;
             
             	//return fixed4(attenuation, attenuation, attenuation, 1);
                return tex2D(_MainTex, i.uv) * float4(diffuse, 1);
			}	        
	        
	        ENDCG
        }
		
		
	
}



}
