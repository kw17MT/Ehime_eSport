using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


// MonoBehaviourPunCallbacksを継承して、PUNのコールバックを受け取れるようにする
public class InGameScript : MonoBehaviourPunCallbacks
{
    private GameObject m_memberListText = null;
    private GameObject m_countDownText = null;
    private GameObject m_resultText = null;
    private GameObject m_paramManager = null;
    private float m_countDownNum = 3.0f;
    private int m_prevCountDownNum = 0;

    private int m_goaledPlayerNum = 0;
    private float[] m_playerGoaledTime = new float[4]{ 0.0f, 0.0f, 0.0f, 0.0f };
    private bool isShownResult = false;
    private bool m_shouldCountDown = true;

    Dictionary<string, float> m_scoreBoard = new Dictionary<string, float>();

    private bool m_isInstantiateAI = false;
    private int m_playerReadyNum = 0;

    private void Start()
    {
        m_paramManager = GameObject.Find("ParamManager");

        if(m_paramManager.GetComponent<ParamManage>().GetIsOfflineMode())
		{
            PhotonNetwork.OfflineMode = true;
            m_paramManager.GetComponent<ParamManage>().SetPlayerID(1);
        }
		else
		{
            // PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
            PhotonNetwork.ConnectUsingSettings();
            //シーン遷移をホストに同期する
            PhotonNetwork.AutomaticallySyncScene = true;
            //インゲームに遷移したら入室拒否にする。
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }

        int id = m_paramManager.GetComponent<ParamManage>().GetPlayerID();
        //Debug.Log("OfflineMode = " + PhotonNetwork.OfflineMode + "PlayerID = " + id);

        // 自身のアバター（ネットワークオブジェクト）を生成する
        string spawnPointName = "PlayerSpawnPoint" + (id - 1);

        GameObject spawnPoint = GameObject.Find(spawnPointName);
        //Debug.Log(spawnPoint.name);
        Debug.Log(PhotonNetwork.CurrentRoom.Name);

        var position = spawnPoint.transform.position;
        GameObject ownPlayer = PhotonNetwork.Instantiate("Player", position, Quaternion.identity);

        Debug.Log(ownPlayer.name + " " + ownPlayer.tag);

        spawnPoint.GetComponent<PlayerSpawnPoint>().SetPlayerSpawned();

        //ホストのみ実行する部分
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            Debug.Log("I am Master Client");
            //何もポップさせていないスポーンポイントを探し、AIを生成する
            FindEmptySpawnPointAndPopAI();

            var hashtable = new ExitGames.Client.Photon.Hashtable();
            hashtable.Add("Player1Invincible", 0); 
            hashtable.Add("Player2Invincible", 0); 
            hashtable.Add("Player3Invincible", 0); 
            hashtable.Add("Player4Invincible", 0);
            PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
        }

        m_memberListText = GameObject.Find("MemberList");
        m_countDownText = GameObject.Find("CountDown");
        m_resultText = GameObject.Find("Result");

        //秒数の整数部分の変化を見るために保存する。
        m_prevCountDownNum = (int)m_countDownNum;
    }

    //オフラインモードの時に使用する
    public override void OnConnectedToMaster()
    {
        if(m_paramManager.GetComponent<ParamManage>().GetIsOfflineMode())
		{
            //作成するルームの設定インスタンス
            RoomOptions roomOptions = new RoomOptions()
            {
                //0だと人数制限なし
                MaxPlayers = 1,
                //部屋に参加できるか
                IsOpen = false,
                //この部屋がロビーにリストされるか
                IsVisible = false,
                //ユーザーIDの発布を行う。
                PublishUserId = true,
            };
            //オフラインの部屋を作る
            PhotonNetwork.CreateRoom(null, roomOptions);
        }
    }

    public override void OnJoinedRoom()
    {
        string spawnPointName;
        for(int i = 0; i < 3; i++)
		{
            spawnPointName = "PlayerSpawnPoint" + (i + 1);
            Vector3 popPos = GameObject.Find(spawnPointName).transform.position;
            GameObject ai = PhotonNetwork.InstantiateRoomObject("AI", popPos, Quaternion.identity);
            ai.gameObject.tag = "Player";
        }
    }

    private void FindEmptySpawnPointAndPopAI()
	{
        if (!m_isInstantiateAI)
        {
            //ルームにいる他のプレイヤーを取得
            Player[] allPlayers = PhotonNetwork.PlayerList;
            //他のプレイヤーに割り当てられている、使えない名前とIDを保存していく配列を定義
            var cantUsePosition = new List<string>();
            foreach (var pl in allPlayers)
            {
                //既に使っているIDを保存していく
                cantUsePosition.Add(pl.NickName);
            }

			//生成しなくてはならないAIの数分回す
			for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers - PhotonNetwork.PlayerList.Length; i++)
			{
				GameObject AISpawnPoint;
				//Player1という名前のユーザーがいなければ、ID1を使用する。
				if (!cantUsePosition.Contains("Player1"))
				{
					AISpawnPoint = GameObject.Find("PlayerSpawnPoint0");
					//PrefabからAIをルームオブジェクトとして生成
					GameObject ai = PhotonNetwork.InstantiateRoomObject("AI", AISpawnPoint.transform.position, Quaternion.identity);
					ai.gameObject.tag = "Player";
					cantUsePosition.Add("Player1");
				}
				else if (!cantUsePosition.Contains("Player2"))
				{
					AISpawnPoint = GameObject.Find("PlayerSpawnPoint1");
					//PrefabからAIをルームオブジェクトとして生成
					GameObject ai = PhotonNetwork.InstantiateRoomObject("AI", AISpawnPoint.transform.position, Quaternion.identity);
					ai.gameObject.tag = "Player";
					cantUsePosition.Add("Player2");
				}
				else if (!cantUsePosition.Contains("Player3"))
				{
					AISpawnPoint = GameObject.Find("PlayerSpawnPoint2");
					//PrefabからAIをルームオブジェクトとして生成
					GameObject ai = PhotonNetwork.InstantiateRoomObject("AI", AISpawnPoint.transform.position, Quaternion.identity);
					ai.gameObject.tag = "Player";
					cantUsePosition.Add("Player3");
				}
				else if (!cantUsePosition.Contains("Player4"))
				{
					AISpawnPoint = GameObject.Find("PlayerSpawnPoint3");
					//PrefabからAIをルームオブジェクトとして生成
					GameObject ai = PhotonNetwork.InstantiateRoomObject("AI", AISpawnPoint.transform.position, Quaternion.identity);
					ai.gameObject.tag = "Player";
					cantUsePosition.Add("Player4");
				}
			}

			//AIを生成した。
			m_isInstantiateAI = true;
        }
    }

    public void AddReadyPlayerNum()
	{
        m_playerReadyNum++;
    }

    //ゴールしたプレイヤー名とタイムをホストに記録
    public void AddGoaledPlayerNameAndRecordTime(string playerName, float time)
    {
        //プレイヤー名をキーに、クリアタイムをバリューに
        m_scoreBoard.Add(playerName, time);
        //ゴールしたプレイヤーの総数をインクリメント
        this.m_goaledPlayerNum++;
    }

    [PunRPC]
    private void SetCountDownTime(int countDownTime)
	{
        m_countDownText.GetComponent<Text>().text = countDownTime.ToString();
    }

    [PunRPC]
    private void SetPlayerMovable()
	{
        GameObject.Find("OwnPlayer").GetComponent<AvatarController>().SetMovable();
        Destroy(m_countDownText.gameObject);
    }

    [PunRPC]
    private void ShowResult(Dictionary<string, float> scoreBoard)
    {
        foreach(var score in scoreBoard)
		{
            m_resultText.GetComponent<Text>().text += "1st : " + score.Key + " : " + score.Value;
        }

        //ここから下にＡＩのことを書いていく
    }

    void Update()
	{
        m_memberListText.GetComponent<Text>().text = ".+*SpecialRoomMember*+.\n";
        foreach (var player in PhotonNetwork.PlayerList)
        {
            m_memberListText.GetComponent<Text>().text += player.NickName + "\n";
        }

        //ホストのみ実行する部分
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            //Debug.Log(m_playerReadyNum + "      /      "+ PhotonNetwork.PlayerList.Length);
            if (m_shouldCountDown && m_playerReadyNum == PhotonNetwork.PlayerList.Length)
            {
                //マッチング待機時間をゲーム時間で減らしていく
                m_countDownNum -= Time.deltaTime;
                //待ち時間がなくなったら
                if (m_countDownNum < 0.0f)
                {
                    m_shouldCountDown = false;
                    //game開始フラグを立てるように通信を送る
                    photonView.RPC(nameof(SetPlayerMovable), RpcTarget.All);

                }
                //待機時間の秒数が変わったらそれを同期する
                if (m_prevCountDownNum != (int)m_countDownNum)
                {
                    //表示時間を更新するようにルームの全員に通知する（ここで自分も残り待機時間を更新）
                    photonView.RPC(nameof(SetCountDownTime), RpcTarget.All, (int)m_countDownNum);
                }

                //現在の待機時間の整数部分を保存しておく
                m_prevCountDownNum = (int)m_countDownNum;
            }

            //ゴールしたプレイヤーの数がルーム内のプレイヤーの数と一致したら
            if (m_goaledPlayerNum == PhotonNetwork.PlayerList.Length && !isShownResult)
            {
                //完走タイムを表示するように全員に通知
                photonView.RPC(nameof(ShowResult), RpcTarget.All, m_scoreBoard);
                //完走通知を行った
                isShownResult = true;
            }
        }

        //Escが押された時
        if (Input.GetKey(KeyCode.Escape))
        {

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
            Application.Quit();//ゲームプレイ終了
#endif
        }
    }
}
