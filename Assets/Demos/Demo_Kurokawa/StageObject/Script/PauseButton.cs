using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PauseButton : MonoBehaviourPunCallbacks
{
    public GameObject m_pausePanel;                     //�|�[�Y���
    private bool m_isDisplayPausePanel = false;         //�|�[�Y��ʂ��o���Ă��邩�ǂ���

    // Start is called before the first frame update
    void Start()
    {
        //�|�[�Y��ʂ��I�t�ɂ���
        m_pausePanel.SetActive(false);
    }

    //�C���Q�[�����Ń|�[�Y��ʂ��I�t�ɂ���
    public void OffPausePanel()
	{
        m_isDisplayPausePanel = false;
        m_pausePanel.SetActive(false);
        Time.timeScale = 1.0f;
    }

    //���݃|�[�Y���Ă��邩�ǂ���
    public bool GetIsPause()
	{
        return m_isDisplayPausePanel;
	}

    public void OnPauseButtonClicked()
	{
        //�|�[�Y�̏�Ԃ𔽓]������
        m_isDisplayPausePanel = !m_isDisplayPausePanel;

        //�_�u���^�b�v�̏�Ԃ����Z�b�g����
        GameObject.Find("OperationSystem").GetComponent<Operation>().ResetDoubleTapParam();

        //�|�[�Y��ʂ��o���Ă���Ȃ��
        if(m_isDisplayPausePanel)
		{
            //�|�[�Y��ʂ��I���ɂ���
            m_pausePanel.SetActive(true);
            if (PhotonNetwork.OfflineMode)
			{
                Time.timeScale = 0.0f;
            }
        }
		else
		{
            m_pausePanel.SetActive(false);
            Time.timeScale = 1.0f;
		}
	}
}
