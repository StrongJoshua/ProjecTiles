// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

 Shader "Custom/Explosion" {
     Properties {
         _Color ("Text Color", Color) = (1,1,1,1)
     }
 
     SubShader {
         Tags 
         {
             "RenderType"="Opaque"
         }
		 CGPROGRAM
		 #pragma surface surf Lambert

		 struct Input {
			float2 uv_MainTex;
		 };

		 
		 void surf(Input IN, inout SurfaceOutput o)
		 {
			o.Albedo = vert3(_Color, _Color, _Color);
		 }
		 ENDCG
     }
 }