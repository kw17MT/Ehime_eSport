Shader "MyShader/ModelShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        [KeywordEnum(Toon, RealAndToon)]_ShadingMode("ShadingMode", int) = 0
        _AmbientLight("AmbientLight", Color) = (0.5,0.5,0.5,1.0)
        _SpecularPow("SpecularPow", float) = 5.0
        [Toggle(IS_SHADOW_RECEIVER)]_IsShadowReceiver("IsShadowReceiver", int) = 1

        [Header(RampColor)]
        _HighLightColor("HighLightColor", Color) = (1.0,1.0,1.0,1.0)
        _NormalLightColor("NormalLightColor", Color) = (0.8,0.8,0.8,1.0)
        _Shadow1Color("Shadow1Color", Color) = (0.5,0.5,0.5,1.0)
        _Shadow2Color("Shadow2Color", Color) = (0.5,0.5,0.5,1.0)

        //[Header(OutLine)]
        //_EdgeColor("EdgeColor", Color) = (0.0,0.0,0.0,1.0)
        //_EdgeWidth("EdgeWidth", Range(0.0,0.01)) = 0.00001

        [Space(10)]
        _Sphericalize("Sphericalize(X,Y,Z,Blend)",Vector) = (0,0,0,0)

        [Header(RimLight)]
        _RimColor("RimColor", Color) = (1.0, 1.0, 0.0,1.0)
        [PowerSlider(3.0)]_RimPower("RimPower", Range(0.3, 8.0)) = 3.0
        [PowerSlider(3.0)]_DirectionRimPower("DirectionRimPower", Range(0.0,8.0)) = 2.0
    }

        SubShader
        {
            Tags
            {
                "RenderType" = "Opaque"
                "RenderPipeline" = "UniversalPipeline"
            }
            LOD 100
            ZWrite On

            // 各Passでcbufferが変わらないようにここに定義する
            HLSLINCLUDE

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                int _ShadingMode;
                float4 _AmbientLight;
                float _SpecularPow;
                int _IsShadowReceiver;
                float4 _HighLightColor;
                float4 _NormalLightColor;
                float4 _Shadow1Color;
                float4 _Shadow2Color;
                //float4 _EdgeColor;
                //float _EdgeWidth;
                float4 _Sphericalize;
                float4 _RimColor;
                float _RimPower;
                float _DirectionRimPower;
            CBUFFER_END

            ENDHLSL

                // トゥーンシェーディング用パス
                Pass
                {
                    Name "ToonLit"
                    Tags
                    {
                        "LightMode" = "UniversalForward"
                    }

                    HLSLPROGRAM
                    #pragma vertex vert
                    #pragma fragment frag
                // make fog work
                #pragma multi_compile_fog

                // Universal Pipeline shadow keywords
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
                #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
                #pragma multi_compile _ _SHADOWS_SOFT

                // シャドウ用のトグルのシェーダーキーワード
                #pragma shader_feature IS_SHADOW_RECEIVER
                // シェーディング方法選択のKeywordEnumのシェーダーキーワード
                #pragma multi_compile _SHADINGMODE_TOON _SHADINGMODE_REALANDTOON

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    float3 normal : NORMAL;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float3 normal : NORMAL;
                    float fogFactor : TEXCOORD1;
                    float3 posWS : TEXCOORD2;
                    float3 viewDir : TEXCOORD3;
                };

                // 影の計算
                inline half CalcShadow(float3 posWS)
                {
                    float4 shadowCoord = TransformWorldToShadowCoord(posWS);
                    Light shadowLight = GetMainLight(shadowCoord);
                    float shadow = shadowLight.shadowAttenuation;
                    Light addLight0 = GetAdditionalLight(0, posWS);
                    shadow *= addLight0.shadowAttenuation;
                    return shadow;
                }

                // 拡散反射光の計算
                inline float CalcDiffuse(float NdotL)
                {
                    float diffusePower = NdotL;
                    return diffusePower;
                }

                // 鏡面反射光の計算
                inline float CalcSpecular(float3 normal, float3 ligDir, float3 toEyeVec, float specPow)
                {
                    float3 refVec = reflect(ligDir, normal);
                    float specularPower = dot(refVec, toEyeVec);

                    specularPower = pow(max(specularPower, 0.0f), specPow);

                    return specularPower;
                }

                // 環境光の計算
                inline float3 CalcAmbientLight(float3 ambientLight, float3 baseColor)
                {
                    return ambientLight * baseColor;
                }

                // トゥーンシェーディングの計算
                inline float3 CalcToonShading(float litPower)
                {
                    half rampRate[3] = { 0.05f,0.99f,0.999f };
                    //half rampRate[3] = { 0.2f,0.7f,0.9f };
                    half rampPower = litPower;
                    //rampPower = pow(rampPower, 5.0f);
                    rampPower = 1.0 - rampPower;
                    float3 rampColor;
                    if (rampPower <= rampRate[0])
                    {
                        rampColor = _HighLightColor.xyz;
                    }
                    else if (rampPower <= rampRate[1])
                    {
                        rampColor = _NormalLightColor.xyz;
                    }
                    else if (rampPower <= rampRate[2])
                    {
                        rampColor = _Shadow1Color.xyz;
                    }
                    else
                    {
                        rampColor = _Shadow2Color.xyz;
                    }
                    return rampColor;
                }

                // リムライトの計算
                inline float3 CalcrRimLight(
                    float3 normal,
                    float3 viewDir,
                    float4 rimColor,
                    float rimPower,
                    float3 ligDir,
                    half dirRimPower
                )
                {
                    // 普通のリムライトの計算
                    half rim = 1.0 - saturate(abs(dot(normal, viewDir)));
                    float3 emission = rimColor.rgb * pow(rim, rimPower);

                    // リムライトのディレクションライトの方向の影響を与える計算
                    // ビュー空間でのディレクションライトの方向と影響を与えたいため、
                    // 法線とディレクションライトの方向をワールド空間からビュー空間に変換する。
                    float3 viewNormal = TransformWorldToView(normal);
                    float3 viewLigDir = TransformWorldToView(ligDir);
                    half dirRim = saturate(dot(ligDir, normal));

                    // なぜかdirRimPowerが0の時に半分黒くなる現象が起こるため0以外の時に実行。
                    // 数値で直接0乗したら普通に1が返ってくるのに・・・。
                    // ナゾ。
                    if (dirRimPower != 0.0)
                    {
                        emission *= pow(dirRim, dirRimPower);
                    }

                    return emission;
                }

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = TransformObjectToHClip(v.vertex.xyz);
                    o.posWS = TransformObjectToWorld(v.vertex.xyz);
                    o.viewDir = GetWorldSpaceViewDir(o.posWS);
                    o.fogFactor = ComputeFogFactor(o.vertex.z);

                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                    o.normal = TransformObjectToWorldNormal(v.normal);
                    float3 sphereNormal = normalize(v.vertex.xyz - _Sphericalize.xyz);
                    o.normal = normalize(lerp(o.normal, sphereNormal, _Sphericalize.w));

                    return o;
                }

                float4 frag(v2f i) : SV_Target
                {
                    float4 baseColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                    Light mainLight = GetMainLight();
                    float NdotL = saturate(dot(i.normal, mainLight.direction));
                    float3 toEyeVec = normalize(_WorldSpaceCameraPos - i.posWS);
                    float4 col = { 0.0f,0.0f,0.0f,1.0f };


                    // 影の計算
                    float shadow = 1.0f;
    #ifdef IS_SHADOW_RECEIVER
                        shadow = CalcShadow(i.posWS);
    #endif

                        // 拡散反射光の計算
                        float diffuse = CalcDiffuse(NdotL);

                        // 鏡面反射光の計算
                        float specular = CalcSpecular(i.normal, mainLight.direction, toEyeVec, _SpecularPow);

                        // トゥーンシェーディングの計算
                        // ライトの影響を乗算
        #ifdef _SHADINGMODE_TOON
                        // よりトゥーン調になる。
                        // ライトの結果全てをトゥーン用に階調化。
                        float3 rampLig = CalcToonShading((diffuse + specular + _AmbientLight.x) * shadow);
                        col.xyz = baseColor.xyz * mainLight.color * rampLig;
        #elif _SHADINGMODE_REALANDTOON
                        // リアルよりの方
                        // トゥーンのカラーを後から乗算する。上よりリアルっぽくなる。
                        float3 rampLig = CalcToonShading(NdotL);
                        col.xyz = baseColor.xyz * mainLight.color * (diffuse + specular) * shadow * rampLig;
        #endif

                        // 環境光の計算
                        col.xyz += CalcAmbientLight(_AmbientLight.xyz, baseColor.xyz);

                        // リムライトの計算
                        col.xyz += CalcrRimLight(
                            i.normal,
                            i.viewDir,
                            _RimColor,
                            _RimPower,
                            mainLight.direction,
                            _DirectionRimPower
                        );

                        // フォグの計算
                        col.rgb = MixFog(col.rgb, i.fogFactor);


                        return col;
                    }
                    ENDHLSL
                }

                // アウトライン描画用パス
                //Pass
                //{
                //    Name "OutLine"

                //    Tags
                //    {
                //        "RenderType" = "Transparent"
                //        "Queue" = "Transparent"
                //        "IgnoreProjector" = "True"
                //    }

                //    Cull front
                //    ZTest Less

                //    HLSLPROGRAM
                //    #pragma vertex vert
                //    #pragma fragment frag

                //    #pragma multi_compile_instancing

                //    struct appdata
                //    {
                //        float4 vertex : POSITION;
                //        float3 normal : NORMAL;
                //        // 頂点カラーのrでエッジ調整
                //        float4 color : COLOR;
                //    };
                //    struct v2f
                //    {
                //        float4 vertex : SV_POSITION;
                //    };


                //    v2f vert(appdata v)
                //    {
                //        v2f o;
                //        o.vertex = TransformObjectToHClip(v.vertex.xyz);
                //        float3 normal = TransformObjectToWorldNormal(v.normal);
                //        // 距離に応じてアウトラインを調整。
                //        // 距離が遠くなるほど線が太くなる。
                //        float dist = length(mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1)).xyz);
                //        // 頂点カラーのrで線の太さを調整。
                //        o.vertex += float4(normal * _EdgeWidth * v.color.r * dist, 0.0f);

                //        return o;
                //    }

                //    float4 frag(v2f i) : SV_Target
                //    {
                //        return _EdgeColor;
                //    }

                //    ENDHLSL
                //}

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

                        // DepthNormals使用時の深度バッファ用パス
                        Pass
                        {
                            Tags
                            {
                                "LightMode" = "DepthNormals"
                            }

                            HLSLPROGRAM
                            #pragma vertex vert
                            #pragma fragment frag

                            #pragma multi_compile_instancing



                            struct appdata
                            {
                                float4 vertex : POSITION;
                                UNITY_VERTEX_INPUT_INSTANCE_ID
                            };

                            struct v2f
                            {
                                float4 vertex : SV_POSITION;
                                UNITY_VERTEX_INPUT_INSTANCE_ID
                            };


                            v2f vert(appdata v)
                            {
                                UNITY_SETUP_INSTANCE_ID(v);
                                v2f o;

                                o.vertex = TransformObjectToHClip(v.vertex.xyz);

                                return o;
                            }

                            float4 frag(v2f i) : SV_Target
                            {

                                return 0.0;
                            }

                            ENDHLSL
                        }

                            // DepthOnly使用時の深度バッファ用パス
                            Pass
                            {
                                Tags
                                {
                                    "LightMode" = "DepthOnly"
                                }

                                HLSLPROGRAM
                                #pragma vertex vert
                                #pragma fragment frag

                                #pragma multi_compile_instancing



                                struct appdata
                                {
                                    float4 vertex : POSITION;
                                    UNITY_VERTEX_INPUT_INSTANCE_ID
                                };

                                struct v2f
                                {
                                    float4 vertex : SV_POSITION;
                                    UNITY_VERTEX_INPUT_INSTANCE_ID
                                };


                                v2f vert(appdata v)
                                {
                                    UNITY_SETUP_INSTANCE_ID(v);
                                    v2f o;

                                    o.vertex = TransformObjectToHClip(v.vertex.xyz);

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