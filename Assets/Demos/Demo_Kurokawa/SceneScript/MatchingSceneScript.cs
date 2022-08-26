using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

//マッチング中の挙動のクラス
public class MatchingSceneScript : MonoBehaviourPunCallbacks
{
    private GameObject m_waitTimeText = null;                       //残り待機時間を表示するテキストインスタンス
    private GameObject m_operation = null;                          //操作管理のインスタンス
    private GameObject m_paramManager = null;                       //シーン以降で保持したいパラメータの保管インスタンス
    private GameObject m_player = null;                             //プレイヤーインスタンス
    private bool m_isInstantiateAI = false;                         //AIインスタンスを生成したか
    private bool m_isJoinedRoom = false;                            //ルームに入れたか
    private int m_prevMatchingWaitTime = 0;                         //前までの残り待機時間の整数部分
    private float m_matchingWaitTime = 30.0f;                       //残り待機時間
    private Vector3 m_position = Vector3.zero;                      //プレイヤーを設定する座標

    void Start()
    {
        //操作を監視するため、操作インスタンスを取得
        m_operation = GameObject.Find("OperationSystem");
        // PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
        PhotonNetwork.ConnectUsingSettings();
        //マッチング待機時間を表示するインスタンスを取得
        m_waitTimeText = GameObject.Find("LimitTimeLabel");
        //シーン間で保持するパラメータインスタンス
        m_paramManager = GameObject.Find("ParamManager");
        //シーンの遷移はホストクライアントに依存する
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    //作成するルームの設定インスタンス
    RoomOptions roomOptions = new RoomOptions()
    {
        //0だと人数制限なし
        MaxPlayers = 4,
        //部屋に参加できるか
        IsOpen = true,
        //この部屋がロビーにリストされるか
        IsVisible = true,
        //ユーザーIDの発布を行う。
        PublishUserId = true,
    };

    // マスターサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnConnectedToMaster()
    {
        //ランダムなルームに参加する
        PhotonNetwork.JoinRandomRoom();
    }

    //上でランダムなルームに参加できなかったら
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //部屋を作る
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    // ゲームサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnJoinedRoom()
    {
        //現在接続しているプレイヤー数を取得する。
        float currentPlayerNumber = (PhotonNetwork.PlayerList.Length - 1);
        //Prefabからプレイヤーが操作するモデルを生成
        m_player =  PhotonNetwork.Instantiate("Player", m_position, Quaternion.AngleAxis(180, Vector3.up));
        m_player.GetComponent<Rigidbody>().isKinematic = true;

        //今までこのルームに何人が入ってきたかでアクターナンバーが増えていく（アクターナンバーに書き込み不可）
        int id = PhotonNetwork.LocalPlayer.ActorNumber;
        //プレイヤーに5以上のIDが割り振られたことはどこかのタイミングで一人以上抜けている
        if (id >= 5)
		{
			//ルームにいる他のプレイヤーを取得
			Player[] otherPlayers = PhotonNetwork.PlayerListOthers;
			//他のプレイヤーに割り当てられている、使えない名前とIDを保存していく配列を定義
			var cantUseId = new List<string>();

			foreach (var pl in otherPlayers)
			{
				//既に使っているIDを保存していく
				cantUseId.Add(pl.NickName);
			}

            //Player1という名前のユーザーがいなければ、ID1を使用する。
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
        //プレイヤーのIDを記録する
		m_paramManager.GetComponent<ParamManage>().SetPlayerID(id);
        string spawnerName = "PlayerSpawner/Player" + id;
        m_position = GameObject.Find(spawnerName).transform.position;

        m_isJoinedRoom = true;
    }

    //関数の通信の際に必要な表記
    [PunRPC]
    //残り待機時間を表示する
    void SetWaitTime(int currentTime)
	{
        if (m_waitTimeText != null)
		{
            //テキストの中身を残り待機時間に書き換える。数値はホストクライアント側で計測
            m_waitTimeText.GetComponent<Text>().text = currentTime.ToString();
            GameObject.Find("SceneManager").GetComponent<LimitTime>().SetLimitTime(currentTime);
        }
    }

    //待機時間が終了した時、一度だけAIを不足プレイヤー分生成する。
    void InstantiateAIOnce()
	{
        //AIを生成していなければ
        if (!m_isInstantiateAI)
        {
            //最大プレイヤー数までAIを生成
            for (int i = 0; i < 4 - PhotonNetwork.PlayerList.Length; i++)
            {
                //プレイヤーを横に並べていく
                var position = new Vector3(i + 1.5f, 0.0f, 0.0f);
                //PrefabからAIを生成
                //PhotonNetwork.Instantiate("AI", position, Quaternion.identity);
            }
            //AIの生成終了
            m_isInstantiateAI = true;
            //残り待機時間のテキストを破棄
            Destroy(m_waitTimeText.gameObject);
        }
    }

    //残り待機時間を他のプレイヤーと同期させる
    void SynchronizeWaitTime()
	{
        //マッチング待機時間をゲーム時間で減らしていく
        m_matchingWaitTime -= Time.deltaTime;
        //現在の待機時間の整数部分を取得
        int currentMatchingWaitTime = (int)m_matchingWaitTime;
        //待ち時間がなくなったら
        if (m_matchingWaitTime < 0.0f)
        {
            //AIを生成
            InstantiateAIOnce();
            //2秒くらい待ってインゲームに移行
            if (m_matchingWaitTime < -2.0f)
            {
                int stageType = GameObject.Find("UserSettingDataStorageSystem").GetComponent<UserSettingData>().GetSetStageType;
                //ユーザーが選択してきたものを記録したインスタンスを取得
                if (stageType == 0)
				{
                    //ゲーム開始
                    SceneManager.LoadScene("08_EasyGameScene");
                }
                else if(stageType == 1)
				{
                    //ゲーム開始
                    SceneManager.LoadScene("08_GameScene");
                }
            }
        }
        //待機時間の秒数が変わったらそれを同期する
        if (m_prevMatchingWaitTime != currentMatchingWaitTime)
        {
            //表示時間を更新するようにルームの全員に通知する（ここで自分も残り待機時間を更新）
            photonView.RPC(nameof(SetWaitTime), RpcTarget.All, currentMatchingWaitTime);
        }

        //現在の待機時間の整数部分を保存しておく
        m_prevMatchingWaitTime = currentMatchingWaitTime;
    }

    void Update()
    {
        //ホストのみ実行する部分
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            //ホストクライアントがボタンを長押しすると
            if(m_operation.GetComponent<Operation>().GetIsLongTouch
                && m_isJoinedRoom)
			{
                int stageType = GameObject.Find("UserSettingDataStorageSystem").GetComponent<UserSettingData>().GetSetStageType;
                //ユーザーが選択してきたものを記録したインスタンスを取得
                if (stageType == 0)
                {
                    //ゲーム開始
                    SceneManager.LoadScene("08_EasyGameScene");
                }
                else if (stageType == 1)
                {
                    //ゲーム開始
                    SceneManager.LoadScene("08_GameScene");
                }
            }

            //残り時間を他プレイヤーと同期する
            SynchronizeWaitTime();
        }


        if (m_player != null)
        {
            m_player.transform.position = m_position;
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
