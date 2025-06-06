Shader "Custom Post-Processing/Gaussian Blur"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white"{}
		_Spread("Standard Deviation", Float) = 0
		_GridSize("Grid Size", Integer) = 1
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always
		Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}

		HLSLINCLUDE
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		#define E 2.71828f

		sampler2D _MainTex;
		float4 _MainTex_TexelSize;
		uint _GridSize;
		float _Spread;

		float gaussian(int x)
		{
			//function for blurring
			float sigmaSq = _Spread * _Spread;
			return (1.0 / sqrt(TWO_PI * sigmaSq)) * pow(E, -(x * x) / (2 * sigmaSq));
		}

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

		v2f vert(appdata v)
		{
			v2f o;
			o.vertex = TransformObjectToHClip(v.vertex.xyz);
			o.uv = v.uv;
			return o;
		}
		ENDHLSL

		//horizontal blur
		Pass
		{
			Name "Horizontal"

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag_horizontal

			float4 frag_horizontal(v2f i) : SV_Target
			{
				float3 col = float3(0.0f, 0.0f, 0.0f);
				float gridSum = 0.0f;
				int upper= ((_GridSize - 1) / 2);
				int lower = -upper;

				//blur horizontally
				for(int x = lower; x <= upper; x++)
				{
					float gaus = gaussian(x);
					gridSum += gaus;
					float2 uv = i.uv + float2(_MainTex_TexelSize.x * x, 0.0f);
					col += gaus * tex2D(_MainTex, uv).xyz;
				}
				col /= gridSum;
				return float4(col, 1.0f);
			}
			ENDHLSL
		}

		//vertical blur
		Pass
		{
			Name "Vertical"

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag_vertical

			float4 frag_vertical(v2f i) : SV_Target
			{
				float3 col = float3(0.0f, 0.0f, 0.0f);
				float gridSum = 0.0f;
				int upper= ((_GridSize - 1) / 2);
				int lower = -upper;

				//blur vertically
				for(int y = lower; y <= upper; y++)
				{
					float gaus = gaussian(y);
					gridSum += gaus;
					float2 uv = i.uv + float2(0.0f, _MainTex_TexelSize.y * y);
					col += gaus * tex2D(_MainTex, uv).xyz;
				}
				col /= gridSum;
				return float4(col, 1.0f);
			}
			ENDHLSL
		}
	}
}