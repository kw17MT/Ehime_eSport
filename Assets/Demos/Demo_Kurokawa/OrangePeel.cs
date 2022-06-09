using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class OrangePeel : MonoBehaviourPunCallbacks
{
    //オレンジの皮と重なったら
    void OnTriggerEnter(Collider col)
	{
        //ぶつかった皮を消す
        photonView.RPC(nameof(DestroyHittedOrangePeel), RpcTarget.All, this.gameObject.name);
    }

    [PunRPC]
    //ぶつかった皮の名前でそのインスタンスを破棄する。
    public void DestroyHittedOrangePeel(string hittedOrangePeelName)
    {
        //引数の皮インスタンスの名前でオブジェクトを検索
        GameObject orange = GameObject.Find(hittedOrangePeelName);
        
        if (orange != null)
        {
            //皮の消去
            Destroy(orange.gameObject);
        }
    }
}
