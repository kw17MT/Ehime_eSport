using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// MonoBehaviourPunCallbacksを継承して、PUNのコールバックを受け取れるようにする
public class MatchingSceneScript : MonoBehaviourPunCallbacks
{
    private GameObject text = null;
    private GameObject playerObject = null;
    private float matchingWaitTime = 30.0f;
    private GameObject operation = null;
    private GameObject timeText = null;
    private int prevMatchingWaitTime = 0;

    private void Start()
    {
        //操作を監視するため、操作インスタンスを取得
        operation = GameObject.Find("OperationManager");
        // PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
        PhotonNetwork.ConnectUsingSettings();
        //名前でメンバーを表示するインスタンスを取得
        text = GameObject.Find("MemberList");
        //マッチング待機時間を表示するインスタンスを取得
        timeText = GameObject.Find("WaitTime");
        //シーンの遷移はホストクライアントに依存する
        PhotonNetwork.AutomaticallySyncScene = true;
    }

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
        playerObject = PhotonNetwork.Instantiate("Player", position, Quaternion.identity);
    }

    //関数の通信の際に必要な表記
    [PunRPC]
    //残り待機時間を表示する
    void SetWaitTime(int currentTime)
	{
        //テキストの中身を残り待機時間に書き換える。数値はホストクライアント側で計測
        timeText.GetComponent<Text>().text = currentTime.ToString();
    }

    void Update()
    {
        //ルームのメンバーリストを更新する。
        text.GetComponent<Text>().text = ".+*SpecialRoomMember*+.\n";
        foreach (var player in PhotonNetwork.PlayerList)
        {
            text.GetComponent<Text>().text += player.NickName + "\n";
        }

        //ホストのみ実行する部分
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            //ホストクライアントでボタンを長押しすると
            if(operation.GetComponent<Operation>().GetIsLongTouch())
			{
                //インゲームに遷移する
                SceneManager.LoadScene("DemoInGame");
            }

            //マッチング待機時間をゲーム時間で減らしていく
            matchingWaitTime -= Time.deltaTime;
            //現在の待機時間の整数部分を取得
            int currentMatchingWaitTime = (int)matchingWaitTime;
            //待ち時間がなくなったら
            if (matchingWaitTime < 0.0f)
            {
                //ゲーム開始
                SceneManager.LoadScene("DemoInGame");
            }
            //待機時間の秒数が変わったらそれを同期する
            if (prevMatchingWaitTime != currentMatchingWaitTime)
			{
                //表示時間を更新するようにルームの全員に通知する（ここで自分も残り待機時間を更新）
                photonView.RPC(nameof(SetWaitTime), RpcTarget.All, currentMatchingWaitTime);
            }

            //現在の待機時間の整数部分を保存しておく
            prevMatchingWaitTime = currentMatchingWaitTime;
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
