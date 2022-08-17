using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


public class AutoSettingPipelineAsset : MonoBehaviour
{

    //// Define
    private enum EnPipelineAsset
    {
        enLow,
        enMideum,
        enHigh,
        enPipelineAssetNum
    }

    //// Serialize Field
    [SerializeField]
    RenderPipelineAsset[] m_pipelineAsset = new RenderPipelineAsset[(int)EnPipelineAsset.enPipelineAssetNum];
    bool m_enableAutoSettingPipelineAsset = true;


    //// Field
    EnPipelineAsset m_pipelinAssetState = EnPipelineAsset.enHigh;
    EnPipelineAsset m_canUseHighestPipelinAssetState = EnPipelineAsset.enLow;
    bool m_isFinishedSetting = false;
    float m_checkIntervalTimer = 0.0f;
    // FPS用変数
    int m_frameCount = 0;
    float m_prevTime = 0.0f;
    float m_fps = 0.0f;

    //// Const Field
    static readonly float m_kFpsUpdateInterval = 0.5f;
    static readonly float m_kFpsThreshold = 24.0f;
    static readonly float m_kCheckIntervalTime = 5.0f;

    //// Unity Function

    // Start is called before the first frame update
    void Start()
    {
        // 下がる様子を見たいときのテスト用。
        //Application.targetFrameRate = 20;

        CheckCurrentPipelineAsset();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isFinishedSetting || m_enableAutoSettingPipelineAsset != true)
        {
            // 設定し終わっていたら、何もしない。
            // または、自動設定が有効でないなら、何もしない。
            return;
        }


        UpdateFps();

        AutoSetting();
    }


    //// Function

    void CheckCurrentPipelineAsset()
    {
        for (int i = 0; i < (int)EnPipelineAsset.enPipelineAssetNum; i++)
        {
            if (GraphicsSettings.currentRenderPipeline.name == m_pipelineAsset[i].name)
            {
                m_pipelinAssetState = (EnPipelineAsset)i;
                Debug.Log("currentPipelineAsset" + m_pipelinAssetState);
                break;
            }
        }

        Debug.Log("NotFound[CurrentPipelineAsset]");

        return;
    }

    void AutoSetting()
    {
        // 一定間隔でチェックする。
        if (m_checkIntervalTimer < m_kCheckIntervalTime)
        {
            m_checkIntervalTimer += Time.deltaTime;
            return;
        }
        m_checkIntervalTimer = 0.0f;

        // 現在のfpsをもとに、パイプラインアセットを変更させる。
        if (m_fps > m_kFpsThreshold)
        {
            CheckToUp();
        }
        else
        {
            CheckToDown();
        }


        return;
    }

    void CheckToUp()
    {
        if (m_pipelinAssetState == EnPipelineAsset.enHigh)
        {
            // これ以上のパイプラインアセットがないため、設定を終了する。
            m_isFinishedSetting = true;
            Debug.Log("AutoSettingIsFinished");


            // 使用可能で一番上のパイプラインアセットを記憶しておく。
            // （これ以上調べないため、冗長かもしれないけど、一応記憶しておく。）
            m_canUseHighestPipelinAssetState = m_pipelinAssetState;
        }
        else
        {
            // 使用可能で一番上のパイプラインアセットを記憶しておく。
            m_canUseHighestPipelinAssetState = m_pipelinAssetState;
            // 1つ上のパイプラインアセットを試してみる。
            m_pipelinAssetState++;
            QualitySettings.renderPipeline = m_pipelineAsset[(int)m_pipelinAssetState];


            Debug.Log("SetUpperPipelineAsset");
            Debug.Log("currentPipelineAsset" + m_pipelinAssetState);

        }

        return;
    }

    void CheckToDown()
    {
        if (m_pipelinAssetState == EnPipelineAsset.enLow)
        {
            // これ以下のパイプラインアセットがないため、設定を終了する。
            m_isFinishedSetting = true;
            Debug.Log("AutoSettingIsFinished");
        }
        else
        {
            // 一つ下のパイプラインアセットを試してみる。
            m_pipelinAssetState--;
            QualitySettings.renderPipeline = m_pipelineAsset[(int)m_pipelinAssetState];

            Debug.Log("SetLowerPipelineAsset");
            Debug.Log("currentPipelineAsset" + m_pipelinAssetState);

            // 一つ下が、使用可能で一番上のパイプラインアセットなら、そこで設定を終了する。
            if (m_pipelinAssetState == m_canUseHighestPipelinAssetState)
            {
                m_isFinishedSetting = true;
                Debug.Log("AutoSettingIsFinished");
            }

        }

        return;
    }

    void UpdateFps()
    {
        m_frameCount++;

        float time = Time.realtimeSinceStartup - m_prevTime;

        if (time >= m_kFpsUpdateInterval)
        {
            m_fps = m_frameCount / time;
            //Debug.Log(m_fps);

            m_frameCount = 0;
            m_prevTime = Time.realtimeSinceStartup;
        }

        return;
    }
}
