using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//自分のプレイヤーのため記録場所で、通信からは遮断したクラス
public class ParamManage : MonoBehaviour
{
    private int m_playerID = 0;                         //プレイヤーのID（原則入ったもの順）
    private int m_place = 0;                            //現在のプレイヤーの順位
    private bool m_isOfflineMode = false;               //オフラインモードでプレイするか

    // Start is called before the first frame update
    void Start()
    {
        //シーン移動で破棄しない
        DontDestroyOnLoad(this);
    }

    //自分のプレイヤーIDを記録
    public void SetPlayerID(int id)
	{
        m_playerID = id;
	}

    //自分のプレイヤーIDを取得
    public int GetPlayerID()
	{
        return m_playerID;
	}

    //オフラインモードで開始するように
    public void SetOfflineMode()
	{
        m_isOfflineMode = true;
	}

    //オフラインモードか
    public bool GetIsOfflineMode()
	{
        return m_isOfflineMode;
	}

    //自分の順位を記録
    public void SetPlace(int place)
    {
        m_place = place;
    }

    //自分の順位を取得
    public int GetPlace()
    {
        return m_place;
    }
}
