Shader "Custom/foregroundRain" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}

		_UVAnimX ("UV Anim X", float) = 0
		_UVAnimY ("UV Anim Y", float) = 0

		_scaleX("scaleX", float) = 1
		_scaleY("scaleY", float) = 1

		_Color ("RainColor", Color) = (1, 1, 1, 1)

		_scaleX1("scaleX", float) = 1
		_scaleY1("scaleY", float) = 1

		_UVAnimX1 ("UV Anim X", float) = 0
		_UVAnimY1 ("UV Anim Y", float) = 0

		_Color1 ("RainColor", Color) = (1, 1, 1, 1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" 
			"Queue"="Transparent+10" 
		  }

		Pass {
			LOD 200
			Blend One One  
			zwrite off
			cull off

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
	        	fixed2 uv1 : TEXCOORD1;
	        };

	        uniform sampler2D _MainTex; 
	        uniform fixed _UVAnimX;
	        uniform fixed _UVAnimY;

	        uniform fixed _scaleX;
	        uniform fixed _scaleY;
	        uniform fixed4 _Color;


	        uniform fixed _scaleX1;
	        uniform fixed _scaleY1;
	        uniform fixed _UVAnimX1;
	        uniform fixed _UVAnimY1;
	        uniform fixed4 _Color1;

	         
	        v2f vert(VertIn v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				fixed2 uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);

				float t1 = _Time.y*_UVAnimX;
				float t2 = _Time.y*_UVAnimY;
				o.uv = uv*fixed2(_scaleX, _scaleY);
				o.uv += fixed2(t1, t2);

				float t3 = _Time.y*_UVAnimX1;
				float t4 = _Time.y*_UVAnimY1;
				o.uv1 = uv*fixed2(_scaleX1, _scaleY1);
				o.uv1 += fixed2(t3, t4);
				return o;
			}

			fixed4 frag(v2f i) : Color {
                fixed4 tex0 = tex2D(_MainTex, i.uv);
                fixed4 col;
                col.rgb = tex0.rgb * _Color;
                col.a = tex0.a;

                fixed4 tex1 = tex2D(_MainTex, i.uv1);
                col.rgb += tex1.rgb*_Color1;

				return col;
			}
			
			ENDCG
		}


	} 
	FallBack "Diffuse"
}
