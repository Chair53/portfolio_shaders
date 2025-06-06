Shader "Custom Post-Processing/Outline"
{
    Properties
    {
        _Thickness ("Outline Thickness", Float) = 1
        _Colour ("Outline Color", Color) = (0, 0, 0, 1)
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType"="Opaque"
        }

        ZWrite Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass 
        {
            Name "EDGE DETECTION OUTLINE"
            
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl" // needed to sample scene depth
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl" // needed to sample scene normals
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl" // needed to sample scene color/luminance

            float _Thickness;
            float4 _Colour;
            float3 _Sensitivities;

            #pragma vertex Vert
            #pragma fragment frag

            //use 4 corner samples
            float RobertsCross(float3 samples[4])
            {
                const float3 difference_1 = samples[1] - samples[2];
                const float3 difference_2 = samples[0] - samples[3];
                return sqrt(dot(difference_1, difference_1) + dot(difference_2, difference_2));
            }

            float RobertsCross(float samples[4])
            {
                const float difference_1 = samples[1] - samples[2];
                const float difference_2 = samples[0] - samples[3];
                return sqrt(difference_1 * difference_1 + difference_2 * difference_2);
            }
            
            //normals remapped from [-1, 1] -> [0, 1].
            float3 SampleSceneNormalsRemapped(float2 uv)
            {
                return SampleSceneNormals(uv) * 0.5 + 0.5;
            }

            float SampleSceneLuminance(float2 uv)
            {
                float3 color = SampleSceneColor(uv);
                return color.r * 0.3 + color.g * 0.59 + color.b * 0.11;
            }

            float4 frag(Varyings i) : SV_Target
            {
                float2 uv = i.texcoord;
                float2 texel_size = float2(1.0 / _ScreenParams.x, 1.0 / _ScreenParams.y);
                float half_width_f = floor(_Thickness * 0.5);
                float half_width_c = ceil(_Thickness * 0.5);
                float2 uvs[4];
                uvs[0] = uv + texel_size * float2(half_width_f, half_width_c) * float2(-1, 1);  // top left
                uvs[1] = uv + texel_size * float2(half_width_c, half_width_c) * float2(1, 1);   // top right
                uvs[2] = uv + texel_size * float2(half_width_f, half_width_f) * float2(-1, -1); // bottom left
                uvs[3] = uv + texel_size * float2(half_width_c, half_width_f) * float2(1, -1);  // bottom right
                float3 normal_samples[4];
                float depth_samples[4], luminance_samples[4];
                
                for (int offset = 0; offset < 4; offset++) {
                    depth_samples[offset] = SampleSceneDepth(uvs[offset]);
                    normal_samples[offset] = SampleSceneNormalsRemapped(uvs[offset]);
                    luminance_samples[offset] = SampleSceneLuminance(uvs[offset]);
                }
                float edge_depth = RobertsCross(depth_samples);
                float edge_normal = RobertsCross(normal_samples);
                float edge_luminance = RobertsCross(luminance_samples);
                
                float depth_threshold = 1 / _Sensitivities.x;
                edge_depth = edge_depth > depth_threshold ? 1 : 0;
                float normal_threshold = 1 / _Sensitivities.y;
                edge_normal = edge_normal > normal_threshold ? 1 : 0;
                float luminance_threshold = 1 / _Sensitivities.z;
                edge_luminance = edge_luminance > luminance_threshold ? 1 : 0;
                
                float isEdge = max(edge_depth, max(edge_normal, edge_luminance));
                return isEdge * _Colour;
            }
            ENDHLSL
        }
    }
}
