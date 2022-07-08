// �e�̌v�Z
inline half CalcShadow(float3 posWS)
{
    float4 shadowCoord = TransformWorldToShadowCoord(posWS);
    Light shadowLight = GetMainLight(shadowCoord);
    float shadow = shadowLight.shadowAttenuation;
    Light addLight0 = GetAdditionalLight(0, posWS);
    shadow *= addLight0.shadowAttenuation;
    return shadow;
}

// �g�U���ˌ��̌v�Z
inline float CalcDiffuse(float NdotL)
{
    float diffusePower = NdotL;
    return diffusePower;
}

// ���ʔ��ˌ��̌v�Z
inline float CalcSpecular(float3 normal, float3 ligDir, float3 toEyeVec, float specPow)
{
    float3 refVec = reflect(ligDir, normal);
    float specularPower = dot(refVec, toEyeVec);

    specularPower = pow(max(specularPower, 0.0f), specPow);

    return specularPower;
}

// �����̌v�Z
inline float3 CalcAmbientLight(float3 ambientLight, float3 baseColor)
{
    return ambientLight * baseColor;
}

// �g�D�[���V�F�[�f�B���O�̌v�Z
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

// �������C�g�̌v�Z
inline float3 CalcrRimLight(
    float3 normal,
    float3 viewDir,
    float4 rimColor,
    float rimPower,
    float3 ligDir,
    half dirRimPower
)
{
    // ���ʂ̃������C�g�̌v�Z
    half rim = 1.0 - saturate(abs(dot(normal, viewDir)));
    float3 emission = rimColor.rgb * pow(rim, rimPower);

    // �������C�g�̃f�B���N�V�������C�g�̕����̉e����^����v�Z
    // �r���[��Ԃł̃f�B���N�V�������C�g�̕����Ɖe����^���������߁A
    // �@���ƃf�B���N�V�������C�g�̕��������[���h��Ԃ���r���[��Ԃɕϊ�����B
    float3 viewNormal = TransformWorldToView(normal);
    float3 viewLigDir = TransformWorldToView(ligDir);
    half dirRim = saturate(dot(ligDir, normal));

    // �Ȃ���dirRimPower��0�̎��ɔ��������Ȃ錻�ۂ��N���邽��0�ȊO�̎��Ɏ��s�B
    // ���l�Œ���0�悵���畁�ʂ�1���Ԃ��Ă���̂ɁE�E�E�B
    // �i�]�B
    if (dirRimPower != 0.0)
    {
        emission *= pow(dirRim, dirRimPower);
    }

    return emission;
}