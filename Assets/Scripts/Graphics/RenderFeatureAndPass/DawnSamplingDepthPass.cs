using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DawnSamplingDepthPass : ScriptableRenderPass
{
    // Field
    readonly Material m_kDawnSamplingDepthMaterial;
    private const string m_kCommandBufferName = nameof(DawnSamplingDepthPass);
    private readonly int m_kDawnSamplingDepthId = Shader.PropertyToID("_DawnSamplingDepth");
    private RenderTargetIdentifier m_currentRenderTarget;
    int m_dawnSampleScale = 2;
    private RenderTargetIdentifier m_dawnSamplingDepthRenderTarget;
    RenderTexture m_danwSamplingDepthTex;


    // Unity Function

    // This method is called before executing the render pass.
    // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
    // When empty this render pass will render to the active camera render target.
    // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
    // The render pipeline will ensure target setup and clearing happens in a performant manner.
    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        CameraData cameraData = renderingData.cameraData;
        int camWidth = cameraData.camera.scaledPixelWidth / m_dawnSampleScale;
        int camHeight = cameraData.camera.scaledPixelHeight / m_dawnSampleScale;

        m_danwSamplingDepthTex = new RenderTexture(camWidth, camHeight, 0);

    }

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        int width = cameraTextureDescriptor.width / m_dawnSampleScale;
        int height = cameraTextureDescriptor.height / m_dawnSampleScale;

        cmd.GetTemporaryRT(m_kDawnSamplingDepthId, width, height, 0, FilterMode.Bilinear);

        m_dawnSamplingDepthRenderTarget = new RenderTargetIdentifier(m_kDawnSamplingDepthId);

        ConfigureTarget(m_dawnSamplingDepthRenderTarget);
    }

    // Here you can implement the rendering logic.
    // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
    // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
    // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer commandBuffer = CommandBufferPool.Get(m_kCommandBufferName);
        //CameraData cameraData = renderingData.cameraData;
        //int camWidth = cameraData.camera.scaledPixelWidth;
        //int camHeight = cameraData.camera.scaledPixelHeight;



        commandBuffer.Blit(m_currentRenderTarget, m_danwSamplingDepthTex, m_kDawnSamplingDepthMaterial);
        //commandBuffer.Blit(m_dawnSamplingDepthRenderTarget, m_danwSamplingDepthTex, m_kDawnSamplingDepthMaterial);

        //commandBuffer.ReleaseTemporaryRT(m_kDawnSamplingDepthId);

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
    public DawnSamplingDepthPass()
    {
        Shader shader = Shader.Find("MyShader/DawnSamplingDephtShader");
        m_kDawnSamplingDepthMaterial = CoreUtils.CreateEngineMaterial(shader);

        m_dawnSamplingDepthRenderTarget = new RenderTargetIdentifier(m_kDawnSamplingDepthId);


    }

    public void SetParam(RenderTargetIdentifier target)
    {
        m_currentRenderTarget = target;

    }

    public RenderTexture GetDawnDamplingDephtTex()
    {
        return m_danwSamplingDepthTex;
    }

    public int GetDawnSanplingScale()
    {
        return m_dawnSampleScale;
    }

}
