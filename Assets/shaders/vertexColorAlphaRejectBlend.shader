// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/vertexColorAlphaRejectBlend" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Ambient Color", Color) = (0.588, 0.588, 0.588, 1)
		
		_LightMap ("Light Map ", 2D) = "white" {}
		_CamPos ("Camera pos", Vector) = (0, 0, 0, 0)
		_CameraSize ("Camera Size", float) = 10
		//_Cutoff("Base alpha cutoff", Range(0, 0.9)) = 0.02
		
	}
	
	SubShader {
		Tags {"RenderType"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		Lighting off
		
		Pass {
			LOD 200
			Blend SrcAlpha OneMinusSrcAlpha
			
			
			CGPROGRAM
			#pragma vertex vert 
		    #pragma fragment frag
		    #include "UnityCG.cginc"
		        
	        struct VertIn {
	        	float4 vertex : POSITION;
	        	float4 texcoord : TEXCOORD0;
	        	float4 color : COLOR;
	        };

			
			struct v2f {
	        	fixed4 pos : SV_POSITION;
	        	fixed2 uv : TEXCOORD0;
	   			fixed4 vertColor : TEXCOORD1;
	   			fixed3 offPos : TEXCOORD2;
	        };
			uniform sampler2D _MainTex;
			uniform fixed4 _Color;
			
			uniform sampler2D _LightMap;
		    uniform float4 _CamPos;
		    uniform float _CameraSize;
			uniform fixed _LightCoff;
			v2f vert(VertIn v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				
				o.vertColor = v.color; 
				o.offPos = mul(unity_ObjectToWorld, v.vertex).xyz-(_WorldSpaceCameraPos+_CamPos);
				return o;
			}
			
			fixed4 frag(v2f i) : Color {
	        	fixed4 tex =  tex2D(_MainTex, i.uv);//*(i.vertColor+_Color);
	        	fixed4 lightCol =  tex2D(_LightMap, (i.offPos.xz+float2(_CameraSize, _CameraSize))/(2*_CameraSize));
	        	fixed4 col;
	        	col.rgb = tex.rgb * ((i.vertColor+UNITY_LIGHTMODEL_AMBIENT)/1.5+lightCol*2);
	        	col.a = tex.a;
				return col;
			}	
			

			ENDCG
		}
	} 
	FallBack "Diffuse"
	
	
}
