Shader "Custom/transparentScroll" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_UVAnimX ("UV Anim X", float) = 0
		_UVAnimY ("UV Anim Y", float) = 0
		
	}
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 200
		Blend SrcAlpha One 
		Lighting Off
		
		CGPROGRAM
		#pragma surface surf Lambert 

		sampler2D _MainTex;
		float _UVAnimX;
		float _UVAnimY;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			float t1 = _Time.y*_UVAnimX;
			float t2 = _Time.y*_UVAnimY;
			float2 uv = IN.uv_MainTex + float2(t1, t2);	
			half4 c = tex2D (_MainTex, uv);
			//o.Albedo = c.rgb;
		
			o.Alpha = c.a;
			o.Emission = c.rgb*0.5f;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
