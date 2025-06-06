Shader "Custom Post-Processing/Colour Effect"
{
	Properties {_MainTex("Main Texture", 2D) = "white" {}}

	SubShader
	{
		Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}

		Pass
		{
			HLSLPROGRAM
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
			#pragma vertex vert
			#pragma fragment frag

			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);
			float _Intensity;
			float4 _OverlayColour;
			int _Effect;

			struct appData
			{
				float4 vertex       : POSITION;
				float2 uv               : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv        : TEXCOORD0;
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_OUTPUT_STEREO
			};


			v2f vert(appData i)
			{
				v2f o = (v2f)0;
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				VertexPositionInputs vertexInput = GetVertexPositionInputs(i.vertex.xyz);
				o.vertex = vertexInput.positionCS;
				o.uv = i.uv;

				return o;
			}


			float4 frag (v2f i) : SV_Target
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				float4 background = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

				switch(_Effect)
				{
					case(0):
					background = background * _OverlayColour * _Intensity;
					break;
					case(1):
					background = 1 - (1-saturate(background)) /  _OverlayColour * _Intensity;
					break;
					case(2):
					background = _OverlayColour * _Intensity * background - 1;
					break;
					case(3):
					background = 1 - (1-_OverlayColour * _Intensity) * (1 -background);
					break;
					case(4):
					background = background / (1-_OverlayColour * _Intensity);
					break;
					case(5):
					background = background + _OverlayColour * _Intensity;
					break;
					default:
					break;

				}
				return (background);
			}

			ENDHLSL
		}
	}
	Fallback "Diffuse"
}