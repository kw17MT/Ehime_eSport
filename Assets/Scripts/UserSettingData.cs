using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���[�U�[���I�񂾃A�E�g�Q�[���̃f�[�^(���[�h�A�ݒ�A�X�e�[�W�A�L�����N�^�[�ACPU�̋�����)
/// ��ۑ����C���Q�[���ɑ��邽�߂̃N���X
/// </summary>
public class UserSettingData : MonoBehaviour
{
    //���[�h
    int m_modeType = 0;
    //�u���C���h���[�h���ǂ���
    bool m_isBlindMode = false;
    //�X�e�[�W
    int m_stageType = 0;
    //�L�����N�^�[
    int m_character = 0;
    //CPU�̋���
    int m_cpuPower = 0;

    void Start()
    {
        //�V�[���J�ڂ��Ă����̑���I�u�W�F�N�g�͍폜���Ȃ�
        DontDestroyOnLoad(this);
    }

    //���[�h�^�C�v�̃v���p�e�B
    public int GetSetModeType
    {
        //�A�N�Z�b�T

        //�Q�b�^�[
        get
        {
            return m_modeType;
        }
        //�Z�b�^�[
        set
        {
            m_modeType = value;
        }
    }

    //�u���C���h���[�h�̃v���p�e�B
    public bool GetSetBlindMode
    {
        //�A�N�Z�b�T

        //�Q�b�^�[
        get
        {
            return m_isBlindMode;
        }
        //�Z�b�^�[
        set
        {
            m_isBlindMode = value;
        }
    }

    //�X�e�[�W�̃v���p�e�B
    public int GetSetStageType
    {
        //�A�N�Z�b�T

        //�Q�b�^�[
        get
        {
            return m_stageType;
        }
        //�Z�b�^�[
        set
        {
            m_stageType = value;
        }
    }

    //�L�����N�^�[�̃v���p�e�B
    public int GetSetCharacter
    {
        //�A�N�Z�b�T

        //�Q�b�^�[
        get
        {
            return m_character;
        }
        //�Z�b�^�[
        set
        {
            m_character = value;
        }
    }

    //CPU�̋����̃v���p�e�B
    public int GetSetCpuPower
    {
        //�A�N�Z�b�T

        //�Q�b�^�[
        get
        {
            return m_cpuPower;
        }
        //�Z�b�^�[
        set
        {
            m_cpuPower = value;
        }
    }

    //�ݒ肳�ꂽ�f�[�^���m�F���邽�߂�
    //�R���\�[���Ƀf�o�b�N�\������֐�
    public void IndicateDebugLog()
    {
        Debug.Log("���[�h�^�C�v�F" + m_modeType);
        Debug.Log("�u���C���h���[�h���ǂ����F" + m_isBlindMode);
        Debug.Log("�X�e�[�W�F" + m_stageType);
        Debug.Log("�L�����N�^�[�F" + m_character);
        Debug.Log("CPU�̋����F" + m_cpuPower);
    }
}