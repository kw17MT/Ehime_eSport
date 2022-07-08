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