Shader "Custom/lightMapGoldShine" {
	Properties {
		_Color("Main Color", Color) = (1, 1, 1, 1)
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		_ShineTex("Shine Tex", 2D) = "white" {}
		_SpecTex("Spec Tex", 2D) = "white" {}
		_UVAnimX ("UV Anim X", float) = 0
		_UVAnimY ("UV Anim Y", float) = 0

	}
	
	
	SubShader {
		Pass {
			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
			Name "Base"
			LOD 200
			Blend SrcAlpha OneMinusSrcAlpha
			Material {
				//Ambient [_Color]
				//Diffuse [_Color]
				Emission [_Color]
				//Specular [Color.blank]
			}
			Lighting On
			SetTexture [_MainTex] {
				combine texture*primary, texture * primary
			}
		}
		
		Pass {
			Tags { "Queue"="Transparent" "IgnoreProjector"="True"  }
			//Blend srcalpha one, zero zero
			
			Blend SrcAlpha one, zero zero
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			struct v2f {
	        	fixed4 pos : SV_POSITION;
	        	fixed2 uv : TEXCOORD0;
	        	fixed2 uv2 : TEXCOORD1;
	        };
	        
	        uniform sampler2D _ShineTex;
	        uniform sampler2D _SpecTex; 
	        uniform fixed _UVAnimX;
	        uniform fixed _UVAnimY;
	        v2f vert(appdata_base v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				
				float t1 = _Time.y*_UVAnimX;
				float t2 = _Time.y*_UVAnimY;
				o.uv2 = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				o.uv2 += fixed2(t1, t2);
				return o;
			}
			
			
			fixed4 frag(v2f i) : Color {
				return tex2D(_ShineTex, i.uv2)*tex2D(_SpecTex, i.uv);
			}
			ENDCG	
		}
		
		/*
		Pass {
			Tags { "Queue"="Transparent" "IgnoreProjector"="True"  }
			Blend srcalpha one, zero zero
			Material {
				//Ambient [Color.blank]
				//Diffuse [Color.blank]
				//Emission [_Color]
			}
			Lighting Off
			SetTexture [_ShineTex] {
				
				combine texture //*previous
			}
			
			SetTexture [_SpecTex] {
				combine texture * previous
			}
			
			
		}
		*/
	} 
	FallBack "Diffuse"
}
