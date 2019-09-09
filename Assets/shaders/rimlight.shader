Shader "Custom/rimlight" {
	Properties {
        _RimColor ("Rim Color", Color) = (0.26, 0.19, 0.16, 0.0)
        _RimPower ("Rim Power", Range(0.5, 8.0)) = 3.0
	}
	
	SubShader {
		Tags { "RenderType" = "Opaque" }
		Pass {
			Name "BASE"
			 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			uniform fixed4 _RimColor;
			uniform fixed _RimPower;
			struct v2f {
				fixed4 pos : SV_POSITION;
				fixed3 diff;
			};
			
			v2f vert(appdata_base v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				float3 viewDir = ObjSpaceViewDir(v.vertex);
				half rim = 1.0 - saturate(dot (normalize(viewDir), v.normal));
	          	o.diff = _RimColor.rgb * pow (rim, _RimPower);
	          	return o;
			}
			fixed4 frag(v2f i) : COLOR {
				return fixed4(i.diff, 1.0);
			}
			
			ENDCG
		}	
		 
	} 
	FallBack "Diffuse"
}
