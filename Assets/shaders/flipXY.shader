Shader "Custom/flipXY" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			float2 newUV = float2(1-IN.uv_MainTex.x, 1-IN.uv_MainTex.y);
			half4 c = tex2D (_MainTex, newUV);
			o.Emission = c.rgb;
			//o.Albedo = c.rgb;
			//o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
