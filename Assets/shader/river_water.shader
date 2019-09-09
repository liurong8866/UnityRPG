// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/river_water" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Freq_X("wave x freq", float) = 0.07
		_Amplitude_X("wave x amplitude", float) = -0.2
		
		_Freq_Y("wave  freq", float) = 0.06
		_Amplitude_Y("wave  amplitude", float) = -0.24

		_rotate("rotate", float) = 55
	
	
		_AnimTex("Animation Texture Pass", 2D) = "white" {}	
		_Freq_X1("wave x freq", float) = 0.02
		_Amplitude_X1("wave x amplitude", float) = 0.05
		
		_Freq_Y1("wave  freq", float) = 0.02
		_Amplitude_Y1("wave  amplitude", float) = 0.05

		_scaleX("scaleX", float) = 1
		_scaleY("scaleY", float) = 1

		_rotate2("rotate2", float) = 55
	}
	

	//wave_xform
	SubShader {
		Tags { "Queue"="Transparent" 
				"IgnoreProjector"="True"
				"RenderType"="Transparent" }
		Pass {
			Name "Base"
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
	        uniform fixed _Freq_X;
	        uniform fixed _Amplitude_X; 
	         
	        uniform fixed _Freq_Y;
	        uniform fixed _Amplitude_Y; 

	        uniform fixed _rotate;


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
				
				float t1 = sin(_Time.y*_Freq_X*6.28)*_Amplitude_X;
				float t2 = sin(_Time.y*_Freq_Y*6.28)*_Amplitude_Y;

				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);

				float sinX = sin(_rotate/180*3.14);
				float cosX = cos(_rotate/180*3.14);
				float2x2 rotationMatrix = float2x2(cosX, -sinX, sinX, cosX);
				o.uv = mul(o.uv, rotationMatrix);

				o.uv = mul(o.uv, rotationMatrix);
				o.uv += fixed2(t1, t2);
				
				o.vertColor = v.color; 

				o.offPos = mul(unity_ObjectToWorld, v.vertex).xyz-(_WorldSpaceCameraPos+_CamPos);
				return o;
			}
			
			fixed4 frag(v2f i) : Color {
                fixed4 tex =  tex2D(_MainTex, i.uv);
                fixed4 col;
                col.rgb = tex.rgb*(i.vertColor);
                col.a = tex.a;

				fixed2 mapUV = (i.offPos.xz+float2(_CameraSize, _CameraSize))/(2*_CameraSize);
				fixed3 lightColor = (_AmbientCol.rgb+tex2D(_LightMap, mapUV).rgb * (1-tex2D(_LightMask, mapUV).a)*_LightCoff );

				col.rgb *= lightColor;
				return col;
			}	
	        
	        
	        ENDCG
		}
		
		
		Pass {
			LOD 200
			blend one one
			zwrite off
			lighting off
			
			CGPROGRAM
			#pragma vertex vert 
	        #pragma fragment frag
	        #include "UnityCG.cginc"
	        
	        struct VertIn {
	        	float4 vertex : POSITION;
	        	float4 texcoord : TEXCOORD0;
	        	float4 color : COLOR;

	   			fixed3 offPos : TEXCOORD2;
	        };
	        
	        struct v2f {
	        	fixed4 pos : SV_POSITION;
	        	fixed2 uv : TEXCOORD0;
	   			fixed4 vertColor : TEXCOORD1;
	   			fixed3 offPos : TEXCOORD2;
	        };
	        
	        uniform sampler2D _AnimTex;  
	        uniform fixed _Freq_X1;
	        uniform fixed _Amplitude_X1; 
	         
	        uniform fixed _Freq_Y1;
	        uniform fixed _Amplitude_Y1; 

	        uniform fixed _scaleX;
	        uniform fixed _scaleY;

	        uniform fixed _rotate2;

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
				
				float t1 = sin(_Time.y*_Freq_X1*6.28)*_Amplitude_X1;
				float t2 = sin(_Time.y*_Freq_Y1*6.28)*_Amplitude_Y1;
	
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);

				float sinX = sin(_rotate2/180*3.14);
				float cosX = cos(_rotate2/180*3.14);
				float2x2 rotationMatrix = float2x2(cosX, -sinX, sinX, cosX);
				o.uv = mul(o.uv, rotationMatrix);

				o.uv *= fixed2(_scaleX, _scaleY);
				o.uv += fixed2(t1, t2);

				o.vertColor = v.color; 
				o.offPos = mul(unity_ObjectToWorld, v.vertex).xyz-(_WorldSpaceCameraPos+_CamPos);
				return o;
			}
			
			fixed4 frag(v2f i) : Color {
                fixed4 tex =  tex2D(_AnimTex, i.uv);
                fixed4 col;
                col.rgb = tex.rgb*(i.vertColor);
                col.a = tex.a;

				fixed2 mapUV = (i.offPos.xz+float2(_CameraSize, _CameraSize))/(2*_CameraSize);
				fixed3 lightColor = (_AmbientCol.rgb+tex2D(_LightMap, mapUV).rgb * (1-tex2D(_LightMask, mapUV).a)*_LightCoff );

				col.rgb *= lightColor;
				return col;
			}	
	        
	        ENDCG
		}
		
	} 
	FallBack "Diffuse"
}
