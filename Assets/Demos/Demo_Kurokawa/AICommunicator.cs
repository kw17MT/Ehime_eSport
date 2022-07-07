using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class AICommunicator : MonoBehaviourPunCallbacks
{
    private string m_aiName = "";
    private float m_runningTime = 0.0f;
    private bool m_isGoaled = false;
    private bool m_isToldRecord = false;
    private bool m_isMoving = false;
    private float m_frameCounter = 0.0f;
    private float UPDATE_DISTANCE_TIMING = 0.5f;        //���̃E�F�C�|�C���g�Ƃ̋������X�V����^�C�~���O

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetGoaled()
	{
        m_isGoaled = true;
	}
    public void SetAIName(string name)
	{
        m_aiName = name;
	}

    public string GetAIName()
	{
        return m_aiName;
	}

    public void SetMoving(bool isMoving)
	{
        m_isMoving = isMoving;
	}

    public void SetNextWayPoint(int number)
	{
        //���̃E�F�C�|�C���g�̔ԍ������[���v���p�e�B�ɕۑ�
        var hashtable = new ExitGames.Client.Photon.Hashtable();
        //�v���C���[���{WayPointNumber�Ƃ������O���쐬 ex.)Player2WayPointNumber
        string name = m_aiName + "WayPointNumber";
        //�E�F�C�|�C���g�ԍ���ݒ�
        hashtable[name] = number;
        //���[���v���p�e�B�̍X�V
        PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isMoving && !m_isGoaled)
        {
            m_runningTime += Time.deltaTime;
        }

        if (m_isGoaled && !m_isToldRecord)
		{
            m_isToldRecord = true;
            photonView.RPC(nameof(AvatarController.TellRecordTime), RpcTarget.MasterClient, m_aiName, m_runningTime);
            Debug.Log("AI : " + m_aiName + " Clear : Time " + m_runningTime);
        }

        //�C���Q�[���Ȃ��
        if (SceneManager.GetActiveScene().name == "08_GameScene")
        {
            //�o�ߎ��Ԃ��v������
            m_frameCounter += Time.deltaTime;
            //�Q�[���Ȃ����Ԃ���莞�Ԃ�������
            if (m_frameCounter >= UPDATE_DISTANCE_TIMING)
            {
                //�v���p�e�B�̖��O
                string key = m_aiName + "Distance";
                //�I�����C���Ŏ擾�ł���悤�ɃJ�X�^���v���p�e�B���X�V
                var hashtable = new ExitGames.Client.Photon.Hashtable();
                //���̃E�F�C�|�C���g�ւ̋������v���C���[�̃J�X�^���v���p�e�B�ɕۑ�
                hashtable[key] = this.GetComponent<WayPointChecker>().GetNextWayPoint() - this.transform.position;
                PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);

                //���Z�b�g
                m_frameCounter = 0.0f;
            }
        }
    }
}