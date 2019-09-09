Shader "Custom/transparentAmb" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Ambient ("Ambient", Color) = (0.588, 0.588, 0.588, 1)
		_Color ("Main Color", Color) = (1, 1, 1, 1)
	}
	
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" 
			"RenderType"="Transparent" }
			
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert alpha

		sampler2D _MainTex;
		fixed4 _Ambient;
		fixed4 _Color;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex)*_Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			o.Emission = c.rgb * _Ambient.rgb;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
