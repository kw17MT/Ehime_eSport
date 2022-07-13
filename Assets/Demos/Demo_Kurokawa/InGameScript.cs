using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

// MonoBehaviourPunCallbacksを継承して、PUNのコールバックを受け取れるようにする
public class InGameScript : MonoBehaviourPunCallbacks
{
    [SerializeField] Sprite[] m_countDownSprite = {null};
    private GameObject m_countDownComponent = null;                                          //カウントダウンを表示するテキストインスタンス
    private GameObject m_paramManager = null;                                           //ゲーム中に使用するパラメータ保存インスタンス
    private GameObject m_operation = null;
    private GameObject m_player = null;
    private int m_goaledPlayerNum = 0;                                                  //ゴールしたプレイヤーの数
    private int m_playerReadyNum = 0;                                                   //走行の準備ができているプレイヤーの数
    private int m_prevCountDownNum = 0;                                                 //カウントダウンしている時、前の数値の整数値
    private float m_countDownNum = 4.0f;                                                //カウントダウンする際の開始数値
    private bool m_isInstantiateAI = false;                                             //プレイヤーの不足分をAIで補ったかどうか
    private bool m_isShownResult = false;                                                 //リザルトを出しているか
    private bool m_shouldCountDown = true;                                              //カウントダウンの数字を出すか
    private bool m_canReturnModeSelection = false;
    private bool m_isBGMStart = false;
    Dictionary<string, float> m_scoreBoard = new Dictionary<string, float>();           //ゴールしたプレイヤーの名前とタイム一覧

    private List<GameObject> m_ai = new List<GameObject>();

    private const int PLAYER_ONE = 1;                                                   //シングルプレイヤーだった時に設定するプレイヤーID
    private const int AI_NUM_IN_SINGLE_PLAY = 3;                                        //シングルプレイヤーだった時のAIの数

    private GameObject m_userSetting = null;
    public GameObject m_resultBoard;

    public GameObject[] m_youLabels;

    private void Start()
    {
        //ゲーム中のパラメータ保存インスタンスを取得する
        m_paramManager = GameObject.Find("ParamManager");
        //操作インスタンスを取得
        m_operation = GameObject.Find("OperationSystem");
        //ユーザーが選択してきたものを記録したインスタンスを取得
        m_userSetting = GameObject.Find("UserSettingDataStorageSystem");

        //ゲームがオフラインで開始されたら
        if (m_userSetting.GetComponent<UserSettingData>().GetSetModeType == 1/*m_paramManager.GetComponent<ParamManage>().GetIsOfflineMode()*/)
		{
            //オフラインモードにする
            PhotonNetwork.OfflineMode = true;
            //オフラインモードなので、プレイヤーのIDを1にする
            m_paramManager.GetComponent<ParamManage>().SetPlayerID(PLAYER_ONE);
        }
        //オンラインモードならば
		else
		{
            // PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
            PhotonNetwork.ConnectUsingSettings();
            //シーン遷移をホストに同期する
            PhotonNetwork.AutomaticallySyncScene = true;
            //インゲームに遷移したら入室拒否にする。
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }

        // 自分のプレイヤーを生成するポイントをIDを使って検索する
        string spawnPointName = "PlayerSpawnPoint" + (m_paramManager.GetComponent<ParamManage>().GetPlayerID() - 1);
        //取得したスポーンポイントの座標を取得
        var position = GameObject.Find(spawnPointName).transform.position;
        //自分のプレイヤーをスポーンポイントの位置へ生成
        m_player = PhotonNetwork.Instantiate("Player", position, Quaternion.identity);
        //ホストのみ実行する部分（オフラインモードでも呼ばれる）
        if (PhotonNetwork.LocalPlayer.IsMasterClient && !PhotonNetwork.OfflineMode)
        {
            //何もポップさせていないスポーンポイントを探し、AIを生成する
            FindEmptySpawnPointAndPopAI();
            //各プレイヤーの無敵状態
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

        //カウントダウンを表示するテキストインスタンスを取得
        m_countDownComponent = GameObject.Find("CountDownImage");

        //秒数の整数部分の変化を見るために保存する。
        m_prevCountDownNum = (int)m_countDownNum;

        ////////////////////////////////////////////////////////////////////////////
        //BGMを停止
        nsSound.BGM.Instance.FadeOutStart();
        ////////////////////////////////////////////////////////////////////////////

    }

    //オフラインモードの時に使用する
    public override void OnConnectedToMaster()
    {
        //オフラインモードならば
        if(m_userSetting.GetComponent<UserSettingData>().GetSetModeType == 1/*m_paramManager.GetComponent<ParamManage>().GetIsOfflineMode()*/)
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

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        //自分のプレイヤーのウェイポイントナンバーを示すキー部分を作成
        string myWayPointNumberKey = PhotonNetwork.NickName + "WayPointNumber";
        //自分のプレイヤーのラップ数を示すキー部分を作成
        string myLapCountKey = PhotonNetwork.NickName + "LapCount";
        //自分の次のウェイポイントを取得
        int nextMyWayPoint = (PhotonNetwork.CurrentRoom.CustomProperties[myWayPointNumberKey] is int value) ? value : 0;
        //現在の自分のラップカウントを取得
        int currentMyLapCount = (PhotonNetwork.CurrentRoom.CustomProperties[myLapCountKey] is int count) ? count : 0;

        //適用する順位
        int currentPlace = 1;
        //順位を決める
        for (int i = 1; i < 5; i++)
		{
            //他のプレイヤーのラップ数取得のためのキー文字配列
            string lapCountKey = "Player" + i + "LapCount";
            //そのキーが自分であれば
            if (myLapCountKey == lapCountKey)
			{
                //順位の比較はしない
                continue;
			}

            //他のプレイヤーのラップ数
            int otherPlayerLapCount = (PhotonNetwork.CurrentRoom.CustomProperties[lapCountKey] is int rapCount) ? rapCount : 0;
            //相手のラップ数が自分より多ければ
            if (currentMyLapCount < otherPlayerLapCount)
            {
                //自分の順位を一つ下す
                currentPlace += 1;
                continue;
            }
            //相手より多く周回していたら、ウェイポイントによる順位比較をしない
			else if(currentMyLapCount > otherPlayerLapCount)
			{
                continue;
			}

            //プレイヤーＮのルームプロパティのキーを作成
            string otherPlayerWayPointName = "Player" + i + "WayPointNumber";
            //プレイヤーＮの次のウェイポイント番号を取得
            int otherPlayerWayPointNumber = (PhotonNetwork.CurrentRoom.CustomProperties[otherPlayerWayPointName] is int point) ? point : 0;
            //他のプレイヤーの方が自分より進んでいれば且つ自分or相手の次のナンバーが0（ゴール最寄り位置）でないなら
            if ((otherPlayerWayPointNumber > nextMyWayPoint && nextMyWayPoint != 0) 
                || otherPlayerWayPointNumber == 0)
			{
                currentPlace += 1;
			}
            //同一ウェイポイントを通過している場合
            else if(otherPlayerWayPointNumber == nextMyWayPoint)
			{
                //他のプレイヤーをとってくる
                foreach (Player pl in PhotonNetwork.PlayerListOthers)
                {
                    //ニックネームと同じやつなら以下の処理を行う
                    if(pl.NickName == "Player" + i)
					{
                        //そのプレイヤーのカスタムプロパティの中の次のウェイポイントへの距離を取得
                        var hashtable = new ExitGames.Client.Photon.Hashtable();
                        string key = "Player" + i + "Distance";

                        float otherPlayerDistance = (PhotonNetwork.CurrentRoom.CustomProperties[key] is float distance) ? distance : 0;

                        float myDistance = GameObject.Find("OwnPlayer").GetComponent<AvatarController>().GetDistanceToNextWayPoint();

                        //Debug.Log("myDistance " + myDistance + " / OtherDistance " + otherPlayerDistance);

                        //他のプレイヤーの方が自分より次のウェイポイントへ近づいていたら
                        if(otherPlayerDistance < myDistance)
						{
                            //自分の順位を1落とす
                            currentPlace += 1;
						}
                        break;
                    }
                }
                //該当プレイヤーがいない場合AIも検索
                foreach(GameObject ai in m_ai)
				{
                    if(ai.gameObject.GetComponent<AICommunicator>().GetAIName() == "Player" + i)
					{
                        //そのプレイヤーのカスタムプロパティの中の次のウェイポイントへの距離を取得
                        var hashtable = new ExitGames.Client.Photon.Hashtable();
                        string key = "Player" + i + "Distance";
                        float otherPlayerDistance = (PhotonNetwork.CurrentRoom.CustomProperties[key] is float distance) ? distance : 0;

                        float myDistance = GameObject.Find("OwnPlayer").GetComponent<AvatarController>().GetDistanceToNextWayPoint();

                        if(photonView.IsMine)
						{
                            //Debug.Log("Other : " + otherPlayerDistance + "My : " + myDistance);
                        }


                        //他のプレイヤーの方が自分より次のウェイポイントへ近づいていたら
                        if (ai.GetComponent<AICommunicator>().GetDistanceToNextWayPoint()/*otherPlayerDistance*/ < myDistance)
                        {
                            //自分の順位を1落とす
                            currentPlace += 1;
                        }
                        break;
                    }
				}
			}
        }

        //Debug.Log(currentPlace);
        //順位を変化させる
        GameObject.Find("RankingImage").GetComponent<NowRankingChange>().ChangeRanking(currentPlace-1);
        //自分の順位を保存
        m_paramManager.GetComponent<ParamManage>().SetPlace(currentPlace);
    }

    //オフラインのルームに入ったら
    public override void OnJoinedRoom()
    {
        //スポーンポイントの検索用名前の定義
        string spawnPointName;
        //オフラインモードのため、AIを3体用意
        for(int i = 0; i < AI_NUM_IN_SINGLE_PLAY; i++)
		{
            //AIにスポーンポイントを1から3まで順番に割り振る
            spawnPointName = "PlayerSpawnPoint" + (i + 1);
            //スポーン位置を取得
            Vector3 popPos = GameObject.Find(spawnPointName).transform.position;
            //AIを生成
            GameObject ai = PhotonNetwork.InstantiateRoomObject("AI", popPos, Quaternion.identity);
            //Playerとタグ付けする
            ai.gameObject.tag = "Player";
            ai.gameObject.name = "Player" + (i + 2);
            ai.GetComponent<AICommunicator>().SetAIName(ai.gameObject.name);
            m_ai.Add(ai);
        }
    }

    //スポーンしていないポイントを見つけ、そこにAIをスポーンさせる（オンラインモード時に使用）
    private void FindEmptySpawnPointAndPopAI()
	{
        //AIを生成していなければ
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
                    ai.GetComponent<AICommunicator>().SetAIName("Player1");
					cantUsePosition.Add("Player1");
                    m_ai.Add(ai);
                }
				else if (!cantUsePosition.Contains("Player2"))
				{
					AISpawnPoint = GameObject.Find("PlayerSpawnPoint1");
					//PrefabからAIをルームオブジェクトとして生成
					GameObject ai = PhotonNetwork.InstantiateRoomObject("AI", AISpawnPoint.transform.position, Quaternion.identity);
					ai.gameObject.tag = "Player";
                    ai.GetComponent<AICommunicator>().SetAIName("Player2");
                    cantUsePosition.Add("Player2");
                    m_ai.Add(ai);
                }
				else if (!cantUsePosition.Contains("Player3"))
				{
					AISpawnPoint = GameObject.Find("PlayerSpawnPoint2");
					//PrefabからAIをルームオブジェクトとして生成
					GameObject ai = PhotonNetwork.InstantiateRoomObject("AI", AISpawnPoint.transform.position, Quaternion.identity);
					ai.gameObject.tag = "Player";
                    ai.GetComponent<AICommunicator>().SetAIName("Player3");
                    cantUsePosition.Add("Player3");
                    m_ai.Add(ai);
                }
				else if (!cantUsePosition.Contains("Player4"))
				{
					AISpawnPoint = GameObject.Find("PlayerSpawnPoint3");
					//PrefabからAIをルームオブジェクトとして生成
					GameObject ai = PhotonNetwork.InstantiateRoomObject("AI", AISpawnPoint.transform.position, Quaternion.identity);
					ai.gameObject.tag = "Player";
                    ai.GetComponent<AICommunicator>().SetAIName("Player4");
                    cantUsePosition.Add("Player4");
                    m_ai.Add(ai);
                }
            }

			//AIを生成した。
			m_isInstantiateAI = true;
        }
    }

    //準備ができたプレイヤーの数をインクリメント（ホストプレイヤーが使用）
    public void AddReadyPlayerNum()
	{
        m_playerReadyNum++;
    }

    //ゴールしたプレイヤー名とタイムをホストに記録
    public void AddGoaledPlayerNameAndRecordTime(string playerName, float time, bool isPlayer)
    {
        //プレイヤー名をキーに、クリアタイムをバリューに
        m_scoreBoard.Add(playerName, time);
        Debug.Log(playerName + "    " + time);
        if (isPlayer)
        {
            //ゴールしたプレイヤーの総数をインクリメント
            m_goaledPlayerNum++;
        }
    }

    //カウントダウンの数値を共有する通信関数（ホストが送信）
    [PunRPC]
    private void SetCountDownTime(int countDownTime)
	{
        m_countDownComponent.GetComponent<Image>().sprite = m_countDownSprite[countDownTime];
    }

    //全てのプレイヤーに移動を許可する通信関数
    [PunRPC]
    private void SetPlayerMovable()
	{
        //自分のプレイヤーインスタンスを移動可能にする
        GameObject.Find("OwnPlayer").GetComponent<AvatarController>().SetMovable();
        for(int i = 0; i < m_ai.Count; i++)
		{
            m_ai[i].GetComponent<RaceAIScript>().SetCanMove(true);
        }
        //カウントダウンのテキストを破棄
        Destroy(m_countDownComponent.gameObject);
    }

    //各プレイヤーから送られてきたタイムを映し出す
    [PunRPC]
    private void ShowResult(Dictionary<string, float> scoreBoard)
    {
        //スコアボードを出す
        m_resultBoard.SetActive(true);
        //モード選択画面に戻れるようにする
        m_canReturnModeSelection = true;

        int labelNumber = 1;
        foreach(KeyValuePair<string, float> scores in scoreBoard)
		{
            string labelName = "Panel" + labelNumber + "/CharaNameLabel";
            GameObject.Find(labelName).GetComponent<Text>().text = scores.Key;
            labelName = "Panel" + labelNumber + "/TimeLabel";
            float time = scores.Value;
            int minute = (int)(time / 60);
            int second = (int)(time - (60 * minute));
            GameObject.Find(labelName).GetComponent<Text>().text = minute.ToString() + " : " + second.ToString();

            if(PhotonNetwork.NickName == scores.Key)
			{
                m_youLabels[labelNumber - 1].SetActive(true);
            }

            labelNumber++;
        }

        for(int i = labelNumber; i < 5; i++)
		{
            string labelName = "Panel" + i + "/CharaNameLabel";
            Text nameLabelText = GameObject.Find(labelName).GetComponent<Text>();
            if (!scoreBoard.ContainsKey("Player1"))
            {
                nameLabelText.text = "Player1";
                scoreBoard.Add("Player1", 0.0f);
            }
            else if (!scoreBoard.ContainsKey("Player2"))
            {
                nameLabelText.text = "Player2";
                scoreBoard.Add("Player2", 0.0f);
            }
            else if (!scoreBoard.ContainsKey("Player3"))
            {
                nameLabelText.text = "Player3";
                scoreBoard.Add("Player3", 0.0f);
            }
			else
			{
                nameLabelText.text = "Player4";
                scoreBoard.Add("Player4", 0.0f);
            }

            labelName = "Panel" + i + "/TimeLabel";
            GameObject.Find(labelName).GetComponent<Text>().text = "";
        }
    }

    void FixedUpdate()
	{
        if(m_isBGMStart)
		{
            /////////////////////////////////////////////////////////////////////////////////////
            //BGM再生開始
            nsSound.BGM.Instance.SetPlayBGM(nsSound.BGMNames.m_race1);
            /////////////////////////////////////////////////////////////////////////////////////
            m_isBGMStart = false;
		}

        //ホストのみ実行する部分
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            //カウントダウンすべきで、発信準備できたプレイヤーの数がルーム内のプレイヤー数と一致した時
            if (m_shouldCountDown && m_playerReadyNum == PhotonNetwork.PlayerList.Length)
            {

                //マッチング待機時間をゲーム時間で減らしていく
                m_countDownNum -= Time.deltaTime;
                //待ち時間がなくなったら
                if (m_countDownNum < 0.0f)
                {
                    //カウントダウンをやめる
                    m_shouldCountDown = false;
                    //game開始フラグを立てるように通信を送る
                    photonView.RPC(nameof(SetPlayerMovable), RpcTarget.All);

                    m_isBGMStart = true;
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
            if (m_goaledPlayerNum == PhotonNetwork.PlayerList.Length && !m_isShownResult)
            {
                //完走タイムを表示するように全員に通知
                photonView.RPC(nameof(ShowResult), RpcTarget.All, m_scoreBoard);
                //完走通知を行った
                m_isShownResult = true;
            }
        }

        //ゲームが終了していて、長押ししたら
        if(m_canReturnModeSelection && m_operation.GetComponent<Operation>().GetIsLongTouch)
		{
            //ルームから出る
            PhotonNetwork.LeaveRoom();
            //サーバーから出る
            PhotonNetwork.Disconnect();
            //モード選択シーンに遷移する
            SceneManager.LoadScene("02_ModeSelectScene");
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

		//シングルプレイモードの時
		if (PhotonNetwork.OfflineMode)
		{
			//現在の順位
			int currentPlace = 1;
			//すべてのAIと比較する
			foreach (GameObject ai in m_ai)
			{
				//AIの方が自分より周回していたら
				if (ai.GetComponent<AIProgressChecker>().GetLapCount() > m_player.GetComponent<ProgressChecker>().GetLapCount())
				{
					//順位を１下げる
					currentPlace += 1;
					continue;
				}
				//プレイヤーの方が周回していたら
				else if (ai.GetComponent<AIProgressChecker>().GetLapCount() < m_player.GetComponent<ProgressChecker>().GetLapCount())
				{
					//以下の処理をスキップ
					continue;
				}


				//AIの方がよりウェイポイントを進んでいたら
				if ((ai.GetComponent<RaceAIScript>().GetNextWayPoint() > m_player.GetComponent<WayPointChecker>().GetNextWayPointNumber()
					&& m_player.GetComponent<WayPointChecker>().GetNextWayPointNumber() != 0)
                    || ai.GetComponent<RaceAIScript>().GetNextWayPoint() == 0)
				{
					//順位を１下げる
					currentPlace += 1;
					continue;
				}
				//AIと同じ位置を目指しているならば
				else if (ai.GetComponent<RaceAIScript>().GetNextWayPoint() == m_player.GetComponent<WayPointChecker>().GetNextWayPointNumber())
				{
					//ウェイポイントとの距離がAIのほうが短ければ
					if (ai.GetComponent<AICommunicator>().GetDistanceToNextWayPoint() < m_player.GetComponent<AvatarController>().GetDistanceToNextWayPoint())
					{
						//順位を１下げる
						currentPlace += 1;
					}
				}
			}

			//順位を変化させる
			GameObject.Find("RankingImage").GetComponent<NowRankingChange>().ChangeRanking(currentPlace - 1);
			//自分の順位を保存
			m_paramManager.GetComponent<ParamManage>().SetPlace(currentPlace);
		}
	}
}
