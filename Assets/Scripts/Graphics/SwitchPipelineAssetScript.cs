using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SwitchPipelineAssetScript : MonoBehaviour
{
    // Define
    private enum EnPipelineAsset
    {
        enHigh,
        enMideum,
        enLow,
        enPipelineAssetNum
    }


    // Serialize Field
    [SerializeField]
    RenderPipelineAsset[] m_pipelineAsset = new RenderPipelineAsset[(int)EnPipelineAsset.enPipelineAssetNum];
    [SerializeField]
    Operation m_operation = null;

    // Field
    EnPipelineAsset m_pipelinAssetState = EnPipelineAsset.enHigh;


    // Unity Function

    // Start is called before the first frame update
    void Start()
    {
        CheckCurrentPipelineAsset();

        return;
    }

    // Update is called once per frame
    void Update()
    {
        SwitchPipelineAsset();

        return;
    }

    void CheckCurrentPipelineAsset()
    {
        for (int i = 0; i < (int)EnPipelineAsset.enPipelineAssetNum; i++)
        {
            if (GraphicsSettings.currentRenderPipeline.name == m_pipelineAsset[i].name)
            {
                m_pipelinAssetState = (EnPipelineAsset)i;
                // Debug.Log("currentPipelineAsset" + m_pipelinAssetState);
                break;
            }
        }

        return;
    }

    // Function
    void SwitchPipelineAsset()
    {
        if (m_operation.GetIsLongTouch)
        {
            if (m_pipelinAssetState == (EnPipelineAsset.enPipelineAssetNum - 1))
            {
                m_pipelinAssetState = 0;
            }
            else
            {
                m_pipelinAssetState++;
            }

            QualitySettings.renderPipeline = m_pipelineAsset[(int)m_pipelinAssetState];
            // Debug.Log("SwitchPipelineAsset" + m_pipelinAssetState);
            // Debug.Log("CurrentPipelineAsset" + GraphicsSettings.renderPipelineAsset.name);
        }

        return;
    }
}
