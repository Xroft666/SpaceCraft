Shader "Custom/UIWithImageEffects" {
	Properties {
		_MainTex ("Main Texture", 2D) = "black" {}
		_OverlayTex ("Occlusion Map", 2D) = "black" {}
	}
	
	SubShader {

		ZTest Always Cull Off ZWrite Off Fog { Mode Off }

		Pass {
			CGPROGRAM
				#pragma vertex vert_img
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#include "UnityCG.cginc"
			
				sampler2D _MainTex;
				sampler2D _OverlayTex;

				fixed4 frag(v2f_img IN) : COLOR 
				{
					return tex2D (_MainTex, IN.uv) + tex2D(_OverlayTex, IN.uv);
				}
			ENDCG
		}
	} 
}
