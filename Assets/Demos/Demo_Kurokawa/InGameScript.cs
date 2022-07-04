using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


// MonoBehaviourPunCallbacks���p�����āAPUN�̃R�[���o�b�N���󂯎���悤�ɂ���
public class InGameScript : MonoBehaviourPunCallbacks
{
    CountDownSpriteChange m_countDownImage = null;                                          //�J�E���g�_�E����\������e�L�X�g�C���X�^���X
    GameObject m_paramManager = null;                                           //�Q�[�����Ɏg�p����p�����[�^�ۑ��C���X�^���X
    int m_goaledPlayerNum = 0;                                                  //�S�[�������v���C���[�̐�
    int m_playerReadyNum = 0;                                                   //���s�̏������ł��Ă���v���C���[�̐�
    int m_prevCountDownNum = 0;                                                 //�J�E���g�_�E�����Ă��鎞�A�O�̐��l�̐����l
    float m_countDownNum = 4.0f;                                                //�J�E���g�_�E������ۂ̊J�n���l
    bool m_isInstantiateAI = false;                                             //�v���C���[�̕s������AI�ŕ�������ǂ���
    bool isShownResult = false;                                                 //���U���g���o���Ă��邩
    bool m_shouldCountDown = true;                                              //�J�E���g�_�E���̐������o����
    Dictionary<string, float> m_scoreBoard = new Dictionary<string, float>();           //�S�[�������v���C���[�̖��O�ƃ^�C���ꗗ

    const int PLAYER_ONE = 1;                                                   //�V���O���v���C���[���������ɐݒ肷��v���C���[ID
    const int AI_NUM_IN_SINGLE_PLAY = 3;                                        //�V���O���v���C���[����������AI�̐�

    GameObject m_userSetting = null;
    public GameObject m_resultBoard;

    void Start()
    {
        //�Q�[�����̃p�����[�^�ۑ��C���X�^���X���擾����
        m_paramManager = GameObject.Find("ParamManager");

        m_userSetting = GameObject.Find("UserSettingDataStorageSystem");

        //�Q�[�����I�t���C���ŊJ�n���ꂽ��
        if (m_userSetting.GetComponent<UserSettingData>().GetSetModeType == 1/*m_paramManager.GetComponent<ParamManage>().GetIsOfflineMode()*/)
		{
            //�I�t���C�����[�h�ɂ���
            PhotonNetwork.OfflineMode = true;
            //�I�t���C�����[�h�Ȃ̂ŁA�v���C���[��ID��1�ɂ���
            m_paramManager.GetComponent<ParamManage>().SetPlayerID(PLAYER_ONE);
        }
        //�I�����C�����[�h�Ȃ��
		else
		{
            // PhotonServerSettings�̐ݒ���e���g���ă}�X�^�[�T�[�o�[�֐ڑ�����
            PhotonNetwork.ConnectUsingSettings();
            //�V�[���J�ڂ��z�X�g�ɓ�������
            PhotonNetwork.AutomaticallySyncScene = true;
            //�C���Q�[���ɑJ�ڂ�����������ۂɂ���B
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }

        // �����̃v���C���[�𐶐�����|�C���g��ID���g���Č�������
        string spawnPointName = "PlayerSpawnPoint" + (m_paramManager.GetComponent<ParamManage>().GetPlayerID() - 1);
        //�擾�����X�|�[���|�C���g�̍��W���擾
        var position = GameObject.Find(spawnPointName).transform.position;
        //�����̃v���C���[���X�|�[���|�C���g�̈ʒu�֐���
        PhotonNetwork.Instantiate("Player", position, Quaternion.identity);
        //�z�X�g�̂ݎ��s���镔���i�I�t���C�����[�h�ł��Ă΂��j
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            //�����|�b�v�����Ă��Ȃ��X�|�[���|�C���g��T���AAI�𐶐�����
            FindEmptySpawnPointAndPopAI();
            //�e�v���C���[�̖��G���
            var hashtable = new ExitGames.Client.Photon.Hashtable();
            hashtable.Add("Player1Invincible", 0);
            hashtable.Add("Player2Invincible", 0);
            hashtable.Add("Player3Invincible", 0);
            hashtable.Add("Player4Invincible", 0);

            hashtable.Add("Player1WayPointNumber", 0);
            hashtable.Add("Player2WayPointNumber", 0);
            hashtable.Add("Player3WayPointNumber", 0);
            hashtable.Add("Player4WayPointNumber", 0);

            hashtable.Add("Player1RapCount", 0);
            hashtable.Add("Player2RapCount", 0);
            hashtable.Add("Player3RapCount", 0);
            hashtable.Add("Player4RapCount", 0);
            PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
        }

        //�J�E���g�_�E����\������e�L�X�g�C���X�^���X���擾
        m_countDownImage = GameObject.Find("CountDownImage").GetComponent<CountDownSpriteChange>();

        //�b���̐��������̕ω������邽�߂ɕۑ�����B
        m_prevCountDownNum = (int)m_countDownNum;
    }

    //�I�t���C�����[�h�̎��Ɏg�p����
    public override void OnConnectedToMaster()
    {
        //�I�t���C�����[�h�Ȃ��
        if(m_userSetting.GetComponent<UserSettingData>().GetSetModeType == 1/*m_paramManager.GetComponent<ParamManage>().GetIsOfflineMode()*/)
		{
            //�쐬���郋�[���̐ݒ�C���X�^���X
            RoomOptions roomOptions = new RoomOptions()
            {
                //0���Ɛl�������Ȃ�
                MaxPlayers = 1,
                //�����ɎQ���ł��邩
                IsOpen = false,
                //���̕��������r�[�Ƀ��X�g����邩
                IsVisible = false,
                //���[�U�[ID�̔��z���s���B
                PublishUserId = true,
            };
            //�I�t���C���̕��������
            PhotonNetwork.CreateRoom(null, roomOptions);
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        //�����̃v���C���[�̖��G��Ԃ������L�[�������쐬
        string myName = PhotonNetwork.NickName + "WayPointNumber";
        string myRapCount = PhotonNetwork.NickName + "RapCount";
        //�����̎��̃E�F�C�|�C���g���擾
        int nextwayPoint = (PhotonNetwork.CurrentRoom.CustomProperties[myName] is int value) ? value : 0;
        int currentMyRapCount = (PhotonNetwork.CurrentRoom.CustomProperties[myRapCount] is int count) ? count : 0;
        int currentPlace = 1;
        //���ʂ����߂�
        for (int i = 1; i < 5; i++)
		{
            //�v���C���[�m�̃��[���v���p�e�B�̃L�[���쐬
            string wayPointName = "Player" + i + "WayPointNumber";
            //���̃L�[�������ł����
            if(myName == wayPointName)
			{
                //���ʂ̔�r�͂��Ȃ�
                continue;
			}
            //�v���C���[�m�̎��̃E�F�C�|�C���g�ԍ����擾
            int wayPointNumber = (PhotonNetwork.CurrentRoom.CustomProperties[wayPointName] is int point) ? point : 0;
            //���̃v���C���[�̕����������i��ł����
            if(wayPointNumber > nextwayPoint)
			{
                string rapCountKey = "Player" + i + "RapCount";
                int otherRapCount = (PhotonNetwork.CurrentRoom.CustomProperties[rapCountKey] is int rapCount) ? rapCount : 0;
                //����̃��b�v�����������
                if(currentMyRapCount <= otherRapCount)
				{
                    //�����̏��ʂ������
                    currentPlace += 1;
                }
			}
        }

        //���ʂ̕\����ύX
        GameObject.Find("NowRankingImage").GetComponent<NowRankingChange>().ChangeRanking(currentPlace-1);
        //�����̏��ʂ�ۑ�
        m_paramManager.GetComponent<ParamManage>().SetPlace(currentPlace);
    }

    //�I�t���C���̃��[���ɓ�������
    public override void OnJoinedRoom()
    {
        //�X�|�[���|�C���g�̌����p���O�̒�`
        string spawnPointName;
        //�I�t���C�����[�h�̂��߁AAI��3�̗p��
        for(int i = 0; i < AI_NUM_IN_SINGLE_PLAY; i++)
		{
            //AI�ɃX�|�[���|�C���g��1����3�܂ŏ��ԂɊ���U��
            spawnPointName = "PlayerSpawnPoint" + (i + 1);
            //�X�|�[���ʒu���擾
            Vector3 popPos = GameObject.Find(spawnPointName).transform.position;
            //AI�𐶐�
            GameObject ai = PhotonNetwork.InstantiateRoomObject("AI", popPos, Quaternion.identity);
            //Player�ƃ^�O�t������
            ai.gameObject.tag = "Player";
        }
    }

    //�X�|�[�����Ă��Ȃ��|�C���g�������A������AI���X�|�[��������i�I�����C�����[�h���Ɏg�p�j
    void FindEmptySpawnPointAndPopAI()
	{
        //AI�𐶐����Ă��Ȃ����
        if (!m_isInstantiateAI)
        {
            //���[���ɂ��鑼�̃v���C���[���擾
            Player[] allPlayers = PhotonNetwork.PlayerList;
            //���̃v���C���[�Ɋ��蓖�Ă��Ă���A�g���Ȃ����O��ID��ۑ����Ă����z����`
            var cantUsePosition = new List<string>();
            foreach (var pl in allPlayers)
            {
                //���Ɏg���Ă���ID��ۑ����Ă���
                cantUsePosition.Add(pl.NickName);
            }

			//�������Ȃ��Ă͂Ȃ�Ȃ�AI�̐�����
			for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers - PhotonNetwork.PlayerList.Length; i++)
			{
				GameObject AISpawnPoint;
                //Player1�Ƃ������O�̃��[�U�[�����Ȃ���΁AID1���g�p����B
                if (!cantUsePosition.Contains("Player1"))
				{
					AISpawnPoint = GameObject.Find("PlayerSpawnPoint0");
					//Prefab����AI�����[���I�u�W�F�N�g�Ƃ��Đ���
					GameObject ai = PhotonNetwork.InstantiateRoomObject("AI", AISpawnPoint.transform.position, Quaternion.identity);
					ai.gameObject.tag = "Player";
					cantUsePosition.Add("Player1");
				}
				else if (!cantUsePosition.Contains("Player2"))
				{
					AISpawnPoint = GameObject.Find("PlayerSpawnPoint1");
					//Prefab����AI�����[���I�u�W�F�N�g�Ƃ��Đ���
					GameObject ai = PhotonNetwork.InstantiateRoomObject("AI", AISpawnPoint.transform.position, Quaternion.identity);
					ai.gameObject.tag = "Player";
					cantUsePosition.Add("Player2");
				}
				else if (!cantUsePosition.Contains("Player3"))
				{
					AISpawnPoint = GameObject.Find("PlayerSpawnPoint2");
					//Prefab����AI�����[���I�u�W�F�N�g�Ƃ��Đ���
					GameObject ai = PhotonNetwork.InstantiateRoomObject("AI", AISpawnPoint.transform.position, Quaternion.identity);
					ai.gameObject.tag = "Player";
					cantUsePosition.Add("Player3");
				}
				else if (!cantUsePosition.Contains("Player4"))
				{
					AISpawnPoint = GameObject.Find("PlayerSpawnPoint3");
					//Prefab����AI�����[���I�u�W�F�N�g�Ƃ��Đ���
					GameObject ai = PhotonNetwork.InstantiateRoomObject("AI", AISpawnPoint.transform.position, Quaternion.identity);
					ai.gameObject.tag = "Player";
					cantUsePosition.Add("Player4");
				}
			}

			//AI�𐶐������B
			m_isInstantiateAI = true;
        }
    }

    //�������ł����v���C���[�̐����C���N�������g�i�z�X�g�v���C���[���g�p�j
    public void AddReadyPlayerNum()
	{
        m_playerReadyNum++;
    }

    //�S�[�������v���C���[���ƃ^�C�����z�X�g�ɋL�^
    public void AddGoaledPlayerNameAndRecordTime(string playerName, float time)
    {
        //�v���C���[�����L�[�ɁA�N���A�^�C�����o�����[��
        m_scoreBoard.Add(playerName, time);
        //�S�[�������v���C���[�̑������C���N�������g
        this.m_goaledPlayerNum++;
    }

    //�J�E���g�_�E���̐��l�����L����ʐM�֐��i�z�X�g�����M�j
    [PunRPC]
    void SetCountDownTime(int countDownTime)
	{
        m_countDownImage.ChangeCountDownSprite(countDownTime);
    }

    //�S�Ẵv���C���[�Ɉړ���������ʐM�֐�
    [PunRPC]
    void SetPlayerMovable()
	{
        //�����̃v���C���[�C���X�^���X���ړ��\�ɂ���
        GameObject.Find("OwnPlayer").GetComponent<AvatarController>().SetMovable();
        //�J�E���g�_�E���̃e�L�X�g��j��
        Destroy(m_countDownImage.gameObject);
    }

    //�e�v���C���[���瑗���Ă����^�C�����f���o��
    [PunRPC]
    void ShowResult(Dictionary<string, float> scoreBoard)
    {
        m_resultBoard.SetActive(true);
        //�������牺�ɂ`�h�̂��Ƃ������Ă���
    }

    void Update()
	{
        //�z�X�g�̂ݎ��s���镔��
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            //�J�E���g�_�E�����ׂ��ŁA���M�����ł����v���C���[�̐������[�����̃v���C���[���ƈ�v������
            if (m_shouldCountDown && m_playerReadyNum == PhotonNetwork.PlayerList.Length)
            {

                //�}�b�`���O�ҋ@���Ԃ��Q�[�����ԂŌ��炵�Ă���
                m_countDownNum -= Time.deltaTime;
                //�҂����Ԃ��Ȃ��Ȃ�����
                if (m_countDownNum < 0.0f)
                {
                    //�J�E���g�_�E������߂�
                    m_shouldCountDown = false;
                    //game�J�n�t���O�𗧂Ă�悤�ɒʐM�𑗂�
                    photonView.RPC(nameof(SetPlayerMovable), RpcTarget.All);

                }
                //�ҋ@���Ԃ̕b�����ς�����炻��𓯊�����
                if (m_prevCountDownNum != (int)m_countDownNum)
                {
                    //�\�����Ԃ��X�V����悤�Ƀ��[���̑S���ɒʒm����i�����Ŏ������c��ҋ@���Ԃ��X�V�j
                    photonView.RPC(nameof(SetCountDownTime), RpcTarget.All, (int)m_countDownNum);
                }

                //���݂̑ҋ@���Ԃ̐���������ۑ����Ă���
                m_prevCountDownNum = (int)m_countDownNum;
            }

            //�S�[�������v���C���[�̐������[�����̃v���C���[�̐��ƈ�v������
            if (m_goaledPlayerNum == PhotonNetwork.PlayerList.Length && !isShownResult)
            {
                //�����^�C����\������悤�ɑS���ɒʒm
                photonView.RPC(nameof(ShowResult), RpcTarget.All, m_scoreBoard);
                //�����ʒm���s����
                isShownResult = true;
            }
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
