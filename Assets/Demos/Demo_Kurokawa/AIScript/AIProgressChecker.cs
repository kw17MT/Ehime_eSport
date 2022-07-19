using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AIProgressChecker : MonoBehaviourPunCallbacks
{
    private int m_lapCount = 0;
    private bool m_canGoaled = false;

    public int MAX_LAP_NUM = 3;

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
        if(m_canGoaled)
		{
            //���b�v�������Z
            m_lapCount++;

            //Debug.Log(this.gameObject.GetComponent<AICommunicator>().GetAIName() + "  Count =  " + m_lapCount);
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

            m_canGoaled = false;

            //Debug.Log(m_lapCount);
        }
	}

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
