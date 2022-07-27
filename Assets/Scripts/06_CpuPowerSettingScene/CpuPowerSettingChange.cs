using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// CPU�̂悳�I����ʃN���X
/// </summary>
public class CpuPowerSettingChange : MonoBehaviour
{
    //CPU�̋�����
    [SerializeField] string[] m_strengthName = null;
    //CPU�̋������x��
    [SerializeField] Text m_strengthLabel = null;

    enum EnCpuPowerType
    {
        enWeak,             //�ア
        enNormal,           //����
        enStrong,           //����
        enMaxCpuPowserNum   //�ő�CPU������
    }
    //���ݑI������Ă��鋭��
    EnCpuPowerType m_nowSelectStrength = EnCpuPowerType.enWeak;
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
        //�I������CPU�̋������ړ�����
        ChangeSelectCpuPower();

        //��ʂ����������ꂽ��A
        if (m_operation.GetIsDoubleTouch())
        {
            //���̃V�[���ɑJ�ڂ�����
            GoNextScene();
        }

        //�d�Ԃ̈ړ��ɍ��킹�đI�����Ă���f�[�^�����킹��J�E���^�[
        Count();

        //CPU�̋������x�����X�V
        m_strengthLabel.text = m_strengthName[(int)m_nowSelectStrength];
    }

    //�I������CPU�̋������ړ�����
    void ChangeSelectCpuPower()
    {
        if (m_selectMove)
        {
            return;
        }
        //��ʂ��E�t���b�N���ꂽ��A
        if (m_operation.GetNowOperation() == "right")
        {
            //����CPU�̋����ɑI�����ړ�
            GoNextCpuPower();
        }
        //��ʂ����t���b�N���ꂽ��A
        if (m_operation.GetNowOperation() == "left")
        {
            //�O��CPU�̋����ɑI�����ړ�
            GoBackStage();
        }
    }

    //����CPU�̋����ɑI�����ړ�����֐�
    void GoNextCpuPower()
    {
        //�I���ړ���Ԃɂ���
        m_selectMove = true;
        //�I������Ă���CPU�̋��������̋����ɂ���
        m_nowSelectStrength++;
        if (m_nowSelectStrength >= EnCpuPowerType.enMaxCpuPowserNum)
        {
            m_nowSelectStrength = EnCpuPowerType.enWeak;
        }
    }
    //�O��CPU�̋����ɑI�����ړ�����֐�
    void GoBackStage()
    {
        //�I���ړ���Ԃɂ���
        m_selectMove = true;
        //�I������Ă���CPU�̋�����O��CPU�̋����ɂ���
        m_nowSelectStrength--;
        if (m_nowSelectStrength < EnCpuPowerType.enWeak)
        {
            m_nowSelectStrength = EnCpuPowerType.enMaxCpuPowserNum - 1;
        }
    }

    //���̃V�[���ɑJ�ڂ�����֐�
    void GoNextScene()
    {
        //����̔����������������
        m_operation.TachDataInit();

        //���[�U�[�ݒ�f�[�^�̃Q�[���I�u�W�F�N�g���������A
        //�Q�[���R���|�[�l���g���擾����
        m_userSettingData = GameObject.Find("UserSettingDataStorageSystem").GetComponent<UserSettingData>();
        //�I�����ꂽCPU�����f�[�^��ۑ�
        m_userSettingData.GetSetCpuPower = (int)m_nowSelectStrength;

        if(m_userSettingData.GetSetModeType == 0)
		{
            //�}�b�`���O�V�[���ɑJ��
            SceneManager.LoadScene("07_MatchingScene");
        }
		else if(m_userSettingData.GetSetModeType == 1)
		{
            SceneManager.LoadScene("08_GameScene");
        }
    }

    //�d�Ԃ̈ړ��ɍ��킹�đI�����Ă���f�[�^�����킹��J�E���^�[
    void Count()
    {
		////�I���ړ���Ԃ���Ȃ��Ƃ��͏��������Ȃ��B
		//if (!m_selectMove) return;

		////�J�E���g�v��
		//m_selectMoveCount++;

		////�J�E���g���w�肵�����l���傫���Ȃ�����A
		//if (m_selectMoveCount > m_circleCenterRotateAround.GetCountTime)
		//{
		//	//�I���ړ����Ă��Ȃ���Ԃɖ߂�
		//	m_selectMove = false;
		//	//�J�E���g�̏�����
		//	m_selectMoveCount = 0;
		//}
	}

    ///////////////////////////////////////////////////////////////////////////////
    //���݂̑I�����Ă����Ԃ��擾
    public int GetNowSelectState()
    {
        return (int)m_nowSelectStrength;
    }
    ///////////////////////////////////////////////////////////////////////////////
}
