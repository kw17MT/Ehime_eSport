using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class AIReaction : MonoBehaviourPunCallbacks
{
	private float SPIN_AMOUNT = 6.0f;                   //被弾時の回転率
	private float MAX_SPIN_AMOUNT = 360.0f;				//被弾時、どのくらいAIを回転させるか
	private float m_spinedAngle = 0.0f;					//被弾した時の回転リアクションを何度分おこなったかの計測変数

	//AIに何かオブジェクトが当たったら、名前でオブジェクトを消す通信関数
	[PunRPC]
	private void DestroyItemWithName(string name)
	{
		GameObject.Find("SceneDirector").GetComponent<ItemStateCommunicator>().DestroyItemWithName(name);
	}

	//当たったのが何なのかを識別する（オフラインモード）
	private void IdentifyObjectInOffline(Collision col)
	{
		//当たった相手がプレイヤーか他のAIならば
		if (col.gameObject.tag == "OwnPlayer")
		{
			//プレイヤーが無敵状態ならば
			if (col.gameObject.GetComponent<AvatarController>().GetIsInvincible())
			{
				//攻撃された判定にする
				this.gameObject.GetComponent<AICommunicator>().SetIsAttacked(true);
			}
		}
		//当たった相手がAIならば
		else if (col.gameObject.tag == "Player")
		{
			//AIは無敵状態ならば
			if (col.gameObject.GetComponent<AICommunicator>().GetIsInvincible())
			{
				//攻撃された判定にする
				this.gameObject.GetComponent<AICommunicator>().SetIsAttacked(true);
			}
		}
		//当たったのがタイだったら
		if (col.gameObject.name.Length >= 7 && col.gameObject.name[0..7] == "Snapper")
		{
			//自分の名前を取得
			string idStr = this.gameObject.name;
			//自分のプレイヤー番号だけを取得
			int id = int.Parse(idStr[6].ToString());
			//自分が出したタイでなければ
			if (col.gameObject.GetComponent<SnapperController>().GetOwnerID() != id)
			{
				//自分のゲーム内のタイインスタンスの削除
				photonView.RPC(nameof(DestroyItemWithName), RpcTarget.All, col.gameObject.name);
				//無敵でなければ
				if (!this.gameObject.GetComponent<AICommunicator>().GetIsInvincible())
				{
					//攻撃された
					this.gameObject.GetComponent<AICommunicator>().SetIsAttacked(true);
				}			}
		}
		//オレンジの皮であれば
		if (col.gameObject.name.Length >= 10 && col.gameObject.name[0..10] == "OrangePeel")
		{
			//その皮の名前を用いてオブジェクトを消すように通信
			photonView.RPC(nameof(DestroyItemWithName), RpcTarget.All, col.gameObject.name);
		}
	}

	//当たったのが何なのかを識別する（オンラインモード）
	private void IdentifyObjectInOnline(Collision col)
	{
		//各プレイヤーの無敵状態を示すキー部分を作成
		string name;// = PhotonNetwork.NickName + "Invincible";

		//当たった相手がプレイヤーならば
		if (col.gameObject.tag == "OwnPlayer")
		{
			//プレイヤーが無敵状態ならば
			if (col.gameObject.GetComponent<AvatarController>().GetIsInvincible())
			{
				//攻撃された判定にする
				this.gameObject.GetComponent<AICommunicator>().SetIsAttacked(true);
			}
		}
		else if (col.gameObject.tag == "Player")
		{
			//AIならば
			if (col.gameObject.GetComponent<AICommunicator>() != null)
			{
				//AIの名前を使ってルームプロパティのキーを制作
				name = col.gameObject.GetComponent<AICommunicator>().GetAIName() + "Invincible";
				//無敵状態か取得
				int isInvincivle = (PhotonNetwork.CurrentRoom.CustomProperties[name] is int value) ? value : 0;
				if (isInvincivle == 1)
				{
					//攻撃を受けた
					this.gameObject.GetComponent<AICommunicator>().SetIsAttacked(true);
				}
			}
			//オンラインプレイヤーであれば
			else if (col.gameObject.GetComponent<PhotonView>() != null)
			{
				//コリジョンの持ち主のPhotonNetworkに関する変数を取得
				Player pl = col.gameObject.GetComponent<PhotonView>().Owner;
				//そのプレイヤーの無敵状態をルームプロパティから持ってくる
				string plName = pl.NickName + "Invincible";
				int isInvincivle = (PhotonNetwork.CurrentRoom.CustomProperties[plName] is int value) ? value : 0;
				//そのプレイヤーが無敵で、自分がスターもキラーも使っていなけば
				if (isInvincivle == 1)
				{
					//攻撃を受けた
					this.gameObject.GetComponent<AICommunicator>().SetIsAttacked(true);
				}
			}
		}

		if (col.gameObject.name.Length >= 7 && col.gameObject.name[0..7] == "Snapper")
		{
			string idStr = this.gameObject.GetComponent<AICommunicator>().GetAIName();
			int id = int.Parse(idStr[6].ToString());
			if (col.gameObject.GetComponent<SnapperController>().GetOwnerID() != id)
			{
				//自分のゲーム内のタイインスタンスの削除
				photonView.RPC(nameof(DestroyItemWithName), RpcTarget.All, col.gameObject.name);
				//無敵状態ならば
				if (!this.gameObject.GetComponent<AICommunicator>().GetIsInvincible())
				{
					//攻撃された
					this.gameObject.GetComponent<AICommunicator>().SetIsAttacked(true);
				}
				//Debug.Log("Attacked By Snapper(Clone)");
			}
			if (col.gameObject.name.Length >= 10 && col.gameObject.name[0..10] == "OrangePeel")
			{
				photonView.RPC(nameof(DestroyItemWithName), RpcTarget.All, col.gameObject.name);
				//Debug.Log("OrangePeel(Clone)");
			}
		}
	}

	private void OnCollisionEnter(Collision col)
	{
		//オフラインモードならば
		if (PhotonNetwork.OfflineMode)
		{
			IdentifyObjectInOffline(col);
		}
		//オンラインモードならば
		else
		{
			IdentifyObjectInOnline(col);
		}
	}

	// Update is called once per frame
	void FixedUpdate()
    {
		if (this.gameObject.GetComponent<AICommunicator>().GetIsAttacked())
		{
			//回転させた量を計測
			m_spinedAngle += SPIN_AMOUNT;
			//十分回転していなければ
			if (m_spinedAngle <= MAX_SPIN_AMOUNT)
			{
				//ワールド座標で回転
				this.transform.Rotate(0.0f, SPIN_AMOUNT, 0.0f, Space.World); // 回転角度を設定            
			}
		}
	}
}
