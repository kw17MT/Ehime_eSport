using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//自分のプレイヤーのため記録場所で、通信からは遮断したクラス
public class ParamManage : MonoBehaviour
{
    private int m_playerID = 0;                                //プレイヤーのID（原則入ったもの順）
    private int m_place = 0;                                   //現在のプレイヤーの順位
    private List<int> m_usedCharaNumber = new List<int>();     //オンラインプレイヤーが使用しているキャラの番号群

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

    public void SaveCharactorNumber(int number)
	{
        m_usedCharaNumber.Add(number);
	}

    public ref List<int> GetCharactorNumber()
	{
        return ref m_usedCharaNumber;
	}

	private void Update()
	{
		//if(SceneManager.GetActiveScene().name == "01_TitleScene")
		//{
  //          Destroy(this.gameObject);
		//}
	}
}
