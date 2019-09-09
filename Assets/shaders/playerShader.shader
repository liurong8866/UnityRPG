// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/playerShader" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_Ambient ("Ambient Color", Color) = (0.588, 0.588, 0.588, 1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		
	}
	
	SubShader {
		Tags { 
			"Queue"="Geometry+5" 
			"IgnoreProjector"="True"
		 }

		 Pass {
			Name "Overlay"
			zwrite off
			ztest greater  
			
			CGPROGRAM
			#pragma vertex vert 
	        #pragma fragment frag
	        #include "UnityCG.cginc" 
	        
	        struct v2f {
	        	float4 pos : SV_POSITION;
	        	float2 uv : TEXCOORD0;
	        };
	        
	        uniform sampler2D _MainTex;
	        uniform fixed4 _OverlayColor;
	        
	        v2f vert(appdata_base v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
	          	return o;
			}
			fixed4 frag(v2f i) : Color {
				return tex2D(_MainTex, i.uv)*_OverlayColor;
			}
			
	        ENDCG
		}


		Pass {
			Name "BASE"
			
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

		/*
		Pass {
			Name "SHADOW"
			Blend One Zero
			
			Offset -1.0, -2.0
			CGPROGRAM
			#pragma vertex vert 
	         #pragma fragment frag
	 
	         #include "UnityCG.cginc"
	 
	         uniform fixed4 _ShadowColor;
	         uniform fixed4x4 _World2Receiver; // transformation from 
	         uniform fixed4 _LightDir;
	        
	         float4 vert(float4 vertexPos : POSITION) : SV_POSITION
	         {
	            float4x4 modelMatrix = _Object2World;
	            float4x4 modelMatrixInverse = 
	               _World2Object * unity_Scale.w;
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
		*/
		
	} 
	FallBack "Diffuse"
}
