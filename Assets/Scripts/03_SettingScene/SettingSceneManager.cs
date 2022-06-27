using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
///�ݒ�V�[���̃��C���N���X
/// </summary>
public class SettingSceneManager : MonoBehaviour
{
    //����V�X�e��
    Operation m_operation = null;

    //�o�C�u���[�V�����g�O���̒l
    ToggleOnOff m_vibrationToggle = null;

    //BGM�̃X���C�_�[
    Slider m_bgmSlider = null;
    //SE�̃X���C�_�[
    Slider m_seSlider = null;
    //�L�����N�^�[�{�C�X�̃X���C�_�[
    Slider m_charaVoiceSlider = null;
    //�i���[�V�����{�C�X�̃X���C�_�[
    Slider m_narrationVoiceSlider = null;

    //�u���C���_�[�g�O���̒l
    ToggleOnOff m_blindToggle = null;
    //���[�U�[���ݒ肵�������i�[���Ēu���ۊǏꏊ
    UserSettingData m_userSettingData = null;

    void Start()
    {
        //�C���X�^���X�̏�����
        InitInstance();
        //�e�l��������
        InitValue();
    }

    void Update()
    {
        //��ʂ����������ꂽ��A
        if (m_operation.GetIsLongTouch)
        {
            //�O�̃V�[���ɖ߂�
            GoPreviousScene();
        }
    }

    //�O�̃V�[���ɖ߂�֐�
    void GoPreviousScene()
    {
        //����̔����������������
        m_operation.TachDataInit();

        //�e�l��ۊǂ���
        StorageData();

        //���[�h�I���V�[���ɑJ��
        SceneManager.LoadScene("02_ModeSelectScene");
    }

    //�C���X�^���X�̏�����
    void InitInstance()
    {
        //����V�X�e���̃Q�[���I�u�W�F�N�g���������Q�[���R���|�[�l���g���擾����
        m_operation = GameObject.Find("OperationSystem").GetComponent<Operation>();

        //���[�U�[�ݒ�f�[�^�̃Q�[���I�u�W�F�N�g���������A
        //�Q�[���R���|�[�l���g���擾����
        m_userSettingData = GameObject.Find("UserSettingDataStorageSystem").GetComponent<UserSettingData>();

        //BGM�X���C�_�[�̃Q�[���I�u�W�F�N�g���������Q�[���R���|�[�l���g���擾����
        m_bgmSlider = GameObject.Find("BgmVolumeSlider").GetComponent <Slider>();
        //SE�X���C�_�[�̃Q�[���I�u�W�F�N�g���������Q�[���R���|�[�l���g���擾����
        m_seSlider = GameObject.Find("SeVolumeSlider").GetComponent<Slider>();
        //�L�����N�^�[�{�C�X�X���C�_�[�̃Q�[���I�u�W�F�N�g���������Q�[���R���|�[�l���g���擾����
        m_charaVoiceSlider = GameObject.Find("CharaVoiceVolumeSlider").GetComponent<Slider>();
        //�i���[�V�����{�C�X�X���C�_�[�̃Q�[���I�u�W�F�N�g���������Q�[���R���|�[�l���g���擾����
        m_narrationVoiceSlider = GameObject.Find("NarrationVoiceVolumeSlider").GetComponent<Slider>();

        //�o�C�u���[�V�����̃I���I�t�̃g�O���̃Q�[���I�u�W�F�N�g���������Q�[���R���|�[�l���g���擾����
        m_vibrationToggle = GameObject.Find("VibrationToggle").GetComponent<ToggleOnOff>();
        //�u���C���h�؂�ւ��̃g�O���̃Q�[���I�u�W�F�N�g���������Q�[���R���|�[�l���g���擾����
        m_blindToggle = GameObject.Find("BlindToggle").GetComponent<ToggleOnOff>();
    }

    //�e�l�̏������֐�
    void InitValue()
    {
        //BGM�X���C�_�[�̒l��������
        m_bgmSlider.value = m_userSettingData.GetSetBgmVolume;
        //SE�X���C�_�[�̒l��������
        m_seSlider.value = m_userSettingData.GetSetSeVolume;
        //�L�����N�^�[�{�C�X�X���C�_�[�̒l��������
        m_charaVoiceSlider.value = m_userSettingData.GetSetCharaVoiceVolume;
        //�i���[�V�����{�C�X�X���C�_�[�̒l��������
        m_narrationVoiceSlider.value = m_userSettingData.GetSetNarrationVoiceVolume;

        //�u���C���h���[�h�̃g�O����������
        m_vibrationToggle.GetSetToggleValue = m_userSettingData.GetSetVibration;
        //�u���C���h���[�h�̃g�O����������
        m_blindToggle.GetSetToggleValue = m_userSettingData.GetSetBlindMode;
    }

    //�e�l��ۊǂ���֐�
    void StorageData()
    {
        //BGM�̒l��ۑ�
        m_userSettingData.GetSetBgmVolume = (int)m_bgmSlider.value;
        //SE�̒l��ۑ�
        m_userSettingData.GetSetSeVolume = (int)m_seSlider.value;
        //�L�����{�C�X�̒l��ۑ�
        m_userSettingData.GetSetCharaVoiceVolume = (int)m_charaVoiceSlider.value;
        //�i���[�V�����{�C�X�̒l��ۑ�
        m_userSettingData.GetSetNarrationVoiceVolume = (int)m_narrationVoiceSlider.value;
        //�o�C�u���[�V�����̃I���I�t�̃f�[�^��ۑ�
        m_userSettingData.GetSetVibration = m_vibrationToggle.GetSetToggleValue;
        //�u���C���h���[�h���ǂ����̃f�[�^��ۑ�
        m_userSettingData.GetSetBlindMode = m_blindToggle.GetSetToggleValue;
    }
}
