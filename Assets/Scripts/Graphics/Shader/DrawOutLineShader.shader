Shader "MyShader/DrawOutLineShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor("OutlineColor", Color) = (0.0,0.0,0.0,1.0)
        _HowToDrawOutline("HowToDrawOutline", int) = 3
        _OutlineThick("OutlineThick", float) = 1.0
        _OutlineThreshold("OutlineThreshold", float) = 0.0001
        _HowToDrawOutline("HowToDrawOutline", int) = 3
    }
    SubShader
    {
        Tags 
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }
        ZWrite Off

        // �ePass��cbuffer���ς��Ȃ��悤�ɂ����ɒ�`����
        HLSLINCLUDE
        
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"

        
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);

        //TEXTURE2D(_CameraDepthTexture);
        //SAMPLER(sampler_CameraDepthTexture);
        float4 _CameraDepthTexture_TexelSize;


        CBUFFER_START(UnityPerMaterial)
            float4 _OutlineColor;
            float4 _MainTex_ST;
            float _OutlineThick;
            float _OutlineThreshold;
            int _HowToDrawOutline;
        CBUFFER_END

        ENDHLSL



        Pass
        {
            Name "DrawOutLine"

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


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            float MyLinearDepth(float z)
            {
                return 1.0 / (_ZBufferParams.x * z + _ZBufferParams.y);
            }

            bool IsDepthOutlineBy4Texel(float2 uv)
            {
                float diffX = _CameraDepthTexture_TexelSize.x * _OutlineThick;
                float diffY = _CameraDepthTexture_TexelSize.y * _OutlineThick;

                float col00 = MyLinearDepth(SampleSceneDepth(uv + half2(-diffX, -diffY)));
                float col10 = MyLinearDepth(SampleSceneDepth(uv + half2(0, -diffY)));
                float col01 = MyLinearDepth(SampleSceneDepth(uv + half2(-diffX, 0)));
                float col11 = MyLinearDepth(SampleSceneDepth(uv + half2(0, 0)));

                float outlineValue = (col00 - col11) * (col00 - col11) + (col10 - col01) * (col10 - col01);

                bool isOutline = false;
                if (outlineValue - _OutlineThreshold > 0.0f)
                {
                    isOutline = true;
                }

                return isOutline;
            }

            bool IsDepthOutlineBy8Texel(float2 uv)
            {
                float diffX = _CameraDepthTexture_TexelSize.x * _OutlineThick;
                float diffY = _CameraDepthTexture_TexelSize.y * _OutlineThick;

                float currentDepth = MyLinearDepth(SampleSceneDepth(uv));

                float nearDepth = 0.0f;
                nearDepth += MyLinearDepth(SampleSceneDepth(uv + half2(0.0f, diffY)));
                nearDepth += MyLinearDepth(SampleSceneDepth(uv + half2(0.0f, -diffY)));
                nearDepth += MyLinearDepth(SampleSceneDepth(uv + half2(diffX, 0.0f)));
                nearDepth += MyLinearDepth(SampleSceneDepth(uv + half2(-diffX, 0.0f)));
                nearDepth += MyLinearDepth(SampleSceneDepth(uv + half2(diffX, diffY)));
                nearDepth += MyLinearDepth(SampleSceneDepth(uv + half2(diffX, -diffY)));
                nearDepth += MyLinearDepth(SampleSceneDepth(uv + half2(-diffX, diffY)));
                nearDepth += MyLinearDepth(SampleSceneDepth(uv + half2(-diffX, -diffY)));

                nearDepth /= 8.0f;


                bool isOutline = false;
                if (abs(currentDepth - nearDepth) > _OutlineThreshold)
                {
                    isOutline = true;
                }

                return isOutline;
            }

            bool IsNormalOutline(float2 uv)
            {
                float diffX = _CameraDepthTexture_TexelSize.x * _OutlineThick;
                float diffY = _CameraDepthTexture_TexelSize.y * _OutlineThick;

                float2 uvOffset[8] =
                {
                    float2(0.0f, diffY),
                    float2(0.0f, -diffY),
                    float2(diffX, 0.0f),
                    float2(-diffX, 0.0f),
                    float2(diffX, diffY),
                    float2(-diffX, diffY),
                    float2(diffX, -diffY),
                    float2(-diffX, -diffY),
                };

                float3 currentNormal = SampleSceneNormals(uv);

                float3 nearNormal = float3(0.0f, 0.0f, 0.0f);

                // �x����
                // gradient instruction used in a loop with varying iteration, attempting to unroll the loop
                // �i��j�O���f�B�G���g�i���z�j���߂����[�v���Ŏg�p����A�������ω����A���[�v�̃A�����[�������݂��܂����B

                // ��̌x����������邽�߂�[unroll]����ꂽ�B
                // �O���f�B�G���g���߂Ƃ́A�g�p����Ă�����@�ȂǁA�g�p�����~�b�v���x����
                // �����Ō��肷�邷�ׂẴe�N�X�`���T���v�����O���@�A�炵���B
                // �O���f�B�G���g���߂͓��I������ł͋@�\���Ȃ����߁A�ÓI�ɂ���K�v������B
                // ���̂��߃��[�v��unroll�œW�J����K�v������A�A�̂��ȁH

                // ���͎Q�l�ɂ����y�[�W�B
                // https://stackoverflow.com/questions/56581141/direct3d11-gradient-instruction-used-in-a-loop-with-varying-iteration-forcing

                [unroll]
                for (int i = 0; i < 8; i++)
                {
                    nearNormal = SampleSceneNormals(uv + uvOffset[i]);

                    if (dot(float3(111.0f,111.0f,111.0f), nearNormal) == 0.0f)
                    {
                        // �[���x�N�g���ƃx�N�g���̓��ς�0.0�ɂȂ�B
                        // �@�����`�悳��Ă��Ȃ��Ƃ��낪�A�E�g���C�����肳��邱�Ƃ�����邽�߁A
                        // currentNoraml�Ɣ��Ȃ��x�N�g���ƁAnearNormal�̓��ς��Ƃ��āA
                        // �[���x�N�g����T���A�@�����`�悳��Ă��Ȃ����ߏ������X�L�b�v�B
                        continue;
                    }
                    if (dot(currentNormal, nearNormal) < 0.5f)
                    {
                        return true;
                    }
                }

                return false;

            }

            bool IsDepthAndNormalOutline(float2 uv)
            {
                return IsDepthOutlineBy8Texel(uv) || IsNormalOutline(uv);
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                float4 finalCol = col;

                switch (_HowToDrawOutline)
                {
                case 0:
                    if (IsDepthOutlineBy4Texel(i.uv))
                    {
                        finalCol = _OutlineColor;
                    }
                    break;
                case 1:
                    if (IsDepthOutlineBy8Texel(i.uv))
                    {
                        finalCol = _OutlineColor;
                    }
                    break;
                case 2:
                    if (IsNormalOutline(i.uv))
                    {
                        finalCol = _OutlineColor;
                    }
                    break;
                case 3:
                    if (IsDepthAndNormalOutline(i.uv))
                    {
                        finalCol = _OutlineColor;
                    }
                    break;
                }

                return finalCol;

            }
            ENDHLSL
        }
    }
}
