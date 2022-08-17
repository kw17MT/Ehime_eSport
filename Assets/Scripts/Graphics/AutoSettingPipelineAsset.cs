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
    // FPS�p�ϐ�
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
        // ������l�q���������Ƃ��̃e�X�g�p�B
        //Application.targetFrameRate = 20;

        CheckCurrentPipelineAsset();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isFinishedSetting || m_enableAutoSettingPipelineAsset != true)
        {
            // �ݒ肵�I����Ă�����A�������Ȃ��B
            // �܂��́A�����ݒ肪�L���łȂ��Ȃ�A�������Ȃ��B
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
        // ���Ԋu�Ń`�F�b�N����B
        if (m_checkIntervalTimer < m_kCheckIntervalTime)
        {
            m_checkIntervalTimer += Time.deltaTime;
            return;
        }
        m_checkIntervalTimer = 0.0f;

        // ���݂�fps�����ƂɁA�p�C�v���C���A�Z�b�g��ύX������B
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
            // ����ȏ�̃p�C�v���C���A�Z�b�g���Ȃ����߁A�ݒ���I������B
            m_isFinishedSetting = true;
            Debug.Log("AutoSettingIsFinished");


            // �g�p�\�ň�ԏ�̃p�C�v���C���A�Z�b�g���L�����Ă����B
            // �i����ȏ㒲�ׂȂ����߁A�璷��������Ȃ����ǁA�ꉞ�L�����Ă����B�j
            m_canUseHighestPipelinAssetState = m_pipelinAssetState;
        }
        else
        {
            // �g�p�\�ň�ԏ�̃p�C�v���C���A�Z�b�g���L�����Ă����B
            m_canUseHighestPipelinAssetState = m_pipelinAssetState;
            // 1��̃p�C�v���C���A�Z�b�g�������Ă݂�B
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
            // ����ȉ��̃p�C�v���C���A�Z�b�g���Ȃ����߁A�ݒ���I������B
            m_isFinishedSetting = true;
            Debug.Log("AutoSettingIsFinished");
        }
        else
        {
            // ����̃p�C�v���C���A�Z�b�g�������Ă݂�B
            m_pipelinAssetState--;
            QualitySettings.renderPipeline = m_pipelineAsset[(int)m_pipelinAssetState];

            Debug.Log("SetLowerPipelineAsset");
            Debug.Log("currentPipelineAsset" + m_pipelinAssetState);

            // ������A�g�p�\�ň�ԏ�̃p�C�v���C���A�Z�b�g�Ȃ�A�����Őݒ���I������B
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
