// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/lightMapAlpha_1" {
	//rubble
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType" = "Transparent" }
		Pass {
			Name "BASE"
			LOD 200
			Blend SrcAlpha OneMinusSrcAlpha
			zwrite off
			Lighting off
			
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
	   			fixed3 shadowPos : TEXCOORD6;
	        };
			uniform sampler2D _MainTex;
			
			uniform sampler2D _LightMap;
		    uniform float4 _CamPos;
		    uniform float _CameraSize;
		    uniform float4 _AmbientCol;
		     
		    uniform sampler2D _LightMask;
		    uniform float _LightCoff;

			uniform sampler2D _ShadowMap;
		    uniform float4 _ShadowCamPos;
		    uniform float _ShadowCameraSize;
		    
			v2f vert(VertIn v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);

				fixed3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.offPos = worldPos-(_WorldSpaceCameraPos+_CamPos);
				o.shadowPos = worldPos-(_WorldSpaceCameraPos+_ShadowCamPos);
				return o;
			}
			
			fixed4 frag(v2f i) : Color {
	        	fixed4 col =  tex2D(_MainTex, i.uv);
	        	fixed4 retCol;
	        	fixed2 mapUV = (i.offPos.xz+float2(_CameraSize, _CameraSize))/(2*_CameraSize);
	        	retCol.rgb = col.rgb*(_AmbientCol.rgb+tex2D(_LightMap, mapUV).rgb * (1-tex2D(_LightMask, mapUV).a)*_LightCoff	);
	        	retCol.a = col.a;
				
				/*
				fixed2 mapShadow = (i.shadowPos.xz+float2(_ShadowCameraSize, _ShadowCameraSize))/(2*_ShadowCameraSize);
				fixed4 shadowC = tex2D(_ShadowMap, mapShadow);
				retCol.rgb = retCol.rgb*(1-shadowC.a)+shadowC.rgb;
				*/
				return retCol;
			}	
			

			ENDCG
			
		}
	} 
	FallBack "Diffuse"
}
