// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'

Shader "Custom/npcAlpha" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		
		//_LightDir ("World light Dir", Vector) = (-1, -1, 1, 0)
		//_ShadowColor ("Shadow Color", Color) = (0, 0, 0, 1)
	}
	SubShader {
		Tags { "Queue" = "Transparent" "IgnoreProjector"="True"  "RenderType"="Transparent" }
		Lighting off
		
		Pass {
			Name "Overlay"
			//Blend SrcAlpha OneMinusSrcAlpha
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
			Blend SrcAlpha OneMinusSrcAlpha
			zwrite on
			ztest on
			
			CGPROGRAM
			#pragma vertex vert 
	        #pragma fragment frag
	        #include "UnityCG.cginc"
	        
	        struct v2f {
	        	fixed4 pos : SV_POSITION;
	        	fixed2 uv : TEXCOORD0;
	        	fixed3 offPos : TEXCOORD2;
	        };
	        
	        uniform sampler2D _MainTex;
	        uniform fixed4 _Color;

	        uniform sampler2D _LightMap;
		    uniform float4 _CamPos;
		    uniform float _CameraSize;
		    uniform float4 _AmbientCol;
		    uniform sampler2D _LightMask;
			uniform float _LightCoff;
	        
	        
	        v2f vert(appdata_base v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);

				o.offPos = mul(unity_ObjectToWorld, v.vertex).xyz-(_WorldSpaceCameraPos+_CamPos);
				
				return o;
			}
			
			fixed4 frag(v2f i) : Color {
				fixed4 texCol = tex2D(_MainTex, i.uv);
				fixed4 col;
				col.rgb = texCol.rgb*_Color;
				col.a = texCol.a;

				fixed4 retCol;
				fixed2 mapUV = (i.offPos.xz+float2(_CameraSize, _CameraSize))/(2*_CameraSize);
				retCol.rgb = col.rgb*(_AmbientCol.rgb+tex2D(_LightMap, mapUV).rgb * (1-tex2D(_LightMask, mapUV).a)*_LightCoff );
				retCol.a = col.a;
				return retCol;
				//return col;
			}
		
	        ENDCG	
		}
		
		Pass {
			Name "Shadow"
			
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
