Shader "MyShader/ShadowCasterShader"
{
    Properties
    {
        [Toggle(IS_SHADOW_CASTER)] _IsShadowCaster("IsShadowCaster", int) = 1
    }
    SubShader
    {
        // �V���h�E�L���X�^�[�p�p�X
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

            // ShadowsCasterPass.hlsl �ɒ�`����Ă���O���[�o���ȕϐ�
            float3 _LightDirection;
            int _IsShadowCaster;

            // �V���h�E�p�̃g�O���̃V�F�[�_�[�L�[���[�h
            #pragma shader_feature IS_SHADOW_CASTER

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
#ifdef IS_SHADOW_CASTER
                v2f o;
                // ShadowsCasterPass.hlsl �� GetShadowPositionHClip() ���Q�l��
                float3 positionWS = TransformObjectToWorld(v.vertex.xyz);
                float3 normalWS = TransformObjectToWorldNormal(v.normal);
                float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));
    #if UNITY_REVERSED_Z
                positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
    #else
                positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
    #endif
                o.pos = positionCS;
#else
                // �V���h�E�L���X�^�[���I�t�Ȃ�A�e���o�Ȃ��悤�ɉ�ʊO�̍��W���w��B
                v2f o;
                o.pos = float4(-100.0f, -100.0f, 0.0f, 0.0f);
#endif

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
