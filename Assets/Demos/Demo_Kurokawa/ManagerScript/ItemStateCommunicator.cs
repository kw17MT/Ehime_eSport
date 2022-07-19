using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class ItemStateCommunicator : MonoBehaviourPunCallbacks
{
    public GameObject m_orangePeel;             //オレンジの皮プレファブ
    public GameObject m_snapper;                //タイプレファブ
    private int m_orangePeelSum = 0;            //ゲーム中生成したオレンジの皮の合計
    private int m_snapperSum = 0;               //ゲーム中生成したタイの合計

    //名前を使用してインスタンスを破壊する（生成通信をもとにローカルで生成した皮やタイを名前で消す）
    [PunRPC]
    public void DestroyItemWithName(string name)
	{
        GameObject go = GameObject.Find(name);
        if(go != null)
		{
            Destroy(go);
        }
	}

    //オレンジの皮か鯛を指定された位置に生成する（下2個の引数はタイの時のみ使用）
    public void PopItem(string prefabName, Vector3 popPos, int wayPointNumber, int playerNumber)
    { 
        //オレンジの皮が指定されていた場合
        if(prefabName == "OrangePeel")
		{
            //生成したオレンジの皮の個数を増やす
            m_orangePeelSum++;
            //オレンジの皮生成
            GameObject peel = Instantiate(m_orangePeel, popPos, Quaternion.identity);
            peel.name = "OrangePeel" + m_orangePeelSum;
        }
        //タイが指定されていた場合
        else if(prefabName == "Snapper")
		{
            //生成したタイの個数を増やす
            m_snapperSum++;
            //タイ生成
            GameObject snapper = Instantiate(m_snapper, popPos, Quaternion.identity);
            //次目指すウェイポイントを設定
            snapper.GetComponent<WayPointChecker>().SetNextWayPointDirectly(popPos, wayPointNumber);
            //発射したプレイヤーのIDを記録
            snapper.GetComponent<SnapperController>().SetOwnerID(playerNumber);
            snapper.name = "Snapper" + m_snapperSum;
        }
    }
}
