// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/shadowMap" {
	Properties {
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		Pass {
			CGPROGRAM
			#pragma vertex vert 
		    #pragma fragment frag
		    #include "UnityCG.cginc"

		    struct VertIn {
	        	float4 vertex : POSITION;
	        };
	        struct v2f {
	        	fixed4 pos : SV_POSITION;
	        	fixed4 screen : TEXCOORD1;
	        	fixed4 viewPos : TEXCOORD2;
	   		};

			//计算对象的世界坐标转化到主镜头的的屏幕坐标
			uniform float4x4 _MainCameraWorldToProj;

	        v2f vert(VertIn v) 
			{
				v2f o;

				fixed4 mvPos = mul(UNITY_MATRIX_MV, v.vertex);
				//o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				//使用MainCamera的配置MVP来将这个数据直接和MainCamera对应起来
				fixed4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.pos = mul(_MainCameraWorldToProj, worldPos);

				o.screen = ComputeScreenPos(o.pos);
				o.screen.z = mvPos.z ;
				o.viewPos = mvPos;
				return o;
			}

			fixed4 frag(v2f i) : Color {
				//return fixed4(i.screen.z, i.screen.z, i.screen.z, 1);
				//return fixed4(i.viewPos.x, i.viewPos.y, -i.viewPos.z, 1);
				//return fixed4(0, 0, -i.viewPos.z, 1);
				//return fixed4(_Z, 0, 0, 1);
				//近平面是0 远平面是1
				float depth = (-i.viewPos.z-_ProjectionParams.y)/(_ProjectionParams.z-_ProjectionParams.y); 
				//return fixed4(depth, depth, depth, 1);
				return EncodeFloatRGBA(depth);
				//return fixed4(0, 0, (-i.viewPos.z)* _ProjectionParams.w, 1);
			}
	        
		    ENDCG
		}

	} 
	FallBack "Diffuse"
}
