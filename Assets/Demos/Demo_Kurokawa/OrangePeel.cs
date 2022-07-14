using UnityEngine;
using Photon.Pun;

//オレンジの皮クラス。ネットワークオブジェクト
public class OrangePeel : MonoBehaviourPunCallbacks
{

    [PunRPC]
    private void DestroyItemWithName(string name)
    {
        //シーンに出ているゲームオブジェクトを名前検索してデリートする
        GameObject.Find("SceneDirector").GetComponent<ItemStateCommunicator>().DestroyItemWithName(name);
    }

    //オレンジの皮と重なったら
    void OnTriggerEnter(Collider col)
	{
        //当たったものが自分が操作するプレイヤーであったら
        if(col.gameObject.tag == "OwnPlayer")
		{
            //当たったプレイヤーを攻撃された判定にする。
            col.gameObject.GetComponent<AvatarController>().SetIsAttacked();
            DestroyItemWithName(this.gameObject.name);
        }
        if(col.gameObject.tag == "Player")
		{
            //当たったAIを攻撃された判定にする。
            if (col.gameObject.GetComponent<AICommunicator>() != null)
            {
                col.gameObject.GetComponent<AICommunicator>().SetIsAttacked(true);
                DestroyItemWithName(this.gameObject.name);
            }
            else if (col.gameObject.GetComponent<AvatarController>() != null)
			{
                //当たったプレイヤーを攻撃された判定にする。
                col.gameObject.GetComponent<AvatarController>().SetIsAttacked();
                DestroyItemWithName(this.gameObject.name);
            }
        }

        if(col.gameObject.name.Length >= 7 && col.gameObject.name[0..7] == "Snapper")
		{
            if((col.gameObject.transform.position - this.gameObject.transform.position).magnitude < 2.0f)
			{   
                //オレンジの皮のインスタンスを消す通信を行う
                DestroyItemWithName(this.gameObject.name);
                //タイのインスタンスを消す通信を行う
                DestroyItemWithName(col.gameObject.name);
            }
        }
	}
}
