Shader "MyShader/MargeSceneAndOutLineShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _OutLineTex("OutLineTex", 2D) = "white"{}

    }
        SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }
        ZWrite Off

        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_OutLineTex);
        SAMPLER(sampler_OutLineTex);
        float4 _CameraDepthTexture_TexelSize;

        CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
        CBUFFER_END

        ENDHLSL


        Pass
        {
            Name "MargeSceneAndOutLine"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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
                float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                float4 outLineCol = SAMPLE_TEXTURE2D(_OutLineTex, sampler_OutLineTex, i.uv);

                col.xyz = lerp(col.xyz, outLineCol.xyz, outLineCol.w);
                col.w = 1.0f;

                //col.xyz = float3(1.0f, 0.0f, 0.0f);
                return col;
            }
            ENDHLSL
        }
    }
}
