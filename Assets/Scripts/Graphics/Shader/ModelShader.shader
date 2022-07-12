Shader "MyShader/ModelShader"
{
    Properties
    {
        [Header(Basic)]
        _MainTex("Texture", 2D) = "white" {}
        [KeywordEnum(Toon, RealAndToon)]_ShadingMode("ShadingMode", int) = 0
        _AmbientLight("AmbientLight", Color) = (0.5,0.5,0.5,1.0)
        _SpecularPow("SpecularPow", float) = 5.0

        [Header(Shadow)]
        [Toggle(IS_SHADOW_RECEIVER)]_IsShadowReceiver("IsShadowReceiver", int) = 1
        [Toggle(IS_SHADOW_CASTER)]_IsShadowCaster("IsShadowCaster", int) = 1

        [Header(RampColor)]
        _HighLightColor("HighLightColor", Color) = (1.0,1.0,1.0,1.0)
        _NormalLightColor("NormalLightColor", Color) = (0.8,0.8,0.8,1.0)
        _Shadow1Color("Shadow1Color", Color) = (0.5,0.5,0.5,1.0)
        _Shadow2Color("Shadow2Color", Color) = (0.5,0.5,0.5,1.0)

        [Space(10)]
        _Sphericalize("Sphericalize(X,Y,Z,Blend)",Vector) = (0,0,0,0)

        [Header(RimLight)]
        [Toggle(IS_RIMLIGHT)]_IsRimLight("IsRimLight", int) = 1
        _RimColor("RimColor", Color) = (1.0, 1.0, 0.0,1.0)
        _RimPower("RimPower", Range(1.0, 10.0)) = 4.0
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
            int _IsShadowCaster;
            float4 _HighLightColor;
            float4 _NormalLightColor;
            float4 _Shadow1Color;
            float4 _Shadow2Color;
            float4 _Sphericalize;
            int _IsRimLight;
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

            // シェーディング方法選択のKeywordEnumのシェーダーキーワード
            #pragma multi_compile _SHADINGMODE_TOON _SHADINGMODE_REALANDTOON
            // シャドウ用のトグルのシェーダーキーワード
            #pragma shader_feature IS_SHADOW_RECEIVER
            #pragma shader_feature IS_SHADOW_CASTER
            // リムライトを行うかのトグルのシェーダーキーワード
            #pragma shader_feature IS_RIMLIGHT


            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            // 自作のライブラリ
            // ライティング用の関数ライブラリ
            #include "Assets/Scripts/Graphics/Shader/Library/LightingFunction.hlsl"

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.posWS = TransformObjectToWorld(v.vertex.xyz);
                o.viewDir = normalize(GetWorldSpaceViewDir(o.posWS));
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

#ifdef IS_RIMLIGHT
                // リムライトの計算
                col.xyz += CalcrRimLight(
                    i.normal,
                    i.viewDir,
                    _RimColor,
                    _RimPower,
                    mainLight.direction,
                    _DirectionRimPower
                );
#endif

                // フォグの計算
                col.rgb = MixFog(col.rgb, i.fogFactor);


                return col;
            }
            ENDHLSL
        }

//#ifdef IS_SHADOW_CASTER
        // シャドウキャスター用パス
        UsePass "MyShader/ShadowCasterShader/ShadowCaster"
//#endif

        // DepthNormals使用時の深度バッファ用パス
        UsePass "MyShader/DepthNormalsShader/DepthNormalsPass"

        // DepthOnly使用時の深度バッファ用パス
        UsePass "MyShader/DepthOnlyShader/DepthOnlyPass"
    }
}