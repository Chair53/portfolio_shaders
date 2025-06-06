Shader "Custom Post-Processing/Tint"
{
	Properties {_MainTex("Main Texture", 2D) = "white" {}}

	SubShader
	{
		Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}

		Pass
		{
			HLSLPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

			sampler2D _MainTex;
			float _Intensity;
			float4 _OverlayColour;
			
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
				o.vertex = TransformObjectToHClip(v.vertex);
				o.uv = v.uv;
				return o;
			}
			float4 frag(v2f i): SV_Target
			{
				float4 col = tex2D(_MainTex, i.uv) * _OverlayColour;
				col.rgb *= _Intensity;
				return col;
			}

			ENDHLSL
		}
	}
}