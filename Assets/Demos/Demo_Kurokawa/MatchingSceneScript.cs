using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// MonoBehaviourPunCallbacksを継承して、PUNのコールバックを受け取れるようにする
public class MatchingSceneScript : MonoBehaviourPunCallbacks
{
    private GameObject m_memberListText = null;                     //メンバーリストを表示するテキストインスタンス
    private GameObject m_waitTimeText = null;                       //残り待機時間を表示するテキストインスタンス
    private GameObject m_operation = null;                          //操作管理のインスタンス
    private int m_prevMatchingWaitTime = 0;                         //前までの残り待機時間の整数部分
    private float m_matchingWaitTime = 50.0f;                        //残り待機時間
    private bool m_isInstantiateAI = false;                         //AIインスタスを生成したか

    private void Start()
    {
        //操作を監視するため、操作インスタンスを取得
        m_operation = GameObject.Find("OperationManager");
        // PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
        PhotonNetwork.ConnectUsingSettings();
        //名前でメンバーを表示するインスタンスを取得
        m_memberListText = GameObject.Find("MemberList");
        //マッチング待機時間を表示するインスタンスを取得
        m_waitTimeText = GameObject.Find("WaitTime");
        //シーンの遷移はホストクライアントに依存する
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    //作成するルームの設定インスタンス
    private RoomOptions roomOptions = new RoomOptions()
    {
        //0だと人数制限なし
        MaxPlayers = 4,
        //部屋に参加できるか
        IsOpen = true,
        //この部屋がロビーにリストされるか
        IsVisible = true, 
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
        int currentPlayerNumber = PhotonNetwork.PlayerList.Length - 1;
        //プレイヤーを横に並べていく
        var position = new Vector3(currentPlayerNumber, 0.0f, 0.0f);
        //Prefabからプレイヤーが操作するモデルを生成
        PhotonNetwork.Instantiate("Player", position, Quaternion.identity);
    }

    //関数の通信の際に必要な表記
    [PunRPC]
    //残り待機時間を表示する
    void SetWaitTime(int currentTime)
	{
        //テキストの中身を残り待機時間に書き換える。数値はホストクライアント側で計測
        m_waitTimeText.GetComponent<Text>().text = currentTime.ToString();
    }

    //待機時間が終了した時、一度だけAIを不足プレイヤー分生成する。
    private void InstantiateAIOnce()
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
                PhotonNetwork.Instantiate("AI", position, Quaternion.identity);
            }
            //AIの生成終了
            m_isInstantiateAI = true;
            //残り待機時間のテキストを破棄
            Destroy(m_waitTimeText.gameObject);
        }
    }

    //残り待機時間を他のプレイヤーと同期させる
    private void SynchronizeWaitTime()
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
                //ゲーム開始
                SceneManager.LoadScene("DemoInGame");
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
        //ルームのメンバーリストを更新する。
        m_memberListText.GetComponent<Text>().text = ".+*SpecialRoomMember*+.\n";
        foreach (var player in PhotonNetwork.PlayerList)
        {
            m_memberListText.GetComponent<Text>().text += player.NickName + "\n";
        }

        //ホストのみ実行する部分
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            //ホストクライアントがボタンを長押しすると
            if(m_operation.GetComponent<Operation>().GetIsLongTouch())
			{
                //強制的にインゲームに遷移する
                SceneManager.LoadScene("DemoInGame");
            }

            //残り時間を他プレイヤーと同期する
            SynchronizeWaitTime();
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
