using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AIProgressChecker : MonoBehaviourPunCallbacks
{
    private int m_lapCount = 0;                     //AI�̎���
    private bool m_canGoaled = false;               //AI�̓S�[���ł��邩
    [SerializeField] int MAX_LAP_NUM = 3;                     //AI�̃S�[�����b�v��

    public void SetCanGoaled()
	{
        m_canGoaled = true;
	}

    public int GetLapCount()
	{
        return m_lapCount;
	}
    public void CheckCanGoal()
	{
        //�S�[���ł���Ȃ��
        if(m_canGoaled)
		{
            //���b�v�������Z
            m_lapCount++;
            if (!PhotonNetwork.OfflineMode)
            {
                //���̃E�F�C�|�C���g�̔ԍ������[���v���p�e�B�ɕۑ�
                var hashtable = new ExitGames.Client.Photon.Hashtable();
                //�v���C���[���{LapCount�Ƃ������O���쐬 ex.)Player2LapCount
                string name = this.gameObject.GetComponent<AICommunicator>().GetAIName() + "LapCount";
                //�E�F�C�|�C���g�ԍ���ݒ�
                hashtable[name] = m_lapCount;
                //���[���v���p�e�B�̍X�V
                PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
            }

            if (m_lapCount >= MAX_LAP_NUM)
			{
                this.GetComponent<AICommunicator>().SetGoaled();
			}
            //���̃S�[������̂��߂Ƀt���O���I�t
            m_canGoaled = false;
        }
	}
}
