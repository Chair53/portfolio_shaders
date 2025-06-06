Shader "Custom Post-Processing/Pixel"
{
	Properties{_MainTex ("Texture", 2D) = "white"}
	
	SubShader
	{
		Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}

		HLSLINCLUDE
		#pragma vertex vert
		#pragma fragment frag
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

		struct appData
		{
			float4 vertex       : POSITION;
			float2 uv               : TEXCOORD0;
		};

		struct v2f
		{
			float2 uv        : TEXCOORD0;
			float4 vertex : SV_POSITION;
		};

		TEXTURE2D(_MainTex);
		float4 _MainTex_TexelSize;
		float4 _MainTex_ST;

		SamplerState sampler_point_clamp;

		uniform float2 _BlockCount;
		uniform float2 _BlockSize;
		uniform float2 _HalfBlockSize;

		v2f vert(appData i)
		{
			v2f o;
			o.vertex = TransformObjectToHClip(i.vertex.xyz);
			o.uv = TRANSFORM_TEX(i.uv, _MainTex);
			return o;
		}

		ENDHLSL

		Pass
		{
			Name "Pixel"
			HLSLPROGRAM

			float4 frag(v2f i) : SV_Target
			{
				float2 pos = floor(i.uv * _BlockCount);
				float2 center = pos * _BlockSize + _HalfBlockSize;
				float4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_point_clamp, center);
				return tex;
			}
			ENDHLSL
		}
	}
}