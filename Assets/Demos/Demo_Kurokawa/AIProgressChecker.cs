using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AIProgressChecker : MonoBehaviourPunCallbacks
{
    private int m_lapNum = 0;
    private bool m_canGoaled = false;

    public int MAX_LAP_NUM = 3;

    public void SetCanGoaled()
	{
        m_canGoaled = true;
	}

    public void CheckCanGoal()
	{
        if(m_canGoaled)
		{
            //���b�v�������Z
            m_lapNum++;

            Debug.Log(this.gameObject.GetComponent<AICommunicator>().GetAIName() + "  Count =  " + m_lapNum);
            //���̃E�F�C�|�C���g�̔ԍ������[���v���p�e�B�ɕۑ�
            var hashtable = new ExitGames.Client.Photon.Hashtable();
            //�v���C���[���{LapCount�Ƃ������O���쐬 ex.)Player2LapCount
            string name = this.gameObject.GetComponent<AICommunicator>().GetAIName() + "LapCount";
            //�E�F�C�|�C���g�ԍ���ݒ�
            hashtable[name] = m_lapNum;
            //���[���v���p�e�B�̍X�V
            PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);

            if (m_lapNum >= MAX_LAP_NUM)
			{
                this.GetComponent<AICommunicator>().SetGoaled();
			}

            m_canGoaled = false;

            Debug.Log(m_lapNum);
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
