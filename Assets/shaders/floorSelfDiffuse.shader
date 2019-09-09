Shader "Custom/floorSelfDiffuse" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		fixed4 _Color;

		struct Input {
			float2 uv_MainTex;
			float2 uv_Illum;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 c = tex ;
			o.Albedo = c.rgb;
			o.Emission = c.rgb* UNITY_LIGHTMODEL_AMBIENT;//*_Color;//;
			//o.Emission = c.rgb/3;
			o.Alpha = c.a;
		}
		ENDCG
		
	} 
	FallBack "Diffuse"
}
