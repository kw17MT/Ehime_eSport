using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatchData : MonoBehaviour
{
    //���[�U�[���ݒ肵�������i�[���Ēu���ۊǏꏊ
    UserSettingData m_userSettingData = null;
    //�u���C���_�[�̍��摜
    Image m_blinderPanelImage = null;

    void Start()
    {
        //���[�U�[�ݒ�f�[�^�̃Q�[���I�u�W�F�N�g���������A
        //�Q�[���R���|�[�l���g���擾����
        m_userSettingData = GameObject.Find("UserSettingDataStorageSystem").GetComponent<UserSettingData>();
        //���܂ŕۑ����ꂽ�f�[�^���R���\�[���Ƀf�o�b�N�\��
        m_userSettingData.IndicateDebugLog();

        //�u���C���h���[�h��ON�̂Ƃ��A
        if(m_userSettingData.GetSetBlindMode)
        {
            //�u���C���_�[�̃Q�[���I�u�W�F�N�g���擾
            m_blinderPanelImage = GameObject.Find("BlinderPanel").GetComponent<Image>();
            //�u���C���_�[��\��������
            m_blinderPanelImage.enabled = true;
        }
    }
}
