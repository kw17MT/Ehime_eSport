using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///�ݒ�V�[������O�̃V�[��(���[�h�I���V�[��)�ɖ߂�N���X
/// </summary>
public class ReturnPreviousScene : MonoBehaviour
{
    //����V�X�e��
    Operation m_operation = null;
    //�O�̃V�[����
    [SerializeField] string m_previousSceneName = null;
    //�ݒ�V�[�����ǂ���
    [SerializeField] bool m_isSettingScene = false;

    void Start()
    {
        //����V�X�e���̃Q�[���I�u�W�F�N�g���������Q�[���R���|�[�l���g���擾����
        m_operation = GameObject.Find("OperationSystem").GetComponent<Operation>();
    }

    //�O�̃V�[���ɖ߂�֐�
    public void GoPreviousScene()
    {
        //����̔����������������
        m_operation.TachDataInit();

        //�ݒ�V�[���̎��̂ݎ��s
        if (m_isSettingScene)
        {
            //�u���C���h���[�h���ǂ����̃f�[�^��ۑ�
            GameObject.Find("UserSettingDataStorageSystem").GetComponent<UserSettingData>().GetSetBlindMode = GameObject.Find("BlindToggle").GetComponent<ToggleOnOff>().GetToggleValue;
        }

        //���[�h�I���V�[���ɑJ��
        SceneManager.LoadScene(m_previousSceneName);

        //�f�o�b�N
        Debug.Log(m_previousSceneName + "�V�[���ɖ߂�܂�");
    }
}
