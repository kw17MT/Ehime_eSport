using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//自分のプレイヤーのため記録場所で、通信からは遮断したクラス
public class ParamManage : MonoBehaviour
{
    private int m_orangePeelNum = 0;                    //オンラインでのゲーム中何個オレンジの皮が置かれたか
    private int m_playerID = 0;                         //プレイヤーのID（原則入ったもの順）

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

    //ステージ上に置かれたオレンジの皮を集計
    public void AddOrangePeelNum()
	{
        m_orangePeelNum++;
	}

    //ゲーム中に置かれたオレンジの皮の総数
    public int GetOrangePeelNumOnField()
	{
        return m_orangePeelNum;
	}
}
