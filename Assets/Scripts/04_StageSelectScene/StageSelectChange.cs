using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// �X�e�[�W�I����ʃN���X
/// </summary>
public class StageSelectChange : MonoBehaviour
{
    //�X�e�[�W��
    [SerializeField] string[] m_stageName = null;
    //�X�e�[�W�����x��
    [SerializeField] Text m_stageNameLabel = null;
    //��Փx�̐��摜
    [SerializeField] Image[] m_difficlutyStarImage = null;
    //�X�e�[�W���Ƃ̓�Փx(�P�F����A�Q�F����A�R�F���O)
    [SerializeField] int[] m_stageDifficluty = { 1 };
    //�X�e�[�W������
    [SerializeField] string[] m_stageExplanationSentence = null;
    //�X�e�[�W���������x��
    [SerializeField] Text m_stageExplanationLabel = null;

    enum EnStageType
    {
        enStage1,       //�X�e�[�W1
        enStage2,       //�X�e�[�W2
        enStage3,       //�X�e�[�W3
        enMaxStageNum   //�ő�X�e�[�W��
    }
    //���ݑI������Ă���X�e�[�W
    EnStageType m_nowSelectStage = EnStageType.enStage1;
    //����V�X�e��
    Operation m_operation = null;
    //�I���ړ������Ă��邩
    bool m_selectMove = false;
    //�ړ����ԃJ�E���^�[
    int m_selectMoveCount = 0;

    CircleCenterRotateAround m_circleCenterRotateAround = null;

    //���[�U�[���ݒ肵�������i�[���Ēu���ۊǏꏊ
    UserSettingData m_userSettingData = null;

    void Start()
    {
        //����V�X�e���̃Q�[���I�u�W�F�N�g���������Q�[���R���|�[�l���g���擾����
        m_operation = GameObject.Find("OperationSystem").GetComponent<Operation>();
        //�~�̒��S��d�Ԃ���]����@�\�t���̃Q�[���I�u�W�F�N�g���������Q�[���R���|�[�l���g���擾����
        m_circleCenterRotateAround = GameObject.Find("Train").GetComponent<CircleCenterRotateAround>();
    }

    //�A�b�v�f�[�g�֐�
    void Update()
    {
        //�I������X�e�[�W���ړ�����
        ChangeSelectStage();

        //��ʂ����������ꂽ��A
        if (m_operation.GetIsLongTouch)
        {
            //���̃V�[���ɑJ�ڂ�����
            GoNextScene();
        }

        //�d�Ԃ̈ړ��ɍ��킹�đI�����Ă���f�[�^�����킹��J�E���^�[
        Count();

        //�X�e�[�W�I���V�[���̃e�L�X�g�Ȃǂ̃f�[�^���X�V
        StageSceneDataUpdate();
    }

    //�I������X�e�[�W���ړ�����֐�
    void ChangeSelectStage()
    {
        if (m_selectMove)
        {
            return;
        }
        //��ʂ��E�t���b�N���ꂽ��A
        if (m_operation.GetNowOperation() == "right")
        {
            //���̃X�e�[�W�ɑI�����ړ�
            GoNextStage();
        }
        //��ʂ����t���b�N���ꂽ��A
        if (m_operation.GetNowOperation() == "left")
        {
            //�O�̃X�e�[�W�ɑI�����ړ�
            GoBackStage();
        }
    }

    //���̃X�e�[�W�ɑI�����ړ�����֐�
    void GoNextStage()
    {
        //�I���ړ���Ԃɂ���
        m_selectMove = true;
        //�I������Ă���X�e�[�W�����̃X�e�[�W�ɂ���
        m_nowSelectStage++;
        if (m_nowSelectStage >= EnStageType.enMaxStageNum)
        {
            m_nowSelectStage = EnStageType.enStage1;
        }
    }
    //�O�̃X�e�[�W�ɑI�����ړ�����֐�
    void GoBackStage()
    {
        //�I���ړ���Ԃɂ���
        m_selectMove = true;
        //�I������Ă���X�e�[�W��O�̃X�e�[�W�ɂ���
        m_nowSelectStage--;
        if (m_nowSelectStage < EnStageType.enStage1)
        {
            m_nowSelectStage = EnStageType.enMaxStageNum - 1;
        }
    }

    //�X�e�[�W�I���V�[���̃e�L�X�g�Ȃǂ̃f�[�^���X�V������֐�
    void StageSceneDataUpdate()
    {
        //��Փx�̐��摜���X�V
        for (int starArrayNum = 0; starArrayNum < 3; starArrayNum++)
        {
            if (m_stageDifficluty[(int)m_nowSelectStage] > starArrayNum)
            {
                m_difficlutyStarImage[starArrayNum].enabled = true;
            }
            else
            {
                m_difficlutyStarImage[starArrayNum].enabled = false;
            }
        }
        //�X�e�[�W�����x�����X�V
        m_stageNameLabel.text = m_stageName[(int)m_nowSelectStage];
        //�X�e�[�W���������X�V
        m_stageExplanationLabel.text = m_stageExplanationSentence[(int)m_nowSelectStage];
    }

    //���̃V�[���ɑJ�ڂ�����֐�
    void GoNextScene()
    {
        //����̔����������������
        m_operation.TachDataInit();

        //���[�U�[�ݒ�f�[�^�̃Q�[���I�u�W�F�N�g���������A
        //�Q�[���R���|�[�l���g���擾����
        m_userSettingData = GameObject.Find("UserSettingDataStorageSystem").GetComponent<UserSettingData>();
        //�I�����ꂽ�X�e�[�W�f�[�^��ۑ�
        m_userSettingData.GetSetStageType = (int)m_nowSelectStage;

        //CPU�����ݒ�I���V�[���ɑJ��
        SceneManager.LoadScene("06_CpuPowerSettingScene");
    }

    //�d�Ԃ̈ړ��ɍ��킹�đI�����Ă���f�[�^�����킹��J�E���^�[
    void Count()
    {
        //�I���ړ���Ԃ���Ȃ��Ƃ��͏��������Ȃ��B
        if (!m_selectMove) return;

        //�J�E���g�v��
        m_selectMoveCount++;

        //�J�E���g���w�肵�����l���傫���Ȃ�����A
        if (m_selectMoveCount > m_circleCenterRotateAround.GetCountTime)
        {
            //�I���ړ����Ă��Ȃ���Ԃɖ߂�
            m_selectMove = false;
            //�J�E���g�̏�����
            m_selectMoveCount = 0;
        }
    }
}