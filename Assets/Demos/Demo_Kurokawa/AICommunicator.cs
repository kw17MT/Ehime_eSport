using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class AICommunicator : MonoBehaviourPunCallbacks
{
    private string m_aiName = "";                       //�ݒ肷��AI�̖��Oex.)Player2
    private bool m_isGoaled = false;                    //�S�[��������
    private bool m_isToldRecord = false;                //�z�X�g�ɃS�[���������Ԃ�`������
    private bool m_isMoving = false;                    //���݈ړ����Ă��邩�i���s���Ԃ̌v���p�t���O�j
    private bool m_isAttacked = false;                  //����AI�͍U�����ꂽ��
    private bool m_isInvincible = false;                //����AI�͖��G��Ԃ�
    private float m_runningTime = 0.0f;                 //���s���ԁ��S�[���^�C��
    private float m_frameCounter = 0.0f;                //�t���[�����Ԃ̌v���p�ϐ�
    private float UPDATE_DISTANCE_TIMING = 0.1f;        //���̃E�F�C�|�C���g�Ƃ̋������X�V����^�C�~���O
    private float m_distanceToNextWayPoint = 0.0f;      //���̃E�F�C�|�C���g�ւ̋���
    private float m_stiffinTime = 0.0f;                 //�d�����Ă��鎞��
    public float MAX_STIFFIN_TIME = 1.5f;               //�U���������������̍ő�d������

    //����AI�͍U�����ꂽ���ǂ������擾����
    public bool GetIsAttacked()
	{
        return m_isAttacked;
	}

    //����AI�͍U�����ꂽ���ǂ����ݒ肷��
    public void SetIsAttacked(bool isAttacked)
	{
        m_isAttacked = isAttacked;
	}

    //����AI�̖��G��Ԃ����擾����
    public bool GetIsInvincible()
	{
        return m_isInvincible;
	}

    //����AI�𖳓G��Ԃ��ǂ����ݒ肷��
    public void SetIsInvincible(bool isInvicible)
    {
        m_isInvincible = isInvicible;
    }

    //����AI�̎��̃E�F�C�|�C���g�ւ̋������擾����
    public float GetDistanceToNextWayPoint()
	{
        return m_distanceToNextWayPoint;
	}

    //����AI���S�[���������Ƃ�ݒ肷��
    public void SetGoaled()
	{
        m_isGoaled = true;
	}

    //����AI�̖��O���擾����
    public string GetAIName()
	{
        return m_aiName;
	}

    //����AI�̖��O��ݒ肷��
    public void SetAIName(string name)
    {
        m_aiName = name;
    }

    //����AI���ړ������ݒ肷��
    public void SetMoving(bool isMoving)
	{
        m_isMoving = isMoving;
	}

    //���̃E�F�C�|�C���g��ݒ肷��
    public void SetNextWayPoint(int number)
	{
        //�I�����C�����[�h�Ȃ��
        if (!PhotonNetwork.OfflineMode)
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
    }

    //�z�X�g�Ɏ����̖��O�ƃS�[�����Ԃ𑗐M����
    [PunRPC]
    private void TellRecordTime(string name, float time, bool isPlayer)
	{
        GameObject.Find("SceneDirector").GetComponent<InGameScript>().AddGoaledPlayerNameAndRecordTime(name, time, isPlayer);
    }

	private void FixedUpdate()
	{
        //�U������Ă�����
        if (m_isAttacked)
        {
            //�d�����Ԃ����Z
            m_stiffinTime += Time.deltaTime;
            //�d������ő厞�Ԃ���������
            if (m_stiffinTime > MAX_STIFFIN_TIME)
            {
                //�U�����ꂽ���Ƃ�����
                m_isAttacked = false;
                //�d�����ԃ��Z�b�g
                m_stiffinTime = 0.0f;
            }
        }
    }

	// Update is called once per frame
	void Update()
    {
        //����AI�������Ă��āA�S�[�����Ă��Ȃ��Ȃ��
        if (m_isMoving && !m_isGoaled)
        {
            //���s���Ԃ����Z
            m_runningTime += Time.deltaTime;
        }
        //�S�[�����Ă��āA�^�C�����z�X�g�ɑ��M���Ă��Ȃ����
        if (m_isGoaled && !m_isToldRecord)
		{
            //�����̖��O���ݒ肳��Ă�����
            if(m_aiName != "")
			{
                //�z�X�g�Ɏ����̖��O�Ǝ��Ԃ𑗐M
                photonView.RPC(nameof(TellRecordTime), RpcTarget.MasterClient, m_aiName, m_runningTime, false);
            }
            //2��ȏ㑗�M���Ȃ��悤�Ƀt���O�Ő����i�I�����C�����Ɖ�����Ă΂�Ă��܂��j
            m_isToldRecord = true;
            //Debug.Log("AI : " + m_aiName + " Clear : Time " + m_runningTime);
        }

        //�C���Q�[������
        if (SceneManager.GetActiveScene().name == "08_GameScene")
        {
            //�I�t���C�����[�h�Ȃ�΁A���t���[�����̃E�F�C�|�C���g�܂ł̈ʒu���L�^����
            if (PhotonNetwork.OfflineMode)
            {
                //���̃E�F�C�|�C���g�܂ł̋������v�Z
                Vector3 distance = this.GetComponent<WayPointChecker>().GetNextWayPoint() - this.transform.position;
                //�����o�ɋ�����ۑ�
                m_distanceToNextWayPoint = distance.magnitude;
            }
            //�I�����C�����[�h�Ȃ��
            else
            {
                //�o�ߎ��Ԃ��v������
                m_frameCounter += Time.deltaTime;
                //�Q�[���Ȃ����Ԃ���莞�Ԃ�������i���t���[���J�X�^���v���p�e�B�ɕۑ�����ƒʐM�����������j
                if (m_frameCounter >= UPDATE_DISTANCE_TIMING)
                {
                    //�v���p�e�B�̖��O
                    string key = m_aiName + "Distance";
                    //�I�����C���Ŏ擾�ł���悤�ɃJ�X�^���v���p�e�B���X�V
                    var hashtable = new ExitGames.Client.Photon.Hashtable();
                    //���̃E�F�C�|�C���g�܂ł̋������v�Z
                    Vector3 distance = this.GetComponent<WayPointChecker>().GetNextWayPoint() - this.transform.position;
                    //�����o�ɋ�����ۑ�
                    m_distanceToNextWayPoint = distance.magnitude;
                    //���̃E�F�C�|�C���g�ւ̋������v���C���[�̃J�X�^���v���p�e�B�ɕۑ�
                    hashtable[key] = distance.magnitude;
                    PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
                    //���Z�b�g
                    m_frameCounter = 0.0f;
                }
            }
        }
    }
}