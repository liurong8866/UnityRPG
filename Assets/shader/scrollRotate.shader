// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/scrollRotate" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_IllumMap ("Illum Map", 2D) = "white" {}
		_UVAnimX ("UV Anim X", float) = 0
		_UVAnimY ("UV Anim Y", float) = 0
		_RotateAnim ("Rotate Anim", float) = 0
		
		_IllUVAnimX ("Illum UV Anim X", float) = 0
		_IllUVAnimY ("Illum UV Anim Y", float) = 0
		_RotateAnim2 ("Rotate Anim2", float) = 0
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
	        };
	        
	        struct v2f {
	        	fixed4 pos : SV_POSITION;
	        	fixed2 uv : TEXCOORD0;
	        	fixed3 offPos : TEXCOORD2;
	        	fixed2 uv2 : TEXCOORD1;
	        };
			
			
			uniform sampler2D _LightMap;
		    uniform float4 _CamPos;
		    uniform float _CameraSize;
		    
		    uniform float4 _AmbientCol;
			
			uniform sampler2D _LightMask;
			uniform float _LightCoff;
			
			
	        uniform sampler2D _MainTex;  
	        uniform sampler2D _IllumMap;
	        uniform fixed _UVAnimX;
	        uniform fixed _UVAnimY;
	        
	        uniform fixed _IllUVAnimX;
	        uniform fixed _IllUVAnimY; 
	        
	        uniform fixed _RotateAnim;
	        uniform fixed _RotateAnim2;
	        
	        v2f vert(VertIn v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				
				float t1 = _Time.y*_UVAnimX;
				float t2 = _Time.y*_UVAnimY;
	
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				o.uv += fixed2(t1, t2);
				
				float sinX = sin(_RotateAnim*_Time.y);
				float cosX = cos(_RotateAnim*_Time.y);
				//-1 ~ 1  0-1 rotate UV 
				float2x2 rotationMatrix = float2x2(cosX*0.5f+0.5f, -sinX*0.5f+0.5f, sinX*0.5f+0.5f, cosX*0.5f+0.5f);
				o.uv = mul(o.uv, rotationMatrix);
				
				
				float t3 = _Time.y*_IllUVAnimX;
				float t4 = _Time.y*_IllUVAnimY;
				o.uv2 = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				o.uv2 += fixed2(t3, t4);
				
				float sin2 = sin(_RotateAnim2*_Time.y);
				float cos2 = cos(_RotateAnim2*_Time.y);
				float2x2 rotationMatrix2 = float2x2(cos2*0.5f+0.5f, -sin2*0.5f+0.5f, sin2*0.5f+0.5f, cos2*0.5f+0.5f);
				//rotationMatrix2 *= 0.5f;
				//rotationMatrix2 += 0.5f;
				o.uv2 = mul(o.uv2, rotationMatrix2);
				
				o.offPos = mul(unity_ObjectToWorld, v.vertex).xyz-(_WorldSpaceCameraPos+_CamPos);
				return o;
			}
			
			
			fixed4 frag(v2f i) : Color {
				fixed4 col =  tex2D(_MainTex, i.uv);
				fixed4 retCol;
	        	
				fixed2 mapUV = (i.offPos.xz+float2(_CameraSize, _CameraSize))/(2*_CameraSize);
				fixed4 illumCol = tex2D(_IllumMap, i.uv2);
				retCol.rgb = col.rgb*(_AmbientCol.rgb+tex2D(_LightMap, mapUV).rgb * (1-tex2D(_LightMask, mapUV).a)*_LightCoff )+illumCol.rgb;
				
				retCol.a = col.a;
				
				return retCol;
			}	
	        
	        ENDCG
		}
	} 
	FallBack "Diffuse"
}
