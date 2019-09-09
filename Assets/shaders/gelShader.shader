// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'

Shader "Custom/gelShader" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_ReflectTex ("", 2D) = "white" {}
		
		_LightDir ("World light Dir", Vector) = (-1, -1, 1, 0)
		_ShadowColor ("Shadow Color", Color) = (0, 0, 0, 1)
	}
	
	SubShader {
		Tags { "Queue"="Transparent" }
		 
		 
		Pass {
			Name "BASE"
			LOD 200
			
	     	Blend SrcAlpha OneMinusSrcAlpha  
	        
			CGPROGRAM
			#pragma vertex vert 
	        #pragma fragment frag
	        #include "UnityCG.cginc"
	        
	        struct v2f {
	        	fixed4 pos : SV_POSITION;
	        	fixed2 uv : TEXCOORD0;
	        	fixed2 uv1 : TEXCOORD1;
	        };
	        
	        uniform sampler2D _MainTex;
	        uniform sampler2D _ReflectTex;
	        
	        uniform fixed4 _Color;
	        
	        v2f vert(appdata_base v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				
				float3 pos1 = normalize(mul(UNITY_MATRIX_MV, v.vertex).xyz);
				
				float3 nor = normalize(mul((float3x3)UNITY_MATRIX_MV, v.normal));
				
				// calculate reflection vector in eye space
				half3 r = reflect(pos1, nor);
				float m = 2*sqrt(r.x*r.x+r.y*r.y+(r.z+1.0)*(r.z+1.0));
				float s = r.x/m+0.5;
				float t = r.y/m+0.5;
				
				o.uv1 = fixed2(s, t);
				return o;
			}
			
			fixed4 frag(v2f i) : Color {
				fixed4 tex = tex2D(_MainTex, i.uv);
				fixed4 refCol = tex2D(_ReflectTex, i.uv1);
				
				fixed4 col;
				col.rgb = tex.rgb+refCol.rgb;
				col.a = tex.a;
				return col*_Color;
			}
		
	        ENDCG	
		}
		
		
		Pass {
			Name "BASE"
			
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
