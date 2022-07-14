Shader "MyShader/ShadowCasterShader"
{
    SubShader
    {
        // シャドウキャスター用パス
        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }

            ZWrite On
            ZTest LEqual
            //ZTest Less

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            // ShadowsCasterPass.hlsl に定義されているグローバルな変数
            float3 _LightDirection;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };



            v2f vert(appdata v)
            {



                UNITY_SETUP_INSTANCE_ID(v);
                v2f o;
                // ShadowsCasterPass.hlsl の GetShadowPositionHClip() を参考に
                float3 positionWS = TransformObjectToWorld(v.vertex.xyz);
                float3 normalWS = TransformObjectToWorldNormal(v.normal);
                float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));
#if UNITY_REVERSED_Z
                positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
#else
                positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
#endif
                o.pos = positionCS;


                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return 0.0;
            }


            ENDHLSL
        }

    }
}
