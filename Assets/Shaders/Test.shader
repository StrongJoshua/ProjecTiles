Shader "Unlit/Test"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_TintColor ("Tint", Color) = (0,0,0,0)
		_Alpha ("Alpha", Range(0.0, 1)) = 0.25
		_Cutout ("Cutout Threshold", Range(0.0, 1)) = 0.25
	}
	SubShader
	{
		Tags { "Queue"="Transparent"	"RenderType"="Transparent" }
		LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _TintColor;
			float _Alpha;
			float _Cutout;

			float rand(float3 co)
			{
				return frac(sin( dot(co.xyz ,float3(12.9898,78.233,45.5432) )) * 43758.5453);
			}

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.vertex.x *= 10;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) + _TintColor;
				col.a = _Alpha;
				clip(col.r - _Cutout); // get rid of pixels with not that much red equivalent to: if (col.r < _Cutout) discard;
				return col;
			}
			ENDCG
		}
	}
}
