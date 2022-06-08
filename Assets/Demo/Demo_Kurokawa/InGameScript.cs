using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// MonoBehaviourPunCallbacksを継承して、PUNのコールバックを受け取れるようにする
public class InGameScript : MonoBehaviourPunCallbacks
{
    private GameObject m_memberListText = null;
    private GameObject m_countDownText = null;
    private float m_countDownNum = 3.0f;
    private int m_prevCountDownNum = 0;

    private void Start()
    {
        // PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
        PhotonNetwork.ConnectUsingSettings();

        int currentPlayerNumber = PhotonNetwork.CountOfPlayersInRooms;

        // 自身のアバター（ネットワークオブジェクト）を生成する
        var position = new Vector3(currentPlayerNumber, 0.0f, 0.0f);
        PhotonNetwork.Instantiate("Player", position, Quaternion.identity);

        m_memberListText = GameObject.Find("MemberList");
        m_countDownText = GameObject.Find("CountDown");

        //シーン遷移をホストに同期する
        PhotonNetwork.AutomaticallySyncScene = true;

        //インゲームに遷移したら入室拒否にする。
        PhotonNetwork.CurrentRoom.IsOpen = false;

        m_prevCountDownNum = (int)m_countDownNum;

    }



    // マスターサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnConnectedToMaster()
    {
        // "Room"という名前のルームに参加する（ルームが存在しなければ作成して参加する）
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
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
            //マッチング待機時間をゲーム時間で減らしていく
            m_countDownNum -= Time.deltaTime;
            //待ち時間がなくなったら
            if (m_countDownNum < 0.0f)
            {
                //game開始フラグを立てるように通信を送る
                //GameObject.Find("OwnPlayer").GetComponent<AvatarController>().SetMovable();
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
