using Photon.Pun;
using Photon.Realtime;
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
        // PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
        PhotonNetwork.ConnectUsingSettings();

        int id = GameObject.Find("ParamManager").GetComponent<ParamManage>().GetPlayerID();

        // 自身のアバター（ネットワークオブジェクト）を生成する
        var position = new Vector3(id, 0.0f, 0.0f);
        PhotonNetwork.Instantiate("Player", position, Quaternion.identity);

        //ホストのみ実行する部分
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            if (!m_isInstantiateAI)
            {
                for (int i = 0; i < 4 - PhotonNetwork.PlayerList.Length; i++)
                {
                    //プレイヤーを横に並べていく
                    var AIPos = new Vector3(i + 1, 0.0f, 0.0f);
                    //PrefabからAIをルームオブジェクトとして生成
                    PhotonNetwork.InstantiateRoomObject("AI", AIPos, Quaternion.identity);
                }
                //AIを生成した。
                m_isInstantiateAI = true;
            }
        }

        m_memberListText = GameObject.Find("MemberList");
        m_countDownText = GameObject.Find("CountDown");
        m_resultText = GameObject.Find("Result");

        //シーン遷移をホストに同期する
        PhotonNetwork.AutomaticallySyncScene = true;

        //インゲームに遷移したら入室拒否にする。
        PhotonNetwork.CurrentRoom.IsOpen = false;

        //秒数の整数部分の変化を見るために保存する。
        m_prevCountDownNum = (int)m_countDownNum;
    }

    public void AddReadyPlayerNum()
	{
        m_playerReadyNum++;
    }

    public void AddGoaledPlayerNameAndRecordTime(string playerName, float time)
    {
        //this.m_playerGoaledTime[m_goaledPlayerNum] = time;
        //this.m_goaledPlayerNum++;
        //Debug.Log("RECORED");
        //Debug.Log("Result " + m_goaledPlayerNum + " / " + PhotonNetwork.PlayerList.Length);

        m_scoreBoard.Add(playerName, time);
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
            Debug.Log(m_playerReadyNum + "      /      "+ PhotonNetwork.PlayerList.Length);
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
