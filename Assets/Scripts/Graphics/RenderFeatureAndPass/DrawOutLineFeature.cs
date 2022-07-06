using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DrawOutLineFeature : ScriptableRendererFeature
{
    // define
    enum EnHowToDrawOutline
    {
        enDepthBy4Texel,
        enDepthBy8Texel,
        enNormal,
        enDepthAndNormal
    }

    // Serialize Field
    [SerializeField]
    Shader m_drawOutLineShader;
    [SerializeField]
    Color m_outLineColor = Color.black;
    [SerializeField]
    float m_outlineThick = 1.0f;
    [SerializeField]
    float m_outlineThreshold = 1.0f;
    [SerializeField]
    EnHowToDrawOutline m_howToDrawOutline = EnHowToDrawOutline.enDepthAndNormal;

    // Field
    DrawOutLinePass m_drawOutLinePass;
    


    // Unity Function
    /// <inheritdoc/>
    public override void Create()
    {
        // 動的に作ったマテリアルは破棄しないとリークするため、
        // CoreUtils.CreateEngineMaterialメソッドで作成する。
        Material mat = CoreUtils.CreateEngineMaterial(m_drawOutLineShader);

        m_drawOutLinePass = new DrawOutLinePass(mat);

        // Configures where the render pass should be injected.
        m_drawOutLinePass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        m_drawOutLinePass.SetParam(
            renderer.cameraColorTarget,
            m_outLineColor,
            m_outlineThick,
            m_outlineThreshold,
            (int)m_howToDrawOutline
            );
        renderer.EnqueuePass(m_drawOutLinePass);
    }


}


