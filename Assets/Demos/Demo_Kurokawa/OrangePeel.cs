using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//オレンジの皮クラス
public class OrangePeel : MonoBehaviourPunCallbacks
{
    //オレンジの皮と重なったら
    void OnTriggerEnter(Collider col)
	{
        //当たったものが自分が操作するプレイヤーであったら
        if(col.gameObject.tag == "OwnPlayer")
		{
            //当たったプレイヤーを攻撃された判定にする。
            col.gameObject.GetComponent<AvatarController>().SetIsAttacked();
		}
        //ぶつかった皮を消す
        //photonView.RPC(nameof(DestroyHittedOrangePeel), RpcTarget.All, this.gameObject.name);
        Debug.Log("Destroy Target : " + this.gameObject.name);

        Destroy(this.gameObject);
    }

    [PunRPC]
    //ぶつかった皮の名前でそのインスタンスを破棄する。
    public void DestroyHittedOrangePeel(string hittedOrangePeelName)
    {
        //引数の皮インスタンスの名前でオブジェクトを検索
        GameObject orange = GameObject.Find(hittedOrangePeelName);
        //存在していたら
        if (orange != null)
        {
            //皮の消去
            Destroy(orange.gameObject);
        }
		else
		{
            Debug.Log("Failed Destroy Peel");
		}
    }
}
