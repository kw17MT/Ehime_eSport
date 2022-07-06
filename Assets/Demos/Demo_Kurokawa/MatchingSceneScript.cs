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
    GameObject m_waitTimeText = null;                       //�c��ҋ@���Ԃ�\������e�L�X�g�C���X�^���X
    GameObject m_operation = null;                          //����Ǘ��̃C���X�^���X
    GameObject m_paramManager = null;                       //�V�[���ȍ~�ŕێ��������p�����[�^�̕ۊǃC���X�^���X

    int m_prevMatchingWaitTime = 0;                         //�O�܂ł̎c��ҋ@���Ԃ̐�������
    float m_matchingWaitTime = 30.0f;                      //�c��ҋ@����
    bool m_isInstantiateAI = false;                         //AI�C���X�^���X�𐶐�������

    void Start()
    {
        //������Ď����邽�߁A����C���X�^���X���擾
        m_operation = GameObject.Find("OperationSystem");
        // PhotonServerSettings�̐ݒ���e���g���ă}�X�^�[�T�[�o�[�֐ڑ�����
        PhotonNetwork.ConnectUsingSettings();
        //�}�b�`���O�ҋ@���Ԃ�\������C���X�^���X���擾
        m_waitTimeText = GameObject.Find("LimitTimeLabel");
        if(m_waitTimeText != null)
		{
            Debug.Log("NULL WAIT TIME");
		}
        //�V�[���Ԃŕێ�����p�����[�^�C���X�^���X
        m_paramManager = GameObject.Find("ParamManager");
        //�V�[���̑J�ڂ̓z�X�g�N���C�A���g�Ɉˑ�����
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    //�v���C���[�����[���ɓ�������
    public override void OnPlayerEnteredRoom(Player newPlayer)
	{

    }

    //�v���C���[�����[������o�Ă�����
    public override void OnPlayerLeftRoom(Player player)
	{

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
        //�v���C���[�����ɕ��ׂĂ���
        var position = new Vector3(currentPlayerNumber * 1.5f, 0.0f, 0.0f);
        //Prefab����v���C���[�����삷�郂�f���𐶐�
        var player =  PhotonNetwork.Instantiate("Player", position, Quaternion.identity);

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
                PhotonNetwork.Instantiate("AI", position, Quaternion.identity);
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
                //�Q�[���J�n
                SceneManager.LoadScene("08_GameScene");
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
            if(m_operation.GetComponent<Operation>().GetIsLongTouch)
			{
                //�����I�ɃC���Q�[���ɑJ�ڂ���
                SceneManager.LoadScene("08_GameScene");
            }

            //�c�莞�Ԃ𑼃v���C���[�Ɠ�������
            SynchronizeWaitTime();
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
