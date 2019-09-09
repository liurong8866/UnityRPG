// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/lightMapAlphaOld" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		_LightMap ("Light Map ", 2D) = "white" {}
		_CamPos ("Camera pos", Vector) = (0, 0, 0, 0)
		_CameraSize ("Camera Size", float) = 10
	}
	SubShader {
		//Tags { "Queue"="Transparent" "IgnoreProjector"="True" }
		
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
	        	fixed3 offPos : TEXCOORD1;
	        };
		    uniform sampler2D _MainTex; 
		    uniform sampler2D _LightMap;
		    uniform float4 _CamPos;
		    uniform float _CameraSize;
		    uniform float4 _Color;
		    
		    v2f vert(appdata_base v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				
				o.offPos = mul(unity_ObjectToWorld, v.vertex).xyz-(_WorldSpaceCameraPos+_CamPos);//_CamPos;//_WorldSpaceCameraPos;
						
				return o;
			}    
			fixed4 frag(v2f i) : Color {
				
	            return tex2D(_MainTex, i.uv)*(tex2D(_LightMap, ((i.offPos.xz+float2(_CameraSize, _CameraSize))/(2*_CameraSize)))+UNITY_LIGHTMODEL_AMBIENT)*2*_Color;
				
			
			}		
					
			ENDCG
		}
		
	} 
	FallBack "Diffuse"
}
