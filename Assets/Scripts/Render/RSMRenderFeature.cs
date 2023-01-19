using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class RSMRenderFeature : ScriptableRendererFeature
{
    public RenderSetting settings;
    private RSMRenderPass m_ScriptablePass;

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new RSMRenderPass(settings);
        
        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = settings.renderPassEvent;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        //m_ScriptablePass.Setup(renderingData.cameraData.cameraTargetDescriptor, _preRSMHandle);
        renderer.EnqueuePass(m_ScriptablePass);
    }
}

[Serializable]
public class RenderSetting
{
    public RenderQueueType renderQueueType;
    public LayerMask layerMask;
    public RenderPassEvent renderPassEvent;
    public FilteringSettings filteringSettings;
    public RenderTexture worldFlux;
    public RenderTexture worldNormal;
    public RenderTexture worldPosition;
    public RenderTexture depth;
}

public class RSMRenderPass : ScriptableRenderPass
{
    private RenderSetting _settings;
    private int _fluxID = Shader.PropertyToID("_WorldFlux");
    private int _normalID = Shader.PropertyToID("_WorldNormal");
    private int _positionID = Shader.PropertyToID("_WorldPosition");
    private int _depthID = Shader.PropertyToID("_Depth");
    private RenderTargetIdentifier _rtIdentifier;
    private Material _preRsmMaterial;
    private const string _passTag = "RSM pre pass";
    private ShaderTagId m_ShaderTagId = new ShaderTagId("Hidden/preRSM");

    public RSMRenderPass( RenderSetting setting)
    {
        Shader shader = Shader.Find("Hidden/preRSM");
        if (shader == null)
        {
            Debug.Log("Shader not found");
            return;
        }
        _preRsmMaterial = new Material(shader);
    }
    
    public void Setup(in RenderTargetIdentifier currentTarget)
    {
        _rtIdentifier = currentTarget;
    }
    
    // This method is called before executing the render pass.
    // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
    // When empty this render pass will render to the active camera render target.
    // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
    // The render pipeline will ensure target setup and clearing happens in a performant manner.
    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
    }

    // Here you can implement the rendering logic.
    // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
    // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
    // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get(_passTag);
        cmd.name = "Pre RSM Buffer";
        /*int width = renderingData.cameraData.camera.scaledPixelWidth;
        int height = renderingData.cameraData.camera.scaledPixelHeight;*/
        cmd.GetTemporaryRT(_fluxID, renderingData.cameraData.cameraTargetDescriptor);
        cmd.GetTemporaryRT(_normalID, renderingData.cameraData.cameraTargetDescriptor);
        cmd.GetTemporaryRT(_positionID, renderingData.cameraData.cameraTargetDescriptor);
        cmd.GetTemporaryRT(_depthID, renderingData.cameraData.cameraTargetDescriptor);
        RenderTargetIdentifier[] buffers =
        {
            new RenderTargetIdentifier(_fluxID),
            new RenderTargetIdentifier(_normalID),
            new RenderTargetIdentifier(_positionID),
            new RenderTargetIdentifier(_depthID)
        };
        cmd.SetRenderTarget(buffers, _depthID);
        
        cmd.ReleaseTemporaryRT(_fluxID);
        cmd.ReleaseTemporaryRT(_normalID);
        cmd.ReleaseTemporaryRT(_positionID);
        cmd.ReleaseTemporaryRT(_depthID);
    }

    // Cleanup any allocated resources that were created during the execution of this render pass.
    public override void OnCameraCleanup(CommandBuffer cmd)
    {
    }
}


