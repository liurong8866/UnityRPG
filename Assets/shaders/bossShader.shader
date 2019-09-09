// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'

Shader "Custom/bossShader" {
	Properties {
		_Color("Main Color", Color) = (1, 1, 1, 1)
		_Emission("Emission", Color) = (1, 1, 1, 1)
		_ReflectColor("Reflect Color", Color) = (1, 1, 1, 1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Cube ("EnvMap (RGB)", Cube) = "" { TexGen CubeReflect }
		_SpecMap ("SpecMap", 2D) = "white" {}
		_IllumMap ("Illum Map", 2D) = "white" {}
		
		_LightDir ("World light Dir", Vector) = (-1, -1, 1, 0)
		_ShadowColor ("Shadow Color", Color) = (0, 0, 0, 1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
			
		Pass {
			Tags {"LightMode" = "Vertex"}
			Name "Base"
			LOD 200
			
			Material {
				Ambient [_Color]
				Diffuse [_Color]
				Emission [_Emission]
				Specular [Color.blank]
			}
			Lighting On
			SetTexture [_MainTex] {
				Combine texture*primary, texture * primary
			}
		}
		
		Pass {
			Tags {"LightMode" = "Vertex"}
			Material {
				Ambient [Color.blank]
				Diffuse [Color.blank]
				Emission [_Emission]
			}
			Lighting Off
			Blend One One
			
			SetTexture [_Cube] {
				constantColor [_ReflectColor]
				combine texture*constant
			}
			
			SetTexture [_SpecMap] {
				combine texture * previous
			}
			
			SetTexture [_IllumMap] {
				combine texture + previous
			}
		}
		
		Pass {
			Name "Shadow"
			Tags {"RenderType"="Opaque"}
			
			Offset -1.0, -2.0
			CGPROGRAM
			#pragma vertex vert 
	         #pragma fragment frag
	 
	         #include "UnityCG.cginc"
	 
	         // User-specified uniforms
	         uniform float4 _ShadowColor;
	         uniform float4x4 _World2Receiver; // transformation from 
	         uniform float4 _LightDir;
	         
	        
	            // world coordinates to the coordinate system of the plane
	 
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
