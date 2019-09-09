Shader "Custom/sunkenWave" {
	Properties {

		_WaveTex1("Animation Texture Pass", 2D) = "white" {}	
		_UVAnimX1 ("UV Anim X", float) = 0
		_UVAnimY1 ("UV Anim Y", float) = 0

		_WaveTex2("Animation Texture Pass", 2D) = "white" {}	
		_UVAnimX2 ("UV Anim X", float) = 0
		_UVAnimY2 ("UV Anim Y", float) = 0

		_Mask("Animation Texture Pass", 2D) = "white" {}	
	}

	SubShader {
		Tags { "Queue"="Transparent" 
				"IgnoreProjector"="True"
				"RenderType"="Transparent" }

		Pass {
			LOD 200
			Blend SrcAlpha OneMinusSrcAlpha 
			zwrite off

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
	        	fixed2 uv2 : TEXCOORD2;
	        };

	        uniform sampler2D _WaveTex1; 
	        uniform sampler2D _WaveTex2;
	        uniform sampler2D _Mask;
	         
	        uniform fixed _UVAnimX1;
	        uniform fixed _UVAnimY1;

	        uniform fixed _UVAnimX2;
	        uniform fixed _UVAnimY2;



	        v2f vert(VertIn v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				
				float t1 = _Time.y*_UVAnimX1;
				float t2 = _Time.y*_UVAnimY1;
	
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				o.uv1 = o.uv + fixed2(t1, t2);

				float t3 = _Time.y*_UVAnimX2;
				float t4 = _Time.y*_UVAnimY2;
				o.uv2 = o.uv + fixed2(t3, t4);

				return o;
			}

			fixed4 frag(v2f i) : Color {
                fixed4 tex1 = tex2D(_WaveTex1, i.uv1);
                fixed4 tex2 = tex2D(_WaveTex2, i.uv2);
                fixed4 mask = tex2D(_Mask, i.uv);

                fixed4 col;
                fixed alpha = tex1.a;
                col.rgb = tex1.rgb+tex2.rgb*(1-alpha);
                col.rgb = col.rgb*mask.rgb;

                col.a = tex1.a*mask.a;

				return col;
			}
			
			ENDCG
		}
	}
	 
	FallBack "Diffuse"
}
