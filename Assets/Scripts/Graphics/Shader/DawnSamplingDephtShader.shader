Shader "MyShader/DawnSamplingDephtShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque" 
            "RenderPipeline" = "UniversalPipeline"
        }
        ZWrite Off

        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        float4 _CameraDepthTexture_TexelSize;

        CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
        CBUFFER_END

        ENDHLSL


        Pass
        {
            Name "DawnSamplingDepth"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag


            //static const int BLUR_SAMPLE_COUNT = 4;
            //static const float2 BLUR_KERNEL[BLUR_SAMPLE_COUNT] = {
            //    float2(-1.0, -1.0),
            //    float2(-1.0, 1.0),
            //    float2(1.0, -1.0),
            //    float2(1.0, 1.0),
            //};
            static const int BLUR_SAMPLE_COUNT = 8;
            static const float2 BLUR_KERNEL[BLUR_SAMPLE_COUNT] = {
                float2(-1.0, -1.0),
                float2(-1.0, 1.0),
                float2(1.0, -1.0),
                float2(1.0, 1.0),
                float2(-0.70711, 0),
                float2(0, 0.70711),
                float2(0.70711, 0),
                float2(0, -0.70711),
            };

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
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {

                float4 col = 0;

                //for (int j = 0; j < BLUR_SAMPLE_COUNT; j++) 
                //{
                //    col.x += SampleSceneDepth(i.uv + BLUR_KERNEL[j] * float2(_CameraDepthTexture_TexelSize.x, _CameraDepthTexture_TexelSize.y));
                //}

                //
                //col.x /= BLUR_SAMPLE_COUNT;

                col.x = SampleSceneDepth(i.uv);

                col.yzw = SampleSceneNormals(i.uv);


                return col;
            }
            ENDHLSL
        }
    }
}
