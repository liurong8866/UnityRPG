Shader "Custom/smokeTail" {
	
Properties {
_Color ("Main Color", Color) = (1,1,1,1)
_Ambient ("Ambient Color", Color) = (0.588, 0.588, 0.588, 1)
_Emissive ("Emissive", Color) = (1, 1, 1, 1)
_MainTex ("Base (RGB)", 2D) = "white" {}
_GradientTex ("Gradient (RGB)", 2D) = "white" {}
_UVAnimX ("UV Anim X", float) = 0
_UVAnimY ("UV Anim Y", float) = 0

}

SubShader {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True"
	"RenderType"="Transparent"}
	LOD 200
	
CGPROGRAM
#pragma surface surf Lambert alpha

sampler2D _MainTex;
fixed4 _Color;
fixed4 _Ambient;
fixed4 _Emissive;
fixed _UVAnimX;
fixed _UVAnimY;
sampler2D _GradientTex;


struct Input {
	float2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	float t1 = _Time.y*_UVAnimX;
	float t2 = _Time.y*_UVAnimY;
	
	float2 uvnew = IN.uv_MainTex+fixed2(t1, t2);
	
	
	fixed4 c = tex2D(_MainTex, uvnew) * _Color;
	
	fixed4 gradientColor = tex2D(_GradientTex, IN.uv_MainTex);
	
	o.Albedo = c.rgb*gradientColor.rgb;
	o.Alpha = c.a*gradientColor.a;
	o.Emission = c.rgb*gradientColor.rgb * _Ambient.rgb;
	
	
}
ENDCG
}


}
