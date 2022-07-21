Shader "MyShader/DrawOutLineShader"
{
    // DrawOutLineParam.csと同じ構造
    // DrawOutLineFeature.csから変更する
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor("OutlineColor", Color) = (0.0,0.0,0.0,1.0)
        _HowToDrawOutline("HowToDrawOutline", int) = 3
        _OutlineThick("OutlineThick", float) = 1.0
        _OutlineThreshold("OutlineThreshold", float) = 0.0001
        _HowToDrawOutline("HowToDrawOutline", int) = 3
        _MaxDepthDistance("MaxDepthDistance", float) = 50.0
        _OutlineBias("OutlineBias", Range(1.0, 2.0)) = 1.0
        _FadeBias("FadeBias", Range(1.0, 8.0)) = 4.0
    }
    SubShader
    {
        Tags 
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }
        ZWrite Off

        // 各Passでcbufferが変わらないようにここに定義する
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
            float _MaxDepthDistance;
            float _OutlineBias;
            float _FadeBias;
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

            bool IsDepthOutlineBy8Texel(float2 uv, inout bool isOutOfRange, inout float outlineColorRate)
            {
                // アウトラインの太さの計算。

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

                // 近傍8テクセルの深度値の調査

                float currentDepth = MyLinearDepth(SampleSceneDepth(uv));
                float neighborDepth = 0.0f;
                float smallestDepth = currentDepth;

                [unroll]
                for (int i = 0; i < 8; i++)
                {
                    float depth = MyLinearDepth(SampleSceneDepth(uv + uvOffset[i]));
                    neighborDepth += depth;
                    smallestDepth = min(smallestDepth, depth);
                }

                // アウトラインを描画する最大距離を、深度値に合わせて正規化
                float maxDepht = _MaxDepthDistance / _ProjectionParams.z;

                // アウトライン判定より前に、アウトラインカラー率の計算をしておく。
                // 法線でアウトラインが描画される可能性もあるため。
                outlineColorRate = 1.0f - (maxDepht - smallestDepth) / maxDepht;
                outlineColorRate = 1.0f - pow(outlineColorRate, _FadeBias);


                // アウトライン判定。
                bool isOutline = false;

                // アウトライン描画範囲外
                if (smallestDepth >= maxDepht)
                {
                    isOutOfRange = true;
                    return isOutline;
                }

                // アウトライン範囲内
                isOutOfRange = false;

                neighborDepth /= 8.0f;
                if ((currentDepth - neighborDepth) / neighborDepth > _OutlineThreshold)
                {
                    isOutline = true;
                }

                return isOutline;
            }

            bool IsDepthOutlineBy8TexelHigh(float2 uv, inout bool isOutOfRange, inout float thickScale)
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
                float currentDepth = MyLinearDepth(SampleSceneDepth(uv));
                float smallestDepth = currentDepth;

                float neighborDepth = 0.0f;

                [unroll]
                for (int i = 0; i < 8; i++)
                {
                    float depth = MyLinearDepth(SampleSceneDepth(uv + uvOffset[i]));
                    neighborDepth += depth;
                    smallestDepth = min(smallestDepth, depth);
                }


                float maxDepht = _MaxDepthDistance / _ProjectionParams.z;
                if (smallestDepth >= maxDepht)
                {
                    isOutOfRange = true;
                    return false;
                }

                smallestDepth = pow(max(smallestDepth,0.0f), _OutlineBias);
                thickScale = (maxDepht - smallestDepth) / maxDepht;
                //thickScale = pow(thickScale, _OutlineBias);
                diffX = _CameraDepthTexture_TexelSize.x * _OutlineThick * thickScale;
                diffY = _CameraDepthTexture_TexelSize.y * _OutlineThick * thickScale;
                float2 uvOffset2[8] =
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
                smallestDepth = currentDepth;
                neighborDepth = 0.0f;
                [unroll]
                for (i = 0; i < 8; i++)
                {
                    float depth = MyLinearDepth(SampleSceneDepth(uv + uvOffset2[i]));
                    neighborDepth += depth;
                    smallestDepth = min(smallestDepth, depth);
                }

                maxDepht = _MaxDepthDistance / _ProjectionParams.z;
                if (smallestDepth >= maxDepht)
                {
                    isOutOfRange = true;
                    return false;
                }

                neighborDepth /= 8.0f;
                isOutOfRange = false;
                bool isOutline = false;
                if ((currentDepth - neighborDepth) / neighborDepth > _OutlineThreshold)
                {
                    isOutline = true;
                }

                return isOutline;
            }

            bool IsNormalOutline(float2 uv, bool isOutOfRage, float thickScale)
            {
                if (isOutOfRage)
                {
                    return false;
                }

                float diffX = _CameraDepthTexture_TexelSize.x * _OutlineThick * thickScale * 0.5f;
                float diffY = _CameraDepthTexture_TexelSize.y * _OutlineThick * thickScale * 0.5f;

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

                // 警告文
                // gradient instruction used in a loop with varying iteration, attempting to unroll the loop
                // （訳）グラディエント（勾配）命令がループ内で使用され、反復が変化し、ループのアンロールが試みられました。

                // 上の警告を回避するために[unroll]を入れた。
                // グラディエント命令とは、使用されている方法など、使用されるミップレベルを
                // 自分で決定するすべてのテクスチャサンプリング方法、らしい。
                // グラディエント命令は動的分岐内では機能しないため、静的にする必要がある。
                // そのためループをunrollで展開する必要がある、、のかな？

                // 下は参考にしたページ。
                // https://stackoverflow.com/questions/56581141/direct3d11-gradient-instruction-used-in-a-loop-with-varying-iteration-forcing

                [unroll]
                for (int i = 0; i < 8; i++)
                {
                    nearNormal = SampleSceneNormals(uv + uvOffset[i]);

                    if (dot(float3(111.0f,111.0f,111.0f), nearNormal) == 0.0f)
                    {
                        // ゼロベクトルとベクトルの内積は0.0になる。
                        // 法線が描画されていないところがアウトライン判定されることを避けるため、
                        // currentNoramlと被らないベクトルと、nearNormalの内積をとって、
                        // ゼロベクトルを探し、法線が描画されていないため処理をスキップ。
                        continue;
                    }
                    if (dot(currentNormal, nearNormal) < 0.5f)
                    {
                        return true;
                    }
                }

                return false;

            }

            bool IsDepthAndNormalOutline(float2 uv, inout bool isOutOfRange, inout float thickScale, inout float outlineColorRate)
            {
                bool outlineFlag = false;
                outlineFlag = IsDepthOutlineBy8Texel(uv, isOutOfRange, outlineColorRate);
                if (outlineFlag != true)
                {
                    outlineFlag = IsNormalOutline(uv, isOutOfRange, thickScale);
                }
                return outlineFlag;
            }

            bool IsDepthHighAndNormalOutline(float2 uv, inout bool isOutOfRange, inout float thickScale)
            {
                bool outlineFlag = false;
                outlineFlag = IsDepthOutlineBy8TexelHigh(uv, isOutOfRange, thickScale);
                if (outlineFlag != true)
                {
                    outlineFlag = IsNormalOutline(uv, isOutOfRange, thickScale);
                }
                return outlineFlag;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                float4 finalCol = col;
                
                bool isOutOfRange = false;
                float thickScale = 1.0f;
                float outlineColorRate = 0.0f;

                switch (_HowToDrawOutline)
                {
                case 0:
                    if (IsDepthOutlineBy4Texel(i.uv))
                    {
                        finalCol = _OutlineColor;
                    }
                    break;
                case 1:
                    if (IsDepthOutlineBy8Texel(i.uv, isOutOfRange, outlineColorRate))
                    {
                        finalCol = lerp(col, _OutlineColor, outlineColorRate);
                    }
                    break;
                case 2:
                    if (IsDepthOutlineBy8TexelHigh(i.uv, isOutOfRange, thickScale))
                    {
                        finalCol = _OutlineColor;
                    }
                    break;
                case 3:
                    if (IsNormalOutline(i.uv, isOutOfRange, thickScale))
                    {
                        finalCol = _OutlineColor;
                    }
                    break;
                case 4:
                    if (IsDepthAndNormalOutline(i.uv, isOutOfRange, thickScale, outlineColorRate))
                    {
                        finalCol = lerp(col, _OutlineColor, outlineColorRate);
                    }
                    break;
                case 5:
                    if (IsDepthHighAndNormalOutline(i.uv, isOutOfRange, thickScale))
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
