using UnityEngine;

//// DrawOutLineShader.shaderÇÃÉpÉâÉÅÅ[É^ ////

public enum EnHowToDrawOutline
{
    enDepthBy4Texel,
    enDepthBy8Texel,
    enDepthBy8TexleHigh,
    enNormal,
    enDepthAndNormal,
    enDepthHighAndNormal,
}

[System.Serializable]
public class CDrawOutLineParam
{
    public Color m_outLineColor = Color.black;
    public float m_outlineThick = 1.0f;
    public float m_outlineThreshold = 0.01f;
    public EnHowToDrawOutline m_howToDrawOutline = EnHowToDrawOutline.enDepthAndNormal;
    public float m_maxDepthDistance = 50.0f;
    [Range(1.0f,2.0f)]
    public float m_outlineBias = 1.0f;
}