// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/lightMapillumAlpha" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_IllumMap ("Illum Map", 2D) = "white" {}
	}
	
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType" = "Transparent" }
	
		Pass {	
			Name "BASE"
			LOD 200
			Lighting Off
			Blend SrcAlpha OneMinusSrcAlpha
			zwrite on
			ztest on
			
			
			
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
			uniform sampler2D _IllumMap;
			
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
				
				//o.vertColor = v.color; 
				o.offPos = mul(unity_ObjectToWorld, v.vertex).xyz-(_WorldSpaceCameraPos+_CamPos);
				return o;
			}
			
			fixed4 frag(v2f i) : Color {
				fixed4 col =  tex2D(_MainTex, i.uv);
	        	fixed4 retCol;
	        	
				fixed2 mapUV = (i.offPos.xz+float2(_CameraSize, _CameraSize))/(2*_CameraSize);
				fixed4 illumCol = tex2D(_IllumMap, i.uv);
				retCol.rgb = col.rgb*(_AmbientCol.rgb+tex2D(_LightMap, mapUV).rgb * (1-tex2D(_LightMask, mapUV).a)*_LightCoff )+illumCol.rgb;
				
				retCol.a = col.a;
				
				
				return retCol;
			}	

			ENDCG
		}
	} 
	FallBack "Diffuse"
	
}
