using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// �L�����N�^�[�I����ʃN���X
/// </summary>
public class CharaSelectChange : MonoBehaviour
{
    //�L�����N�^�[��
    [SerializeField] string[] m_charaName = null;
    //�L�����N�^�[�����x��
    [SerializeField] Text m_charaNameLabel = null;
    //�L�����N�^�[�X�e�[�^�X
    [SerializeField] string[] m_charaStatus = null;
    //�L�����X�e�[�^�X���x��
    [SerializeField] Text m_charaStatusLabel = null;
    //�L�����N�^�[������
    [SerializeField] string[] m_charaExplanationSentence = null;
    //�L�����N�^�[�������x��
    [SerializeField] Text m_charaExplanationLabel = null;

    enum EnCharaType
    {
        enMikyan,        //�݂����
        enKomikyan,      //�q�݂����
        enDarkmikyan,    //�_�[�N�݂����
        enMaxCharaNum    //�ő�L�����N�^�[��
    }
    //���ݑI������Ă���L�����N�^�[
    EnCharaType m_nowSelectChara = EnCharaType.enMikyan;
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
        //�I������L�����N�^�[���ړ�����
        ChangeSelectChara();

        //��ʂ����������ꂽ��A
        if (m_operation.GetIsDoubleTouch())
        {
            //���̃V�[���ɑJ�ڂ�����
            GoNextScene();
        }

        //�d�Ԃ̈ړ��ɍ��킹�đI�����Ă���f�[�^�����킹��J�E���^�[
        Count();

        //�L�����I���V�[���̃e�L�X�g�Ȃǂ̃f�[�^���X�V
        CharaSelectSceneDataUpdate();
    }

    //�I������L�����N�^�[���ړ�����֐�
    void ChangeSelectChara()
    {
        if (m_selectMove)
        {
            return;
        }
        //��ʂ��E�t���b�N���ꂽ��A
        if (m_operation.GetNowOperation() == "right")
        {
            //���̃L�����N�^�[�ɑI�����ړ�
            GoNextChara();
        }
        //��ʂ����t���b�N���ꂽ��A
        if (m_operation.GetNowOperation() == "left")
        {
            //�O�̃L�����N�^�[�ɑI�����ړ�
            GoBackChara();
        }
    }

    //���̃L�����N�^�[�ɑI�����ړ�����֐�
    void GoNextChara()
    {
        //�I���ړ���Ԃɂ���
        m_selectMove = true;
        //�I������Ă���L�����N�^�[�����̃L�����N�^�[�ɂ���
        m_nowSelectChara++;
        if (m_nowSelectChara >= EnCharaType.enMaxCharaNum)
        {
            m_nowSelectChara = EnCharaType.enMikyan;
        }
    }
    //�O�̃L�����N�^�[�ɑI�����ړ�����֐�
    void GoBackChara()
    {
        //�I���ړ���Ԃɂ���
        m_selectMove = true;
        //�I������Ă��郂�[�h��O�̃��[�h�ɂ���
        m_nowSelectChara--;
        if (m_nowSelectChara < EnCharaType.enMikyan)
        {
            m_nowSelectChara = EnCharaType.enMaxCharaNum - 1;
        }
    }

    //�L�����I���V�[���̃e�L�X�g�Ȃǂ̃f�[�^���X�V������֐�
    void CharaSelectSceneDataUpdate()
    {
        //�L�����N�^�[�����x�����X�V
        m_charaNameLabel.text = m_charaName[(int)m_nowSelectChara];
        //�L�����N�^�[�X�e�[�^�X���x�����X�V
        m_charaStatusLabel.text =
            m_charaStatus[(int)m_nowSelectChara][0] + "\n" +
            m_charaStatus[(int)m_nowSelectChara][1] + "\n" +
            m_charaStatus[(int)m_nowSelectChara][2] + "\n" +
            m_charaStatus[(int)m_nowSelectChara][3];
        //�L�����N�^�[�������x�����X�V
        m_charaExplanationLabel.text = m_charaExplanationSentence[(int)m_nowSelectChara];
    }

    //���̃V�[���ɑJ�ڂ�����֐�
    void GoNextScene()
    {
        //����̔����������������
        m_operation.TachDataInit();

        //���[�U�[�ݒ�f�[�^�̃Q�[���I�u�W�F�N�g���������A
        //�Q�[���R���|�[�l���g���擾����
        m_userSettingData = GameObject.Find("UserSettingDataStorageSystem").GetComponent<UserSettingData>();
        //�I�����ꂽ�L�����N�^�[�f�[�^��ۑ�
        m_userSettingData.GetSetCharacter = (int)m_nowSelectChara;

        //�X�e�[�W�I���V�[���ɑJ��
        SceneManager.LoadScene("05_StageSelectScene");
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

    ///////////////////////////////////////////////////////////////////////////////
    //���݂̑I�����Ă����Ԃ��擾
    public int GetNowSelectState()
    {
        return (int)m_nowSelectChara;
    }
    ///////////////////////////////////////////////////////////////////////////////
}