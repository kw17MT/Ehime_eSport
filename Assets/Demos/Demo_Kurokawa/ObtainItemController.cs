using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ObtainItemController : MonoBehaviourPunCallbacks
{
    private enum EnItemType
	{
        enOrangePeel,                                   //オレンジの皮
        enOrangeJet,                                    //オレンジジュースジェット
        enTrain,                                        //坊ちゃん列車キラー
        enStar,                                         //スター
        enSnapperCannon,                                //タイ砲
        enItemTypeNum                                   //アイテムの種類の数
	}

    private GameObject m_paramManager = null;           //パラメータを保存するインスタンス（シーン跨ぎ）

    private void Start()
	{
        //ゲーム中のパラメータを保存するインスタンスを取得
        m_paramManager = GameObject.Find("ParamManager");
	}

    //オレンジの皮を自分の後ろに置く
    [PunRPC]
    public void InstantiateOrangePeel(Vector3 popPos)
    {
        //ゲーム全体で生成したオレンジの皮の数を把握できるように、ローカルのインスタンスでもメモする。
        m_paramManager.GetComponent<ParamManage>().AddOrangePeelNum();
        //ローカルでオレンジの皮を指定された座標に生成
        var orange = PhotonNetwork.Instantiate("OrangePeel", popPos, Quaternion.identity);
        //こちら側でも名前に総合生成数を付与する。
        orange.name = "OrangePeel" + m_paramManager.GetComponent<ParamManage>().GetOrangePeelNumOnField();
    }

    //タイを自分の前に置く
    [PunRPC]
    public void InstantiateSnapper(Vector3 popPos)
    {
        //ローカルでオレンジの皮を指定された座標に生成
        var snapper = PhotonNetwork.Instantiate("Snapper", popPos, Quaternion.identity);
        //こちら側でも名前に総合生成数を付与する。
        snapper.name = "Snapper";
    }

    void Update()
    {
        //自分が生成したインスタンスならば
        if (photonView.IsMine)
        {
            //テストでボタンを押したらバナナが出るようにする。
            if (Input.GetKeyDown(KeyCode.K))
            {
                Vector3 orangePeelPos = this.gameObject.transform.position + (this.gameObject.transform.forward * -2.0f);
                photonView.RPC(nameof(InstantiateOrangePeel), RpcTarget.All, orangePeelPos);
            }
            //テストでボタンを押したらスター使用状態にする。
            if (Input.GetKeyDown(KeyCode.J))
            {
                this.GetComponent<AvatarController>().SetIsUsingStar();
            }
            //テストでボタンを押したらスター使用状態にする。
            if (Input.GetKeyDown(KeyCode.L))
            {
                this.GetComponent<AvatarController>().SetIsUsingJet();
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                Vector3 snapperPos = this.gameObject.transform.position + (this.gameObject.transform.forward * -2.0f);
                photonView.RPC(nameof(InstantiateSnapper), RpcTarget.All, snapperPos);
            }
        }
    }
}
