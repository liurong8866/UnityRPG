Shader "Custom/newBurnShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_cloud ("cloud", 2D) = "white" {}
		_timeLerp ("timeLerp", Range(0, 1)) = 0
		_BurnColor ("bur color", Color) = (1, 1, 1, 1)
	}
	
	SubShader {
		Tags { "Queue"="Transparent" }
        
        Pass {
        	Tags {"LightMode" = "Vertex"}
			Name "Base"
			LOD 200
			lighting off
            Blend SrcAlpha OneMinusSrcAlpha
            //zwrite off
            //Fog {Mode Off}
            CGPROGRAM
            #pragma multi_compile_fwdbase
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            
            
            uniform sampler2D _MainTex;
            uniform sampler2D _cloud; 
            uniform float _timeLerp;
            uniform float4 _BurnColor;
            
            struct VertexInput {
                float4 vertex : POSITION;
                float4 texcoord : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                fixed2 uv : TEXCOORD0;
            };
            
             VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
                return o;
            }
            float lp(float a, float b, float t) {
            	return (b-a)*t+a;
            }
            
            float cp(float v, float s, float e) {
            	return min(max(v, s), e);
            }
            fixed4 frag(VertexOutput i) : COLOR {    
                float node_238 = cp(tex2D(_cloud, i.uv).r, 0.0, 0.7);
                float av = cp(pow(node_238, lp(0,7,_timeLerp)), 0.0, 1.0);

                float4 node_2 = tex2D(_MainTex, i.uv);
                node_2.a = av;
                //node_2.rgb *= _BurnColor*av;
				return node_2;
            }
            ENDCG
        }
        
        
	} 
	
	FallBack "Diffuse"
}
