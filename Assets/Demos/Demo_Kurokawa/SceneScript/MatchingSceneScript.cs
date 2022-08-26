using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

//�}�b�`���O���̋����̃N���X
public class MatchingSceneScript : MonoBehaviourPunCallbacks
{
    private GameObject m_waitTimeText = null;                       //�c��ҋ@���Ԃ�\������e�L�X�g�C���X�^���X
    private GameObject m_operation = null;                          //����Ǘ��̃C���X�^���X
    private GameObject m_paramManager = null;                       //�V�[���ȍ~�ŕێ��������p�����[�^�̕ۊǃC���X�^���X
    private GameObject m_player = null;                             //�v���C���[�C���X�^���X
    private bool m_isInstantiateAI = false;                         //AI�C���X�^���X�𐶐�������
    private bool m_isJoinedRoom = false;                            //���[���ɓ��ꂽ��
    private int m_prevMatchingWaitTime = 0;                         //�O�܂ł̎c��ҋ@���Ԃ̐�������
    private float m_matchingWaitTime = 30.0f;                       //�c��ҋ@����
    private Vector3 m_position = Vector3.zero;                      //�v���C���[��ݒ肷����W

    void Start()
    {
        //������Ď����邽�߁A����C���X�^���X���擾
        m_operation = GameObject.Find("OperationSystem");
        // PhotonServerSettings�̐ݒ���e���g���ă}�X�^�[�T�[�o�[�֐ڑ�����
        PhotonNetwork.ConnectUsingSettings();
        //�}�b�`���O�ҋ@���Ԃ�\������C���X�^���X���擾
        m_waitTimeText = GameObject.Find("LimitTimeLabel");
        //�V�[���Ԃŕێ�����p�����[�^�C���X�^���X
        m_paramManager = GameObject.Find("ParamManager");
        //�V�[���̑J�ڂ̓z�X�g�N���C�A���g�Ɉˑ�����
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    //�쐬���郋�[���̐ݒ�C���X�^���X
    RoomOptions roomOptions = new RoomOptions()
    {
        //0���Ɛl�������Ȃ�
        MaxPlayers = 4,
        //�����ɎQ���ł��邩
        IsOpen = true,
        //���̕��������r�[�Ƀ��X�g����邩
        IsVisible = true,
        //���[�U�[ID�̔��z���s���B
        PublishUserId = true,
    };

    // �}�X�^�[�T�[�o�[�ւ̐ڑ��������������ɌĂ΂��R�[���o�b�N
    public override void OnConnectedToMaster()
    {
        //�����_���ȃ��[���ɎQ������
        PhotonNetwork.JoinRandomRoom();
    }

    //��Ń����_���ȃ��[���ɎQ���ł��Ȃ�������
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //���������
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    // �Q�[���T�[�o�[�ւ̐ڑ��������������ɌĂ΂��R�[���o�b�N
    public override void OnJoinedRoom()
    {
        //���ݐڑ����Ă���v���C���[�����擾����B
        float currentPlayerNumber = (PhotonNetwork.PlayerList.Length - 1);
        //Prefab����v���C���[�����삷�郂�f���𐶐�
        m_player =  PhotonNetwork.Instantiate("Player", m_position, Quaternion.AngleAxis(180, Vector3.up));
        m_player.GetComponent<Rigidbody>().isKinematic = true;

        //���܂ł��̃��[���ɉ��l�������Ă������ŃA�N�^�[�i���o�[�������Ă����i�A�N�^�[�i���o�[�ɏ������ݕs�j
        int id = PhotonNetwork.LocalPlayer.ActorNumber;
        //�v���C���[��5�ȏ��ID������U��ꂽ���Ƃ͂ǂ����̃^�C�~���O�ň�l�ȏ㔲���Ă���
        if (id >= 5)
		{
			//���[���ɂ��鑼�̃v���C���[���擾
			Player[] otherPlayers = PhotonNetwork.PlayerListOthers;
			//���̃v���C���[�Ɋ��蓖�Ă��Ă���A�g���Ȃ����O��ID��ۑ����Ă����z����`
			var cantUseId = new List<string>();

			foreach (var pl in otherPlayers)
			{
				//���Ɏg���Ă���ID��ۑ����Ă���
				cantUseId.Add(pl.NickName);
			}

            //Player1�Ƃ������O�̃��[�U�[�����Ȃ���΁AID1���g�p����B
			if (!cantUseId.Contains("Player1"))
			{
				id = 1;
			}
			else if (!cantUseId.Contains("Player2"))
			{
				id = 2;
			}
			else if (!cantUseId.Contains("Player3"))
			{
				id = 3;
			}
			else if (!cantUseId.Contains("Player4"))
			{
				id = 4;
			}
		}
        //�v���C���[��ID���L�^����
		m_paramManager.GetComponent<ParamManage>().SetPlayerID(id);
        string spawnerName = "PlayerSpawner/Player" + id;
        m_position = GameObject.Find(spawnerName).transform.position;

        m_isJoinedRoom = true;
    }

    //�֐��̒ʐM�̍ۂɕK�v�ȕ\�L
    [PunRPC]
    //�c��ҋ@���Ԃ�\������
    void SetWaitTime(int currentTime)
	{
        if (m_waitTimeText != null)
		{
            //�e�L�X�g�̒��g���c��ҋ@���Ԃɏ���������B���l�̓z�X�g�N���C�A���g���Ōv��
            m_waitTimeText.GetComponent<Text>().text = currentTime.ToString();
            GameObject.Find("SceneManager").GetComponent<LimitTime>().SetLimitTime(currentTime);
        }
    }

    //�ҋ@���Ԃ��I���������A��x����AI��s���v���C���[����������B
    void InstantiateAIOnce()
	{
        //AI�𐶐����Ă��Ȃ����
        if (!m_isInstantiateAI)
        {
            //�ő�v���C���[���܂�AI�𐶐�
            for (int i = 0; i < 4 - PhotonNetwork.PlayerList.Length; i++)
            {
                //�v���C���[�����ɕ��ׂĂ���
                var position = new Vector3(i + 1.5f, 0.0f, 0.0f);
                //Prefab����AI�𐶐�
                //PhotonNetwork.Instantiate("AI", position, Quaternion.identity);
            }
            //AI�̐����I��
            m_isInstantiateAI = true;
            //�c��ҋ@���Ԃ̃e�L�X�g��j��
            Destroy(m_waitTimeText.gameObject);
        }
    }

    //�c��ҋ@���Ԃ𑼂̃v���C���[�Ɠ���������
    void SynchronizeWaitTime()
	{
        //�}�b�`���O�ҋ@���Ԃ��Q�[�����ԂŌ��炵�Ă���
        m_matchingWaitTime -= Time.deltaTime;
        //���݂̑ҋ@���Ԃ̐����������擾
        int currentMatchingWaitTime = (int)m_matchingWaitTime;
        //�҂����Ԃ��Ȃ��Ȃ�����
        if (m_matchingWaitTime < 0.0f)
        {
            //AI�𐶐�
            InstantiateAIOnce();
            //2�b���炢�҂��ăC���Q�[���Ɉڍs
            if (m_matchingWaitTime < -2.0f)
            {
                int stageType = GameObject.Find("UserSettingDataStorageSystem").GetComponent<UserSettingData>().GetSetStageType;
                //���[�U�[���I�����Ă������̂��L�^�����C���X�^���X���擾
                if (stageType == 0)
				{
                    //�Q�[���J�n
                    SceneManager.LoadScene("08_EasyGameScene");
                }
                else if(stageType == 1)
				{
                    //�Q�[���J�n
                    SceneManager.LoadScene("08_GameScene");
                }
            }
        }
        //�ҋ@���Ԃ̕b�����ς�����炻��𓯊�����
        if (m_prevMatchingWaitTime != currentMatchingWaitTime)
        {
            //�\�����Ԃ��X�V����悤�Ƀ��[���̑S���ɒʒm����i�����Ŏ������c��ҋ@���Ԃ��X�V�j
            photonView.RPC(nameof(SetWaitTime), RpcTarget.All, currentMatchingWaitTime);
        }

        //���݂̑ҋ@���Ԃ̐���������ۑ����Ă���
        m_prevMatchingWaitTime = currentMatchingWaitTime;
    }

    void Update()
    {
        //�z�X�g�̂ݎ��s���镔��
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            //�z�X�g�N���C�A���g���{�^���𒷉��������
            if(m_operation.GetComponent<Operation>().GetIsLongTouch
                && m_isJoinedRoom)
			{
                int stageType = GameObject.Find("UserSettingDataStorageSystem").GetComponent<UserSettingData>().GetSetStageType;
                //���[�U�[���I�����Ă������̂��L�^�����C���X�^���X���擾
                if (stageType == 0)
                {
                    //�Q�[���J�n
                    SceneManager.LoadScene("08_EasyGameScene");
                }
                else if (stageType == 1)
                {
                    //�Q�[���J�n
                    SceneManager.LoadScene("08_GameScene");
                }
            }

            //�c�莞�Ԃ𑼃v���C���[�Ɠ�������
            SynchronizeWaitTime();
        }


        if (m_player != null)
        {
            m_player.transform.position = m_position;
        }

        //Esc�������ꂽ��
        if (Input.GetKey(KeyCode.Escape))
        {

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;//�Q�[���v���C�I��
#else
            Application.Quit();//�Q�[���v���C�I��
#endif
        }
    }
}
