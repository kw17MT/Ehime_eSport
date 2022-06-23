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
    private GameObject m_countDownText = null;                                          //カウントダウンを表示するテキストインスタンス
    private GameObject m_paramManager = null;                                           //ゲーム中に使用するパラメータ保存インスタンス
    private int m_goaledPlayerNum = 0;                                                  //ゴールしたプレイヤーの数
    private int m_playerReadyNum = 0;                                                   //走行の準備ができているプレイヤーの数
    private int m_prevCountDownNum = 0;                                                 //カウントダウンしている時、前の数値の整数値
    private float m_countDownNum = 4.0f;                                                //カウントダウンする際の開始数値
    private bool m_isInstantiateAI = false;                                             //プレイヤーの不足分をAIで補ったかどうか
    private bool isShownResult = false;                                                 //リザルトを出しているか
    private bool m_shouldCountDown = true;                                              //カウントダウンの数字を出すか
    Dictionary<string, float> m_scoreBoard = new Dictionary<string, float>();           //ゴールしたプレイヤーの名前とタイム一覧

    private const int PLAYER_ONE = 1;                                                   //シングルプレイヤーだった時に設定するプレイヤーID
    private const int AI_NUM_IN_SINGLE_PLAY = 3;                                        //シングルプレイヤーだった時のAIの数

    private GameObject m_userSetting = null;
    public GameObject m_resultBoard;

    private void Start()
    {
        //ゲーム中のパラメータ保存インスタンスを取得する
        m_paramManager = GameObject.Find("ParamManager");

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
        PhotonNetwork.Instantiate("Player", position, Quaternion.identity);
        //ホストのみ実行する部分（オフラインモードでも呼ばれる）
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
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

            hashtable.Add("Player1RapCount", 0);
            hashtable.Add("Player2RapCount", 0);
            hashtable.Add("Player3RapCount", 0);
            hashtable.Add("Player4RapCount", 0);
            PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
        }

        //カウントダウンを表示するテキストインスタンスを取得
        m_countDownText = GameObject.Find("CountDown");

        //秒数の整数部分の変化を見るために保存する。
        m_prevCountDownNum = (int)m_countDownNum;
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
        //自分のプレイヤーの無敵状態を示すキー部分を作成
        string myName = PhotonNetwork.NickName + "WayPointNumber";
        string myRapCount = PhotonNetwork.NickName + "RapCount";
        //自分の次のウェイポイントを取得
        int nextwayPoint = (PhotonNetwork.CurrentRoom.CustomProperties[myName] is int value) ? value : 0;
        int currentMyRapCount = (PhotonNetwork.CurrentRoom.CustomProperties[myRapCount] is int count) ? count : 0;
        int currentPlace = 1;
        //順位を決める
        for (int i = 1; i < 5; i++)
		{
            //プレイヤーＮのルームプロパティのキーを作成
            string wayPointName = "Player" + i + "WayPointNumber";
            //そのキーが自分であれば
            if(myName == wayPointName)
			{
                //順位の比較はしない
                continue;
			}
            //プレイヤーＮの次のウェイポイント番号を取得
            int wayPointNumber = (PhotonNetwork.CurrentRoom.CustomProperties[wayPointName] is int point) ? point : 0;
            //他のプレイヤーの方が自分より進んでいれば
            if(wayPointNumber > nextwayPoint)
			{
                string rapCountKey = "Player" + i + "RapCount";
                int otherRapCount = (PhotonNetwork.CurrentRoom.CustomProperties[rapCountKey] is int rapCount) ? rapCount : 0;
                //相手のラップ数が多ければ
                if(currentMyRapCount <= otherRapCount)
				{
                    //自分の順位を一つ下す
                    currentPlace += 1;
                }
			}
        }

        GameObject.Find("Ranking").GetComponent<NowRankingChange>().SetRanking(currentPlace);
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

    //準備ができたプレイヤーの数をインクリメント（ホストプレイヤーが使用）
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

    //カウントダウンの数値を共有する通信関数（ホストが送信）
    [PunRPC]
    private void SetCountDownTime(int countDownTime)
	{
        m_countDownText.GetComponent<Text>().text = countDownTime.ToString();
    }

    //全てのプレイヤーに移動を許可する通信関数
    [PunRPC]
    private void SetPlayerMovable()
	{
        //自分のプレイヤーインスタンスを移動可能にする
        GameObject.Find("OwnPlayer").GetComponent<AvatarController>().SetMovable();
        //カウントダウンのテキストを破棄
        Destroy(m_countDownText.gameObject);
    }

    //各プレイヤーから送られてきたタイムを映し出す
    [PunRPC]
    private void ShowResult(Dictionary<string, float> scoreBoard)
    {
        m_resultBoard.SetActive(true);
        //ここから下にＡＩのことを書いていく
    }

    void Update()
	{
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
