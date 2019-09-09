// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/chestReflect" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Cube ("EnvMap (RGB)", Cube) = "" { TexGen CubeReflect }
		_SpecMap ("SpecMap", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Pass {	
			Lighting Off

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
	   			fixed3 offPos : TEXCOORD2;
	        };
			uniform sampler2D _MainTex;
			uniform sampler2D _LightMap;
		    uniform float4 _CamPos;
		    uniform float _CameraSize;
		    uniform float4 _AmbientCol;
			uniform sampler2D _LightMask;
			uniform float _LightCoff;
			
			v2f vert(VertIn v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				o.offPos = mul(unity_ObjectToWorld, v.vertex).xyz-(_WorldSpaceCameraPos+_CamPos);
				return o;
			}
			fixed4 frag(v2f i) : Color {
	        	
	        	fixed4 col =  tex2D(_MainTex, i.uv);
	        	fixed4 retCol;
				fixed2 mapUV = (i.offPos.xz+float2(_CameraSize, _CameraSize))/(2*_CameraSize);
				retCol.rgb = col.rgb*(_AmbientCol.rgb+tex2D(_LightMap, mapUV).rgb * (1-tex2D(_LightMask, mapUV).a)*_LightCoff );
				retCol.a = col.a;
				return retCol;
			}	

			ENDCG
		}

		Pass {
			Lighting Off
			Blend One One

			CGPROGRAM
			#pragma vertex vert 
		    #pragma fragment frag
		    #include "UnityCG.cginc"

	        struct VertIn {
	        	float4 vertex : POSITION;
	        	float4 texcoord : TEXCOORD0;
	        	float3 normal : TEXCOORD1;
	        };
			
			struct v2f {
	        	fixed4 pos : SV_POSITION;
	        	fixed2 uv : TEXCOORD0;
	        	fixed3 viewDir : TEXCOORD1;
	        	fixed3 normalDir : TEXCOORD2;
	        };

			uniform samplerCUBE _Cube ;
			uniform sampler2D _SpecMap ;

			v2f vert(VertIn v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);

				float4x4 modelMatrix = unity_ObjectToWorld;
            	float4x4 modelMatrixInverse = unity_WorldToObject;
				o.viewDir = normalize(_WorldSpaceCameraPos - mul(modelMatrix, v.vertex).xyz);	
				o.normalDir = normalize(mul(float4(v.normal, 0.0), modelMatrixInverse).xyz);
				return o;
			}

			fixed4 frag(v2f input) : Color {
	        	float3 reflectedDir = reflect(input.viewDir, normalize(input.normalDir));
	        	fixed4 tex1 = texCUBE(_Cube, reflectedDir);
	        	fixed4 tex2 = tex2D(_SpecMap, input.uv);
				return tex1*tex2;
				//return tex1;
			}	


	        ENDCG


		}
	} 
	FallBack "Diffuse"
}
