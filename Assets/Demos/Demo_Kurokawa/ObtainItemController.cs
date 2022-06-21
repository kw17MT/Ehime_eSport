using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//プレイヤーが取得しているアイテムの管理と使用するクラス。
public class ObtainItemController : MonoBehaviourPunCallbacks
{
    private GameObject m_paramManager = null;           //パラメータを保存するインスタンス（シーン跨ぎ）
    private EnItemType m_obtainItemType = EnItemType.enNothing;

    //アイテムの種類
    private enum EnItemType
	{
        enNothing = -1,
        enOrangePeel,                                   //オレンジの皮
        enOrangeJet,                                    //オレンジジュースジェット
        enTrain,                                        //坊ちゃん列車キラー
        enStar,                                         //スター
        enSnapperCannon,                                //タイ砲
        enItemTypeNum                                   //アイテムの種類の数
	}

    private void Start()
	{
        //ゲーム中のパラメータを保存するインスタンスを取得
        m_paramManager = GameObject.Find("ParamManager");
	}

    //ランダムなアイテムを抽選する
    public void GetRandomItem()
	{
        //何も持っていなければ
        if(m_obtainItemType == EnItemType.enNothing)
		{
            //アイテムのナンバーをランダムに取得
            int type = (int)Random.Range((float)EnItemType.enOrangePeel, (float)EnItemType.enItemTypeNum);
            m_obtainItemType = (EnItemType)type;
        }

        Debug.Log("取得したアイテム番号　＝　" + m_obtainItemType);
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

    //プレイヤーが呼び出す。タイを自分の前に置く
    [PunRPC]
    public void InstantiateSnapper(Vector3 popPos)
    {
        //ローカルでオレンジの皮を指定された座標に生成
        var snapper = PhotonNetwork.Instantiate("Snapper", popPos, Quaternion.identity);
        //名前。
        snapper.name = "Snapper";
        //プレイヤーが直近で通過したウェイポイントの番号、座標を与える
        snapper.GetComponent<WayPointChecker>().SetCurrentWayPointDirectly(popPos, this.gameObject.GetComponent<WayPointChecker>().GetCurrentWayPointNumber());
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
			//テストでボタンを押したらキノコ使用状態にする。
			if (Input.GetKeyDown(KeyCode.L))
			{
				this.GetComponent<AvatarController>().SetIsUsingJet();
			}
			//鯛を出す
			if (Input.GetKeyDown(KeyCode.I))
			{
				Vector3 snapperPos = this.gameObject.transform.position + (this.gameObject.transform.forward * 2.0f);
				photonView.RPC(nameof(InstantiateSnapper), RpcTarget.All, snapperPos);
			}
			//テストでボタンを押したらキラー使用状態にする。
			if (Input.GetKeyDown(KeyCode.P))
			{
				this.GetComponent<AvatarController>().SetIsUsingKiller();
			}

			if (Input.GetKeyDown(KeyCode.K))
			{
                switch(m_obtainItemType)
				{
                    case EnItemType.enOrangePeel :
                        Vector3 orangePeelPos = this.gameObject.transform.position + (this.gameObject.transform.forward * -2.0f);
                        photonView.RPC(nameof(InstantiateOrangePeel), RpcTarget.All, orangePeelPos);
                        m_obtainItemType = EnItemType.enNothing;
                        break;
                    case EnItemType.enOrangeJet:
                        this.GetComponent<AvatarController>().SetIsUsingJet();
                        m_obtainItemType = EnItemType.enNothing;
                        break;
                    case EnItemType.enTrain:
                        this.GetComponent<AvatarController>().SetIsUsingKiller();
                        m_obtainItemType = EnItemType.enNothing;
                        break;
                    case EnItemType.enStar:
                        this.GetComponent<AvatarController>().SetIsUsingStar();
                        m_obtainItemType = EnItemType.enNothing;
                        break;
                    case EnItemType.enSnapperCannon:
                        Vector3 snapperPos = this.gameObject.transform.position + (this.gameObject.transform.forward * 2.0f);
                        photonView.RPC(nameof(InstantiateSnapper), RpcTarget.All, snapperPos);
                        m_obtainItemType = EnItemType.enNothing;
                        break;
                    default:
                        return;
                }
			}
        }
    }
}
