// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "francoface/render UV" {

    SubShader {
        Tags { "RenderType"="Opaque" }

        Pass {

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct VertexInput {
                float4 vertex : POSITION;
                float4 texcoord : TEXCOORD;
            };

            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD;
            };

            VertexOutput vert (VertexInput v) {
                VertexOutput o;

                o.pos = UnityObjectToClipPos(v.vertex );
                o.uv = v.texcoord.xy;

                return o;
            }

            float4 frag(VertexOutput i) : COLOR {

                return float4(i.uv.y, i.uv.y, i.uv.y, 1);
            }

            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
