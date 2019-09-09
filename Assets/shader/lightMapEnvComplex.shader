// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/lightMapEnvComplex" {
	//floor blank
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
	
		Pass {	
			LOD 200
			Lighting Off
			
			
			CGPROGRAM
			#pragma vertex vert 
		    #pragma fragment frag
		    #include "UnityCG.cginc"
		        
	        struct VertIn {
	        	float4 vertex : POSITION;
	        	float4 texcoord : TEXCOORD0;
	        	float3 normal : NORMAL;
	        };

			
			struct v2f {
	        	fixed4 pos : SV_POSITION;
	        	fixed2 uv : TEXCOORD0;
	   			fixed3 offPos : TEXCOORD2;
	   			fixed3 worldPos : TEXCOORD3;
	   			fixed3 noisePos : TEXCOORD4;
	   			//fixed3 rimPower : TEXCOORD5;
	   			fixed3 shadowPos : TEXCOORD6;
	   			fixed3 specPower : TEXCOORD7;
	        };

			uniform sampler2D _MainTex;
			uniform sampler2D _LightMap;
			uniform sampler2D _SpecMap;
			uniform float _SpecCoff;
			uniform float _SpecSize;

		    uniform float4 _CamPos;
		    uniform float _CameraSize;

		    uniform float4 _AmbientCol;
			
			uniform sampler2D _LightMask;
			uniform float _LightCoff;

			uniform float _SpecFreqX;
			uniform float _SpecFreqY;
			uniform float _SpecAmpX;
			uniform float _SpecAmpY;

			uniform sampler2D _CloudNoise;

			uniform fixed4 _RimColor2;
			uniform fixed _RimPower2;

			uniform sampler2D _ShadowMap;
		    uniform float4 _ShadowCamPos;
		    uniform float _ShadowCameraSize;

	        uniform fixed3 _LightDir;
	        uniform fixed _Shinness;
	        uniform fixed4 _SpecColor;
	        uniform fixed _NoiseScale;

			v2f vert(VertIn v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);

				fixed3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.offPos = worldPos-(_WorldSpaceCameraPos+_CamPos);
				o.shadowPos = worldPos-(_WorldSpaceCameraPos+_ShadowCamPos);

				fixed t1 = sin(_Time.y*_SpecFreqX*6.28)*_SpecAmpX;
				fixed t2 = sin(_Time.y*_SpecFreqY*6.28)*_SpecAmpY;
				o.worldPos = worldPos+fixed3(t1, 0, t2);
				o.noisePos = worldPos*_NoiseScale;

				/*
				fixed3 viewDir = ObjSpaceViewDir(v.vertex);
				half rim = 1.0 - saturate(dot (normalize(viewDir), v.normal));
	          	o.rimPower = _RimColor2.rgb * pow (rim, _RimPower2);
	          	*/

	            fixed3 lightDirection = normalize(_LightDir);
	            fixed3 reflectDir = reflect(lightDirection, v.normal);
	            fixed3 viewDir = normalize(_WorldSpaceCameraPos-worldPos);
	            fixed cosAngle = max(0.0, dot(viewDir, reflectDir));
	            fixed specCoff = pow(cosAngle, _Shinness);
	            o.specPower = specCoff;

				return o;
			}
			
			fixed4 frag(v2f i) : Color {
	        	fixed4 col =  tex2D(_MainTex, i.uv);
	        	fixed4 retCol;
				
				fixed2 mapUV = (i.offPos.xz+float2(_CameraSize, _CameraSize))/(2*_CameraSize);

				retCol.rgb = col.rgb*(_AmbientCol.rgb+tex2D(_LightMap, mapUV).rgb * (1-tex2D(_LightMask, mapUV).a)*_LightCoff );
				retCol.a = col.a;

				fixed2 specUV = i.worldPos.xz * fixed2(_SpecSize, _SpecSize)+tex2D(_CloudNoise, i.noisePos).rg;
				fixed4 specColor = tex2D(_SpecMap, specUV);

				retCol.rgb += specColor.rgb*_SpecCoff+i.specPower*_SpecColor*tex2D(_CloudNoise, i.noisePos);//+ i.rimPower*tex2D(_CloudNoise, i.noisePos);

				fixed2 mapShadow = (i.shadowPos.xz+float2(_ShadowCameraSize, _ShadowCameraSize))/(2*_ShadowCameraSize);
				fixed4 shadowC = tex2D(_ShadowMap, mapShadow);
				retCol.rgb = retCol.rgb*(1-shadowC.a)+shadowC.rgb;
				return retCol;
			}	

			ENDCG
		}
	} 
	FallBack "Diffuse"
}
