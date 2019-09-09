// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'

Shader "Custom/playerBodyEquipShader" {
	
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_Ambient ("Ambient Color", Color) = (0.588, 0.588, 0.588, 1)
		
		_MainTex ("Base (RGB)", 2D) = "white" {}
		//Alpha Blend Two Texture 
		_BodyTex("Body Tex", 2D) = "white" {}
		
		_RimColor ("Rim Color", Color) = (0.26, 0.19, 0.16, 0.0)
	    _RimPower ("Rim Power", Range(0.5, 8.0)) = 3.0
	    
	    _LightDir ("World light Dir", Vector) = (-1, -1, 1, 0)
		_ShadowColor ("Shadow Color", Color) = (0, 0, 0, 1)
	
		_HighLightDir ("Hight Light Dir", Vector) = (-1, -1, 1, 0)	
		_LightDiffuseColor ("Light Diffuse Color", Color) = (1, 1, 1, 1)
		
		//_LightAtten("", float) = 1
		//_Emissive ("Emissive Color", Color) = (0.588, 0.588, 0.588, 1)
		//_Diffuse ("Diffuse Color", Color) = (1, 1, 1, 1)
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
	        uniform fixed4 _RimColor;
			uniform fixed _RimPower;
			uniform fixed4 _HighLightDir;
			uniform fixed4 _Ambient;
			//uniform fixed4 _Emissive;
			uniform fixed4 _LightDiffuseColor;
			//uniform fixed4 _Diffuse;
			
	        struct v2f {
	        	float4 pos : SV_POSITION;
	        	float2 uv : TEXCOORD0;
	        	//fixed3 diff;
	        	float4 colour : TEXCOORD1;
	        };
	        
	        uniform sampler2D _MainTex;
	        uniform sampler2D _BodyTex;
	        
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
				fixed4 equipCol = tex2D(_MainTex, i.uv);
				fixed4 bodyCol = tex2D(_BodyTex, i.uv);
				fixed3 col = equipCol.a*equipCol.rgb + (1-equipCol.a)*bodyCol.rgb;
				return fixed4(col, 1)*i.colour;
				
				//+fixed4(i.diff, 1.0);
				
			}
			
	        ENDCG
		}
		
		Pass {
			Name "SHADOW"
			Tags {"RenderType"="Opaque"}
			
			Offset -1.0, -2.0
			CGPROGRAM
			#pragma vertex vert 
	         #pragma fragment frag
	 
	         #include "UnityCG.cginc"
	 
	         // User-specified uniforms
	         uniform fixed4 _ShadowColor;
	         uniform fixed4x4 _World2Receiver; // transformation from 
	         uniform fixed4 _LightDir;
	        
	         float4 vert(float4 vertexPos : POSITION) : SV_POSITION
	         {
	            float4x4 modelMatrix = unity_ObjectToWorld;
	            float4x4 modelMatrixInverse = 
	               unity_WorldToObject * 1.0;
	            modelMatrixInverse[3][3] = 1.0; 
	            float4x4 viewMatrix = 
	               mul(UNITY_MATRIX_MV, modelMatrixInverse);
	 
	            float4 lightDirection = _LightDir;
	            lightDirection = normalize(lightDirection);
	            
	            float4 vertexInWorldSpace = mul(modelMatrix, vertexPos);
	            
	         
	           	float4 world2ReceiverRow1 = 
	               float4(_World2Receiver[1][0], _World2Receiver[1][1], 
	               _World2Receiver[1][2], _World2Receiver[1][3]);
	           
	            
	            float distanceOfVertex = 
	               dot(world2ReceiverRow1, vertexInWorldSpace);
	            
	            float lengthOfLightDirectionInY = 
	               dot(world2ReceiverRow1, lightDirection); 
	 
	            if (distanceOfVertex > 0.0 && lengthOfLightDirectionInY < 0.0)
	            {
	               lightDirection = lightDirection 
	                  * (distanceOfVertex / (-lengthOfLightDirectionInY));
	            }
	            else
	            {
	               lightDirection = float4(0.0, 0.0, 0.0, 0.0); 
	                  // don't move vertex
	            }
	 
	            return mul(UNITY_MATRIX_P, mul(viewMatrix, 
	               vertexInWorldSpace + lightDirection));
	         	
	         }
	 
	         float4 frag(void) : COLOR 
	         {
	            return _ShadowColor;
	         }
			
			ENDCG
		}
	} 
	FallBack "Diffuse"
	
	
	
}
