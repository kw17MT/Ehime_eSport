using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// MonoBehaviourPunCallbacksを継承して、photonViewプロパティを使えるようにする
public class AvatarController : MonoBehaviourPunCallbacks
{
    Rigidbody m_rb = null;
    Vector3 m_moveDir = Vector3.zero;
    Vector3 m_rot = Vector3.zero;
    public GameObject m_orangePrefab;
    private GameObject m_paramManager = null;
    private bool m_canMove = false;

    void Start()
	{
        PhotonNetwork.NickName = "Player";
        if (photonView.IsMine)
        {
            PhotonNetwork.NickName = "OwnPlayer"/* + PhotonNetwork.LocalPlayer.UserId*/;
            this.gameObject.tag = "OwnPlayer";
            this.gameObject.name = "OwnPlayer";
        }

        GameObject text = GameObject.Find("MemberList");
        text.GetComponent<Text>().text += PhotonNetwork.NickName + "\n";

        m_rb = GetComponent<Rigidbody>();
        //インゲーム中であれば重力をオンにする
        if (SceneManager.GetActiveScene().name == "DemoInGame")
		{
            m_rb.useGravity = true;
        }

        m_paramManager = GameObject.Find("ParamManager");

        //1秒間に何回通信するか
        PhotonNetwork.SendRate = 3;
        //1秒間に何回同期を行うか
        PhotonNetwork.SerializationRate = 3;
    }

    public void SetMovable()
	{
        m_canMove = true;
	}

    private void Update()
	{
        if (SceneManager.GetActiveScene().name == "DemoInGame" && m_canMove)
        {
            // 自身が生成したオブジェクトだけに移動処理を行う
            if (photonView.IsMine)
            {
                //前方向に移動
                m_moveDir = this.transform.forward * (Input.GetAxis("Vertical") * 100.0f);
                //m_rb.AddForce(m_moveDir);
                //回転
                m_rot = new Vector3(0.0f, Input.GetAxis("Horizontal") / 2.0f, 0.0f);

                //テストでボタンを押したらバナナが出るようにする。
                if (Input.GetKeyDown(KeyCode.K))
                {
                    Vector3 orangePeelPos = this.transform.position + (this.transform.forward * -2.0f);
                    photonView.RPC(nameof(InstantiateOrangePeel), RpcTarget.All, orangePeelPos/*, "Orange"*/);
                }





            }



            //Yキル
            if (this.transform.position.y <= -2.0f)
            {
                this.transform.position = new Vector3(0.0f, 2.0f, 0.0f);
            }
        }


    }

    [PunRPC]
    public void InstantiateOrangePeel(Vector3 popPos/*, string name*/)
    {
        m_paramManager.GetComponent<ParamManage>().AddOrangePeelNum();
        
        var orange = PhotonNetwork.Instantiate("OrangePeel", popPos, Quaternion.identity);
        orange.name = "OrangePeel" + m_paramManager.GetComponent<ParamManage>().GetOrangePeelNumOnField();
    }

    private void FixedUpdate()
    {
        //Debug.Log(m_moveDir);
        m_rb.AddForce(m_moveDir);
        transform.Rotate(m_rot);
    }
}