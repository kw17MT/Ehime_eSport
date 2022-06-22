using UnityEngine;
using Photon.Pun;

//オレンジの皮クラス。ネットワークオブジェクト
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
        //オレンジを消す
        Destroy(this.gameObject);
    }
}
