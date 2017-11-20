// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

 Shader "Custom/Explosion" {
     Properties {
         _Color ("_Color", Color) = (1,1,1,1)
     }

    SubShader {
		CGPROGRAM
		#pragma surface surf Lambert
		struct Input {
			fixed4 _Color;
		};

		fixed4 _Color;

		void surf(Input IN, inout SurfaceOutput o) {
			o.Albedo = _Color;
		}

		ENDCG
    }
}