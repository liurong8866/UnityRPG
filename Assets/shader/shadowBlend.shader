Shader "Custom/shadowBlend" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_ShadowMap ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
	
		Pass {
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
	        	//fixed4 screen : TEXCOORD1;
	   		};
	        uniform sampler2D _MainTex;
	        uniform sampler2D _ShadowMap;

	        uniform sampler2D _CameraDepthTexture;


	        v2f vert(VertIn v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				//o.screen = ComputeScreenPos(o.pos);
				return o;
			}

			fixed4 frag(v2f i) : Color {
				fixed4 col = tex2D(_MainTex, i.uv);
				//屏幕坐标转化到主镜头世界坐标 * 10 明显
				fixed shadowDepth = DecodeFloatRGBA(tex2D(_ShadowMap, i.uv))*10;
				//fixed shadowDepth = DecodeFloatRGBA(tex2D(_ShadowMap, i.uv))*10;
				//fixed d = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screen));

				//fixed d = i.uv.x;
				//fixed d = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
				//d = LinearEyeDepth(d);

				//从主镜头的DepthTexture中获取的数据需要转化到01范围才能和我存储的比较
				//fixed mainDepth = (d-_ProjectionParams.y)/(_ProjectionParams.z-_ProjectionParams.y); 
				//直接可以比较
				//clip(d-shadowDepth+1e-2f);
				//return fixed4(1, 1, 1, 1);
				//场景深度信息
				//return fixed4(mainDepth, mainDepth, mainDepth, 1);
				//return fixed4(d, d, d, 1);
				//return fixed4(i.uv, 0, 1);
				//return fixed4(i.screen.xy, 0, 1);

				fixed d2 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
				d2 = Linear01Depth(d2)*10;
				//d2 = Linear01Depth(d2)*10; //深度效果明显

				//fixed mainDepth = (d2-_ProjectionParams.y)/(_ProjectionParams.z-_ProjectionParams.y); 
				//return fixed4(mainDepth, mainDepth, mainDepth, 1);

				//return fixed4(d2, d2, d2, 1);
				return fixed4(shadowDepth, shadowDepth, shadowDepth, 1);
				//clip();

				clip(d2-shadowDepth+1e-2f);

				return col;

			}
	        
		    ENDCG
		}

	} 
	FallBack "Diffuse"
}
