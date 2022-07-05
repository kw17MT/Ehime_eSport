using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class DrawOutLinePass : ScriptableRenderPass
{

    // Field
    readonly Material m_kDrawOutlineMaterial;
    private const string m_kCommandBufferName = nameof(DrawOutLinePass);
    private readonly int m_kRenderTargetTexId = Shader.PropertyToID("_RenderTargetTex");
    private RenderTargetIdentifier m_currentRenderTarget;



    // Unity Function

    // This method is called before executing the render pass.
    // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
    // When empty this render pass will render to the active camera render target.
    // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
    // The render pipeline will ensure target setup and clearing happens in a performant manner.
    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        ConfigureInput(ScriptableRenderPassInput.Normal);
    }

    // Here you can implement the rendering logic.
    // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
    // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
    // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer commandBuffer = CommandBufferPool.Get(m_kCommandBufferName);
        CameraData cameraData = renderingData.cameraData;
        int camWidth = cameraData.camera.scaledPixelWidth;
        int camHeight = cameraData.camera.scaledPixelHeight;

        commandBuffer.GetTemporaryRT(m_kRenderTargetTexId, camWidth, camHeight, 0, FilterMode.Bilinear);

        commandBuffer.Blit(m_currentRenderTarget, m_kRenderTargetTexId, m_kDrawOutlineMaterial);

        commandBuffer.Blit(m_kRenderTargetTexId, m_currentRenderTarget);
        commandBuffer.ReleaseTemporaryRT(m_kRenderTargetTexId);

        context.ExecuteCommandBuffer(commandBuffer);
        context.Submit();

        CommandBufferPool.Release(commandBuffer);

        return;
    }

    // Cleanup any allocated resources that were created during the execution of this render pass.
    public override void OnCameraCleanup(CommandBuffer cmd)
    {
    }



    // Public Function
    public DrawOutLinePass(Material mat)
    {
        m_kDrawOutlineMaterial = mat;
    }

    public void SetParam(RenderTargetIdentifier target, Color color, float thick, float threshold, int howToDrawOutline)
    {
        m_currentRenderTarget = target;
        m_kDrawOutlineMaterial.SetColor("_OutlineColor", color);
        m_kDrawOutlineMaterial.SetFloat("_OutlineThick", thick);
        m_kDrawOutlineMaterial.SetFloat("_OutlineThreshold", threshold);
        m_kDrawOutlineMaterial.SetInt("_HowToDrawOutline", howToDrawOutline);

    }

}
