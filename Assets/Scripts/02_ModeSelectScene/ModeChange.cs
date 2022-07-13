using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//���[�h�I����ʃN���X
public class ModeChange : MonoBehaviour
{
    //���[�h��
    [SerializeField] string[] m_modeName = null;
    //���[�h���x��
    [SerializeField] Text m_modeLabel = null;
    //�I���ړ������Ă��邩
    bool m_selectMove = false;
    //�ړ����ԃJ�E���^�[
    int m_selectMoveCount = 0;
    //���[�h�^�C�v
    enum EnModeType
    {
        enOnlineMode,       //�I�����C���ΐ탂�[�h
        enCpuMode,          //CPU�ΐ탂�[�h
        enTimeAttackMode,   //�^�C���A�^�b�N���[�h
        enSettingMode,      //�ݒ胂�[�h
        enMaxModeNum        //���[�h��
    }
    //���ݑI������Ă��郂�[�h
    EnModeType m_nowSelectMode = EnModeType.enOnlineMode;
    //���[�h������
    [SerializeField] string[] m_modeExplanationSentence = null;
    //���[�h���������x��
    [SerializeField] Text m_modeExplanationLabel = null;
    //����V�X�e��
    Operation m_operation = null;

    CircleCenterRotateAround m_circleCenterRotateAround = null;

    //���[�U�[���ݒ肵�������i�[���Ēu���ۊǏꏊ
    UserSettingData m_userSettingData = null;

    void Start()
    {
        //����V�X�e���̃Q�[���I�u�W�F�N�g���������Q�[���R���|�[�l���g���擾����
        m_operation = GameObject.Find("OperationSystem").GetComponent<Operation>();

        //�~�̒��S��d�Ԃ���]����@�\�t���̃Q�[���I�u�W�F�N�g���������Q�[���R���|�[�l���g���擾����
        m_circleCenterRotateAround = GameObject.Find("Train").GetComponent<CircleCenterRotateAround>();

        ///////////////////////////
        //BGM�̍Đ�
        nsSound.BGM.Instance.SetPlayBGM(nsSound.BGMNames.m_menu);
        ///////////////////////////
    }

    //�A�b�v�f�[�g�֐�
    void Update()
    {
        //�I�����Ă��郂�[�h���ړ�������֐�
        //����͍��E�t���b�N
        ChangeSelectMode();

        //��ʂ����������ꂽ��A
        if (m_operation.GetIsLongTouch)
        {
            //���̃V�[���ɑJ�ڂ�����
            GoNextScene();
        }

        //�d�Ԃ̈ړ��ɍ��킹�đI�����Ă���f�[�^�����킹��J�E���^�[
        Count();

        //���[�h�V�[���̃e�L�X�g�Ȃǂ̃f�[�^���X�V
        ModeSceneDataUpdate();
    }

    //�I�����Ă��郂�[�h���ړ�������֐�
    void ChangeSelectMode()
    {
        if (m_selectMove)
        {
            return;
        }

        //��ʂ��E�t���b�N���ꂽ��A
        if (m_operation.GetNowOperation() == "right")
        {
            //���̃��[�h�ɑI�����ړ�
            GoNextMode();
        }
        //��ʂ����t���b�N���ꂽ��A
        if (m_operation.GetNowOperation() == "left")
        {
            //�O�̃��[�h�ɑI�����ړ�
            GoBackMode();
        }
    }

    //���̃��[�h�ɑI�����ړ�����֐�
    void GoNextMode()
    {
        //�I���ړ���Ԃɂ���
        m_selectMove = true;
        //�I������Ă��郂�[�h�����̃��[�h�ɂ���
        m_nowSelectMode++;
        if (m_nowSelectMode >= EnModeType.enMaxModeNum)
        {
            m_nowSelectMode = EnModeType.enOnlineMode;
        }
    }
    //�O�̃��[�h�ɑI�����ړ�����֐�
    void GoBackMode()
    {
        //�I���ړ���Ԃɂ���
        m_selectMove = true;
        //�I������Ă��郂�[�h��O�̃��[�h�ɂ���
        m_nowSelectMode--;
        if (m_nowSelectMode < EnModeType.enOnlineMode)
        {
            m_nowSelectMode = EnModeType.enMaxModeNum-1;
        }
    }

    //���[�h�V�[���̃e�L�X�g�Ȃǂ̃f�[�^���X�V������֐�
    void ModeSceneDataUpdate()
    {
        //���[�h�����X�V
        m_modeLabel.text = m_modeName[(int)m_nowSelectMode];
        //���[�h���������X�V
        m_modeExplanationLabel.text = m_modeExplanationSentence[(int)m_nowSelectMode];
    }

    //���̃V�[���ɑJ�ڂ�����֐�
    void GoNextScene()
    {
        //����̔����������������
        m_operation.TachDataInit();

        //���[�U�[�ݒ�f�[�^�̃Q�[���I�u�W�F�N�g���������A
        //�Q�[���R���|�[�l���g���擾����
        m_userSettingData = GameObject.Find("UserSettingDataStorageSystem").GetComponent<UserSettingData>();
        //�I�����ꂽ���[�h�f�[�^��ۑ�
        m_userSettingData.GetSetModeType = (int)m_nowSelectMode;

        //�I������Ă��郂�[�h�ɂ���ĕ���
        switch (m_nowSelectMode)
        {
            //�I�����C���ΐ탂�[�h
            case EnModeType.enOnlineMode:
                //�ݒ胂�[�h�V�[���ɑJ��
                SceneManager.LoadScene("04_CharaSelectScene");
                break;
            //CPU�ΐ탂�[�h
            case EnModeType.enCpuMode:
                //�ݒ胂�[�h�V�[���ɑJ��
                SceneManager.LoadScene("04_CharaSelectScene");
                break;
            //�^�C���A�^�b�N���[�h
            case EnModeType.enTimeAttackMode:
                //�ݒ胂�[�h�V�[���ɑJ��
                SceneManager.LoadScene("04_CharaSelectScene");
                break;
            //�ݒ胂�[�h
            case EnModeType.enSettingMode:
                //�ݒ胂�[�h�V�[���ɑJ��
                SceneManager.LoadScene("03_SettingScene");
                break;
        }
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