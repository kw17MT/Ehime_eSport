using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

// MonoBehaviourPunCallbacks���p�����āAPUN�̃R�[���o�b�N���󂯎���悤�ɂ���
public class InGameScript : MonoBehaviourPunCallbacks
{
    private GameObject m_countDownText = null;                                          //�J�E���g�_�E����\������e�L�X�g�C���X�^���X
    private GameObject m_paramManager = null;                                           //�Q�[�����Ɏg�p����p�����[�^�ۑ��C���X�^���X
    private GameObject m_operation = null;
    private int m_goaledPlayerNum = 0;                                                  //�S�[�������v���C���[�̐�
    private int m_playerReadyNum = 0;                                                   //���s�̏������ł��Ă���v���C���[�̐�
    private int m_prevCountDownNum = 0;                                                 //�J�E���g�_�E�����Ă��鎞�A�O�̐��l�̐����l
    private float m_countDownNum = 4.0f;                                                //�J�E���g�_�E������ۂ̊J�n���l
    private bool m_isInstantiateAI = false;                                             //�v���C���[�̕s������AI�ŕ�������ǂ���
    private bool m_isShownResult = false;                                                 //���U���g���o���Ă��邩
    private bool m_shouldCountDown = true;                                              //�J�E���g�_�E���̐������o����
    private bool m_canReturnModeSelection = false;
    Dictionary<string, float> m_scoreBoard = new Dictionary<string, float>();           //�S�[�������v���C���[�̖��O�ƃ^�C���ꗗ

    private List<GameObject> m_ai = new List<GameObject>();

    private const int PLAYER_ONE = 1;                                                   //�V���O���v���C���[���������ɐݒ肷��v���C���[ID
    private const int AI_NUM_IN_SINGLE_PLAY = 3;                                        //�V���O���v���C���[����������AI�̐�

    private GameObject m_userSetting = null;
    public GameObject m_resultBoard;

    private void Start()
    {
        //�Q�[�����̃p�����[�^�ۑ��C���X�^���X���擾����
        m_paramManager = GameObject.Find("ParamManager");
        //����C���X�^���X���擾
        m_operation = GameObject.Find("OperationSystem");
        //���[�U�[���I�����Ă������̂��L�^�����C���X�^���X���擾
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

            hashtable.Add("Player1LapCount", 0);
            hashtable.Add("Player2LapCount", 0);
            hashtable.Add("Player3LapCount", 0);
            hashtable.Add("Player4LapCount", 0);

            hashtable.Add("Player1Distance", 0);
            hashtable.Add("Player2Distance", 0);
            hashtable.Add("Player3Distance", 0);
            hashtable.Add("Player4Distance", 0);
            PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
        }

        //�J�E���g�_�E����\������e�L�X�g�C���X�^���X���擾
        m_countDownText = GameObject.Find("CountDown");

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
        //�����̃v���C���[�̃E�F�C�|�C���g�i���o�[�������L�[�������쐬
        string myWayPointNumberKey = PhotonNetwork.NickName + "WayPointNumber";
        //�����̃v���C���[�̃��b�v���������L�[�������쐬
        string myLapCountKey = PhotonNetwork.NickName + "LapCount";
        //�����̎��̃E�F�C�|�C���g���擾
        int nextMyWayPoint = (PhotonNetwork.CurrentRoom.CustomProperties[myWayPointNumberKey] is int value) ? value : 0;
        //���݂̎����̃��b�v�J�E���g���擾
        int currentMyLapCount = (PhotonNetwork.CurrentRoom.CustomProperties[myLapCountKey] is int count) ? count : 0;

        //�K�p���鏇��
        int currentPlace = 1;
        //���ʂ����߂�
        for (int i = 1; i < 5; i++)
		{
            //���̃v���C���[�̃��b�v���擾�̂��߂̃L�[�����z��
            string lapCountKey = "Player" + i + "LapCount";
            //���̃L�[�������ł����
            if (myLapCountKey == lapCountKey)
			{
                //���ʂ̔�r�͂��Ȃ�
                continue;
			}

            //���̃v���C���[�̃��b�v��
            int otherPlayerLapCount = (PhotonNetwork.CurrentRoom.CustomProperties[lapCountKey] is int rapCount) ? rapCount : 0;
            //����̃��b�v����������葽�����
            if (currentMyLapCount < otherPlayerLapCount)
            {
                //�����̏��ʂ������
                currentPlace += 1;
                continue;
            }
            //�����葽�����񂵂Ă�����A�E�F�C�|�C���g�ɂ�鏇�ʔ�r�����Ȃ�
			else if(currentMyLapCount > otherPlayerLapCount)
			{
                continue;
			}

            //�v���C���[�m�̃��[���v���p�e�B�̃L�[���쐬
            string otherPlayerWayPointName = "Player" + i + "WayPointNumber";
            //�v���C���[�m�̎��̃E�F�C�|�C���g�ԍ����擾
            int otherPlayerWayPointNumber = (PhotonNetwork.CurrentRoom.CustomProperties[otherPlayerWayPointName] is int point) ? point : 0;
            //���̃v���C���[�̕����������i��ł���Ί�����or����̎��̃i���o�[��0�i�S�[���Ŋ��ʒu�j�łȂ��Ȃ�
            if (otherPlayerWayPointNumber > nextMyWayPoint
				&& nextMyWayPoint != 0 /*&& otherPlayerWayPointNumber != 0*/)
			{
                currentPlace += 1;

                //Debug.Log("myName = " + myLapCountKey + "��" + currentMyLapCount + "// pl2 = " + lapCountKey + "��" + otherPlayerLapCount);
			}
            //����E�F�C�|�C���g��ʉ߂��Ă���ꍇ
            else if(otherPlayerWayPointNumber == nextMyWayPoint)
			{
                //���̃v���C���[���Ƃ��Ă���
                foreach (Player pl in PhotonNetwork.PlayerListOthers)
                {
                    //�j�b�N�l�[���Ɠ�����Ȃ�ȉ��̏������s��
                    if(pl.NickName == "Player" + i)
					{
                        //���̃v���C���[�̃J�X�^���v���p�e�B�̒��̎��̃E�F�C�|�C���g�ւ̋������擾
                        var hashtable = new ExitGames.Client.Photon.Hashtable();
                        string key = "Player" + i + "Distance";

                        float otherPlayerDistance = (PhotonNetwork.CurrentRoom.CustomProperties[key] is float distance) ? distance : 0;

                        float myDistance = GameObject.Find("OwnPlayer").GetComponent<AvatarController>().GetDistanceToNextWayPoint();

                        Debug.Log("myDistance " + myDistance + " / OtherDistance " + otherPlayerDistance);

                        //���̃v���C���[�̕���������莟�̃E�F�C�|�C���g�֋߂Â��Ă�����
                        if(otherPlayerDistance < myDistance)
						{
                            //�����̏��ʂ�1���Ƃ�
                            currentPlace += 1;
						}
                        break;
                    }
                }
                //�Y���v���C���[�����Ȃ��ꍇAI������
                foreach(GameObject ai in m_ai)
				{
                    if(ai.gameObject.GetComponent<AICommunicator>().GetAIName() == "Player" + i)
					{
                        //���̃v���C���[�̃J�X�^���v���p�e�B�̒��̎��̃E�F�C�|�C���g�ւ̋������擾
                        var hashtable = new ExitGames.Client.Photon.Hashtable();
                        string key = "Player" + i + "Distance";
                        float otherPlayerDistance = (PhotonNetwork.CurrentRoom.CustomProperties[key] is float distance) ? distance : 0;

                        float myDistance = GameObject.Find("OwnPlayer").GetComponent<AvatarController>().GetDistanceToNextWayPoint();

                        if(photonView.IsMine)
						{
                            Debug.Log("Other : " + otherPlayerDistance + "My : " + myDistance);
                        }


                        //���̃v���C���[�̕���������莟�̃E�F�C�|�C���g�֋߂Â��Ă�����
                        if (ai.GetComponent<AICommunicator>().GetDistanceToNextWayPoint()/*otherPlayerDistance*/ < myDistance)
                        {
                            //�����̏��ʂ�1���Ƃ�
                            currentPlace += 1;
                        }
                        break;
                    }
				}
			}
        }

        Debug.Log(currentPlace);
        GameObject.Find("Ranking").GetComponent<NowRankingChange>().SetRanking(currentPlace);
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
    private void FindEmptySpawnPointAndPopAI()
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
                    ai.GetComponent<AICommunicator>().SetAIName("Player1");
					cantUsePosition.Add("Player1");
                    m_ai.Add(ai);
                }
				else if (!cantUsePosition.Contains("Player2"))
				{
					AISpawnPoint = GameObject.Find("PlayerSpawnPoint1");
					//Prefab����AI�����[���I�u�W�F�N�g�Ƃ��Đ���
					GameObject ai = PhotonNetwork.InstantiateRoomObject("AI", AISpawnPoint.transform.position, Quaternion.identity);
					ai.gameObject.tag = "Player";
                    ai.GetComponent<AICommunicator>().SetAIName("Player2");
                    cantUsePosition.Add("Player2");
                    m_ai.Add(ai);
                }
				else if (!cantUsePosition.Contains("Player3"))
				{
					AISpawnPoint = GameObject.Find("PlayerSpawnPoint2");
					//Prefab����AI�����[���I�u�W�F�N�g�Ƃ��Đ���
					GameObject ai = PhotonNetwork.InstantiateRoomObject("AI", AISpawnPoint.transform.position, Quaternion.identity);
					ai.gameObject.tag = "Player";
                    ai.GetComponent<AICommunicator>().SetAIName("Player3");
                    cantUsePosition.Add("Player3");
                    m_ai.Add(ai);
                }
				else if (!cantUsePosition.Contains("Player4"))
				{
					AISpawnPoint = GameObject.Find("PlayerSpawnPoint3");
					//Prefab����AI�����[���I�u�W�F�N�g�Ƃ��Đ���
					GameObject ai = PhotonNetwork.InstantiateRoomObject("AI", AISpawnPoint.transform.position, Quaternion.identity);
					ai.gameObject.tag = "Player";
                    ai.GetComponent<AICommunicator>().SetAIName("Player4");
                    cantUsePosition.Add("Player4");
                    m_ai.Add(ai);
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
    private void SetCountDownTime(int countDownTime)
	{
        m_countDownText.GetComponent<Text>().text = countDownTime.ToString();
    }

    //�S�Ẵv���C���[�Ɉړ���������ʐM�֐�
    [PunRPC]
    private void SetPlayerMovable()
	{
        //�����̃v���C���[�C���X�^���X���ړ��\�ɂ���
        GameObject.Find("OwnPlayer").GetComponent<AvatarController>().SetMovable();
        for(int i = 0; i < m_ai.Count; i++)
		{
            m_ai[i].GetComponent<RaceAIScript>().SetCanMove(true);
        }
        //�J�E���g�_�E���̃e�L�X�g��j��
        Destroy(m_countDownText.gameObject);
    }

    //�e�v���C���[���瑗���Ă����^�C�����f���o��
    [PunRPC]
    private void ShowResult(Dictionary<string, float> scoreBoard)
    {
        //�X�R�A�{�[�h���o��
        m_resultBoard.SetActive(true);
        //���[�h�I����ʂɖ߂��悤�ɂ���
        m_canReturnModeSelection = true;
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
            if (m_goaledPlayerNum == PhotonNetwork.PlayerList.Length && !m_isShownResult)
            {
                //�����^�C����\������悤�ɑS���ɒʒm
                photonView.RPC(nameof(ShowResult), RpcTarget.All, m_scoreBoard);
                //�����ʒm���s����
                m_isShownResult = true;
            }
        }

        //�Q�[�����I�����Ă��āA������������
        if(m_canReturnModeSelection && m_operation.GetComponent<Operation>().GetIsLongTouch)
		{
            //���[������o��
            PhotonNetwork.LeaveRoom();
            //�T�[�o�[����o��
            PhotonNetwork.Disconnect();
            //���[�h�I���V�[���ɑJ�ڂ���
            SceneManager.LoadScene("02_ModeSelectScene");
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
