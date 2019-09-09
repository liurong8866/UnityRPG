// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

/*
Shader "Custom/cubeShader" {
	Properties {
		_Cube ("EnvMap (RGB)", Cube) = "" {}
		//{ TexGen CubeReflect }
	}
	SubShader {
		Pass {
			Name "Base"
			Tags {"RenderType"="Opaque"}
			
			CGPROGRAM
			#pragma vertex vert 
	        #pragma fragment frag
	        #include "UnityCG.cginc"

	         uniform samplerCUBE _Cube;

	         struct vin {
	         	fixed4 vertex : POSITION;
	         	fixed3 normal : NORMAL;
	         }

	         struct v2f {
	         	fixed4 pos : SV_POSITION;
	         	fixed3 reflectDir : TEXCOORD2;
	         }
	 
	         v2f vert(vin ver)
	         {
	         	v2f o;
	         	fixed4x4 modelMatrix = _Object2Model;
	         	fixed4x4 modelMatrixInverse = _World2Object;
	         	fixed3 viewDir = mul(modelMatrix, ver.vertex).xyz
	         		- _WorldSpaceCameraPos;
	         	fixed3 normalDir = normalize(
	         		mul(fixed4(ver.normal, 0.0), modelMatrixInverse).xyz);
	         	o.pos = mul(UNITY_MATRIX_MVP, ver.vertex);

	         	o.reflectDir = reflect(viewDir, normalDir);
	         	return o;
	         }
	 
	         fixed4 frag(v2f i) : COLOR 
	         {
	         	float3 refDir = normalize(i.reflectDir);
	         	//fixed4 col = texCUBE(_Cube, refDir);
	         	fixed4 col;
	         	return col;
	         }
			
			ENDCG
		}
		
	} 
	FallBack "Diffuse"
}
*/


Shader "Custom/cubeShader" {
   Properties {
      _Cube("Reflection Map", Cube) = "" {}
   }
   SubShader {
      Pass {   
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 

         #include "UnityCG.cginc"

         // User-specified uniforms
         uniform samplerCUBE _Cube;   

         struct vertexInput {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float3 reflectDir : TEXCOORD0;
            //float3 normalDir : TEXCOORD0;
            float3 viewDir : TEXCOORD1;
            float3 objNormalDir : TEXCOORD2;
            float3 objPos : TEXCOORD3;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
            float4x4 modelMatrix = unity_ObjectToWorld;
            float4x4 modelMatrixInverse = unity_WorldToObject; 
 
            float3 viewDir = mul(modelMatrix, input.vertex).xyz 
               - _WorldSpaceCameraPos;
            float3 normalDir = normalize(
               mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
            output.reflectDir = 
               reflect(viewDir, normalize(normalDir));
            output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
            output.viewDir = viewDir;
            output.objNormalDir = input.normal;
            output.objPos = input.vertex;
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
            //float3 reflectedDir = 
            //   reflect(input.viewDir, normalize(input.normalDir));
            float3 refDir = normalize(input.reflectDir);
            float3 viewDir = normalize(input.viewDir);
            float3 objNor = normalize(input.objNormalDir);
            float3 objPos = normalize(input.objPos);
            return texCUBE(_Cube,  objPos);
         }
 
         ENDCG
      }
   }
}